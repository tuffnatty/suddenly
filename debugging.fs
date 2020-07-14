REQUIRE util.fs

: bin.  ( u -- )
  BASE @  2 BASE !  SWAP .  BASE ! ;
: 2bin.  ( ud -- )
  BASE @ >R  2 BASE !  D.  R> BASE ! ;

: .xt  ( xt -- )
   DUP >NAME ?DUP-IF .ID DROP ELSE xt-see THEN ;

VARIABLE n-forms
2VARIABLE timer

0 VALUE debug-mode?
128 1024 * CONSTANT depth-stack-limit
CREATE depth-stack depth-stack-limit CELLS ALLOT
0 VALUE depth-stack-depth
: depth-stack@ ( -- u )
  depth-stack depth-stack-depth CELLS + @ ;
: depth-stack-push  ( n  -- )
  depth-stack-depth depth-stack-limit = ABORT" DEPTH STACK OVERFLOW!"
  DUP 0< ABORT" PUSHING NEGATIVE DEPTH"
  depth-stack depth-stack-depth CELLS + !  ( )
  depth-stack-depth 1+ TO depth-stack-depth ;
: depth-stack-drop  ( -- )
  depth-stack-depth 1- TO depth-stack-depth ;
: depth-stack-check ( -- f )
  depth-stack-drop
  depth-stack-depth 0< ABORT" DEPTH STACK UNDERFLOW!"
  \ depth-stack-depth 0 DO BL EMIT LOOP ." \stack-check" CR
  DEPTH depth-stack@ <> IF
    ." UNEXPECTED STACK DEPTH CHANGE! (" DEPTH . ." instead of " depth-stack@ . ." )"
    ~~bt
    TRUE
  ELSE FALSE THEN ;

: debug-init
  utime timer 2!
  0 n-forms ! ;

DEFER debug-bye
:noname
  n-forms @ . ."  hypotheses generated in " utime timer 2@ D- D. ." μs." cr
  \ trans-timer 2@ D. ." μs in transform." cr
  ; IS debug-bye

: \."
  debug-mode? IF
    POSTPONE ."
  ELSE
    POSTPONE \
  THEN ; IMMEDIATE

: \.s
  debug-mode? IF
    [CHAR] < ]]L EMIT  DEPTH .  [[ [CHAR] > ]]L EMIT  DUP .  [[ $2026 ]]L XEMIT  CR [[
  THEN ; IMMEDIATE

: \\." debug-mode? 1 > IF POSTPONE ." ELSE POSTPONE \ THEN ; IMMEDIATE

: \stack-mark  ( -- )
  debug-mode? IF  ]] DEPTH depth-stack-push                TRY [[  THEN ; IMMEDIATE

: \stack-check  ( -- )
  debug-mode? IF  ]] ENDTRY-IFERROR depth-stack-drop THROW THEN depth-stack-check IF WTF?? THEN [[  THEN ; IMMEDIATE

: checked-execute(0-1)  ( xt -- u )
  debug-mode? IF
    ]] >R DEPTH 1+ depth-stack-push R> TRY EXECUTE \stack-check [[
  ELSE ]] EXECUTE [[ THEN ; IMMEDIATE

variable last-timer
CREATE timer-stack 256 CELLS ALLOT
variable timer-stack-depth
[UNDEFINED] timer-list [IF]  \ reuse gforth 0.7.9 timer-list
  variable timer-list
[THEN]
[UNDEFINED] profile( [IF]
  6 CONSTANT assert-canary
  : debug) POSTPONE THEN ;
  : debug-does>  DOES>
    ]] Literal @ IF [[ ['] debug) assert-canary ;
  : debug: ( -- )
    Create false , immediate debug-does> ;
  : )else( ( -- ) 2>r postpone ELSE 2r> ; immediate compile-only
  : ) assert-canary <> ABORT" unmatched debug: or assertion" EXECUTE ; IMMEDIATE
  debug: profile(
  : +db ( "word" -- ) (') >body on ;
  : -db ( "word" -- ) (') >body off ;
[THEN]

[UNDEFINED] +t [IF]
  2Variable last-tick

  : 2+! ( d addr -- )  >r r@ 2@ d+ r> 2! ;
  : +t ( addr -- )
      utime 2dup last-tick dup 2@ 2>r 2! 2r> d- rot 2+! ;
[THEN]

: timer: Create $0. , , here timer-list !@ ,
  DOES> profile(
    timer-stack-depth @ ?DUP-IF 1- CELLS timer-stack + @  +t THEN
    timer-stack timer-stack-depth @ CELLS + !
    1 timer-stack-depth +!
  )else( drop ) ;
: +record ( -- ) PROFILE( -1 timer-stack-depth +!  timer-stack timer-stack-depth @ CELLS + @  +t ) ;

-DB PROFILE(

CREATE t_name CHAR + C, 127 ALLOT
: :+
  PROFILE(
    PARSE-NAME  ( addr u )
    2DUP t_name 1+ SWAP MOVE
    t_name OVER 1+ NEXTNAME TIMER:
    LATESTXT { t }
    NEXTNAME : ( )
    ['] +record >BODY ]]L >R [[ t ]]L EXECUTE [[
  )ELSE(
    :
  ) ;

[undefined] SEE-THREADED [IF]
[undefined] see-voc [if]
: see-threaded see ;
[else]
ALSO see-voc
: see-threaded SEE-THREADED ;
PREVIOUS
[then]
[THEN]
