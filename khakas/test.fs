#! /usr/local/bin/gforth -m22M

S" ~+"  FPATH  ALSO-PATH
REQUIRE test/ttester.fs

S" khakas"  FPATH  ALSO-PATH
REQUIRE debugging.fs
\ 1 TO debug-mode?
\ 2 TO debug-mode?
REQUIRE parser.fs

CREATE wordform-buffer 0 , 255 ALLOT
0 VALUE expected-str
0 VALUE expected-len
0 VALUE expected-found
FALSE VALUE expect-headword?
0 VALUE n_failures
0 VALUE n_tests
: check-result-headword  ( stem -- )
  stem-dict @ dict-headword COUNT expected-str expected-len STR= IF
    expected-found 1+ TO expected-found
  THEN
  ;
: check-result  { stem -- }
  expected-str expected-len { D: pattern }
  \ ." check-result:" stem .stem-single ." gloss:" guessed-stem TYPE  formform .bstr  ."  expected pattern:" pattern TYPE CR
  guessed-stem { dict-str dict-len }
  dict-len expected-len <= IF
    \ ." len is enough" CR
    dict-str dict-len expected-str dict-len STR= IF
      \ ." stem is equal" CR
      FALSE { plusfound }
      pattern  dict-len right-slice { D: pattern-rest }
      formform cstr-get BEGIN DUP 0> WHILE  ( addr u )
        \ ." checking " 2DUP TYPE ."  against " pattern-rest TYPE CR
        OVER XC@ CASE
          [CHAR] - OF
            \ ." minus" CR
            plusfound 0= IF
              pattern-rest string-length 0 = IF
                \ ." pattern-rest is empty" CR
                TRUE TO plusfound
              ELSE pattern-rest first-sound  [CHAR] +  = IF
                TRUE TO plusfound
                pattern-rest 1 right-slice TO pattern-rest
                \ ." pattern-rest is " pattern-rest TYPE CR
              ELSE
                \ ." failed" ~~ CR
                2DROP EXIT                          ( )
              THEN THEN
            THEN
          ENDOF
          pattern-rest string-length 0= IF DROP 2DROP EXIT THEN  ( )
          pattern-rest first-sound OF
            FALSE TO plusfound
            pattern-rest +X/STRING TO pattern-rest
          ENDOF
          DROP 2DROP EXIT                           ( )
        ENDCASE
        +X/STRING                          ( addr' u' )
      REPEAT
      2DROP
      pattern-rest string-length 0= IF
        expected-found 1+ TO expected-found
      THEN
    THEN
  THEN
  \ ." match count is " expected-found . CR
  ;
: headword?  ( -- )
  TRUE TO expect-headword? ;
: parse-test  { D: expected D: wordform }
  n_tests 1+ TO n_tests
  wordform wordform-buffer s-to-cs
  expected TO expected-len TO expected-str
  0 TO expected-found
  expect-headword? IF ['] check-result-headword ELSE ['] check-result THEN IS yield-stem
  ['] noop IS debug-bye
  wordform-buffer parse-khak expected-found 0>
  FALSE TO expect-headword? ;

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

2VARIABLE timer  utime timer 2!
REQUIRE khakas/gentest.fs
utime timer 2@ D- D. ." µs" CR
\ .TIMES CR

: check-failures
  n_failures . ." / " n_tests . ." tests FAILED ( " n_failures 100 * n_tests / . ." % )" CR
  n_failures ABORT"  test failures found!" ;
check-failures
BYE
