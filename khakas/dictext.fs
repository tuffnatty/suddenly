REQUIRE ../cleave.fs
REQUIRE ./phonetics.fs

%001 CONSTANT dictflag-no-envoice
%010 CONSTANT dictflag-poss
%100 CONSTANT dictflag-rus

:noname  ( dictflags -- )
  ." <"
  cleave[ dictflag-no-envoice AND IF ." +no-envoice" THEN
       ][ dictflag-poss AND IF ." +poss" THEN
       ][ dictflag-rus AND IF ." +rus" THEN ];  ( )
  ." >"
  ; IS .dictflags

: stem-postprocess-auto-no-envoice  { D: stem dict -- }
  stem last-sound [CHAR] т = IF
    stem prev-sound vowel? IF
      dict dict-headword COUNT  stem  STRING-PREFIX? IF
        dictflag-no-envoice dict dict-flags !
  THEN THEN THEN
  ;
: stem-postprocess  { D: stem dict -- }
  \ stem dict stem-postprocess-auto-no-envoice
  ;


GET-CURRENT dictionary-wordlist SET-CURRENT

: +poss  ( -- )
  dictflag-poss dictionary-ptr @ dict-flags +! ;

: +rus  ( -- )
  dictflag-rus dictionary-ptr @ dict-flags +! ;

: +unvoiced ( -- )
  dictflag-no-envoice dictionary-ptr @ dict-flags +! ;

SET-CURRENT
