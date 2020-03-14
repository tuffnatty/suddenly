#! /usr/local/bin/gforth -m22M

INCLUDE ../util.fs

utime 2VALUE test-timer

: language-path  ( -- addr u )
  S" khakas" ;

INCLUDE ../compat/ttester.fs
INCLUDE ../debugging.fs
\ 1 TO debug-mode?
\ 2 TO debug-mode?

TIMER: +init
TIMER: +rest
TIMER: +compile
+init +record +compile

REQUIRE ../parser.fs

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
  \." check-result:" stem .stem-single ." gloss:" guessed-stem TYPE  formform .dstack  ."  expected pattern:" pattern TYPE CR
  guessed-stem { dict-str dict-len }
  dict-len expected-len <= IF
    \." len is enough" CR
    pattern  [CHAR] + SCAN  string-addr expected-str - { pattern-stem-len }
    dict-str dict-len expected-str pattern-stem-len STR= IF
      \." stem is equal" CR
      FALSE { plusfound }
      pattern pattern-stem-len right-slice { D: pattern-rest }
      formform >O dstack-depth O> 1+ { depth }
      \." checking " formform .dstack ."  against " pattern-rest TYPE ." :" CR
      0 BEGIN DUP n-slots <= WHILE ( n )
        DUP IF DUP formform formstack-slot ELSE $0. THEN ( n addr u )
        \." checking " 2DUP TYPE ."  against " pattern-rest TYPE CR
        ?DUP-0=-IF
            \." minus" CR
            DROP                          ( n )
            plusfound 0= IF
              pattern-rest string-length 0 = IF
                \." pattern-rest is empty" CR
                TRUE TO plusfound
              ELSE pattern-rest first-sound  [CHAR] +  = IF
                TRUE TO plusfound
                pattern-rest 1 right-slice TO pattern-rest
                \." pattern-rest is " pattern-rest TYPE CR
              ELSE
                \." failed" ~~ CR
                DROP EXIT
              THEN THEN
            THEN
        ELSE
          pattern-rest string-length 0= IF 2DROP DROP EXIT THEN
          pattern-rest  [CHAR] +  SCAN  { D: pattern-next }
          pattern-next string-addr  pattern-rest string-addr  -  { chunk-len }
          pattern-rest chunk-len left-slice STR= IF  ( n )
            pattern-next string-length 0> TO plusfound
            pattern-next plusfound IF 1 /STRING THEN TO pattern-rest
          ELSE DROP EXIT THEN
        THEN
        1+
      REPEAT
      DROP
      pattern-rest string-length 0= IF
        expected-found 1+ TO expected-found
      THEN
    THEN
  THEN
  \." match count is " expected-found . CR
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

:+ test-error
   ERROR1
   n_failures 1+ TO n_failures
;

' test-error ERROR-XT !

: check-failures
  n_failures . ." / " n_tests . ." tests FAILED ( " n_failures 100 * n_tests / . ." % )" CR
  n_failures ABORT"  test failures found!" ;

REQUIRE khakas/phonetics.fs
REQUIRE phonetics-common.fs

FALSE [IF]
S" ар" rule-cv-vu-fb rule-apply . 0
S" ер" rule-cv-vu-fb rule-apply . 1
S" а" rule-cv-vu-fb rule-apply . 2
S" е" rule-cv-vu-fb rule-apply . 3
S" ах" rule-cv-vu-fb rule-apply . 4
S" ех" rule-cv-vu-fb rule-apply . 5
T{ CHAR ң class-nvu -> cl-nasal }T
T{ S" пас+са+ңар" S" пассар" parse-test -> TRUE }T
T{ S" пас+ты+ңар" S" пастар" parse-test -> TRUE }T
[THEN]

\." tries-region: " tries-region .region
tries-region region-dispose
." compact-tries-region: " compact-tries-region .region

+record +rest
utime test-timer D- ." compiling: " D. ." µs" CR  utime TO test-timer
REQUIRE khakas/gentest.fs
utime test-timer D- ." testing: " D. ." µs" CR
PROFILE( .TIMES CR )

check-failures
BYE
