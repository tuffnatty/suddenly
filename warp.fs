REQUIRE morphonology.fs

STRUCT
  CELL% 64 * FIELD warp-text
  CELL%      FIELD warp-fugitive
  CELL% 5 *  FIELD warp-seg-len
  CELL%      FIELD warp-segs#
  CELL% 64 * FIELD warp-result
  CELL%      FIELD warp-result-segs#
  CELL% 64 * FIELD warp-cached-result
  CELL%      FIELD warp-prev-form-len
END-STRUCT warp%

: warp-cached-result-seg  ( warp -- addr )
  DUP warp-text 1+  ( warp ptr )
  OVER warp-result-segs# @ ROT warp-seg-len DUP >R + ( warp ptr end )
  R> ?DO I C@ + LOOP ;

: warp-fugitive?  ( warp -- flag )
  POSTPONE warp-fugitive POSTPONE @ ; IMMEDIATE

: .warp  ( warp -- )
  ." {"
  DUP warp-text DUP C@ . ." :" COUNT TYPE
  ." |"
  DUP warp-segs# @ 0 ?DO DUP warp-seg-len I + C@ . LOOP
  ." |"
  DUP warp-result COUNT TYPE
  ." |"
  DUP warp-result-segs# @ .
  ." |"
  DUP warp-cached-result-seg SWAP DUP warp-result-segs# @ SWAP warp-seg-len + C@ TYPE
  ." }" CR ;

: copy-warp  ( warp1 warp2 -- )
  OVER warp-text C@  OVER warp-text C!
  >R warp-segs# @  R@ warp-segs# !   ( R: warp2 )
  R@ warp-text  R@ warp-result  R> warp-seg-len C@  cs-copy-truncate ;

: copy-warp-full  ( warp1 warp2 -- )
  OVER warp-text     OVER warp-text 64 CELLS CMOVE
  OVER warp-fugitive @ OVER warp-fugitive !
  OVER warp-segs# @ >R
  R@ OVER warp-segs# !
  SWAP warp-seg-len OVER warp-seg-len R> CMOVE  ( warp2 )
  1 OVER warp-result-segs# !
  DUP warp-text  OVER warp-result  ROT warp-seg-len C@  cs-copy-truncate
  ( OVER warp-text         OVER warp-text         OVER C@ 1+ MOVE
  OVER warp-result       OVER warp-result       OVER C@ 1+ MOVE
  OVER warp-result-segs# OVER warp-result-segs# CELL       MOVE
  OVER warp-segs#        OVER warp-segs#        CELL       MOVE
  >R warp-seg-len R>     DUP warp-seg-len SWAP warp-segs# @ MOVE
  ) \ warp% %SIZE 64 CELLS - MOVE
  ;

: make-warp  ( cs -- warp )
  warp% %ALLOT >R
  R@ warp-text  cs-copy ( )
  R@ warp-text C@  R@ warp-seg-len C!
  1 R@ warp-segs# !
  R@ warp-text  R@ warp-result  cs-copy
  1 R@ warp-result-segs# !
  R@ warp-text  stem-fugitive?  R@ warp-fugitive !
  R> ;
