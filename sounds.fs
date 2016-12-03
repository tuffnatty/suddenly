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
  POSTPONE BEGIN POSTPONE DUP POSTPONE @ POSTPONE ?DUP-IF
  1 CS-ROLL ; IMMEDIATE
: sound-next  ( ptr )
  POSTPONE CELL+ POSTPONE AGAIN POSTPONE THEN POSTPONE DROP ; IMMEDIATE

: sound-class;  { len size buf -- }
  size CELLS ALLOT  0 ,
  \ Create vowels# VARIABLE with class size
  len 1+ { len' }
  [CHAR] #  sound-buf-name len' +  C!
  sound-buf-name len' 1+ NEXTNAME CREATE
  size ,
  \ Create vowel? ( xc -- f ) word
  [CHAR] ?  sound-buf-name len + C!
  S" : " string-addr  sound-buf 2 MOVE
  sound-buf 2 + len' + ( ptr )
  size 0 ?DO
    S"  DUP " s+/
    buf I CELLS + @ S>D <<# #s #> s+/ #>>
    S"  = IF EXIT THEN" S+/
  LOOP
  S"  DROP 0 ;" s+/
  sound-buf - sound-buf SWAP EVALUATE
  ;


TABLE CONSTANT morphonemes-table

: morphoneme-choose-variant  ( xt n -- xc )
  >R >BODY R> 2 + CELLS + @ ;

: morphoneme  ( addr u rule "name" -- )
  GET-CURRENT morphonemes-table SET-CURRENT  >R
  CREATE , DUP , BOUNDS BEGIN 2DUP > WHILE XC@+ , REPEAT 2DROP R> SET-CURRENT
DOES>  ( cs buf -- xc )
  >R R@ @ EXECUTE R> SWAP morphoneme-choose-variant ;

: morphoneme-rule  ( xt -- rule )
  >BODY @ ;
\ : morphoneme-rule  ( xt -- rule )
\   POSTPONE >BODY  POSTPONE @ ; IMMEDIATE

: morphoneme-size  ( xt -- rule )
  POSTPONE >BODY  POSTPONE CELL+  POSTPONE @ ; IMMEDIATE

: morphoneme-width  ( addr u1 -- u2 )
  OVER XC@ DUP [CHAR] ( = IF  ( addr u1 xc )
    DROP 1 /STRING X-SIZE 2 +         ( u2 )
  ELSE >R 2DROP R> XC-SIZE THEN ;

: morphoneme-skip  ( addr u -- addr' u')
  2DUP morphoneme-width /STRING ;

: morphoneme-find  ( addr u -- xt true | false )
  2DUP morphoneme-width NIP morphonemes-table SEARCH-WORDLIST ;
