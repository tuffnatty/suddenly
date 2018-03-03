0 CONSTANT pos-i
1 CONSTANT pos-i1
2 CONSTANT pos-n
3 CONSTANT pos-v

: .p-o-s  ( p-o-s -- )
  CASE
    pos-v OF ." v" ENDOF
    pos-n OF ." n" ENDOF
    pos-i1 OF ." i₁" ENDOF
    ." i"
  ENDCASE ;
