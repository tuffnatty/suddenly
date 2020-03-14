S" ~+"  FPATH  ALSO-PATH

require strings.fs
require khakas/phonetics.fs

GET-CURRENT  ( wid )
VOCABULARY minire ALSO minire DEFINITIONS

FALSE CONSTANT wide-charclass

\ : ]] [CHAR] [ PARSE POSTPONE SLITERAL POSTPONE TYPE POSTPONE CR [CHAR] [ PARSE 2DROP ; IMMEDIATE
\ : ]]L POSTPONE LITERAL POSTPONE . POSTPONE SLITERAL POSTPONE TYPE [CHAR] [ PARSE POSTPONE SLITERAL POSTPONE TYPE POSTPONE CR [CHAR] [ PARSE 2DROP ; IMMEDIATE
0 CONSTANT state-0
1 CONSTANT state-class
FALSE VALUE state-needif
0 VALUE level

wide-charclass [IF]
0 VALUE charclass-addr
0 VALUE charclass-len
0 VALUE charclass-start
CREATE vowels-str vowels# @ cyrs ALLOT
0 VALUE vowels-str-len
: populate-vowels
  vowels-str vowels# @ cyrs 2DUP { D: buf D: ptr }
  vowels sound-each
    ptr XC!+? 0= ABORT" minire class buffer overflow" TO ptr
  sound-next buf string-length TO vowels-str-len
  ;
populate-vowels
: startclass  ( D: rest -- )
  string-addr TO charclass-addr ;
: endclass ( D: rest -- f )
  string-addr 1-  charclass-addr -  TO charclass-len
  -1 0 { min max }
  charclass-addr charclass-len BEGIN DUP WHILE +x/string@  ( addr' len' xc )
    DUP min U< IF DUP TO min THEN
    DUP max U> IF TO max ELSE DROP THEN
  REPEAT 2DROP
  max 1+ min - { bits } bits ALLOCATE IF ABORT" Can't allocate charclass" THEN { buf }
  buf bits ERASE
  min buf !
  charclass-addr charclass-len BEGIN DUP WHILE +x/string@  ( addr' len' xc )
    min - buf +  1 SWAP C!                                 ( addr' len' )
  REPEAT 2DROP
  ]] DUP [[  min  ]]L [[  max 1+  ]]L WITHIN IF [[  buf min - ]]L + C@ ELSE DROP FALSE THEN [[
  ;
[THEN]
: checkif  ( -- if-sys )
  state-needif IF  ]] IF [[  level 1+ TO level  THEN ;

( wid ) SET-CURRENT  \ public words follow

: ~/  ( "regex/ -- f )
  FALSE TO state-needif
  state-0 { state }
  [CHAR] / PARSE  BEGIN DUP WHILE  +x/string@  { D: rest xc }
    state CASE
      state-0 OF
        xc CASE
          [CHAR] [ OF  checkif ]] DUP IF +x/string@ [[
[ wide-charclass [IF] ]
                       rest startclass
[ [ELSE] ]
                       ]] CASE [[
[ [THEN] ]
                       level 1+ TO level  state-class TO state  ENDOF
          [CHAR] $ OF  checkif ]] DUP 0= [[                     ENDOF
          ( xc )  DROP checkif ]] +x/string@ [[ xc ]]L = [[     xc
        ENDCASE
        TRUE TO state-needif
      ENDOF
      state-class OF
        xc CASE
[ wide-charclass [IF] ]
          [CHAR] C OF  consonants-str consonants-str-len startclass  consonants-str consonants-str-len + 1+ 0 endclass  rest 1 /STRING TO rest  state-0 TO state  ENDOF
          [CHAR] V OF  vowels-str vowels-str-len startclass  vowels-str vowels-str-len + 1+ 0 endclass  rest 1 /STRING TO rest  state-0 TO state  ENDOF
          [CHAR] ] OF  rest endclass  state-0 TO state  ENDOF
[ [ELSE] ]
          [CHAR] C OF  consonant sound-each ]]L OF TRUE ENDOF [[    sound-next     ENDOF
          [CHAR] V OF  vowel sound-each ]]L OF TRUE ENDOF [[    sound-next     ENDOF
          [CHAR] ] OF  ]] FALSE SWAP ENDCASE [[  state-0 TO state  ENDOF
          ( xc )       ]]L OF TRUE ENDOF [[                        xc
[ [THEN] ]
        ENDCASE
      ENDOF
    ENDCASE
  rest REPEAT 2DROP
  BEGIN level WHILE  ]] ELSE FALSE THEN [[  level 1- TO level  REPEAT
  ]] >R 2DROP R> [[
  ; IMMEDIATE


\ Trie-based pattern matching:

DEFER (t~/)

: string-parse  ( addr u c -- addr+u' u-u' addr u' )
  { D: orig  c }                            ( )
  orig BEGIN DUP WHILE             ( addr' u' )
    OVER C@  c  <>  WHILE
      1 /STRING
  REPEAT THEN
  OVER  orig string-addr -  { len }
  DUP IF  1 /STRING  THEN  \ skip delimiter
  orig string-addr len ;

: (t~/)-doclass  { D: rest  wid  trie -- }
  \ Build subtrie for the first sound in wid.
  \ Refer to that subtrie for every sound in wid.
  \\." (t~/)-doclass " rest type cr
  0 { child }
  wid sound-each-str  ( c-addr c-u )
    child IF
      trie trie-get-ref-ptr  child  SWAP !
    ELSE
      trie trie-get TO child      ( )
      rest child (t~/)
    THEN
  sound-next
  \\." doclass-done" cr
  ;

: parse-class  ( addr u -- wid )
  [CHAR] } string-parse   FIND-NAME NAME>INT EXECUTE ;

:noname  ( addr u trie -- )
  \\." (t~/) " >r 2dup type ." ," r> dup .trie ." ." cr
  { trie }  ( addr u )
  DUP IF
    OVER C@ CASE
      [CHAR] { OF   1 /STRING   parse-class trie (t~/)-doclass   ENDOF
      [CHAR] C OF   1 /STRING     consonant trie (t~/)-doclass   ENDOF
      [CHAR] V OF   1 /STRING         vowel trie (t~/)-doclass   ENDOF
      [CHAR] R OF   1 /STRING      sonorant trie (t~/)-doclass   ENDOF
      ( c )  >R                               ( addr u  R: c )
        OVER 1 trie trie-get   >R 1 /STRING R>   RECURSE   ( )
        R>                                               ( c )
    ENDCASE
  ELSE
    2DROP
    TRUE trie trie-data !
  THEN
  ; IS (t~/)


: t~/  ( addr u "regex" -- f )
  \\." t~/ start: " RP@ . cr
  \stack-mark
  trie-new { trie }
  BL PARSE          ( addr u )
  BEGIN DUP WHILE
    [CHAR] | string-parse trie (t~/)
  REPEAT 2DROP             ( )
  trie trie-compact ]]L compact-trie-find-prefix [[
  trie trie-forget
  \stack-check
  \\." t~/ end " RP@ . cr
  ; IMMEDIATE COMPILE-ONLY


PREVIOUS

FALSE [IF]
: test-t~/
  T{ S" а" t~/ V -> TRUE }T
  T{ S" б" t~/ V -> FALSE }T
  T{ S" 0̸" t~/ гV -> FALSE }T
  T{ S" 0̸" t~/ гV|ғV -> FALSE }T
  ." OK" cr
  ;
[THEN]
