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
