CREATE input-word 0 C, 255 ALLOT
: fix-percent ( addr len )
  0 ?DO
    DUP I + C@
    DUP [CHAR] % = IF
      DROP DUP I + 1+ 0 0 ROT 2 ['] >NUMBER 16 base-execute 2DROP D>S 3
    ELSE 1 THEN
    >R
    input-word DUP C@ + 1+ C!
    input-word C@ 1+ input-word C!
  R> +LOOP
  DROP
;

VARIABLE parse-mode
REQUIRE debugging.fs

: parse-args  ( ptr len -- )
  2DUP S" debug=" SEARCH IF  ( ptr len value n-rest )
    6 /STRING 2>R $0. 2R> >NUMBER 2DROP D>S TO debug-mode? ( ptr len )
  ELSE 2DROP THEN ( ptr len )
  \ 2DUP S" language=" SEARCH IF
  \   9 /STRING 2DUP S" &" SEARCH IF ( ptr len lang-start n-rest lang-end n-rest )
  \     DROP SWAP DROP OVER - ( ptr len lang-start lang-len )
  \     FPATH ALSO-PATH ( ptr len )
  \   ELSE 2DROP 2DROP THEN
  \ ELSE 2DROP THEN ( ptr len )
  2DUP S" parse=" SEARCH IF
    TRUE parse-mode !
    6 /STRING fix-percent input-word ." Parsing: " COUNT TYPE CR CR
  ELSE
    2DROP S" generate=" SEARCH IF
      9 /STRING fix-percent input-word ." Paradigm for: " COUNT TYPE CR CR
    ELSE ." arguments?" CR BYE THEN
  THEN
;
