require mini-oof2.fs

object CLASS
  2 CELLS 32 * +FIELD dstack-data
  VALUE: dstack-depth
END-CLASS DStack

: dstack:
  [: DStack new ;] STATIC-A WITH-ALLOCATER DUP CONSTANT >O -1 TO dstack-depth O> ;

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

: .dstack  ( dstack -- )
  >O dstack-depth 1+ BEGIN DUP WHILE
    DUP 1- dstack-nth-ptr 2@ TYPE
    [CHAR] -  EMIT
  1- REPEAT DROP O> ;
