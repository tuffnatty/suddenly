0 CONSTANT pos-i
1 CONSTANT pos-n
2 CONSTANT pos-v

: .p-o-s  ( p-o-s -- )
  DUP pos-v = IF DROP ." v"
  ELSE pos-n = IF ." n"
  ELSE ." i" THEN THEN ;
