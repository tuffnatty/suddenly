REQUIRE phonetics.fs
REQUIRE strings.fs

: vowel-long?  ( ptr -- f )
  DUP XC@ SWAP XCHAR+ XC@ = ;

: vowel-long-middle?  ( ptr -- f )
  DUP XC@ SWAP XCHAR- XC@ = ;

: rule-cv   ( addr u -- addr u table-index )
  2DUP last-sound class-cv ;
: rule-cv ['] rule-cv ;

: rule-cv-fb   ( addr u -- addr u table-index )
  2DUP last-sound class-cv >R 2DUP last-char-vowel class-fb 2* R> + ;
: rule-cv-fb ['] rule-cv-fb ;

: rule-fb      ( addr u -- addr u table-index )
  2DUP last-char-vowel class-fb ;
: rule-fb ['] rule-fb ;

: rule-nvu  ( addr u -- addr u table-index )
  2DUP last-sound class-nvu ;
: rule-nvu ['] rule-nvu ;

: rule-nvu-fb  ( addr u -- addr u table-index )
  2DUP last-sound class-nvu >R 2DUP last-char-vowel class-fb 3 * R> + ;
: rule-nvu-fb ['] rule-nvu-fb ;

: rule-vu  ( addr u -- addr u table-index )
  2DUP last-sound class-vu ;
: rule-vu ['] rule-vu ;

: rule-vu-fb   ( addr u -- addr u table-index )
  2DUP last-sound class-vu >R 2DUP last-char-vowel class-fb 2* R> + ;
: rule-vu-fb ['] rule-vu-fb ;

\ : rule-cv-nvu-fb  ( addr u -- addr u table-index )
\   2DUP last-sound class-cv >R  ( addr u R: class-cv )
\   R@ cl-vowel = IF cl-voiced ELSE R@ class-nvu THEN 2* 2*
: rule-cv-vu  ( addr u -- addr u table-index )
  2DUP last-sound >R R@ class-cv  ( addr u class-cv  R: xc )
  DUP cl-vowel = IF cl-voiced RDROP ELSE R> class-vu THEN 2* + ;
: rule-cv-vu ['] rule-cv-vu ;

: rule-cv-nvu  ( addr u -- addr u table-index )
  2DUP last-sound >R R@ class-cv  ( addr u class-cv  R: xc )
  DUP cl-vowel = IF cl-voiced RDROP ELSE R> class-nvu THEN 2* + ;
: rule-cv-nvu ['] rule-cv-nvu ;

: rule-cv-vu-fb  ( addr u -- addr u table-index )
  rule-cv-vu EXECUTE  ( addr u class-cv-vu )
  >R 2DUP last-char-vowel class-fb R> 2* + ;
: rule-cv-vu-fb ['] rule-cv-vu-fb ;

: rule-cv-nvu-fb  ( addr u -- addr u table-index )
  rule-cv-nvu EXECUTE  ( addr u class-cv-nvu )
  >R 2DUP last-char-vowel class-fb R> 2* + ;
: rule-cv-nvu-fb ['] rule-cv-nvu-fb ;

: .rule  ( xt -- )
  >NAME ?DUP-IF .NAME ELSE ." rule-0" THEN ;

: rule+  ( rule1 rule2 -- rule1+rule2 )
  DUP 0= IF DROP EXIT THEN
  2DUP = IF DROP EXIT THEN
  DUP rule-cv-nvu-fb = IF NIP EXIT THEN
  2DUP rule-cv-fb rule-vu-fb D= IF 2DROP rule-cv-vu-fb EXIT THEN
  2DUP rule-vu-fb rule-cv-fb D= IF 2DROP rule-cv-vu-fb EXIT THEN
  2DUP rule-fb rule-nvu-fb D= IF 2DROP rule-nvu-fb EXIT THEN
  2DUP rule-vu rule-nvu-fb D= IF 2DROP rule-nvu-fb EXIT THEN
  2DUP rule-vu rule-fb D= IF 2DROP rule-vu-fb EXIT THEN
  2DUP rule-fb rule-vu D= IF 2DROP rule-vu-fb EXIT THEN
  2DUP rule-cv rule-vu D= IF 2DROP rule-cv-vu EXIT THEN
  2DUP rule-vu rule-cv D= IF 2DROP rule-cv-vu EXIT THEN
  2DUP rule-nvu rule-fb D= IF 2DROP rule-nvu-fb EXIT THEN
  2DUP rule-fb rule-nvu D= IF 2DROP rule-nvu-fb EXIT THEN
  2DUP rule-vu rule-nvu D= IF 2DROP rule-nvu EXIT THEN
  2DUP rule-nvu rule-vu D= IF 2DROP rule-nvu EXIT THEN
  2DUP rule-vu rule-vu-fb D= IF 2DROP rule-vu-fb EXIT THEN
  2DUP rule-fb rule-vu-fb D= IF 2DROP rule-vu-fb EXIT THEN
  2DUP rule-nvu rule-vu-fb D= IF 2DROP rule-nvu-fb EXIT THEN
  2DUP rule-cv-fb rule-vu D= IF 2DROP rule-cv-vu-fb EXIT THEN
  2DUP rule-vu-fb rule-fb D= IF 2DROP rule-vu-fb EXIT THEN
  2DUP rule-vu-fb rule-cv-vu-fb D= IF 2DROP rule-cv-vu-fb EXIT THEN
  2DUP rule-vu-fb rule-vu D= IF 2DROP rule-vu-fb EXIT THEN
  2DUP rule-vu-fb rule-nvu D= IF 2DROP rule-nvu-fb EXIT THEN
  2DUP rule-vu-fb rule-nvu-fb D= IF 2DROP rule-nvu-fb EXIT THEN
  2DUP rule-fb rule-cv D= IF 2DROP rule-cv-fb EXIT THEN
  2DUP rule-fb rule-cv-fb D= IF 2DROP rule-cv-fb EXIT THEN
  2DUP rule-fb rule-cv-vu D= IF 2DROP rule-cv-vu-fb EXIT THEN
  2DUP rule-fb rule-cv-vu-fb D= IF 2DROP rule-cv-vu-fb EXIT THEN
  2DUP rule-fb rule-cv-nvu D= IF 2DROP rule-cv-nvu-fb EXIT THEN
  2DUP rule-nvu rule-cv D= IF 2DROP rule-cv-nvu EXIT THEN
  ." Could not combine rules " >NAME .ID ." and " >NAME .ID
  ABORT ;

: rule-index-convert  ( n rule1 rule2 -- n' )
  2DUP = IF 2DROP EXIT THEN
  2DUP rule-nvu-fb rule-nvu D= IF 2DROP 3 MOD EXIT THEN
  2DUP rule-nvu-fb rule-vu D= IF 2DROP 3 MOD 2 MOD EXIT THEN
  2DUP rule-nvu-fb rule-fb D= IF 2DROP 3 / EXIT THEN
  2DUP rule-nvu-fb rule-vu-fb D= IF 2DROP 3 /MOD >R 2 MOD R> 2* + EXIT THEN
  2DUP rule-vu-fb rule-vu D= IF 2DROP 2 MOD EXIT THEN
  2DUP rule-vu-fb rule-fb D= IF 2DROP 2/ EXIT THEN
  2DUP rule-cv-fb rule-fb D= IF 2DROP 2/ EXIT THEN
  2DUP rule-cv-fb rule-cv D= IF 2DROP 2 MOD EXIT THEN
  2DUP rule-cv-nvu rule-cv D= IF 2DROP 2 MOD EXIT THEN
  2DUP rule-cv-nvu rule-nvu D= IF 2DROP 2/ EXIT THEN
  2DUP rule-cv-vu rule-cv D= IF 2DROP 2 MOD EXIT THEN
  2DUP rule-cv-vu rule-vu D= IF 2DROP 2/ EXIT THEN
  2DUP rule-cv-nvu-fb rule-fb D= IF 2DROP 2 MOD EXIT THEN
  2DUP rule-cv-nvu-fb rule-cv D= IF 2DROP 2/ 2 MOD EXIT THEN
  2DUP rule-cv-nvu-fb rule-nvu D= IF 2DROP 2/ 2/ EXIT THEN
  2DUP rule-cv-vu-fb rule-fb D= IF 2DROP 2 MOD EXIT THEN
  2DUP rule-cv-vu-fb rule-cv D= IF 2DROP 2/ 2 MOD EXIT THEN
  2DUP rule-cv-vu-fb rule-vu D= IF 2DROP 2/ 2/ EXIT THEN
  2DUP rule-cv-vu-fb rule-cv-fb D= IF 2DROP 4 MOD EXIT THEN
  2DUP rule-cv-vu-fb rule-vu-fb D= IF 2DROP DUP 2 MOD 2* SWAP 2 MOD + EXIT THEN
  ." Could not convert index to rule " >NAME .ID ."  from " >NAME .ID ."  index " .
  ABORT ;

: rule-capacity  ( rule -- n )
  DUP 0= IF DROP 1 EXIT THEN
  DUP rule-nvu-fb = IF DROP 6 EXIT THEN
  DUP rule-vu = IF DROP 2 EXIT THEN
  DUP rule-fb = IF DROP 2 EXIT THEN
  DUP rule-nvu = IF DROP 3 EXIT THEN
  DUP rule-vu-fb = IF DROP 4 EXIT THEN
  DUP rule-cv-fb = IF DROP 4 EXIT THEN
  DUP rule-cv-vu = IF DROP 4 EXIT THEN
  DUP rule-cv-nvu = IF DROP 6 EXIT THEN
  DUP rule-cv-vu-fb = IF DROP 8 EXIT THEN
  DUP rule-cv-nvu-fb = IF DROP 12 EXIT THEN
  ." Could not compute capacity of rule " >NAME .ID
  ABORT ;

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
VARIABLE paradigm-stems
2VARIABLE paradigm-stem
REQUIRE p-o-s.fs
: verb?  ( -- f )
  paradigm-p-o-s @  pos-v = ;

: indecl?  ( -- f )
  paradigm-p-o-s @  pos-i = ;
