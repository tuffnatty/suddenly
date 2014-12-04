REQUIRE phonetics.fs
REQUIRE strings.fs

: vowel-long?  ( ptr -- f )
  DUP XC@ SWAP XCHAR+ XC@ = ;

: vowel-long-middle?  ( ptr -- f )
  DUP XC@ SWAP XCHAR- XC@ = ;

: last-sound-ptr  ( addr u -- ptr )
  + XCHAR- ;

: prev-sound-ptr  ( addr u -- ptr )
  + XCHAR- XCHAR- ;

: last-char-vowel ( addr u -- u | 0 )
  OVER >R
  0 -ROT last-sound-ptr BEGIN ( vowel cs-cur  r: cs )
    DUP XC@ DUP unchar-vowel? IF  ( vowel cs-cur cs-cur@ )
      -ROT NIP XCHAR- DUP R@ <    ( vowel cs-cur 0 )
    ELSE DUP front-vowel? IF      ( vowel cs-cur cs-cur@ )
      -ROT NIP 1                  ( vowel cs-cur 1 )
    ELSE DUP back-vowel? IF
      -ROT NIP 1
    ELSE DROP XCHAR- DUP R@ <     ( vowel cs-next f )
    THEN THEN THEN
  UNTIL
  DROP RDROP ;

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


0 CONSTANT cl-voiced
1 CONSTANT cl-unvoiced
2 CONSTANT cl-nasal
: class-nvu  ( u -- class )
  DUP nasal? IF DROP cl-nasal
  ELSE unvoiced? IF cl-unvoiced
  ELSE cl-voiced THEN THEN ;
: class-vu  ( u -- class )
  unvoiced? IF cl-unvoiced ELSE cl-voiced THEN ;

0 CONSTANT cl-back
1 CONSTANT cl-front
: class-fb  ( u -- class )
  back-vowel? IF cl-back ELSE cl-front THEN ;

0 CONSTANT cl-consonant
1 CONSTANT cl-vowel
: class-cv  ( u -- class )
  vowel? IF cl-vowel ELSE cl-consonant THEN ;

: rule-cv-fb   ( addr u -- addr u table-index )
  2DUP last-sound class-cv >R 2DUP last-char-vowel class-fb 2* R> + ;
: rule-fb      ( addr u -- addr u table-index )
  2DUP last-char-vowel class-fb ;
: rule-nvu  ( addr u -- addr u table-index )
  2DUP last-sound class-nvu ;
: rule-nvu-fb  ( addr u -- addr u table-index )
  2DUP last-sound class-nvu >R 2DUP last-char-vowel class-fb 3 * R> + ;
: rule-vu  ( addr u -- addr u table-index )
  2DUP last-sound class-vu ;
: rule-vu-fb   ( addr u -- addr u table-index )
  2DUP last-sound class-vu >R 2DUP last-char-vowel class-fb 2* R> + ;

: .rule  ( xt -- )
  >NAME ?DUP-IF .NAME ELSE ." 0" THEN ;

: envoice  ( u -- u )
  unvoiceds# CELLS 0 ?DO
    unvoiceds I + @ OVER = IF
      DROP voiceds I + @ LEAVE
    THEN
  CELL +LOOP ;

: unvoice  ( u -- u )
  voiceds# CELLS 0 ?DO
    voiceds I + @ OVER = IF
      DROP unvoiceds I + @ LEAVE
    THEN
  CELL +LOOP ;

: polysyllabic?  ( cs -- f )
  0 SWAP DUP COUNT + SWAP 1+  ( cnt cs-end ptr )
  BEGIN 2DUP > WHILE
    XC@+ vowel? IF
      ROT IF DROP EXIT THEN       ( cs-end ptr )
      1 -ROT
    THEN
  REPEAT 2DROP DROP 0 ;

VARIABLE paradigm-p-o-s
REQUIRE p-o-s.fs
: verb?  ( -- f )
  paradigm-p-o-s @  pos-v = ;

