\ after entering new data, repeat this command until no matches:
\ :3,$s/"\(.*\)\([aceiopxyöÿ]\)/\="\"" . submatch(1) . tr(submatch(2),"aceiopxyöÿ", "асеіорхуӧӱ")/gI

require khakas/flags.fs
require khakas/constraints.fs


slot: <Distr>  \ 1
  \ Навесим глобальные фильтры на 1-ю позицию с любым
  \ (в том числе нулевым!) аффиксом

  filter-start constraint-0
    filter-start constraint-1
      filter-start constraint-2
        filter-start constraint-27
          filter-start constraint-non-envoiceable-stem
            filter-start constraint-(СА|ТІ)ңАр-fallout
              filter-start constraint-[Гң]Г-fallout
                filter-start constraint-V[кх]V-fallout
                  filter-start constraint-VГV-fallout
                    filter-start constraint-VVГV-fallout
                      filter-start constraint-VңV-fallout
                        filter-start constraint-CCC-fallout
                          1 slot-empty!
                          form" -nodistr "

                          1 slot-full!
                          form" Distr КлА"
                        filter-end
                      filter-end
                    filter-end
                  filter-end
                filter-end
              filter-end
            filter-end
          filter-end
        filter-end
      filter-end
    filter-end
  filter-end
  ;

slot: <Conv1>  \ 2
  2 slot-empty!
  form" -noconv1 "

  filter-start constraint-4
    filter-start constraint-11
      2 slot-full!

      form" Conv.p (І)п"

      flag-Conv.Neg flag-set
        form" Conv.Neg Пин"
      flag-Conv.Neg flag-clear
    filter-end
  filter-end
  ;

slot: <Ptcl1>  \ 3
  3 slot-empty!
  form" -noptcl1 "

  filter-start constraint-5
    3 slot-full!
    form" Add ТАА"
    form" Cont LА"
    form" Ass1 ОQ"
  filter-end
  ;

slot: <Perf/Prosp>  \ 4
  4 slot-empty!
  form" -noperf "

  4 slot-full!

  filter-start constraint-3
    flag-Perf flag-set
      form" Perf (І)бІс"
    flag-Perf flag-clear
  filter-end

  filter-start constraint-6
    form" Prosp АК"
  filter-end
  ;

slot: <Dur>  \ 5
  5 slot-empty!
  form" -nodur "

  5 slot-full!
  filter-start constraint-7
    filter-start constraint-8
      filter-start constraint-8.1ᵢ
        flag-Dur1.i flag-set
          form" Dur1₁ и"
        flag-Dur1.i flag-clear
      filter-end
      filter-start constraint-8.1ᵢᵣ
        flag-Dur1.ir flag-set
          form" Dur1₂ ир"
        flag-Dur1.ir flag-clear
      filter-end
    filter-end
  filter-end

  filter-start constraint-26₅
    flag-Dur flag-set
      form" Dur чАт"
    flag-Dur flag-clear
  filter-end
  ;

slot: <Neg/Iter>  \ 6
  6 slot-empty!
  form" -noneg/iter "

  filter-start constraint-11
    6 slot-full!

    filter-start constraint-11.1
      flag-Neg6 flag-set
      form" Neg ПА"
      flag-Neg6 flag-clear
    filter-end

    flag-Iter flag-set
      filter-start constraint-14
        form" Iter АдІр"
      filter-end
    flag-Iter flag-clear

    filter-start constraint-26₆
      flag-Dur.Iter flag-set
        form" Dur.Iter чАдІр"
      flag-Dur.Iter flag-clear
    filter-end
  filter-end
  ;

