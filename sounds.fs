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

: sound-class;  ( len size buf -- )
  OVER CELLS ALLOT
  ROT 1+                              ( size buf len' )
  DUP sound-buf-name + [CHAR] # SWAP C!
  >R sound-buf-name R@ 1+ NEXTNAME CREATE  ( size buf )
  OVER , R> 1-                        ( size buf len' )
  DUP sound-buf-name + [CHAR] ? SWAP C!
  S" : " DROP sound-buf 2 MOVE
  sound-buf 2 + + 1+                   ( size buf ptr )
  ROT 0 ?DO                                 ( buf ptr )
    S"  DUP " s+/                          ( buf ptr' )
    OVER I CELLS + @ S>D <<# #s #> s+/ #>>
    S"  = IF EXIT THEN" s+/
  LOOP
  S"  DROP 0 ;" s+/
  sound-buf - sound-buf SWAP EVALUATE DROP ;
