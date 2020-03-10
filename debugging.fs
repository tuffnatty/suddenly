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
: depth-stack-push  ( -- )
  depth-stack-depth depth-stack-limit = ABORT" DEPTH STACK OVERFLOW!"
  DEPTH depth-stack depth-stack-depth CELLS + !
  depth-stack-depth 1+ TO depth-stack-depth ;
: depth-stack-check ( -- f )
  depth-stack-depth 1- TO depth-stack-depth
  depth-stack-depth 0< ABORT" DEPTH STACK OVERFLOW!"
  \ depth-stack-depth 0 DO BL EMIT LOOP ." \stack-check" CR
  DEPTH depth-stack@ <> IF
    ." UNEXPECTED STACK DEPTH CHANGE! (" DEPTH . ." instead of " depth-stack@ . ." )"
    TRUE
  ELSE FALSE THEN ;

: debug-init
  utime timer 2! ;

DEFER debug-bye
:noname
  n-forms @ . ."  wordforms generated in " utime timer 2@ D- D. ." μs." cr
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
  debug-mode? IF  ]] depth-stack-push                [[  THEN ; IMMEDIATE

: \stack-check  ( -- )
  debug-mode? IF  ]] depth-stack-check IF WTF?? THEN [[  THEN ; IMMEDIATE

variable last-timer
CREATE timer-stack 256 CELLS ALLOT
variable timer-stack-depth
[UNDEFINED] timer-list [IF]
  variable timer-list
[THEN]
: timer: Create $0. , , here timer-list !@ ,
  DOES> profile(
    timer-stack-depth @ ?DUP-IF 1- CELLS timer-stack + @  +t THEN
    timer-stack timer-stack-depth @ CELLS + !
    1 timer-stack-depth +!
  )else( drop ) ;
: +record ( -- ) profile( -1 timer-stack-depth +!  timer-stack timer-stack-depth @ CELLS + @  +t ) ;

[undefined] SEE-THREADED [IF]
[undefined] see-voc [if]
: see-threaded see ;
[else]
ALSO see-voc
: see-threaded SEE-THREADED ;
PREVIOUS
[then]

