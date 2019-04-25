: +enum  ( enumvalue "name" -- enumvalue' )
  1+ DUP CONSTANT ;
: enum:  ( -- 0 )
  0 ;
: enum;  ( enumvalue -- )
  DROP ;

enum:
  +enum <Distr>  \ 1
  +enum <NF>  \ 2
  +enum <Ptcl1>  \ 3
  +enum <Perf>  \ 4
  +enum <Prosp,Dur1>  \ 5
  +enum <Dur>  \ 6
  +enum <Neg/Iter>  \ 7
  +enum <Tense/Mood/Conv2>  \ 8
  +enum <Transp>  \ 9
  +enum <Pl₁>  \ 10
  +enum <Poss₁>  \ 11
  +enum <Case₁>  \ 12
  +enum <Attr>  \ 13
  +enum <Pl₂>  \ 14
  +enum <Poss₂>  \ 15
  +enum <Case₂>  \ 16
  +enum <Ptcl₂>  \ 17
  +enum <Affirm>  \ 18
  +enum <Person>  \ 19
  +enum <PredPl>  \ 20
  +enum <Ptcl₃>  \ 21
enum;
