: bl/string  ( addr len -- addr' len' )
  BEGIN DUP 0 > WHILE
    OVER C@ BL = WHILE
    1 /STRING
  REPEAT THEN ;

: nbl/string  ( addr len -- addr' len' )
  BEGIN DUP 0 > WHILE
    OVER C@ BL <> WHILE
    1 /STRING
  REPEAT THEN ;

: nth-word  ( n addr len -- addr1 len1 )
  OVER SWAP               ( n addr addr len )
  nbl/string bl/string  ( n addr addr' len' )
  >R OVER -        ( n addr recsize R: len' )
  DUP R> + >R       ( n addr recsize R: len )
  ROT * DUP >R + R> R> SWAP -  ( addr' len' )
  OVER >R             ( addr' len' R: addr' )
  nbl/string
  DROP R@ - R> SWAP ;
