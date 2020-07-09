REQUIRE lists.fs
REQUIRE trie.fs


1 VALUE sound-class-mask
trie-new CONSTANT sound-trie
compact-tries-region region-here VALUE sound-compact-trie
trie-new CONSTANT orthographic-variants-initials-trie
trie-new CONSTANT orthographic-variants-trie
0 VALUE orthographic-variants-initials-compact-trie
0 VALUE orthographic-variants-compact-trie


: sounds-compile  ( -- )
  sound-trie                          trie-compact TO sound-compact-trie
  orthographic-variants-initials-trie trie-compact TO orthographic-variants-initials-compact-trie
  orthographic-variants-trie          trie-compact TO orthographic-variants-compact-trie
  \." orthographic-variants-initials-trie: " orthographic-variants-initials-trie .trie
  \." orthographic-variants-initials-compact-trie: " orthographic-variants-initials-compact-trie .twolevel-trie
  \." orthographic-variants-compact-trie: "          orthographic-variants-compact-trie          .twolevel-trie
  sound-trie trie-forget ;

: sound-class  ( "name" -- sound-sys )
  PARSE-NAME NEXTNAME
  TABLE  DUP CONSTANT  GET-CURRENT SWAP SET-CURRENT  LATEST NAME>STRING ;

: sound  ( "letter" -- )
  PARSE-NAME 2DUP sound-class-mask -ROT sound-trie trie-put-mask OVER XC@ -ROT NEXTNAME VALUE ;

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


REQUIRE triere.fs  \ needs sound-each-str to be defined above

: orthographic-variants-initials  ( "pattern" -- )
  BL PARSE  orthographic-variants-initials-trie twolevel-trie-put ;

: orthographic-variants  ( "pattern" -- )
  BL PARSE 2bi[  orthographic-variants-trie twolevel-trie-put
             ][  orthographic-variants-initials-trie twolevel-trie-put ]; ;

DEFER trie-get-with-orthographic-variants-each
:noname ( addr u trie xt -- )  ( xt: ... trie -- ... )
  { trie xt }  ( addr u )
    \stack-mark
  \\." trie-get-with-orthographic-variants " 2DUP TYPE trie HEX. xt HEX. CR
  TRUE { initial? }
  BEGIN ?DUP WHILE
    \\." at " 2DUP TYPE CR
    initial? IF
      orthographic-variants-initials-compact-trie
    ELSE orthographic-variants-compact-trie THEN  { variants-trie-trie }
    FALSE TO initial?
    \\." variants-trie-trie" variants-trie-trie .compact-trie
    2DUP variants-trie-trie compact-trie-find-prefix-full OVER IF  { variants-compact-trie prefix-length }
      \\." found " variants-compact-trie .compact-trie prefix-length . ." xt:" xt hex. CR
      prefix-length /STRING  trie  xt  [: { D: rest trie xt D: prefix -- D: rest trie xt}  ( )
        \\." HERE:" rest type trie hex. xt hex. prefix type cr
        prefix trie trie-get  { trie' }
        rest trie' xt trie-get-with-orthographic-variants-each
        rest trie xt
      ;] variants-compact-trie compact-trie-each-prefix  ( D: rest trie xt )
      2DROP 2DROP EXIT
    ELSE
      \\." not found" CR
      2DROP  ( addr u )
      OVER 1 trie trie-get TO trie
      1 /STRING  ( addr' u' )
    THEN
  REPEAT DROP
  \\." done" CR
  trie xt \stack-check EXECUTE
; IS trie-get-with-orthographic-variants-each

: trie-put-with-orthographic-variants  ( data addr u trie -- )
  \\." trie-put-with-orthographic-variants " >R >R >R DUP HEX. R> R> 2DUP TYPE R> DUP HEX. .s CR
  [: ( data trie -- data ) trie-data OVER SWAP ! ;]  trie-get-with-orthographic-variants-each  ( data )
  DROP
  ;

0 VALUE morphonemes-trie

: morphonemes-init  ( -- )
  trie-new TO morphonemes-trie
  \ \." ========== ALLOCATED " morphonemes-trie HEX. CR
  ;

: morphoneme-choose-variant  ( ptr n -- xc )
  2 + CELLS + @ ;

: morphoneme  ( addr u rule "name" -- )
  \ \." *** morphoneme : " morphonemes-trie .trie
  morphonemes-trie 0= IF morphonemes-init THEN
  HERE  PARSE-NAME  morphonemes-trie  trie-put
  , DUP , BOUNDS BEGIN 2DUP > WHILE XC@+ , REPEAT 2DROP
  \ \." *** morphoneme exit : " morphonemes-trie .trie
  ;

list%
  CELL% FIELD msub-source
  CELL% FIELD msub-rule
  CELL% FIELD msub-class
  CELL% FIELD msub-target
END-STRUCT msublist%  \ Morphoneme substitutions list

0 VALUE msubs

: morphoneme-rule  ( ptr -- rule )
  ]] @ [[ ; IMMEDIATE

: morphoneme-size  ( ptr -- rule )
  ]] CELL+ @ [[ ; IMMEDIATE

: morphoneme-width  ( addr u1 -- u2 )
  \ \." morphoneme-width  ( " 2dup type OVER XC@ DUP . XEMIT
  OVER XC@ DUP [CHAR] ( = IF  ( addr u1 xc )
    DROP 1 /STRING X-SIZE 2 +         ( u2 )
  ELSE >R 2DROP R> XC-SIZE THEN
  \ \."  -- " dup . ." )" cr
  ;

0 VALUE morphonemes-compact-trie

: morphonemes-compile  ( -- )
  \ \." ***** morphonemes-compile: " morphonemes-trie .trie
  morphonemes-trie trie-compact TO morphonemes-compact-trie
  morphonemes-trie trie-forget ;

: morphoneme-skip  ( addr u -- addr' u')
  2DUP morphoneme-width /STRING ;

: morphoneme-find  ( addr u -- ptr true | false )
  2DUP morphoneme-width NIP morphonemes-compact-trie compact-trie-find DUP IF TRUE THEN ;

: morphoneme-get-internal  ( addr u -- xt )
  2DUP morphoneme-find 0= IF
    TYPE ABORT" no such morphoneme"
  ELSE
    NIP NIP
  THEN ;

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
