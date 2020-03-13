REQUIRE lists.fs
REQUIRE p-o-s.fs
REQUIRE strings.fs
REQUIRE util.fs

: language-require  ( "name" -- )
  language-path PAD SWAP MOVE
  language-path NIP PAD +  ( ptr )
  [CHAR] / OVER C! 1+  ( ptr' )
  BL PARSE  ROT  2DUP + >R  SWAP MOVE  PAD R> PAD - REQUIRED ;

32 CELLS  CONSTANT max-headword
32 CELLS  CONSTANT max-semgloss

STRUCT
  CELL%      FIELD dict-id
  CELL%      FIELD dict-p-o-s
  CELL% 16 * FIELD dict-headword
  CELL%      FIELD dict-headnum
  CELL% 16 * FIELD dict-semgloss
  CELL%      FIELD dict-stems        \ pointer to strlist%
  CELL%      FIELD dict-flags
END-STRUCT dict%

DEFER .dictflags  ( dictflags -- )

: .dict  ( dict -- )
  DUP dict-p-o-s @ .p-o-s BL EMIT
  DUP dict-headword COUNT TYPE BL EMIT
  DUP dict-headnum @ ?DUP-IF . BL EMIT THEN
  DUP dict-flags @ ?DUP-IF .dictflags BL EMIT THEN
  DUP dict-semgloss $201B XEMIT COUNT TYPE $2019 XEMIT BL EMIT
  DUP dict-stems @ .strlist BL EMIT
  dict-id @ . ;

list%
  CELL%      FIELD stem-dict
END-STRUCT stem%

: .stem-single ( stem -- )
  stem-dict @ .dict CR ;

: .stem  ( stem -- )
  ['] .stem-single list-map ;

: stem-p-o-s ( stem -- p-o-s )
  stem-dict @  dict-p-o-s  @ ;

TABLE CONSTANT stem-table
TABLE CONSTANT fuzzy-stem-table
VARIABLE dictionary-ptr

: stem-find-in-table  ( key len table -- stem|0 )
  SEARCH-WORDLIST IF EXECUTE ELSE 0 THEN ;
: fuzzy-stem-find  ( key len -- stem|0 )
  fuzzy-stem-table stem-find-in-table ;
: stem-find  ( key len -- stem|0 )
  stem-table stem-find-in-table ;

: find-headwords  { D: s -- dicts|0 }
  0 >R                                              ( R: dicts )
  s fuzzy-stem-find ?DUP-IF                             ( stem )
    BEGIN
      DUP stem-dict @                              ( stem dict )
      DUP dict-headword COUNT  s STR= IF
        R> SWAP ptrlist-prepend >R           ( stem  R: dicts' )
      THEN
    list-next @ DUP WHILE REPEAT DROP
  THEN R> ;

: stem-create-for-dict  ( dict -- stem )
  stem% %ALLOT >R      ( dict  R: stem )
  R@ stem-dict !             ( R: stem )
  0 R@ list-next !
  R> ;

: stem-get-for-dict  ( dict stem -- stem )
  >R                                 ( dict  R: stem )
  BEGIN DUP R@ stem-dict @ <> WHILE
    R@ list-next @ ?DUP-IF RDROP >R  ( dict  R: stem' )
    ELSE
      DUP stem-create-for-dict          ( dict stem' )
      DUP R> list-next ! >R         ( dict  R: stem' )
    THEN
  REPEAT DROP R> ;

: stem-table-add-key  { D: key dict table -- }
  key table stem-find-in-table ?DUP-IF     ( stem )
    dict SWAP stem-get-for-dict DROP            ( )
  ELSE
    \ ." add key " 2DUP TYPE cr
    GET-CURRENT table SET-CURRENT      ( wordlist )
    key NEXTNAME
    dict stem-create-for-dict CONSTANT
    SET-CURRENT                                 ( )
  THEN ;

: (dict-check-headword)  ( head len -- head len )
  DUP max-headword > IF TYPE ABORT"  headword too long" THEN ;

: (dict-check-semgloss)  ( head len -- head len )
  DUP max-semgloss > IF ( TYPE ABORT"  semgloss too long" ) DROP max-semgloss 1- THEN ;

: dict-add  ( "word" id pos -- )
  unused 1000000 <= if bl parse 2drop 2drop ." out of dictionary space" cr exit then
  \ Allocate article
  dict% %ALLOT { id pos dict }       ( "word" )
  pos  dict dict-p-o-s  !
  id   dict dict-id     !
  BL PARSE  (dict-check-headword)  dict dict-headword  s-to-cs  ( )
  0  dict dict-stems    !
  0  dict dict-headnum  !
  0  dict dict-flags    !

  \ Add stem for search
  dict dict-headword COUNT      ( head len )
  pos pos-v <> IF 2DUP dict stem-table stem-table-add-key THEN
  pos pos-n = IF str-trim-last-cyr str-trim-last-cyr THEN
  dict fuzzy-stem-table stem-table-add-key ;

TABLE CONSTANT dictionary-wordlist

language-require dictext.fs

utime 2CONSTANT dict-timer

GET-CURRENT dictionary-wordlist SET-CURRENT

: i  ( "word" -- ; -- pos-i )
  HERE dictionary-ptr !
  pos-i dict-add ;

: i1  ( "word" -- ; -- pos-i1 )
  HERE dictionary-ptr !
  pos-i1 dict-add ;

: n  ( "word" -- ; -- pos-n )
  HERE dictionary-ptr !
  pos-n dict-add ;

: v  ( "word" -- ; -- verb-stems-addr )
  HERE dictionary-ptr !
  pos-v dict-add ;

: stem  ( "word" -- )
  BL PARSE dictionary-ptr @  { D: stem dict }
  dict dict-stems @  stem strlist-prepend  dict dict-stems !
  stem dict stem-table stem-table-add-key
  stem str-trim-last-cyr str-trim-last-cyr  dict  fuzzy-stem-table stem-table-add-key
  stem dict stem-postprocess ;

: #  ( "number" -- )
  BL PARSE S>NUMBER? IF
    DROP dictionary-ptr @ dict-headnum !
  ELSE
    ABORT"  cannot parse headnum"
  THEN ;

: semgloss"  ( "text" -- )
  [CHAR] " PARSE  (dict-check-semgloss)  dictionary-ptr @ dict-semgloss s-to-cs ;

ALSO dictionary-wordlist CONTEXT !
language-require dict.fs
PREVIOUS

SET-CURRENT

utime dict-timer D- ." parsing dictionary: " D. ." us." CR
