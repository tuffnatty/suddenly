\ In GForth, NOT is undefined
[UNDEFINED] NOT [IF]
: NOT  ( f -- !f )
  POSTPONE 0= ; IMMEDIATE
[THEN]

\ GForth-compatibility defines for SwiftForth
[UNDEFINED] REQUIRE [IF]
  INCLUDE compat/required.fs
  : PARSE-NAME POSTPONE BL POSTPONE PARSE ; IMMEDIATE
  : REQUIRE PARSE-NAME REQUIRED ;
[THEN]

\ GForth-compatibility defines for VFX Forth
[undefined] ]L [if]
: ]L ( compilation: n -- ; run-time: -- n ) \ gforth
    ] postpone literal ;
[then]

[undefined] ]] [if]
  : fliteral postpone literal ; immediate
  require compat/macros.fs
[then]

[UNDEFINED] ]]L [IF]
: postpone-literal  postpone  literal ;
: ]]L ( postponing: x -- ; compiling: -- x )
    \ Shortcut for @code{]] literal}.
    ]] postpone-literal ]] [[ ; immediate
[THEN]

[undefined] ?dup-if [if]
  : ?dup-if ]] ?dup if [[ ; immediate
[then]

[undefined] ?exit [if]
: ?exit ]] IF EXIT THEN [[ ; immediate
[then]

[UNDEFINED] .id [IF]
: .id ]] .name [[ ; immediate
[THEN]

[UNDEFINED] xt-see [IF]
  [UNDEFINED] dasm [IF]
    : xt-see ]] dis [[ ; immediate  \ VFX
  [ELSE]
    : xt-see ]] dasm [[ ; immediate  \ SwiftForth
  [THEN]
[THEN]

[undefined] 0>= [if]
: 0>= 0 ]]L >= [[ ; immediate
[then]

[undefined] cell% [if]
  \ require /home/phil/build/VfxLinEval/Lib/x86/Ndp387.fth  \ FIXME (VFX)
  REQUIRES fpmath  \ FIXME (SwiftForth )
  require compat/struct.fs
[then]

[UNDEFINED] NEXTNAME [IF]
  0 VALUE nextname-addr
  0 VALUE nextname-len
  : nextname TO nextname-len TO nextname-addr ;
  [DEFINED] (WID-CREATE) [IF]
  : created GET-CURRENT (WID-CREATE) ;
  [ELSE]
  : created ($CREATE) ;
  [THEN]
  : create
    nextname-addr IF
      nextname-addr nextname-len  ( addr len )
      0 TO nextname-addr
    ELSE PARSE-NAME THEN  ( addr len )
    created ;
[THEN]

[undefined] rdrop [if]
: rdrop ]] R> DROP [[ ; immediate
[then]

[UNDEFINED] S>NUMBER? [IF]
  : S>NUMBER?  2>R 0. 2R> >NUMBER 2DROP TRUE ;
[THEN]

[UNDEFINED] str= [IF]
  : str= COMPARE 0= ;
  : string-prefix? ( c-addr1 u1 c-addr2 u2 -- f ) \ gforth
    tuck 2>r min 2r> str= ;
[THEN]

[UNDEFINED] table [IF]
\ require compat/table.fs
: TABLE WORDLIST ;
[THEN]

[UNDEFINED] utime [IF]
  [DEFINED] ucounter [IF]
    : utime  ]] ucounter [[ ; IMMEDIATE  \ SwiftForth
  [ELSE]
    : utime ]] ticks 1000 UM* [[ ; IMMEDIATE  \ VFX
  [THEN]
[THEN]

[UNDEFINED] U>= [IF]
  : U>=  ]] U< NOT [[ ; IMMEDIATE
[THEN]

[undefined] xemit [if]
DEFER xemit
:noname  ( u -- )
  DUP 128 U< IF EMIT EXIT THEN
  0 SWAP  $3F
  BEGIN  2DUP U>  WHILE
    2/ >R  DUP $3F AND $80 OR SWAP 6 RSHIFT R>
  REPEAT  $7F XOR 2* OR
  BEGIN  DUP $80 U>= WHILE  EMIT  REPEAT  DROP ; IS xemit
[then]

[UNDEFINED] locals-types [IF]
  : \g POSTPONE \ ; IMMEDIATE
  CREATE locals-stack 2048 CELLS ALLOT
  HERE VALUE lp
  : @local# lp + @ ;
  : @local0 lp @ ;
  : @local1 CELL lp + @ ;
  : @local2 2 CELLS lp + @ ;
  : @local3 3 CELLS lp + @ ;
  : f@local# lp + F@ ;
  : f@local0 lp F@ ;
  : f@local1 1 FLOATS lp + F@ ;
  : laddr# lp + ;
  : lp+!# +TO lp ;
  : lp- CELL NEGATE +TO lp ;
  : lp+ 1 FLOATS +TO lp ;
  : lp+2 2 FLOATS +TO lp ;
  : >l lp- lp ! ;
  : f>l -1 FLOATS +TO lp lp F! ;
  REQUIRE compat/glocals.fs
[THEN]

[UNDEFINED] WTF?? [IF]
  : WTF?? ~~ store-backtrace dobacktrace BYE ;
[THEN]

[UNDEFINED] 2VALUE [IF]
  : 2Value ( d "name" -- ) \ Forth200x
    Create 2,
    DOES> 2@ ;
  0. 2VALUE some-2value
  :noname
    ' dup >definer CASE
      [ ' some-2value ]L >definer OF >BODY 2! ENDOF
      [ ' locals-wordlist ]L >definer OF >BODY ! ENDOF
      -&32 THROW
    ENDCASE ;
  :noname
    comp' drop dup >definer CASE
      [ ' some-2value ]L >definer OF >BODY ]] ALiteral 2! [[ ENDOF
      [ ' locals-wordlist ]L >definer OF >body ]] Aliteral ! [[ ENDOF
      [ comp' some-clocal drop ]L >definer OF POSTPONE laddr# >body @ lp-offset, POSTPONE c! ENDOF
      [ comp' some-wlocal drop ]L >definer OF POSTPONE laddr# >body @ lp-offset, POSTPONE ! ENDOF
      [ comp' some-dlocal drop ]L >definer OF POSTPONE laddr# >body @ lp-offset, POSTPONE 2! ENDOF
      [ comp' some-flocal drop ]L >definer OF POSTPONE laddr# >body @ lp-offset, POSTPONE f! ENDOF
      -&32 THROW
    ENDCASE ;
  interpret/compile: TO
[THEN]

[UNDEFINED] !@ [IF]
  : !@  ( u1 addr -- u2 )
    DUP @ >R ! R> ;
  : +!@  ( u1 addr -- u2 )
    DUP @ >R +! R> ;
[THEN]

[UNDEFINED] [: [IF]
  : [:  ( -- )
    POSTPONE AHEAD :noname ; IMMEDIATE COMPILE-ONLY
  : ;]  ( -- xt )
    POSTPONE ; ] POSTPONE THEN LATESTXT POSTPONE LITERAL ; IMMEDIATE COMPILE-ONLY
[THEN]

[UNDEFINED] safe/string [IF]
  : safe/string  ( c_addr1 u1 n -- c_addr2 u2 )
    tuck - >r + r> dup 0< IF  - 0  THEN ;
[THEN]

[UNDEFINED] >LINK [IF]
  ' noop ALIAS >LINK
[THEN]

: ||  ( f  R: x r -- true R: x PC: r | R: x r )  \ OR with boolean shortcircuiting
  ]] ?DUP-IF EXIT THEN [[ ; IMMEDIATE COMPILE-ONLY

: &&  ( f R: x r -- false R: x PC: r | R: x r )  \ AND with boolean shortcircuiting
  ]] ?DUP-0=-IF FALSE EXIT ELSE DROP THEN [[ ; IMMEDIATE COMPILE-ONLY

: array-reverse  ( arr len -- )
  1- CELLS OVER + BEGIN 2DUP < WHILE  ( a1 a2 )
    2DUP 2DUP @ SWAP @ ROT ! SWAP !
    CELL - SWAP CELL+ SWAP          ( a1' a2' )
  REPEAT 2DROP ;

CREATE emit-buffer 1 ALLOT
: with-type  ( xt type-xt -- )
  ACTION-OF TYPE  ACTION-OF EMIT  2>R
    IS TYPE
    [: emit-buffer C! emit-buffer 1 TYPE ;] IS EMIT
    EXECUTE
  2R>  IS EMIT  IS TYPE ;
: stderr-type  ( addr u -- )
  STDERR WRITE-FILE THROW  STDERR FLUSH-FILE THROW ;
