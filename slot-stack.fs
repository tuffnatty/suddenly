REQUIRE util.fs

CREATE slot-stack 0 , 32 CELLS ALLOT

: slot-stack@  ( -- xt | 0 )
  slot-stack DUP @ 1+ CELLS + @ ;

: slot-stack-pop  ( -- xt|0 )
  slot-stack 1 OVER +!  ( stk )
  DUP @ CELLS + @ ;

: slot-stack-push  ( -- )
  -1 slot-stack +! ;

: slot-stack-reset  ( -- )
  0 slot-stack ! ;

: slot-stack-set  ( slot-stack slot-stack-len -- )
  slot-stack-reset SWAP slot-stack CELL+ ROT 1+ CELLS CMOVE ;

: slot-stack-reverse  ( -- )
  slot-stack CELL+ DUP BEGIN
    DUP @ 0<> WHILE
    CELL+
  REPEAT SWAP - CELL /                     ( len )
  slot-stack CELL+ SWAP array-reverse ;

: .slot-stack  ( -- )
  slot-stack
  ." Depth:" DUP @ .
  BEGIN CELL+
    DUP @ 0<>
  WHILE
    DUP @ >NAME DUP IF .ID ELSE DROP ." :noname " THEN
  REPEAT DROP CR ;
