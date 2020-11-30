: +enum  ( enumvalue "name" -- enumvalue' )
  1+ DUP CONSTANT ;
: enum:  ( -- 0 )
  0 ;
: enum;  ( enumvalue -- )
  DROP ;

enum:
  +enum <Distr>
  +enum <Voice>
  +enum <NF,Dur₁>
  +enum <Ptcl1>
  +enum <Perf>
  +enum <Prosp>
  +enum <Dur>
  +enum <Neg/Iter>
  +enum <Tense/Mood/Conv2>
  +enum <Transp>
  +enum <Pl₁>
  +enum <Poss₁>
  +enum <Case₁>
  +enum <Attr>
  +enum <Pl₂>
  +enum <Poss₂>
  +enum <Case₂>
  +enum <Ptcl₂>
  +enum <Affirm>
  +enum <Person>
  +enum <PredPl>
  +enum <Ptcl₃>
enum;
