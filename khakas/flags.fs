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
%000000000000000000000000000000000001. flag flag-Abl2
%000000000000000000000000000000000010. flag flag-All1
%000000000000000000000000000000000100. flag flag-Ass3
%000000000000000000000000000000001000. flag flag-Cond
%000000000000000000000000000000010000. flag flag-Conv.Neg
%000000000000000000000000000000100000. flag flag-Conv.p
%000000000000000000000000000001000000. flag flag-Conv2
%000000000000000000000000000010000000. flag flag-Cunc
%000000000000000000000000000100000000. flag flag-Dur
%000000000000000000000000001000000000. flag flag-Dur1.i
%000000000000000000000000010000000000. flag flag-Dur1.ir
%000000000000000000000000100000000000. flag flag-Dur.Iter
%000000000000000000000001000000000000. flag flag-Fut.a
%000000000000000000000010000000000000. flag flag-Fut.ar
%000000000000000000000100000000000000. flag flag-Gen.3pos
%000000000000000000001000000000000000. flag flag-Hab.ca
%000000000000000000010000000000000000. flag flag-Hab.cang
%000000000000000000100000000000000000. flag flag-Imp
%000000000000000001000000000000000000. flag flag-Imp.3
%000000000000000010000000000000000000. flag flag-Iter
%000000000000000100000000000000000000. flag flag-Neg6
%000000000000001000000000000000000000. flag flag-Neg7
%000000000000010000000000000000000000. flag flag-Neg.Fut
%000000000000100000000000000000000000. flag flag-Opt-or-Assum
%000000000001000000000000000000000000. flag flag-Past
%000000000010000000000000000000000000. flag flag-Perf
%000000000100000000000000000000000000. flag flag-Person.br
%000000001000000000000000000000000000. flag flag-Poss1.nonpl
%000000010000000000000000000000000000. flag flag-Poss2.nonpl
%000000100000000000000000000000000000. flag flag-Pres
%000001000000000000000000000000000000. flag flag-RPast
%000010000000000000000000000000000000. flag flag-1sg.br
%000100000000000000000000000000000000. flag flag-1.pl
%001000000000000000000000000000000000. flag flag-2pl.br
%010000000000000000000000000000000000. flag flag-2pos.pl
%100000000000000000000000000000000000. flag flag-3pos1

flag-Dur flag-Pres flag-OR                            flag-mask flag-Dur-or-Pres
flag-Hab.ca flag-Hab.cang flag-OR                     flag-mask flag-Hab
flag-RPast flag-Cond flag-OR                          flag-mask flag-RPast-or-Cond
flag-RPast-or-Cond flag-Pres flag-OR flag-Conv2 flag-OR    flag-mask flag-RPast-or-Cond-or-Pres-or-Conv2
flag-Past flag-Hab flag-OR                            flag-mask flag-Past-or-Hab
flag-Iter flag-Dur.Iter flag-OR
flag-Opt-or-Assum flag-OR flag-Cunc flag-OR
flag-Dur1.ir flag-OR flag-Hab.cang flag-OR
flag-Fut.ar flag-OR flag-Neg.Fut flag-OR              flag-mask flag-Iter-or-Opt-or-Assum-or-Cunc-or-Neg.Fut-or-Dur1.ir-or-Hab.cang-or-Fut.ar
flag-Pres flag-Dur1.i flag-OR flag-Past flag-OR
flag-Hab.ca flag-OR flag-Fut.a flag-OR                flag-mask flag-Pres-or-Dur1.i-or-Past-or-Hab.ca-or-Fut.a
