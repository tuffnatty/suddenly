: +enum  ( enumvalue "name" -- enumvalue' )
  1+ DUP CONSTANT ;
: enum:  ( -- 0 )
  0 ;
: enum;  ( enumvalue -- )
  DROP ;

enum:
  +enum <Distr>
  +enum <Voice>
  +enum <NF,Dur1>
  +enum <Ptcl1>
  +enum <Perf>
  +enum <Prosp>
  +enum <Dur>
  +enum <Neg/Gener>
  +enum <Tense/Mood/Conv>
  +enum <Pl₁>
  +enum <Poss₁>
  +enum <Case₁>
  +enum <Transp>
  +enum <Pl₂>
  +enum <Poss₂>
  +enum <Case₂>
  +enum <Ptcl₂>
  +enum <Vis>
  +enum <Person>
  +enum <PredPl>
  +enum <Ptcl₃>
enum;
