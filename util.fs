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
: :[: ( <name> -- )
  POSTPONE AHEAD : ; IMMEDIATE
: ];  ( -- xt )
  POSTPONE ; ] POSTPONE THEN LATESTXT POSTPONE LITERAL ; IMMEDIATE

: rdepth rp@ rp0 - ;

[IFUNDEF] ]]L
: ]]L ]] postpone-literal ]]  [[ ; immediate
[THEN]
