#! /usr/local/bin/gforth -m22M

S" ~+"  FPATH  ALSO-PATH
REQUIRE test/ttester.fs

S" khakas"  FPATH  ALSO-PATH
REQUIRE debugging.fs
\ 2 TO debug-mode?
REQUIRE parser.fs
CREATE wordform-buffer 0 , 255 ALLOT
0 VALUE expected-str
0 VALUE expected-len
0 VALUE expected-found
0 VALUE n_failures
0 VALUE n_tests
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
  n_tests 1+ TO n_tests
  wordform wordform-buffer 1+ SWAP MOVE
  wordform NIP wordform-buffer C!
  expected TO expected-len TO expected-str
  0 TO expected-found
  ['] check-result IS yield-stem
  ['] noop IS debug-bye
  wordform-buffer parse-khak expected-found 0> ;

: test-error
   ERROR1
   n_failures 1+ TO n_failures
;

' test-error ERROR-XT !

REQUIRE khakas/phonetics.fs
REQUIRE phonetics-common.fs

FALSE [IF]
S" ар" rule-cv-vu-fb EXECUTE . 0
S" ер" rule-cv-vu-fb EXECUTE . 1
S" а" rule-cv-vu-fb EXECUTE . 2
S" е" rule-cv-vu-fb EXECUTE . 3 
S" ах" rule-cv-vu-fb EXECUTE . 4 
S" ех" rule-cv-vu-fb EXECUTE . 5
T{ CHAR ң class-nvu -> cl-nasal }T
T{ S" пас+са+ңар" S" пассар" parse-test -> TRUE }T
T{ S" пас+ты+ңар" S" пастар" parse-test -> TRUE }T
[THEN]
\ T{ S" ті+ген" S" теен" parse-test -> TRUE }T

utime 2VALUE timer
REQUIRE khakas/gentest.fs
utime timer D- D. ." µs" CR
.TIMES CR

: check-failures
  n_failures . ." / " n_tests . ." tests FAILED ( " n_failures 100 * n_tests / . ." % )" CR
  n_failures ABORT"  test failures found!" ;
check-failures
BYE
