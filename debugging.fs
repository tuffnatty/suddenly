: .ss
  .\" <" depth 0 .r .\" > " depth 0 max maxdepth-.s @ min dup 0 
  ?DO    dup i - pick dup . count type
  LOOP
  drop ;

: bin.  ( u -- )
  BASE @  2 BASE !  SWAP .  BASE ! ;

VARIABLE n-forms
2VARIABLE timer

0 VALUE debug-mode?
CREATE depth-stack 256 CELLS ALLOT
0 VALUE depth-stack-depth
: depth-stack-push  ( -- )
  DEPTH depth-stack depth-stack-depth CELLS + !
  depth-stack-depth 1+ TO depth-stack-depth ;
: depth-stack-check ( -- )
  depth-stack-depth 1- TO depth-stack-depth
  DEPTH depth-stack depth-stack-depth CELLS + @ <> IF
    ABORT" UNEXPECTED STACK DEPTH CHANGE!"
  THEN ;

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

: \\." debug-mode? 1 > IF POSTPONE ." ELSE POSTPONE \ THEN ; IMMEDIATE
: .as  ( -- )
  \." AS:" slot-stack DUP @ HEX. CELL+ BEGIN DUP @ DUP HEX. WHILE CELL+ REPEAT DROP CR
  ;

: \stack-mark  ( -- )
  debug-mode? IF POSTPONE depth-stack-push THEN ; immediate

: \stack-check  ( -- )
  debug-mode? IF POSTPONE depth-stack-check THEN ; immediate

