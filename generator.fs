REQUIRE slot-stack.fs
REQUIRE debug.fs
REQUIRE dictionary.fs
REQUIRE formname.fs
REQUIRE grammar.fs
REQUIRE lists.fs
REQUIRE transforms.fs
REQUIRE warp.fs

: add-slot  ( warp n sstr -- warp )
  sstr-select ( nth-word ) ?DUP-IF  ( warp addr len )
    2 PICK >R DUP >R cs+ R>          ( len  R: warp )
    R@ warp-segs# @              ( len ofs  R: warp )
    R@ warp-seg-len + C! R>                  ( warp )
    1 OVER warp-segs# +!
  ELSE DROP THEN ;

:noname  ( warp -- warp top-warp )
  warp% %ALLOT   ( warp top-warp )
  0 OVER warp-cached-result C!
  2DUP copy-warp-full ;
IS (slot-prolog)

:noname  ( warp top-warp -- warp )
  [ warp% %SIZE NEGATE ]L ALLOT DROP ;
IS (slot-epilog)

:noname  ( warp top-warp c-addr len -- warp top-warp' )
  form-push >R 2DUP copy-warp R> OVER warp-prev-form-len ! ;
IS (form-prolog)

: out-form  ( warp form-name -- )
  COUNT TYPE \ 9 EMIT  transform-flags-form @ 6 ['] .R 2 BASE-EXECUTE
  9 EMIT  warp-result COUNT TYPE  CR ;

: (more)  ( warp top-warp -- warp ... )
  slot-stack-pop ?DUP-IF
    EXECUTE (slot-epilog)
  ELSE
    DUP warp-cached-result C@ IF
      DUP warp-cached-result  OVER warp-result  cs-copy
      DUP warp-segs# @ 1-  OVER warp-result-segs#  !
    THEN
    \ DUP COUNT TYPE .s
    transform2
    DUP warp-cached-result C@ 0= IF
      DUP warp-result  OVER warp-cached-result  cs-copy
    THEN
    \ DUP warp-result COUNT TYPE .s
    DUP form-name out-form
    1 n-forms +!
  THEN
  slot-stack-push ;

:noname  ( rule addr u -- )
  2>R EXECUTE 2R>  ( n addr u )
  add-slot (more) DUP warp-prev-form-len @ form-name C! ;
IS (form-epilog)


: generate  ( dict cs slot-stack slot-stack-len -- dict )
  slot-stack-set                ( dict cs )
  make-warp                      ( dict warp )
  slot-stack-pop EXECUTE DROP ;

: filter-start(  ( -- )
  ; IMMEDIATE
: )  ( f -- )
  POSTPONE IF ; IMMEDIATE
: filter-else
  POSTPONE ELSE ; IMMEDIATE
: filter-end ( -- )
  POSTPONE THEN ; IMMEDIATE

REQUIRE loaddefs.fs

: generate-for-headword  ( dict cs -- dict )
  slot-stack-khak /slot-stack-khak generate ;

: generate-for-stem  ( dict stemlist -- dict )
  strlist-str generate-for-headword ;

: generate-for-dict-list  ( ptrlist-node -- )
  ptrlist-ptr @                      ( dict )
  ." Dictionary entry: " DUP .dict CR
  DUP dict-p-o-s @  paradigm-p-o-s !
  DUP dict-p-o-s @ pos-v = IF
    DUP dict-stems @  ['] generate-for-stem  list-map
  ELSE DUP dict-p-o-s @ pos-n = IF
    DUP dict-headword generate-for-headword
  ELSE
    ." -" 9 EMIT DUP dict-headword COUNT TYPE CR
  THEN THEN
  ;

: paradigm  ( cs -- )
  debug-init
  COUNT find-headwords  ['] generate-for-dict-list  list-map
  debug-bye
  ;
