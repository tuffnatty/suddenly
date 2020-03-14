REQUIRE cleave.fs


STRUCT
  CELL% FIELD region-size
  CELL% FIELD region-addr
  CELL% FIELD region-here
END-STRUCT region%

: .region  ( region -- )
  ." region " DUP HEX.
  ."  size " DUP region-size @ .
  ."  addr " DUP region-addr @ HEX.
  ."  here " DUP region-here @ HEX.
  ."  free " DUP region-size @ SWAP  DUP region-here @ SWAP region-addr @ - - . CR ;

: region-make ( size -- )
  DUP ,
  DUP ALLOCATE ABORT" cannot allocate region"   DUP ROT 0 FILL   DUP , , ;

: region-offset  ( region -- offset )
  bi[  region-here @  ][  region-addr @  ];  - ;

: region-will-overflow?  ( size region -- f )
  bi[  region-offset +  ][  region-size @  ]; > ;

: region-allot  ( size region -- addr )
  2bi[  region-will-overflow? ABORT" region will overflow"
    ][  region-here +!@ ]; ;

: region-dispose  ( region -- )
  DUP region-offset ABORT" region is not empty"
  DUP region-addr @ FREE ABORT" wior while FREEing region"
  region% %SIZE 0 FILL ;
