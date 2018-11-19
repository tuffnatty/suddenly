CREATE sound-buf 4000 ALLOT
sound-buf 2 + CONSTANT sound-buf-name

: s+/  ( ptr addr u -- ptr' )
  >R OVER R@ MOVE R> + ;

: sound-class  ( "name" -- len size addr )
  sound-buf-name PARSE-NAME DUP >R s+/             ( ptr' )
  [CHAR] s OVER C! 1+
  sound-buf-name - sound-buf-name SWAP NEXTNAME CREATE  ( )
  R> 0 HERE ;

: sound  ( size buf "letter" -- size' buf )
  OVER CELLS OVER +       ( size buf ptr )
  PARSE-NAME DROP XC@ SWAP !  ( size buf )
  SWAP 1+ SWAP ;

: sound-each  ( start )
  ]] BEGIN DUP @ ?DUP-IF [[
  1 CS-ROLL ; IMMEDIATE
: sound-next  ( ptr )
  ]] CELL+ AGAIN THEN DROP [[ ; IMMEDIATE

: sound-class;  { len size buf -- }
  size CELLS ALLOT  0 ,
  \ Create vowels# VARIABLE with class size
  [CHAR] #  sound-buf-name len 1+ +  C!
  sound-buf-name len 1+ 1+ NEXTNAME CREATE
  size ,
  \ Create vowel? ( xc -- f ) word
  [CHAR] ?  sound-buf-name len + C!
  S" : " string-addr  sound-buf 2 MOVE
  sound-buf 3 + len + ( ptr )
  size 0 ?DO
    S"  DUP " s+/
    buf I CELLS + @ S>D <# #s #> s+/
    S"  = IF DROP TRUE EXIT THEN" S+/
  LOOP
  S"  DROP 0 ;" s+/
  sound-buf - sound-buf SWAP EVALUATE
  ;


TABLE CONSTANT morphonemes-table

: morphoneme-choose-variant  ( xt n -- xc )
  SWAP >BODY SWAP 2 + CELLS + @ ;

: morphoneme  ( addr u rule "name" -- )
  GET-CURRENT morphonemes-table SET-CURRENT  >R
  CREATE , DUP , BOUNDS BEGIN 2DUP > WHILE XC@+ , REPEAT 2DROP R> SET-CURRENT
DOES>  ( cs buf -- xc )
  >R R@ @ EXECUTE R> SWAP morphoneme-choose-variant ;

list%
  CELL% FIELD msub-source
  CELL% FIELD msub-rule
  CELL% FIELD msub-class
  CELL% FIELD msub-target
END-STRUCT msublist%  \ Morphoneme substitutions list

0 VALUE msubs

: morphoneme-rule  ( xt -- rule )
  >BODY @ ;
\ : morphoneme-rule  ( xt -- rule )
\   POSTPONE >BODY  POSTPONE @ ; IMMEDIATE

: morphoneme-size  ( xt -- rule )
  ]] >BODY CELL+ @ [[ ; IMMEDIATE

: morphoneme-width  ( addr u1 -- u2 )
  OVER XC@ DUP [CHAR] ( = IF  ( addr u1 xc )
    DROP 1 /STRING X-SIZE 2 +         ( u2 )
  ELSE >R 2DROP R> XC-SIZE THEN ;

: morphoneme-skip  ( addr u -- addr' u')
  2DUP morphoneme-width /STRING ;

: morphoneme-find  ( addr u -- xt true | false )
  2DUP morphoneme-width NIP morphonemes-table SEARCH-WORDLIST ;

: morphoneme-get-internal  ( addr u -- xt )
  morphoneme-find 0= IF ABORT" no such morphoneme" THEN ;

: morphoneme-substitution  ( D: source  rule  cls  D: target -- )
  msublist% %ALLOT >R
  morphoneme-get-internal  R@ msub-target !  ( D: source rule cls )
                           R@ msub-class !   ( D: source rule )
                           R@ msub-rule !    ( D: source )
  morphoneme-get-internal  R@ msub-source !  ( )
  msubs                    R@ list-next !
  R> TO msubs ;

: rule-apply  ( addr u xt -- table-index )
  EXECUTE NIP NIP ;

: morphoneme-get  { D: morphoneme context-cstr -- xt }
  morphoneme morphoneme-get-internal { xt }
  msubs BEGIN ?DUP WHILE  { this-msub }
    this-msub msub-source @  xt  = IF
      context-cstr cstr-get  this-msub msub-rule @ rule-apply  this-msub msub-class @  = IF
        this-msub msub-target @ EXIT
      THEN
    THEN
    this-msub list-next @
  REPEAT
  xt ;
