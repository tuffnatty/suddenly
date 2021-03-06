[IFUNDEF] XC!+
: XC!+  ( u addr -- addr' )
  5 XC!+? 2DROP ;
[THEN]

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
  2DUP cstr-len +!
  OVER NEGATE OVER cstr-ptr +!
  cstr-ptr @ SWAP CMOVE ;

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
  ]] 2DUP C!  1+ SWAP CMOVE [[ ; IMMEDIATE

: cs-copy  ( src target -- )
  ]] OVER C@ 1+ CMOVE [[ ; IMMEDIATE

: cs-copy-truncate  ( src target len -- )
  ]] 2DUP SWAP C!
  ROT 1+ ROT 1+ ROT CMOVE [[ ; IMMEDIATE

: stuff  { cs ptr n-del addr u -- }
  cs COUNT +             ( cs-end )
  u n-del - cs C@ + cs C!                    \ Update length byte
  ptr n-del + ptr u + OVER 3 PICK SWAP - MOVE         \ Move tail
  addr ptr u MOVE DROP ;

: string-ends  { addr1 u1 addr2 u2 -- f }
  \ \." " addr1 u1 type ." |" addr2 u2 type ."  string-ends?" cr
  u1 u2 - DUP 0>= IF  ( len-delta )
    addr1 + u2 addr2 u2 STR=
    \ \." result " dup . cr
  ELSE DROP FALSE THEN ;
: cs-ends ( cs1 cs2 -- f )
  SWAP COUNT ROT COUNT string-ends ;

: string-addr  ( addr u -- addr )
  POSTPONE DROP ; IMMEDIATE

: string-end  ( addr u -- addr' )
  POSTPONE + ; IMMEDIATE

: string-append-char  { xc addr u -- addr u' }
  addr u  string-end { ptr }
  xc ptr XC!  addr  ptr XCHAR+ addr - ;

: string-length  ( addr u -- u )
  POSTPONE NIP ; IMMEDIATE

: string-strip  ( addr u count -- addr u' )
  POSTPONE - ; IMMEDIATE

: string-copy  ( addr u -- addr1 u )
  >R PAD 42 + R@ CMOVE PAD 42 + R> ;

: left-slice  ( addr u1 u2 -- addr u2 )
  POSTPONE NIP ; IMMEDIATE

: right-slice  ( addr u1 u2 -- addr' u1' )
  POSTPONE /STRING ; IMMEDIATE

: left-slice+xc  ( addr u1 u2 -- addr u3 )
  left-slice  2DUP string-end  XC@ XC-SIZE  + ;

: u-search  ( u array size -- f )
  CELLS SWAP >R 0 SWAP R@ + R> ?DO  ( u f )
    DROP DUP I @ = DUP ?LEAVE
  CELL +LOOP NIP ;

: cs-buf-size  ( cs -- cs u )
  ]] DUP C@ 1+ [[ ; IMMEDIATE

: xc+@  ( addr1 -- addr2 u )
  ]] XCHAR+ DUP XC@ [[ ; IMMEDIATE

: +x/string@  ( addr u -- addr' u' xc )
  ]] OVER XC@ -ROT +X/STRING ROT [[ ; IMMEDIATE


:noname 2* ;
:noname POSTPONE 2* ;
INTERPRET/COMPILE: cyrs
: cyr 2 POSTPONE LITERAL ; IMMEDIATE
: 2cyrs 2 cyrs POSTPONE LITERAL ; IMMEDIATE
: 3cyrs 3 cyrs POSTPONE LITERAL ; IMMEDIATE
: cyr+ ]] cyr + [[ ; IMMEDIATE

: cyr?  ( addr -- f )
  ]] C@ 128 AND [[ ; IMMEDIATE

: skip-cyrs  ( addr -- addr' )
  BEGIN DUP cyr? WHILE cyr+ REPEAT ;

: skip-latin ( addr -- addr' )
  BEGIN DUP cyr? 0= WHILE 1+ REPEAT ;

: count-cyrs  ( addr -- n )
  DUP skip-cyrs SWAP - ;

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

: first-sound-ptr  ( addr u -- ptr )
  POSTPONE string-addr ; IMMEDIATE

: second-sound-ptr  ( addr u -- ptr )
  ]] string-addr cyr+ [[ ; IMMEDIATE

: last-sound-ptr  ( addr u -- ptr )
  string-end XCHAR- ;

: prev-sound-ptr  ( addr u -- ptr )
  string-end XCHAR- XCHAR- ;

: first-sound  ( addr u -- xc )
  first-sound-ptr ?DUP-IF XC@ ELSE 0 THEN ;
: second-sound  ( addr u -- xc )
  ]] second-sound-ptr XC@ [[ ; IMMEDIATE
: last-sound ( addr u -- xc )
  last-sound-ptr XC@ ;
: prev-sound ( addr u -- xc )
  prev-sound-ptr XC@ ;
: third-sound ( addr u -- xc )
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