slot: <Tense/Mood/Conv2>  \ 7
  7 slot-empty!
  form" -notense "

  7 slot-full!

  filter-start constraint-26₇
    flag-Pres flag-set
      form" Pres чА"
    flag-Pres flag-clear
  filter-end

  flag-Neg7 flag-set
    flag-Neg.Fut flag-set
      form" Neg.Fut ПАс"
    flag-Neg.Fut flag-clear
  flag-Neg7 flag-clear

  flag-Past flag-set
    filter-start constraint-9.1
      form" Past₁ ГА"
    filter-else
      form" Past₂ ГАн"
    filter-end
  flag-Past flag-clear

  filter-start constraint-10.1
    filter-start constraint-9.3
      flag-Fut.a flag-set
        form" Fut₁ А"
      flag-Fut.a flag-clear
    filter-end
    flag-Fut.ar flag-set
      form" Fut₂ Ар"
    flag-Fut.ar flag-clear
  filter-end

  filter-start constraint-9.2
    flag-Hab.ca flag-set
      form" Hab₁ ЧА"
    flag-Hab.ca flag-clear
  filter-end
  flag-Hab.cang flag-set
    form" Hab₂ ЧАң"
  flag-Hab.cang flag-clear

  flag-RPast flag-set
    filter-start constraint-12
      form" RPast ТІ"
    filter-end
  flag-RPast flag-clear

  filter-start constraint-10
    flag-Cunc flag-set
      form" Cunc ГАлАQ"
    flag-Cunc flag-clear
  filter-end

  flag-Cond flag-set
    filter-start constraint-13
      form" Cond СА"
    filter-end
  flag-Cond flag-clear
  flag-Opt-or-Assum flag-set
    form" Opt ГАй"
    form" Assum ГАдАG"
  flag-Opt-or-Assum flag-clear

  filter-start constraint-25
    form" Lim ГАли"

    flag-Neg7 flag-set
      flag-Conv2 flag-set
        form" Neg.Conv Пин"
        form" Neg.Conv.Abl ПинАң"
      flag-Conv2 flag-clear
    flag-Neg7 flag-clear

    flag-Conv2 flag-set
      flag-Conv.p flag-set
        form" Conv.p (І)п"
      flag-Conv.p flag-clear
      form" Conv.pas АбАс"
      form" Conv.a А"
    flag-Conv2 flag-clear
  filter-end
  ;

slot: <Indir>  \ 8
  8 slot-empty!
  form" -noindir "

  filter-start constraint-15
    filter-start constraint-26₈
      8 slot-full!
      filter-start constraint-voicedstem+Indir
        form" Indir тІр"
      filter-else
        form" Indir ТІр"
      filter-end
    filter-end
  filter-end
  ;

slot: <Comit>  \ 9
  9 slot-empty!
  form" -nocomit "

  9 slot-full!

  form" Comit ЛІG"
  ;

slot: <Affirm>  \ 10
  10 slot-empty!
  form" -noaffirm "

  10 slot-full!
  form" Affirm ЧІQ"
  ;

slot: <Pl₁>  \ 11
  11 slot-empty!
  form" -nopl1 "

  filter-start constraint-16₁₁
    11 slot-full!
    form" Pl₁ ЛАр"
  filter-end
  ;

slot: <Poss₁>  \ 12
  12 slot-empty!
  form" -noposs1 "

  filter-start constraint-16₁₂
    filter-start constraint-31
      12 slot-full!

      flag-Poss1.nonpl flag-set
        form" 1pos.sg (І)м"
        \ filter-start( 12 form-slot-flags untransformed-fallout AND NOT )
          form" 2pos.sg (І)ң"
        \ filter-end
        flag-3pos1 flag-set
          form" 3pos (з)І"
        flag-3pos1 flag-clear
      flag-Poss1.nonpl flag-clear
      form" 1pos.pl (І)бІс"
      flag-2pos.pl flag-set
        form" 2pos.pl (І)ңАр"
      flag-2pos.pl flag-clear
    filter-end
  filter-end
  ;

slot: <Case₁>  \ 13
  13 slot-empty!
  form" -nocase1 "

  filter-start constraint-16_2а
    13 slot-full!

    form" Gen НІң"
  filter-end

  filter-start constraint-16_2б
    13 slot-full!

    filter-start constraint-17₁₃
      form" Loc ТА"
    filter-else
      form" Loc (н)ТА"
    filter-end
  filter-end

  filter-start constraint-16_2д
    13 slot-full!

    flag-All1 flag-set
      filter-start constraint-17₁₃
        form" All САр"
      filter-else
        form" All (н)САр"
      filter-end
    flag-All1 flag-clear
  filter-end
  ;

slot: <Attr>  \ 14
  14 slot-empty!
  form" -noattr "

  filter-start constraint-16_1б
    14 slot-full!
    form" Attr КІ"
  filter-end
  ;

slot: <Pl₂>  \ 15
  15 slot-empty!
  form" -nopl2 "

  15 slot-full!
  form" Pl₂ ЛАр"
  ;

slot: <Poss₂>  \ 16
  16 slot-empty!
  form" -noposs2 "

  16 slot-full!

  flag-Poss2.nonpl flag-set
    form" 1pos.sg (І)м"
    \ filter-start( 16 form-slot-flags untransformed-fallout AND NOT )
      form" 2pos.sg (І)ң"
    \ filter-end
    form" 3pos (з)І"
    flag-Gen.3pos flag-set
      form" Gen.3pos Ни"
    flag-Gen.3pos flag-clear
  flag-Poss2.nonpl flag-clear
  form" 1pos.pl (І)бІс"
  flag-2pos.pl flag-set
    form" 2pos.pl (І)ңАр"
  flag-2pos.pl flag-clear
  ;

