CREATE utf8-size-table
  $80 0 [DO] 1 C, [LOOP]
  $C0 $80 [DO] 0 C, [LOOP]
  $E0 $C0 [DO] 2 C, [LOOP]
  $F0 $E0 [DO] 3 C, [LOOP]
  $F8 $F0 [DO] 4 C, [LOOP]
  $FC $F8 [DO] 5 C, [LOOP]
  $FE $FC [DO] 6 C, [LOOP]
  7 C, 8 C,
: @xc-size  ( addr -- u )
  \ length of UTF-8 char starting at addr
  C@ utf8-size-table + C@ ;
: XCHAR+ DUP @xc-size + ;

[UNDEFINED] XC!+? [IF]
-77 Constant UTF-8-err
128 CONSTANT max-single-byte
: u8len ( u8 -- n )
    dup      max-single-byte u< IF  drop 1  EXIT  THEN \ special case ASCII
    $800  2 >r
    BEGIN  2dup u>=  WHILE  5 lshift r> 1+ >r  dup 0= UNTIL  THEN
    2drop r> ;
: x-size ( u8-addr u -- u1 )
    \ length of UTF-8 char starting at u8-addr (accesses only u8-addr)
    drop @xc-size ;
: XC@+ ( u8addr -- u8addr' u )
    count  dup max-single-byte u< ?EXIT  \ special case ASCII
    dup $C2 u< IF  UTF-8-err throw  THEN  \ malformed character
    $7F and  $40 >r
    BEGIN  dup r@ and  WHILE  r@ xor
	    6 lshift r> 5 lshift >r >r count
	    dup $C0 and $80 <> IF   UTF-8-err throw  THEN
	    $3F and r> or
    REPEAT  rdrop ;
: XC@ ]] XC@+ nip [[ ; immediate
: XCHAR- ( u8addr -- u8addr' )
    BEGIN  1- dup c@ $C0 and max-single-byte <>  UNTIL ;
: +x/string ( xc-addr1 u1 -- xc-addr2 u2 )
    over dup XCHAR+ swap - /string ;
: u8!+ ( u u8addr -- u8addr' )
    over max-single-byte u< IF  tuck c! 1+  EXIT  THEN \ special case ASCII
    >r 0 swap  $3F
    BEGIN  2dup u>  WHILE
	    2/ >r  dup $3F and $80 or swap 6 rshift r>
    REPEAT  $7F xor 2* or  r>
    BEGIN   over $80 u>= WHILE  tuck c! 1+  REPEAT  nip ;
: XC!+?  ( u addr len -- addr' len' f )
    >r over u8len r@ over u< if ( xc xc-addr1 len r: u1 )
	\ not enough space
	drop nip r> false
    else
	>r u8!+ r> r> swap - true
    then ;
: XC-SIZE u8len ;
[THEN]

\ for older GForth versions
[UNDEFINED] XC!+ [IF]
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

: cstr-append-xc  ( xc cstr -- )
  2DUP cstr-get +  XC!
  SWAP XC-SIZE  SWAP cstr-len  +! ;

cstr%
  CELL% FIELD sstr-count
  CELL% FIELD sstr-arr
  CELL% 2* FIELD sstr-morphonemic
END-STRUCT sstr%

: .sstr  { sstr -- }
  ." [" sstr .
  sstr IF
    ." :"  sstr .cstr  ." /"  sstr sstr-count @ .  ." |"
    sstr sstr-arr @                              ( arr )
    sstr sstr-count @  0 ?DO
      DUP  I [ cstr% %SIZE ]L * +  .cstr  ." ,"
    LOOP
    DROP                                             ( )
    ."  < " sstr sstr-morphonemic 2@ TYPE
  THEN
  ." ]" ;

: sstr-select  ( n sstr -- addr u )
  ?DUP-IF
    sstr-arr @ SWAP [ cstr% %SIZE ]L * + cstr-get
  ELSE DROP 0 0 THEN ;

VARIABLE sstr-last

: sstr-allocate-arr  { sstr -- }
  sstr sstr-count @  1+  [ cstr% %SIZE ]L *  ALLOCATE IF
    ABORT" Allocation error while preparsing"
  THEN  ( addr )
  sstr sstr-arr ! ;

: sstr-create  ( buf-len -- sstr )
  DUP ALLOCATE IF ABORT" ALLOCATION ERROR" THEN  ( buf-len buf )
  sstr% %ALLOC { sstr }
  sstr cstr-ptr !  sstr cstr-len !  sstr ;

: cs+  ( cs addr u -- )  \ Concatenate addr u to cs
  2 PICK COUNT + SWAP DUP >R CMOVE DUP C@ R> + SWAP C! ;

: s-to-cs  ( addr u cs -- )
  2DUP C!  1+ SWAP CMOVE ;

: cs-copy  ( src target -- )
  ]] OVER C@ 1+ CMOVE [[ ; IMMEDIATE

: cs-copy-truncate  ( src target len -- )
  2DUP SWAP C!
  ROT 1+ ROT 1+ ROT CMOVE ;

: string-ends  { addr1 u1 addr2 u2 -- f }
  \ \." " addr1 u1 type ." |" addr2 u2 type ."  string-ends?" cr
  u1 u2 - DUP 0>= IF  ( len-delta )
    addr1 + u2 addr2 u2 STR=
    \ \." result " dup . cr
  ELSE DROP FALSE THEN ;

: string-addr  ( addr u -- addr )
  POSTPONE DROP ; IMMEDIATE

: string-end  ( addr u -- addr' )
  POSTPONE + ; IMMEDIATE

: string-length  ( addr u -- u )
  POSTPONE NIP ; IMMEDIATE

: string-strip  ( addr u count -- addr u' )
  POSTPONE - ; IMMEDIATE

: string-copy  ( addr u -- addr1 u )
  >R PAD 42 + R@ CMOVE PAD 42 + R> ;

: string-create  ( addr len -- addr' len )
  >R                      ( addr  R: len )
  R@ ALLOCATE IF
    ABORT" Cannot allocate memory in string-create!"
  THEN                      ( addr addr' )
  SWAP OVER R@ CMOVE R> ;

: left-slice  ( addr u1 u2 -- addr u2 )
  POSTPONE NIP ; IMMEDIATE

: right-slice  ( addr u1 u2 -- addr' u1' )
  POSTPONE /STRING ; IMMEDIATE

: +x/string@  ( addr u -- addr' u' xc )
  ]] OVER XC@ -ROT +X/STRING ROT [[ ; IMMEDIATE


: cyrs ]] 2* [[ ; IMMEDIATE
: cyr 2 POSTPONE LITERAL ; IMMEDIATE
: 2cyrs 2 cyrs POSTPONE LITERAL ; IMMEDIATE
: 3cyrs 3 cyrs POSTPONE LITERAL ; IMMEDIATE
: cyr+ ]] cyr + [[ ; IMMEDIATE

: cyr?  ( addr -- f )
  ]] C@ [[ 128 ]]L AND [[ ; IMMEDIATE

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
  ]] string-end XCHAR- [[ ; IMMEDIATE

: prev-sound-ptr  ( addr u -- ptr )
  ]] string-end XCHAR- XCHAR- [[ ; IMMEDIATE

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
: last-sound-change  ( new-addr new-u addr u -- )
  last-sound-ptr SWAP MOVE ;
: last-sound-add  { str len cs -- }
  str  cs COUNT +  len  CMOVE
  cs @ len +  cs C! ;
: last-sound-cut  ( cs -- )
  DUP COUNT last-sound-ptr OVER 1+ - SWAP C! ;
: prev-sound-cut  ( cs -- )
  DUP COUNT last-sound-ptr DUP XC@ SWAP XCHAR- XC!
  DUP C@ cyr - SWAP C! ;
