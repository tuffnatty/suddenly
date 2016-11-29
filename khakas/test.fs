#! /usr/bin/gforth-fast -m22M

S" ~+"  FPATH  ALSO-PATH
REQUIRE test/ttester.fs

REQUIRE khakas/phonetics.fs
REQUIRE phonetics-common.fs

T{ CHAR ң class-nvu -> cl-nasal }T

S" khakas"  FPATH  ALSO-PATH
REQUIRE ~+/debug.fs
REQUIRE parser.fs
CREATE wordform-buffer 0 , 255 ALLOT
0 VALUE expected-str
0 VALUE expected-len
0 VALUE expected-found
: check-result  { stem -- }
  \ ~~ stem .stem-single CR formform .bstr CR  paradigm-stem 2@ TYPE CR
  \ ." <found" expected-found . CR
  paradigm-stem 2@ { dict-str dict-len }
  dict-len expected-len <= IF
    \ ~~ ." len is enough" CR
    dict-str dict-len expected-str dict-len STR= IF
      \ ~~ ." stem is equal" CR
      FALSE { plusfound }
      expected-str expected-len  dict-len /STRING { D: str }
      formform cstr-get BEGIN DUP 0> WHILE  ( addr u )
        \ ~~ ." checking " 2DUP TYPE ."  against " str TYPE CR
        OVER XC@ CASE
          [CHAR] - OF
            \ ." minus" CR
            plusfound 0= IF
              str NIP 0 = IF
                \ ." str is empty" CR
                TRUE TO plusfound
              ELSE str DROP XC@  [CHAR] +  = IF
                TRUE TO plusfound
                str 1 /STRING TO str
                \ ." str is " str TYPE CR
              ELSE
                \ ." failed" ~~ CR
                2DROP EXIT
              THEN THEN
            THEN
          ENDOF
          str NIP 0= IF DROP 2DROP EXIT THEN
          str DROP XC@ OF
            FALSE TO plusfound
            str  str DROP XC@ XC-SIZE  /STRING TO str
          ENDOF
          DROP 2DROP EXIT
        ENDCASE
        OVER XC@ XC-SIZE /STRING           ( addr' u' )
      REPEAT
      2DROP
      expected-found 1+ TO expected-found
    THEN
  THEN
  ;
: parse-test  { D: expected D: wordform }
  wordform wordform-buffer 1+ SWAP MOVE
  wordform NIP wordform-buffer C!
  expected TO expected-len TO expected-str
  0 TO expected-found
  ['] check-result IS yield-stem
  ['] noop IS debug-bye
  wordform-buffer parse-khak expected-found 0> ;

REQUIRE khakas/gentest.fs
BYE
