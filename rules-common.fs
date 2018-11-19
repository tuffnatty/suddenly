REQUIRE strings.fs
language-require phonetics.fs

: vowel-long?  ( addr u -- f )
  2 cyrs < IF DROP FALSE EXIT THEN  ( addr )
  XC@+ SWAP XC@ = ;

: vowel-long-middle?  ( ptr -- f )
  DUP XC@ SWAP XCHAR- XC@ = ;

: rule-cv   ( addr u -- addr u table-index )
  2DUP last-sound class-cv ;
: rule-cv ['] rule-cv ;

: last-sound-except-ь-ptr  ( addr u -- addr' )
  2DUP last-sound [CHAR] ь = IF prev-sound-ptr ELSE last-sound-ptr THEN ;

: last-sound-except-ь  ( addr u -- xc )
  last-sound-except-ь-ptr XC@ ;

: rule-cv-fb   ( addr u -- addr u table-index )
  2DUP last-sound class-cv >R 2DUP last-char-vowel class-fb 2* R> + ;
: rule-cv-fb ['] rule-cv-fb ;

: rule-fb      ( addr u -- addr u table-index )
  2DUP last-char-vowel class-fb ;
: rule-fb ['] rule-fb ;

: rule-nvu  ( addr u -- addr u table-index )
  2DUP last-sound-except-ь class-nvu ;
: rule-nvu ['] rule-nvu ;

: rule-nvu-fb  ( addr u -- addr u table-index )
  2DUP last-sound-except-ь class-nvu >R 2DUP last-char-vowel class-fb 3 * R> + ;
: rule-nvu-fb ['] rule-nvu-fb ;

: rule-vu  ( addr u -- addr u table-index )
  2DUP last-sound-except-ь class-vu ;
: rule-vu ['] rule-vu ;

: rule-vu-fb   ( addr u -- addr u table-index )
  2DUP last-sound-except-ь class-vu >R 2DUP last-char-vowel class-fb 2* R> + ;
: rule-vu-fb ['] rule-vu-fb ;

\ : rule-cv-nvu-fb  ( addr u -- addr u table-index )
\   2DUP last-sound class-cv >R  ( addr u R: class-cv )
\   R@ cl-vowel = IF cl-voiced ELSE R@ class-nvu THEN 2* 2*
: rule-cv-vu  ( addr u -- addr u table-index )
  2DUP last-sound-except-ь >R R@ class-cv  ( addr u class-cv  R: xc )
  DUP cl-vowel = IF cl-voiced RDROP ELSE R> class-vu THEN 2* + ;
: rule-cv-vu ['] rule-cv-vu ;

: rule-cv-nvu  ( addr u -- addr u table-index )
  2DUP last-sound-except-ь >R R@ class-cv  ( addr u class-cv  R: xc )
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
  2DUP rule-vu-fb rule-cv D= IF 2DROP rule-cv-vu-fb EXIT THEN
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
  2DUP rule-cv-vu-fb rule-fb D= IF 2DROP rule-cv-vu-fb EXIT THEN
  2DUP rule-cv-vu-fb rule-vu-fb D= IF 2DROP rule-cv-vu-fb EXIT THEN
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
  2DUP rule-cv-vu-fb rule-vu-fb D= IF 2DROP DUP 2 MOD 2* SWAP 2/ 2/ + EXIT THEN
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

: envoiced-copy  ( addr u -- addr1 u )
  string-copy  OVER  DUP XC@ envoice  SWAP XC! ;

: unvoice  ( u -- u )
  voiceds# CELLS 0 ?DO
    voiceds I + @ OVER = IF
      DROP unvoiceds I + @ LEAVE
    THEN
  CELL +LOOP ;

: polysyllabic?  ( addr u -- f )
  OVER + SWAP ( cs-end ptr )
  0 { cnt }
  BEGIN 2DUP > WHILE
    XC@+ vowel? IF
      cnt IF 2DROP TRUE EXIT THEN
      1 TO cnt
    THEN
  REPEAT 2DROP FALSE ;
: polysyllabic-cs?  ( cs -- f )
  COUNT polysyllabic? ;

VARIABLE paradigm-p-o-s
VARIABLE paradigm-stems
2VARIABLE paradigm-stem
VARIABLE paradigm-dict-flags
REQUIRE p-o-s.fs
: verb?  ( -- f )
  paradigm-p-o-s @  pos-v = ;

: nomen?  ( -- f )
  paradigm-p-o-s @  pos-n = ;

: indecl?  ( -- f )
  paradigm-p-o-s @  pos-i = ;

: invar1?  ( -- f )
  paradigm-p-o-s @  pos-i1 = ;

: dictflag-empty?  ( dictflag -- f )
  paradigm-dict-flags @ AND 0= ;

: dictflag-is?  ( dictflag -- f )
  paradigm-dict-flags @ AND ;

: guessed-stem  ( -- addr u )
  paradigm-stem 2@ ;
