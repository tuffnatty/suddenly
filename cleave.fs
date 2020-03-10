: bi  ( x xt1 xt2 -- )
  \G apply both xts to x
  >R OVER >R EXECUTE  ( R: x xt2 )
  R> R> EXECUTE ;

: 2bi  ( x y xt1 xt2 -- )
  \G apply both xts to x y
  2OVER 2>R >R EXECUTE R> 2R> ROT EXECUTE ;

: tri  ( x xt1 xt2 xt3 -- )
  \G apply all three xts to x
  >R >R OVER >R EXECUTE  ( R: x xt2 xt3 )
  R> R> OVER >R EXECUTE  ( R: x xt3 )
  R> R> EXECUTE ;

: cleave[  ( C: -- cleave-sys )
  ]] >R R@ [[ 1 ; IMMEDIATE
: 2cleave[  ( C: -- cleave-sys )
  ]] 2>R 2R@ [[ 2 ; IMMEDIATE
: ][  ( C: cleave-sys -- cleave-sys )
  DUP 1 = IF  ]] R@ [[  ELSE  ]] 2R@ [[  THEN ; IMMEDIATE
: ];  ( C: cleave-sys -- )
  1 = IF  ]] RDROP [[  ELSE  ]] 2RDROP [[  THEN ; IMMEDIATE

: bi[
  ]] cleave[ [[ ; IMMEDIATE
: 2bi[
  ]] 2cleave[ [[ ; IMMEDIATE
