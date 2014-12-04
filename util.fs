: NOT  ( f -- !f )
  POSTPONE 0= ; IMMEDIATE

: array-reverse  ( arr len -- )
  1- CELLS OVER + BEGIN 2DUP < WHILE  ( a1 a2 )
    2DUP 2DUP @ SWAP @ ROT ! SWAP !
    CELL - SWAP CELL+ SWAP          ( a1' a2' )
  REPEAT 2DROP ;

\ semi-lambda words
: :[  ( -- )
  POSTPONE AHEAD :noname ; IMMEDIATE
: ];  ( -- xt )
  POSTPONE ; ] >R POSTPONE THEN R> POSTPONE LITERAL ; IMMEDIATE

: rdepth rp@ rp0 - ;  
