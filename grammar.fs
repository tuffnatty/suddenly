REQUIRE morphonemic.fs

DEFER (slot-prolog)  ( warp -- warp top-warp )
DEFER (slot-epilog)  ( warp -- warp top-warp )
DEFER (form-prolog)  ( warp top-warp c-addr len -- warp top-warp' )
DEFER (form-epilog)  ( n addr u -- )

: slot:  ( "name" -- )
  : POSTPONE (slot-prolog) ;

: form"  ( "name affix" -- )
  BL PARSE [CHAR] " PARSE morphonemic-to-sstr-and-rule 2SWAP  ( sstr rule affix-name name-len )
  POSTPONE SLITERAL POSTPONE (form-prolog)                                        ( sstr rule )
  POSTPONE LITERAL POSTPONE LITERAL POSTPONE (form-epilog)
  ; IMMEDIATE

VARIABLE paradigm-slot-bitmap
: .slots
  paradigm-slot-bitmap @  ( mask )
  CELL 8 * 0 DO
    1 I LSHIFT OVER AND IF I . THEN
  LOOP DROP ;
  
: slot-empty!  ( n -- )
  1 SWAP LSHIFT INVERT paradigm-slot-bitmap @ AND paradigm-slot-bitmap ! ;
: slot-full!  ( n -- )
  1 SWAP LSHIFT paradigm-slot-bitmap @ OR paradigm-slot-bitmap ! ;
: slot-empty?  ( n -- f )
  1 SWAP LSHIFT paradigm-slot-bitmap @ AND 0= ;
: slot-range-full?  ( n1 n2 -- f)
  0 { mask }
  BEGIN 2DUP <= WHILE
    1 OVER LSHIFT mask OR TO mask
  1- REPEAT 2DROP paradigm-slot-bitmap @ mask AND ;
: slot-range-empty?  ( n1 n2 -- f )
  slot-range-full? NOT ;
: slot-all-empty?  ( -- f )
  paradigm-slot-bitmap @ 0= ;
: slot-full?  ( n -- f )
  1 SWAP LSHIFT paradigm-slot-bitmap @ AND ;


CELL 4 = [IF]
  2VARIABLE paradigm-flags-ptr
  : paradigm-flags  ]] paradigm-flags-ptr 2@ [[ ; IMMEDIATE
  : paradigm-flags! ]] paradigm-flags-ptr 2! [[ ; IMMEDIATE
  : flag-DUP  ( ud -- ud ud )
    POSTPONE 2DUP ; IMMEDIATE
  : flag-OR  ( ud1 ud2 -- ud' )
    >R ROT OR SWAP R> OR ;
  : flag-AND  ( ud1 ud2 -- ud' )
    >R ROT AND SWAP R> AND ;
  : flag-invert  ( ud -- ud' )
    INVERT SWAP INVERT SWAP ;
  : flag-empty?  ( ud -- f )
    paradigm-flags flag-AND D0= ;
  : flag-is?  ( ud -- f )
    paradigm-flags flag-AND D0<> ;
  : flags.  ( ud -- )
    POSTPONE 2bin. ; IMMEDIATE
[ELSE]
  VARIABLE paradigm-flags-ptr
  : paradigm-flags  ]] paradigm-flags-ptr @ [[ ; IMMEDIATE
  : paradigm-flags! ]] paradigm-flags-ptr ! [[ ; IMMEDIATE
  : flag-DUP  ( u -- u u )
    POSTPONE DUP ; IMMEDIATE
  : flag-OR  ( u1 u2 -- u' )
    OR ;
  : flag-AND  ( u1 u2 -- u' )
    POSTPONE AND ; IMMEDIATE
  : flag-invert  ( u -- u')
    POSTPONE INVERT ; IMMEDIATE
  : flag-empty?  ( u -- f )
    paradigm-flags flag-AND 0= ;
  : flag-is?  ( u -- f )
    paradigm-flags flag-AND 0<> ;
  : flags.  ( u -- )
    POSTPONE bin. ; IMMEDIATE
[THEN]
: flag-set  ( flag -- )
  \\." setting flag "  flag-DUP flags.  BL EMIT
  paradigm-flags flag-OR  paradigm-flags!
  \\." flags are " paradigm-flags flags.  CR
  ;
: flag-clear  ( flag -- )
  \\." clearing flag "  flag-DUP flags.  BL EMIT
  flag-invert paradigm-flags flag-AND  paradigm-flags!
  \\." flags are " paradigm-flags flags. CR
  ;
