require mini-oof2.fs

object CLASS
  2 CELLS 32 * +FIELD dstack-data
  VALUE: dstack-depth
END-CLASS DStack

: dstack-clear  ( dstack -- )
  >O -1 TO dstack-depth O> ;

: dstack:
  [: DStack new ;] STATIC-A WITH-ALLOCATER DUP CONSTANT dstack-clear ;

: dstack-nth-ptr  ( n -- addr )
  2 CELLS ]]L * dstack-data + [[ ; IMMEDIATE

: >dstack  ( d dstack -- )
  >O dstack-depth 1+ TO dstack-depth
  dstack-depth dstack-nth-ptr 2! O> ;

: dstack>  ( dstack -- )
  >O dstack-depth 1- TO dstack-depth O> ;

: dstack@  ( dstack -- d )
  >O dstack-depth dstack-nth-ptr 2@ O> ;

: dstack-pick  ( n dstack -- d )
  >O dstack-depth SWAP - DUP 0>= IF dstack-nth-ptr 2@ ELSE DROP $0. THEN O> ;

: .circled  ( n -- )
  DUP 35 > IF 12941 + XEMIT ELSE
  DUP 20 > IF 12861 + XEMIT ELSE
  DUP  0 > IF  9311 + XEMIT ELSE
  DROP 9450 XEMIT THEN THEN THEN ;

: (.dstack)  ( dstack enumerate -- )
  { enumerate }  >O dstack-depth 1+ BEGIN DUP WHILE
    DUP 1- dstack-nth-ptr 2@  ( content )
    enumerate IF DUP IF dstack-depth 1+ 3 PICK 1- - .circled THEN THEN
    TYPE ( )
    [CHAR] -  EMIT
  1- REPEAT DROP O> ;

: .dstack  ( dstack -- )
  FALSE (.dstack) ;

: .dstack-enum  ( dstack -- )
  TRUE (.dstack) ;
