: NOT  ( f -- !f )
  POSTPONE 0= ; IMMEDIATE

: ||  ( f  R: x r -- true R: x PC: r | R: x r )  \ OR with boolean shortcircuiting
  ]] IF TRUE EXIT THEN [[ ; IMMEDIATE

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
: postpone-literal  postpone  literal ;
: ]]L ( postponing: x -- ; compiling: -- x )
    \ Shortcut for @code{]] literal}.
    ]] postpone-literal ]] [[ ; immediate
[THEN]
