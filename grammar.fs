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

VARIABLE paradigm-flags
: flag-set  ( n -- )
  \." setting flag " DUP . BL EMIT
  paradigm-flags @ OR paradigm-flags !
  \." flags are " paradigm-flags @ . CR
  ;
: flag-clear  ( n -- )
  \." clearing flag " DUP . CR
  INVERT paradigm-flags @ AND paradigm-flags !
  \." flags are " paradigm-flags @ . CR
  ;
: flag-is?  ( n -- f )
  paradigm-flags @ AND ;
: flag-empty?  ( n -- f )
  paradigm-flags @ AND 0= ;
