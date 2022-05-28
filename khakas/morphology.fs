REQUIRE dictionary.fs

: is-reduplication?  ( addr u -- f )
  0 0 0  { C V1 V2 }
        ?DUP-0=-IF DROP 0 EXIT THEN             OVER XC@  consonant?   IF  +x/string@ TO C  THEN  ( addr' u')
        ?DUP-0=-IF DROP 0 EXIT THEN  +x/string@ TO V1  V1 vowel? NOT   IF 2DROP 0 EXIT THEN
        ?DUP-0=-IF DROP 0 EXIT THEN             OVER XC@  V1 =         IF +x/string@ TO V2  THEN
        ?DUP-0=-IF DROP 0 EXIT THEN  +x/string@           [CHAR] п <>  IF 2DROP 0 EXIT THEN
        ?DUP-0=-IF DROP 0 EXIT THEN  +x/string@           [CHAR] - <>  IF 2DROP 0 EXIT THEN
  C IF  ?DUP-0=-IF DROP 0 EXIT THEN  +x/string@           C        <>  IF 2DROP 0 EXIT THEN  THEN 
        ?DUP-0=-IF DROP 0 EXIT THEN  +x/string@           V1       <>  IF 2DROP 0 EXIT THEN
  V2 IF ?DUP-0=-IF DROP 0 EXIT THEN  +x/string@           V2       <>  IF 2DROP 0 EXIT THEN  THEN
  2DROP TRUE ;

: skip-reduplication  ( addr u -- addr' u' )
  S" -" SEARCH IF 1 /STRING THEN ;

: .reduplication  ( -- )
  dict-reduplication @ IF ." Magn" THEN ;
