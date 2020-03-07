REQUIRE flagtype.fs
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
: slot-mask  ( n1 n2 -- mask )
  0 { mask }
  BEGIN 2DUP <= WHILE
    1 OVER LSHIFT mask OR TO mask
  1- REPEAT 2DROP mask ;
: slot-range-full?  ( n1 n2 -- f)
  slot-mask  paradigm-slot-bitmap @  AND 0<> ;
: slot-range-empty?  ( n1 n2 -- f )
  slot-range-full? NOT ;
: slot-all-empty?  ( -- f )
  paradigm-slot-bitmap @ 0= ;
: slot-full?  ( n -- f )
  1 SWAP LSHIFT paradigm-slot-bitmap @ AND 0<> ;


debug-mode? [IF]
: slots[  ( "n" -- n1 )
  ; IMMEDIATE
: slots(  ( "n" -- n1 )
  PARSE-NAME FIND-NAME NAME>INT EXECUTE 1+ POSTPONE LITERAL ; IMMEDIATE
: ]-empty?  ( n1 n2 -- f )
  ]] slot-range-empty? [[ ; IMMEDIATE
: )-empty?  ( n1 n2 -- f )
  ]] 1- slot-range-empty? [[ ; IMMEDIATE
: ]-full?  ( n1 n2 -- f )
  ]] slot-range-full? [[ ; IMMEDIATE
: )-full?  ( n1 n2 -- f )
  ]] 1- slot-range-full? [[ ; IMMEDIATE
[ELSE]
: slots[  ( "n1" "n2" -- slots-sys )
  POSTPONE [ ; IMMEDIATE COMPILE-ONLY
: slots(  ( "n1" "n2" -- slots-sys )
  PARSE-NAME FIND-NAME NAME>INT EXECUTE 1+
  POSTPONE [ ; IMMEDIATE COMPILE-ONLY
: ]-empty?  ( slots-sys -- )  ( runtime: -- f )
  ] slot-mask ]]L  paradigm-slot-bitmap @  AND 0= [[ ;
: )-empty?  ( slots-sys -- )  ( runtime: -- f )
  ] 1- slot-mask ]]L  paradigm-slot-bitmap @  AND 0= [[ ;
: ]-full?  ( slots-sys -- )  ( runtime: -- f )
  ] slot-mask ]]L  paradigm-slot-bitmap @  AND 0<> [[ ;
: )-full?  ( slots-sys -- )  ( runtime: -- f )
  ] 1- slot-mask ]]L  paradigm-slot-bitmap @  AND 0<> [[ ;
[THEN]

flag/VARIABLE paradigm-flags-ptr
: paradigm-flags  ]] paradigm-flags-ptr flag/@ [[ ; IMMEDIATE
: paradigm-flags! ]] paradigm-flags-ptr flag/! [[ ; IMMEDIATE

flag/VARIABLE local-flag
: local-flag@ ]] local-flag flag/@ [[ ; IMMEDIATE
: local-flag! ]] local-flag flag/! [[ ; IMMEDIATE

: flag-empty?  ( ud -- f )
  paradigm-flags flag/AND flag/0= ;
: flag-is?  ( ud -- f )
  paradigm-flags flag/AND flag/0<> ;
: flag-any?  ( ud -- f )
  POSTPONE flag/0<> ; IMMEDIATE

: flag-set  ( flag -- )
  \\." setting flag "  flag/DUP flags.  BL EMIT
  paradigm-flags flag/OR  paradigm-flags!
  \\." flags are " paradigm-flags flags.  CR
  ;
: flag-clear  ( flag -- )
  \\." clearing flag "  flag/DUP flags.  BL EMIT
  flag/INVERT paradigm-flags flag/AND  paradigm-flags!
  \\." flags are " paradigm-flags flags. CR
  ;
: flag-with  ( "name" -- )
  PARSE-NAME flagtype/FIND-NAME ?DUP-IF  ( nt )
    NAME>INT EXECUTE          ( flag )
    flag/DUP flag/]]L flag-set [[
    local-flag!
  ELSE 1 ABORT"  word not found!" THEN ; IMMEDIATE

0 VALUE (<this>)
: slot:  ( slot-pos -- )
  DUP TO (<this>)
  :noname POSTPONE (slot-prolog) ;
: <this>  ( -- slot-pos )
  (<this>) POSTPONE LITERAL ; IMMEDIATE

: (compile-flag-if-exists)  ( D: affix-name -- )
  flagtype/FIND-NAME ?DUP-IF  ( ... nt )
    NAME>INT EXECUTE  ( mask )
    flag/DUP local-flag@ flag/AND flag/0= IF
      flag/DUP local-flag@ flag/OR  local-flag!  flag/]]L flag-set [[  ( ... )
    ELSE flag/DROP THEN
  THEN ;

: (compile-pop-flag)  ( -- )
  local-flag@ flag-any?  IF
    local-flag@ flag/]]L flag-clear [[
    flag-none local-flag!
  THEN ;

: form"  ( "name affix" -- )
  BL PARSE [CHAR] " PARSE morphonemic-to-sstr-and-rule 2SWAP  ( sstr rule affix-name name-len )
  2DUP (compile-flag-if-exists)
  POSTPONE SLITERAL  POSTPONE (form-prolog)                                       ( sstr rule )
  POSTPONE LITERAL  POSTPONE LITERAL  POSTPONE (form-epilog)
  (compile-pop-flag) ; IMMEDIATE

