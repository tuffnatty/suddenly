REQUIRE lists.fs
REQUIRE trie.fs


1 VALUE sound-class-mask
trie-new CONSTANT sound-trie
compact-tries-region region-here VALUE sound-compact-trie

: sounds-compile  ( -- )
  sound-trie trie-compact TO sound-compact-trie
  sound-trie trie-forget ;

: sound-class  ( "name" -- sound-sys )
  PARSE-NAME NEXTNAME
  TABLE  DUP CONSTANT  GET-CURRENT SWAP SET-CURRENT  LATEST NAME>STRING ;

: sound  ( "letter" -- )
  PARSE-NAME 2DUP sound-class-mask -ROT sound-trie trie-put OVER XC@ -ROT NEXTNAME VALUE ;

: sound-each  ( wid -- xc )
  ]] WORDLIST-ID @ BEGIN ?DUP-IF [[
  1 CS-ROLL
  ]] >R R@ NAME>INT EXECUTE [[
  ; IMMEDIATE COMPILE-ONLY

: sound-each-str  ( wid -- addr u )
  ]] WORDLIST-ID @ BEGIN ?DUP-IF [[
  1 CS-ROLL
  ]] >R R@ NAME>STRING [[
  ; IMMEDIATE COMPILE-ONLY

: sound-next  ( -- )
  ]] R> >LINK @ AGAIN THEN [[
  ; IMMEDIATE COMPILE-ONLY

: sound-class;  ( sound-sys -- )
  GET-CURRENT { wid }
  TUCK PAD SWAP MOVE  ( wid len )

  [CHAR] ?  OVER PAD +  C!

  1+ PAD SWAP NEXTNAME  SET-CURRENT  :  ( )
    wid WORDLIST-ID @  BEGIN DUP WHILE  ( nt )
      ]]  DUP  [[ DUP NAME>INT EXECUTE ]]L  = ?EXIT [[
      >LINK @
    REPEAT DROP
    ]] DROP FALSE ; [[

  [CHAR] @  PAD C!
  LATEST NAME>STRING PAD 1+ SWAP MOVE
  PAD  LATEST NAME>STRING string-length 1+ NEXTNAME  :
    \ ]] DUP @xc-size [[ wid ]]L FIND-NAME-IN ; [[
    ]] DUP @xc-size sound-compact-trie compact-trie-find [[ sound-class-mask ]]L AND ; [[

  sound-class-mask 2* TO sound-class-mask
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
