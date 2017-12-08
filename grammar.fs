REQUIRE morphonemic.fs

DEFER (slot-prolog)  ( warp -- warp top-warp )
DEFER (slot-epilog)  ( warp -- warp top-warp )
DEFER (form-prolog)  ( warp top-warp c-addr len -- warp top-warp' )
DEFER (form-epilog)  ( n addr u -- )


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
  1- REPEAT 2DROP paradigm-slot-bitmap @ mask AND 0<> ;
: slot-range-empty?  ( n1 n2 -- f )
  slot-range-full? NOT ;
: slot-all-empty?  ( -- f )
  paradigm-slot-bitmap @ 0= ;
: slot-full?  ( n -- f )
  1 SWAP LSHIFT paradigm-slot-bitmap @ AND 0<> ;


CELL 4 = [IF]
  $0. 2CONSTANT flag-none
  2VARIABLE paradigm-flags-ptr
  : paradigm-flags  ]] paradigm-flags-ptr 2@ [[ ; IMMEDIATE
  : paradigm-flags! ]] paradigm-flags-ptr 2! [[ ; IMMEDIATE
  2VARIABLE local-flag
  : local-flag@ ]] local-flag 2@ [[ ; IMMEDIATE
  : local-flag! ]] local-flag 2! [[ ; IMMEDIATE
  : flag-]]L POSTPONE ]]2L ; IMMEDIATE
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
  : flag-any?  ( ud -- f )
    POSTPONE D0<> ; IMMEDIATE
  : flags.  ( ud -- )
    POSTPONE 2bin. ; IMMEDIATE
[ELSE]
  0 CONSTANT flag-none
  VARIABLE paradigm-flags-ptr
  : paradigm-flags  ]] paradigm-flags-ptr @ [[ ; IMMEDIATE
  : paradigm-flags! ]] paradigm-flags-ptr ! [[ ; IMMEDIATE
  VARIABLE local-flag
  : local-flag@ ]] local-flag @ [[ ; IMMEDIATE
  : local-flag! ]] local-flag ! [[ ; IMMEDIATE

  : flag-DUP  ( u -- u u )
    POSTPONE DUP ; IMMEDIATE
  : flag-]]L POSTPONE ]]L ; IMMEDIATE
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
  : flag-any?  ( u -- f )
    POSTPONE 0<> ; IMMEDIATE
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
: flag-with  ( "name" -- )
  PARSE-NAME FIND-NAME ?DUP-IF  ( nt )
    NAME>INT EXECUTE          ( flag )
    flag-DUP flag-]]L flag-set [[
    local-flag!
  ELSE 1 ABORT"  word not found!" THEN ; IMMEDIATE


: slot:  ( "name" -- )
  : POSTPONE (slot-prolog) ;

CREATE flagname-buffer CHAR f C, CHAR l C, CHAR a C, CHAR g C, CHAR - C, 32 ALLOT

: form"  ( "name affix" -- )
  BL PARSE [CHAR] " PARSE morphonemic-to-sstr-and-rule 2SWAP  ( sstr rule affix-name name-len )
  2DUP flagname-buffer 5 + SWAP MOVE
  flagname-buffer OVER 5 + FIND-NAME ?DUP-IF  ( ... nt )
    NAME>INT EXECUTE flag-DUP local-flag! flag-]]L flag-set [[  ( ... )
  THEN
  POSTPONE SLITERAL POSTPONE (form-prolog)                                        ( sstr rule )
  POSTPONE LITERAL POSTPONE LITERAL POSTPONE (form-epilog)
  local-flag@ flag-any?  IF
    local-flag@ flag-]]L flag-clear [[
    flag-none local-flag!
  THEN
  ; IMMEDIATE

