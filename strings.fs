: XC!  ( u addr -- )
  5 XC!+? DROP 2DROP ;

STRUCT
  CELL% FIELD cstr-len
  CELL% FIELD cstr-ptr
END-STRUCT cstr%

: cstr-get  ( cstr -- addr u )
  DUP cstr-ptr @  SWAP cstr-len @ ;

: .cstr  ( cstr -- )
  cstr-get TYPE ;

: cstr-append-xc  { xc cstr -- }
  xc  cstr cstr-get +  XC!
  xc XC-SIZE  cstr cstr-len  +! ;

cstr%
  CELL% FIELD sstr-count
  CELL% FIELD sstr-arr
END-STRUCT sstr%

: .sstr  ( sstr -- )
  ." [" DUP .
  ?DUP-IF
    ." :" DUP .cstr ." /" DUP sstr-count @ . ." |"
    DUP sstr-arr @ SWAP sstr-count @ 0 ?DO DUP I [ cstr% %SIZE ]L * + .cstr ." ," LOOP
    DROP
  THEN
  ." ]" ;

: sstr-select  ( n sstr -- addr u )
  ?DUP-IF
    sstr-arr @ SWAP [ cstr% %SIZE ]L * + cstr-get
  ELSE DROP 0 0 THEN ;

VARIABLE sstr-last

: count-words  ( addr u -- n )
  0 0 { count in-word }
  OVER + SWAP DO
    I C@ BL = IF
      in-word IF FALSE TO in-word THEN
    ELSE
      in-word 0= IF TRUE TO in-word  1 count + TO count THEN
    THEN
  LOOP count ;

: sstr-allocate-arr  { sstr -- }
  sstr sstr-count @ cstr% %SIZE * ALLOCATE IF
    ABORT" Allocation error while preparsing"
  THEN
  sstr sstr-arr ! ;

: sstr-preparse  { sstr -- }
  sstr sstr-count @ IF sstr sstr-arr @ FREE IF ABORT" Free error while preparsing" THEN THEN
  sstr cstr-get  count-words  sstr sstr-count !
  sstr sstr-allocate-arr
  sstr cstr-ptr @ 0 0 0 0 { start n in-word cur arr-ptr }
  sstr cstr-len @  0 DO
    sstr cstr-ptr @ I + DUP C@ BL = IF              ( ptr )
      in-word IF
        sstr sstr-arr @ n cstr% %SIZE * + TO arr-ptr
        start   arr-ptr cstr-ptr !
        start - arr-ptr cstr-len !                   ( )
        FALSE TO in-word
        n 1+ TO n
      ELSE DROP THEN
    ELSE
      in-word IF DROP
      ELSE
        TO start                                       ( )
        TRUE TO in-word
      THEN
    THEN
  LOOP
  in-word IF
    sstr sstr-arr @ n cstr% %SIZE * + TO arr-ptr
    start arr-ptr cstr-ptr !
    sstr cstr-get + start - arr-ptr cstr-len !
  THEN
  ;

: sstr-create  ( buf-len -- sstr )
  DUP ALLOCATE IF ABORT" ALLOCATION ERROR" THEN  ( buf-len buf )
  sstr% %ALLOC { sstr }
  sstr cstr-ptr !  sstr cstr-len !  sstr ;

: "  ( "string" -- sstr )
  [CHAR] " PARSE { str len }
  len sstr-create { sstr }
  str  sstr cstr-ptr @  len CMOVE
  0 sstr sstr-count !
  sstr sstr-preparse
  sstr sstr-last !
  sstr POSTPONE LITERAL ; IMMEDIATE

