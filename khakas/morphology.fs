REQUIRE dictionary.fs

: is-reduplication?  ( addr u -- f )
  0 0 { C V }
        ?DUP-0=-IF DROP FALSE EXIT THEN  OVER XC@  consonant?   IF  +x/string@ TO C  THEN  ( addr' u')
        ?DUP-0=-IF DROP FALSE EXIT THEN  +x/string@        TO V  V vowel? NOT  IF 2DROP FALSE EXIT THEN
        ?DUP-0=-IF DROP FALSE EXIT THEN  +x/string@               [CHAR] п <>  IF 2DROP FALSE EXIT THEN
        ?DUP-0=-IF DROP FALSE EXIT THEN  +x/string@               [CHAR] - <>  IF 2DROP FALSE EXIT THEN
  C IF  ?DUP-0=-IF DROP FALSE EXIT THEN  +x/string@               C        <>  IF 2DROP FALSE EXIT THEN  THEN 
        ?DUP-0=-IF DROP FALSE EXIT THEN  +x/string@               V        <>  IF 2DROP FALSE EXIT THEN
        ?DUP-0=-IF DROP TRUE EXIT THEN   +x/string@  [CHAR] п =  OVER 0=  AND  IF 2DROP FALSE EXIT THEN
  2DROP TRUE ;

: skip-reduplication  ( addr u -- addr' u' )
  S" -" SEARCH IF 1 /STRING THEN ;

: .reduplication  ( -- )
  dict-reduplication @ IF ." Magn" THEN ;
