REQUIRE khakas/phonetics.fs

%001 CONSTANT dictflag-no-envoice
%010 CONSTANT dictflag-poss
%100 CONSTANT dictflag-rus

:noname  ( dictflags -- )
  ." <"
  DUP dictflag-no-envoice AND IF ." +no-envoice" THEN
  DUP dictflag-poss AND IF ." +poss" THEN
  DUP dictflag-rus AND IF ." +rus" THEN
  DROP ." >"
  ; IS .dictflags

: stem-postprocess  { D: stem dict -- }
  stem last-sound [CHAR] т = IF
    stem prev-sound vowel? IF
      dict dict-headword COUNT  stem  STRING-PREFIX? IF
        dictflag-no-envoice dict dict-flags !
  THEN THEN THEN
  ;


GET-CURRENT dictionary-wordlist SET-CURRENT

: +poss  ( -- )
  dictflag-poss dictionary-ptr @ dict-flags +! ;

: +rus  ( -- )
  dictflag-rus dictionary-ptr @ dict-flags +! ;

SET-CURRENT