: +"  ( sstr "string" -- sstr )
  sstr-last @ { sstr }
  [CHAR] " PARSE                                       ( addr u )
  DUP  sstr cstr-get  ROT +                   ( addr u buf len' )
  DUP -ROT                               ( addr u len' buf len' )
  RESIZE IF ABORT" REALLOCATION ERROR" THEN  ( addr u len' buf' )
  DUP sstr cstr-ptr !
  sstr cstr-len @ +                           ( addr u len' ptr )
  -ROT sstr cstr-len !                             ( addr ptr u )
  CMOVE                                                       ( )
  sstr sstr-preparse
  ; IMMEDIATE

cstr%
  CELL% 100 * FIELD bstr-buffer
END-STRUCT bstr%  \ a string for fast prepending

: .bstr  ( bstr -- )
  .cstr ;

: bstr-init  ( bstr -- )
  DUP bstr-buffer 100 CELLS + OVER cstr-ptr !
  0 SWAP cstr-len ! ;

: bstr-prepend  ( addr u bstr -- )
  >R                                ( addr u  R: bstr )
  DUP R@ cstr-len +!
  DUP NEGATE R@ cstr-ptr +!
  R> cstr-ptr @ SWAP CMOVE
  ;

: bstr-pop  ( bstr -- )
  \ ." popping" dup .bstr cr
  1 OVER cstr-ptr +!
  -1 OVER cstr-len +!
  BEGIN
  DUP cstr-len @ WHILE
  DUP cstr-ptr @ C@ [CHAR] - <> WHILE
    1 OVER cstr-ptr +!
    -1 OVER cstr-len +!
  REPEAT THEN
  \ ." popped" dup .bstr cr
  DROP ;

: cs+  ( cs addr u -- )
  2 PICK COUNT + SWAP DUP >R CMOVE DUP C@ R> + SWAP C! ;

: cs=  ( cs1 cs2 -- f )
  COUNT ROT COUNT STR= ;

: s-to-cs  ( addr u cs -- )
  >R DUP R@ C!  ( addr u  R: cs )
  R> 1+ SWAP CMOVE ;

: cs-copy  ( src target -- )
  POSTPONE OVER POSTPONE C@ POSTPONE 1+ POSTPONE CMOVE ; IMMEDIATE

: cs-copy-truncate  ( src target len -- )
  POSTPONE 2DUP POSTPONE SWAP POSTPONE C!
  POSTPONE >R POSTPONE >R POSTPONE 1+  POSTPONE R> POSTPONE 1+  POSTPONE R> POSTPONE CMOVE ; IMMEDIATE

: stuff  { cs ptr n-del addr u -- }
  cs COUNT +             ( cs-end )
  u n-del - cs C@ + cs C!                    \ Update length byte
  ptr n-del + ptr u + OVER 3 PICK SWAP - MOVE         \ Move tail
  addr ptr u MOVE DROP ;

: string-ends  { addr1 u1 addr2 u2 -- f }
  \ addr1 u1 type ." |" addr2 u2 type ."  string-ends?" cr
  u1 u2 - DUP 0>= IF  ( len-delta )
    addr1 + u2 addr2 u2 STR=
    \ ." result " dup . cr
  ELSE DROP FALSE THEN ;
: cs-ends ( cs1 cs2 -- f )
  SWAP COUNT ROT COUNT string-ends ;

: string-addr  ( addr u -- addr )
  POSTPONE DROP ; IMMEDIATE

: string-length  ( addr u -- u )
  POSTPONE NIP ; IMMEDIATE

: string-strip  ( addr u count -- addr u' )
  POSTPONE - ; IMMEDIATE

: u-search  ( u array size -- f )
  CELLS SWAP >R 0 SWAP R@ + R> ?DO  ( u f )
    DROP DUP I @ = DUP ?LEAVE
  CELL +LOOP NIP ;

: cs-buf-size  ( cs -- cs u )
  POSTPONE DUP POSTPONE C@ POSTPONE 1+ ; IMMEDIATE

:noname  ( addr -- u )
  @ DUP 128 AND IF
    DUP 31 AND 6 LSHIFT SWAP 8 RSHIFT 63 AND OR
  ELSE 127 AND THEN ;
IS XC@
: xc+@  ( addr1 -- addr2 u )
  XCHAR+ DUP XC@ ;

:noname 2* ;
:noname POSTPONE 2* ;
INTERPRET/COMPILE: cyrs
: cyr 2 POSTPONE LITERAL ; IMMEDIATE
: 2cyrs 2 cyrs POSTPONE LITERAL ; IMMEDIATE
: 3cyrs 3 cyrs POSTPONE LITERAL ; IMMEDIATE
: cyr+ POSTPONE cyr POSTPONE + ; IMMEDIATE

: is-1-char?  ( addr len -- f )
  cyr = NIP ;

: is-1-char?1  ( addr len -- f )
  OVER XCHAR+ >R + R> = ;

: is-2-char?  ( addr len -- f )
  [ 2 cyrs ]L = NIP ;

: is-2-char?1  ( addr len -- f )
  OVER + SWAP       ( end-addr addr )
  XCHAR+ 2DUP = IF  ( end-addr addr )
    2DROP 0                     ( 0 )
  ELSE XCHAR+ = THEN ;

: str-trim-last-cyr  ( addr len -- addr len' )
  DUP cyr > IF cyr - THEN ;

: last-sound-ptr  ( addr u -- ptr )
  + XCHAR- ;

: prev-sound-ptr  ( addr u -- ptr )
  + XCHAR- XCHAR- ;

: last-sound ( addr u -- u )
  last-sound-ptr XC@ ;
: prev-sound ( addr u -- u )
  prev-sound-ptr XC@ ;
: third-sound ( addr u -- u )
  prev-sound-ptr XCHAR- XC@ ;
: last-sound-change  ( xc addr u -- )
  last-sound-ptr XC! ;
: last-sound-add ( u cs -- )
  SWAP OVER COUNT + XC!  ( cs )
  DUP C@ cyr+ SWAP C! ;
: last-sound-cut  ( cs -- )
  DUP COUNT last-sound-ptr OVER 1+ - SWAP C! ;
: prev-sound-cut  ( cs -- )
  DUP COUNT last-sound-ptr DUP XC@ SWAP XCHAR- XC!
  DUP C@ cyr - SWAP C! ;

