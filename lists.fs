REQUIRE strings.fs

STRUCT
  CELL%      FIELD list-next
END-STRUCT list%

: list-swallow  ( list -- list' | 0 ) \ DROP for ALLOCed lists
  DUP list-next @
  SWAP FREE IF ." list-swallow release error" BYE THEN
  ;

: list-map  ( ... list xt -- ... )
  >R BEGIN DUP WHILE
    R@ SWAP >R R@ SWAP EXECUTE R>
  list-next @ REPEAT DROP RDROP ;


list%
  CELL%      FIELD ptrlist-ptr
END-STRUCT ptrlist%

: ptrlist-prepend  ( list ptr -- list' )
  ptrlist% %ALLOT >R  ( list ptr  R: list' )
  R@ ptrlist-ptr  !                 ( list )
  R@ list-next  !  R> ;

: .ptrlist-3cyrs
  BEGIN DUP WHILE DUP ptrlist-ptr @ 3cyrs TYPE BL EMIT list-next @ REPEAT ." <" . ." >" CR ;

: ptrlist-append  ( list ptr -- )
  \ \." Appending " dup 3cyrs type ."  to " over .ptrlist-3cyrs
  0 SWAP ptrlist-prepend SWAP  ( new-list list )
  BEGIN DUP list-next @ WHILE list-next @ REPEAT  ( new-list list-last )
  list-next ! ;


list%
  CELL% 64 * FIELD strlist-str
END-STRUCT strlist%

: strlist-get  ( list -- addr u )
  strlist-str COUNT ;

: .strlist-node  ( strlist -- )
  strlist-get TYPE BL EMIT ;

: .strlist  ( strlist -- )
  ['] .strlist-node list-map ;

: strlist-prepend-common  ( list addr u list' -- list' )
  >R                           ( list addr u  R: list' )
  R@ strlist-str  s-to-cs              ( list R: list' )
  R@ list-next  !  R> ;

: strlist-prepend  ( list addr u -- list' )
  strlist% %ALLOT strlist-prepend-common ;

: strlist-prepend-alloc  ( list addr u -- list' )
  strlist% %ALLOC strlist-prepend-common
  ;

: strlist-parse-alloc  { list addr u -- list' }
  BEGIN u DUP 0> WHILE  ( u )
    addr SWAP BEGIN OVER C@ BL <> WHILE
      1 /STRING
    REPEAT  ( addr' u' )
    u OVER - list SWAP addr SWAP strlist-prepend-alloc TO list
    1 /STRING
    TO u  TO addr
  REPEAT
  list ;

: strlist-in?  ( addr u list -- f )
  BEGIN DUP WHILE  { list' }                          ( addr u )
    list' strlist-get  2OVER STR= IF  2DROP TRUE EXIT  THEN
    list' list-next @                           ( addr u list' )
  REPEAT DROP 2DROP FALSE ;

: strlists-intersect?  ( list1 list2 -- f )
  { list2 }                       ( list1 )
  BEGIN DUP WHILE
    DUP strlist-get  list2  strlist-in? IF  DROP TRUE EXIT  THEN
    list-next @
  REPEAT                              ( 0 )
  ;


list%
  CELL%      FIELD pair-1-len
  CELL% 32 * FIELD pair-1-buf
  CELL%      FIELD pair-2-len
  CELL% 5  * FIELD pair-2-buf
  CELL%      FIELD pair-flags
END-STRUCT pairlist%

: pairlist-prepend  ( list addr1 len1 addr2 len2 -- list' )
  pairlist% %ALLOC >R  ( R: list' )
  R@ pair-2-len !
  R@ pair-2-buf  R@ pair-2-len @ CMOVE
  R@ pair-1-len !
  R@ pair-1-buf  R@ pair-1-len @ CMOVE
  0 R@ pair-flags !
  R@ list-next !
  \ \." allocated " r@ . ."  with next " r@ list-next @ . cr
  R> ;

: pair-1  ( list -- addr u )
  \ ." getting pair for " dup . cr
  DUP pair-1-buf SWAP pair-1-len @ ;

: pair-2  ( list -- addr u )
  DUP pair-2-buf SWAP pair-2-len @ ;

: .pairlist-node  ( pairlist -- )
  DUP pair-1 TYPE ." +" DUP pair-2 TYPE [CHAR] . EMIT pair-flags @ bin. ;

: .pairlist  ( pairlist -- )
  ['] .pairlist-node list-map ;
