CREATE form-name 0 C, 255 ALLOT

: form-ptr-full?  ( ptr -- f )
  C@ [CHAR] - <> ;

: form-push  ( c-addr len -- origlen )
  form-name COUNT >R R@ +    ( c-addr len endaddr R: origlen )
  [CHAR] - OVER C! 1+ >R     ( c-addr len R: endaddr origlen )
  OVER form-ptr-full? IF
    R> SWAP >R R@ CMOVE R>                  ( len R: origlen )
  ELSE RDROP 2DROP 0  ( 0 )  THEN
  form-name C@ + 1+ form-name C!                           ( )
  [CHAR] -  form-name COUNT + C!  R> ;

: form-name-at  ( depth -- ptr )
  form-name 1+ SWAP ( ptr depth )
  BEGIN DUP 0> WHILE
    >R DUP C@ [CHAR] - = IF R> 1- >R THEN
  1+ R> REPEAT
  DROP ;

: form-name-back  ( depth -- ptr )
  form-name COUNT + SWAP  ( name-end depth )
  1+ 0 ?DO  ( ptr )
    BEGIN 1- DUP form-name > WHILE
      DUP C@ [CHAR] - <> WHILE
    REPEAT THEN
  LOOP
  1+ ;
