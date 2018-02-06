: flag  ( u "name" -- )
  \ debug-mode? IF
  \   CELL 4 = IF 2VALUE ELSE DROP VALUE THEN
  \ ELSE
    CELL 4 = IF 2CONSTANT ELSE D>S CONSTANT THEN
  \ THEN
  ; IMMEDIATE

: flag-mask  ( u "name" -- )
  \ debug-mode? IF
  \   CELL 4 = IF 2VALUE ELSE DROP VALUE THEN
  \ ELSE
    CELL 4 = IF 2CONSTANT ELSE CONSTANT THEN
  \ THEN
  ; IMMEDIATE

\ flags in different slots must be distinct!
%000000000000000000000000000000000000000001. flag flag-Abl₂
%000000000000000000000000000000000000000010. flag flag-All1
%000000000000000000000000000000000000000100. flag flag-Ass₁
%000000000000000000000000000000000000001000. flag flag-Ass₂
%000000000000000000000000000000000000010000. flag flag-Ass₃
%000000000000000000000000000000000000100000. flag flag-Cond
%000000000000000000000000000000000001000000. flag flag-Convₚ
%000000000000000000000000000000000010000000. flag flag-Conv2
%000000000000000000000000000000000100000000. flag flag-Cunc
%000000000000000000000000000000001000000000. flag flag-Dur
%000000000000000000000000000000010000000000. flag flag-Dur1ᵢ
%000000000000000000000000000000100000000000. flag flag-Dur1ᵢᵣ
%000000000000000000000000000001000000000000. flag flag-Dur.Iter
%000000000000000000000000000010000000000000. flag flag-Futₐ
%000000000000000000000000000100000000000000. flag flag-Futₐᵣ
%000000000000000000000000001000000000000000. flag flag-Gen₁
%000000000000000000000000010000000000000000. flag flag-Gen.3pos
%000000000000000000000000100000000000000000. flag flag-Hab₁
%000000000000000000000001000000000000000000. flag flag-Hab₂
%000000000000000000000010000000000000000000. flag flag-Imp
%000000000000000000000100000000000000000000. flag flag-Imp.3
%000000000000000000001000000000000000000000. flag flag-Iter
%000000000000000000010000000000000000000000. flag flag-Neg
%000000000000000000100000000000000000000000. flag flag-Neg7
%000000000000000001000000000000000000000000. flag flag-Neg.Fut
%000000000000000010000000000000000000000000. flag flag-NF₀
%000000000000000100000000000000000000000000. flag flag-NF.Neg
%000000000000001000000000000000000000000000. flag flag-Opt-or-Assum
%000000000000010000000000000000000000000000. flag flag-Past
%000000000000100000000000000000000000000000. flag flag-Perf
%000000000001000000000000000000000000000000. flag flag-Person.br
%000000000010000000000000000000000000000000. flag flag-Poss1.nonpl
%000000000100000000000000000000000000000000. flag flag-Poss2.nonpl
%000000001000000000000000000000000000000000. flag flag-Pres
%000000010000000000000000000000000000000000. flag flag-PresPt.dial
%000000100000000000000000000000000000000000. flag flag-Prosp
%000001000000000000000000000000000000000000. flag flag-RPast
%000010000000000000000000000000000000000000. flag flag-1sg.br
%000100000000000000000000000000000000000000. flag flag-1.pl
%001000000000000000000000000000000000000000. flag flag-2pl.br
%010000000000000000000000000000000000000000. flag flag-2pos.pl
%100000000000000000000000000000000000000000. flag flag-3pos₁

flag-Dur flag-Pres flag-OR flag-PresPt.dial flag-OR   flag-mask flag-Dur-or-Pres-or-PresPt
flag-Hab₁ flag-Hab₂ flag-OR                           flag-mask flag-Hab
flag-RPast flag-Cond flag-OR                          flag-mask flag-RPast-or-Cond
flag-RPast-or-Cond flag-Pres flag-OR flag-Conv2 flag-OR    flag-mask flag-RPast-or-Cond-or-Pres-or-Conv2
flag-Past flag-Hab flag-OR                            flag-mask flag-Past-or-Hab
flag-Iter flag-Dur.Iter flag-OR
flag-Opt-or-Assum flag-OR flag-Cunc flag-OR
flag-Dur1ᵢᵣ flag-OR flag-Hab₂ flag-OR
flag-Futₐᵣ flag-OR flag-Neg.Fut flag-OR              flag-mask flag-Iter-or-Opt-or-Assum-or-Cunc-or-Neg.Fut-or-Dur1ᵢᵣ-or-Hab₂-or-Futₐᵣ
flag-Pres flag-Dur1ᵢ flag-OR flag-Past flag-OR
flag-Hab₁ flag-OR flag-Futₐ flag-OR                flag-mask flag-Pres-or-Dur1ᵢ-or-Past-or-Hab₁-or-Futₐ
