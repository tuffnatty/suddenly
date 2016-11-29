: .ss
  .\" <" depth 0 .r .\" > " depth 0 max maxdepth-.s @ min dup 0 
  ?DO    dup i - pick dup . count type
  LOOP
  drop ;

VARIABLE n-forms
2VARIABLE timer

0 VALUE debug-mode?

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
  \." AS:" slot-stack DUP @ . CELL+ BEGIN DUP @ DUP . WHILE CELL+ REPEAT DROP CR
  ;