slot: <Case₂>  \ 17
  17 slot-empty!
  form" -nocase2 "

  17 slot-full!

  filter-start constraint-29
    form" Gen НІң"
    form" Instr нАң"
  filter-end

  filter-start constraint-17₁₇
    form" Dat ГА"
    filter-start constraint-29
      form" Acc НІ"
    filter-end
    form" Loc ТА"
    form" Abl₁ ДАң"
    form" All САр"
    form" Prol ЧА"
    form" Delib ДАңАр"
    form" Comp ТАG"
    filter-start constraint-30
      flag-Abl2 flag-set
        form" Abl₂ тІн"
      flag-Abl2 flag-clear
    filter-end
  filter-else
    form" Dat (н)А"
    filter-start constraint-29
      filter-start constraint-V+Acc
        form" Acc₂ Н"
      filter-else
        form" Acc₁ НІ"
      filter-end
    filter-end
    form" Loc (н)ТА"
    form" Abl нАң"
    form" All (н)САр"
    form" Prol (н)ЧА"
    form" Delib нАңАр"
    form" Comp (н)ТАG"
  filter-end
  ;

slot: <Ptcl₂>  \ 18
  18 slot-empty!
  form" -noptcl2 "

  18 slot-full!

  form" Ass ОQ"

  filter-start constraint-18
    form" Adv Ли"
  filter-end
  ;

slot: <Person>  \ 19
  19 slot-empty!
  form" -3prs.sg "

  19 slot-full!

  filter-start constraint-20-full-person
    form" 1sg ПІн"
    form" 1sg.dial СІм"
    form" 2sg СІң"

    flag-1.pl flag-set
      form" 1pl ПІс"
      form" 1pl.dial СІПІс"
    flag-1.pl flag-clear
    form" 2pl САр"
    form" 2pl.dial СІңАр"
  filter-end

  filter-start constraint-20-mix-person
    flag-1sg.br flag-set
      form" 1sg.mix м"
    flag-1sg.br flag-clear
    form" 2sg.mix СІң"

    flag-1.pl flag-set
      form" 1pl.mix ПІс"
    flag-1.pl flag-clear
    form" 2pl.mix САр"
  filter-end

  filter-start constraint-20-short-person
    flag-Person.br flag-set
      flag-1sg.br flag-set
        form" 1sg.br м"
      flag-1sg.br flag-clear
      form" 2sg.br ң"
      flag-1.pl flag-set
        form" 1pl.br ПІс"
      flag-1.pl flag-clear
      flag-2pl.br flag-set
        form" 2pl.br (І)ңАр"
      flag-2pl.br flag-clear
    flag-Person.br flag-clear
  filter-end

  filter-start constraint-19
    flag-Imp flag-set
      form" Imp.1sg им"
      form" Imp.1pl ибІс"
      form" Imp.1.Incl Аң"
      form" Imp.1pl.Incl АңАр"
      form" Imp.1pl.Incl.dial АлАр"
      form" Imp.2pl (І)ңАр"
      flag-Imp.3 flag-set
        form" Imp.3 СІн"
      flag-Imp.3 flag-clear
    flag-Imp flag-clear
  filter-end
  ;

slot: <PredPl>  \ 20
  20 slot-empty!
  form" -nopredpl "

  20 slot-full!
  filter-start constraint-21
    form" PredPl ЛАр"
  filter-end
  ;

slot: <Ptcl₃>  \ 21
  21 slot-empty!
  form" -noptcl3 "

  21 slot-full!

  flag-Ass3 flag-set
    form" Ass ОQ"
  flag-Ass3 flag-clear

  filter-start constraint-23
    form" Foc ТІр"
  filter-end

  filter-start constraint-22
    form" Perm ТАQ"
  filter-end
  ;

CREATE slot-stack
 ' <Distr> , ' <Conv1> , ' <Ptcl1> , ' <Perf/Prosp> ,
 ' <Dur> , ' <Neg/Iter> , ' <Tense/Mood/Conv2> ,
 ' <Indir> , ' <Comit> , ' <Affirm> , ' <Pl₁> ,
 ' <Poss₁> ,
 ' <Case₁> ,
 ' <Attr> ,
 ' <Pl₂> ,
 ' <Poss₂> ,
 ' <Case₂> ,
 ' <Ptcl₂> ,
 ' <Person> ,
 ' <PredPl> ,
 ' <Ptcl₃> , 0 ,
HERE slot-stack - 1- CELL / CONSTANT /slot-stack
