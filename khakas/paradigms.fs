\ after entering new data, repeat this command until no matches:
\ :3,$s/"\(.*\)\([aceiopxyöÿ]\)/\="\"" . submatch(1) . tr(submatch(2),"aceiopxyöÿ", "асеіорхуӧӱ")/gI

require khakas/flags.fs
require khakas/constraints.fs

CREATE slot-stack 32 CELLS ALLOT
VARIABLE slot-stack-here  slot-stack slot-stack-here !
: slot-add  LATESTXT slot-stack-here @ !  CELL slot-stack-here +!  0 slot-stack-here @ ! ;

slot: <Distr>  \ 1
  \ Навесим глобальные фильтры на 1-ю позицию с любым
  \ (в том числе нулевым!) аффиксом
  filters( constraint-0
           constraint-1
           constraint-2
           constraint-27
           constraint-non-envoiceable-stem
           constraint-non-envoiced-rus
           constraint-(СА|ТІ)ңАр-fallout
           constraint-[Гң]Г-fallout
           constraint-V[кх]V-fallout
           constraint-VГV-fallout
           constraint-VVГV-fallout
           constraint-VңV-fallout
           constraint-CCC-fallout
           constraint-broken-harmony )
    1 slot-empty!
    form" -nodistr "

    1 slot-full!
    form" Distr КлА"
  filters-end
  ; slot-add

slot: <NF>  \ 2
  2 slot-empty!
  form" -noconv1 "

  filters( constraint-4 constraint-11 )
    2 slot-full!

    filters( constraint-4.1ₚ )
      form" NF (І)п"
    filters-end
    filters( constraint-4.1₀ )
      form" NF₀ 0̸"
    filters-end
    form" NF.Neg Пин"
  filters-end
  ; slot-add

slot: <Ptcl1>  \ 3
  3 slot-empty!
  form" -noptcl1 "

  filters( constraint-5 )
    3 slot-full!
    form" Add ТАА"
    form" Cont LА"
    form" Ass₁ ОQ"
  filters-end
  ; slot-add

slot: <Perf/Prosp>  \ 4
  4 slot-empty!
  form" -noperf "

  4 slot-full!

  filters( constraint-3 )
    form" Perf (І)бІс"
  filters-end

  filters( constraint-6 )
    form" Prosp АК"
  filters-end
  ; slot-add

slot: <Dur>  \ 5
  5 slot-empty!
  form" -nodur "

  5 slot-full!
  filters( constraint-7 constraint-8 )
    filters( constraint-8.1ᵢ )
      form" Dur1ᵢ и"
    filters-end
    filters( constraint-8.1ᵢᵣ )
      form" Dur1ᵢᵣ ир"
    filters-end
  filters-end

  filters( constraint-26₅ )
    form" Dur чАт"
  filters-end
  ; slot-add

slot: <Neg/Iter>  \ 6
  6 slot-empty!
  form" -noneg/iter "

  filters( constraint-11 )
    6 slot-full!

    filters( constraint-11.1 )
      form" Neg ПА"
    filters-end

    filters( constraint-14 )
      form" Iter АдІр"
    filters-end

    filters( constraint-26₆ )
      form" Dur.Iter чАдІр"
    filters-end
  filters-end
  ; slot-add

slot: <Tense/Mood/Conv2>  \ 7
  7 slot-empty!
  form" -notense "

  7 slot-full!

  filters( constraint-26₇ )
    form" Pres чА"
  filters-end

  flag-Neg7 flag-set
    form" Neg.Fut ПАс"
  flag-Neg7 flag-clear

  flag-Past flag-set
    filters( constraint-9.1 )
      form" Past₁ ГА"
    filter-else
      form" Past₂ ГАн"
    filters-end
  flag-Past flag-clear

  filters( constraint-10.1 )
    filters( constraint-9.3 )
      form" Futₐ А"
    filters-end
    form" Futₐᵣ Ар"
  filters-end

  filters( constraint-9.2 )
    form" Hab₁ ЧА"
  filters-end
  form" Hab₂ ЧАң"

  filters( constraint-12 )
    form" RPast ТІ"
  filters-end

  filters( constraint-10 )
    form" Cunc ГАлАQ"
  filters-end

  filters( constraint-13 )
    form" Cond СА"
  filters-end
  flag-Opt-or-Assum flag-set
    form" Opt ГАй"
    form" Assum ГАдАG"
  flag-Opt-or-Assum flag-clear

  filters( constraint-25 )
    form" Lim ГАли"

    flag-Neg7 flag-set
      flag-Conv2 flag-set
        form" Neg.Conv Пин"
        form" Neg.Conv.Abl ПинАң"
      flag-Conv2 flag-clear
    flag-Neg7 flag-clear

    flag-Conv2 flag-set
      form" Convₚ (І)п"
      form" Conv.pas АбАс"
      form" Conv.a А"
    flag-Conv2 flag-clear
  filters-end
  ; slot-add

slot: <Indir>  \ 8
  8 slot-empty!
  form" -noindir "

  filters( constraint-15 constraint-26₈ )
    8 slot-full!
    filters( constraint-voicedstem+Indir )
      form" Indir тІр"
    filter-else
      form" Indir ТІр"
    filters-end
  filters-end
  ; slot-add

slot: <Comit>  \ 9
  9 slot-empty!
  form" -nocomit "

  9 slot-full!

  form" Comit ЛІG"
  ; slot-add

slot: <Affirm>  \ 10
  10 slot-empty!
  form" -noaffirm "

  10 slot-full!
  form" Affirm ЧІQ"
  ; slot-add

slot: <Pl₁>  \ 11
  11 slot-empty!
  form" -nopl1 "

  filters( constraint-16.1 )
    11 slot-full!
    form" Pl₁ ЛАр"
  filters-end
  ; slot-add

slot: <Poss₁>  \ 12
  12 slot-empty!
  form" -noposs1 "

  filters( constraint-16.1 )
    12 slot-full!

    flag-Poss1.nonpl flag-set
      form" 1pos.sg (І)м"
      form" 2pos.sg (І)ң"
      form" 3pos₁ (з)І"
    flag-Poss1.nonpl flag-clear
    form" 1pos.pl (І)бІс"
    form" 2pos.pl (І)ңАр"
  filters-end
  ; slot-add

slot: <Case₁>  \ 13
  13 slot-empty!
  form" -nocase1 "

  filters( constraint-16.1 )
    13 slot-full!

    form" Gen₁ НІң"

    filters( constraint-17₁₃ )
      form" Loc ТА"
    filter-else
      form" Loc (н)ТА"
    filters-end

    flag-All1 flag-set
      filters( constraint-17₁₃ )
        form" All САр"
      filter-else
        form" All (н)САр"
      filters-end
    flag-All1 flag-clear
  filters-end
  ; slot-add

slot: <Attr>  \ 14
  14 slot-empty!
  form" -noattr "

  filters( constraint-16.2₁₄ )
    14 slot-full!
    form" Attr КІ"
  filters-end
  ; slot-add

slot: <Pl₂>  \ 15
  15 slot-empty!
  form" -nopl2 "

  filters( constraint-16.3 constraint-16.4 )
    15 slot-full!
    form" Pl₂ ЛАр"
  filters-end
  ; slot-add

slot: <Poss₂>  \ 16
  16 slot-empty!
  form" -noposs2 "

  filters( constraint-16.5₁₇ )
    16 slot-full!

    flag-Poss2.nonpl flag-set
      form" 1pos.sg (І)м"
      form" 2pos.sg (І)ң"
      form" 3pos (з)І"
      form" Gen.3pos Ни"
    flag-Poss2.nonpl flag-clear
    form" 1pos.pl (І)бІс"
    form" 2pos.pl (І)ңАр"
  filters-end
  ; slot-add

slot: <Case₂>  \ 17
  17 slot-empty!
  form" -nocase2 "

  17 slot-full!

  filters( constraint-16.5₁₇ )
    filters( constraint-29 )
      form" Gen₂ НІң"
      form" Instr нАң"
    filters-end

    filters( constraint-17₁₇ )
      form" Dat ГА"
      filters( constraint-29 )
        form" Acc НІ"
      filters-end
      form" Loc ТА"
      form" Abl₁ ДАң"
      form" All САр"
      form" Prol ЧА"
      form" Delib ДАңАр"
      form" Comp ТАG"
      filters( constraint-30 )
        form" Abl₂ тІн"
      filters-end
    filter-else
      form" Dat (н)А"
      filters( constraint-29 )
        filters( constraint-V+Acc )
          form" Acc₂ Н"
        filter-else
          form" Acc₁ НІ"
        filters-end
      filters-end
      form" Loc (н)ТА"
      form" Abl нАң"
      form" All (н)САр"
      form" Prol (н)ЧА"
      form" Delib нАңАр"
      form" Comp (н)ТАG"
    filters-end
  filters-end
  ; slot-add

slot: <Ptcl₂>  \ 18
  18 slot-empty!
  form" -noptcl2 "

  18 slot-full!

  form" Ass₂ ОQ"

  filters( constraint-18 )
    form" Adv Ли"
  filters-end
  ; slot-add

slot: <Person>  \ 19
  19 slot-empty!
  form" -3prs.sg "

  19 slot-full!

  filters( constraint-20-full-person )
    form" 1sg ПІн"
    form" 1sg.dial СІм"
    form" 2sg СІң"

    flag-1.pl flag-set
      form" 1pl ПІс"
      form" 1pl.dial СІПІс"
    flag-1.pl flag-clear
    form" 2pl САр"
    form" 2pl.dial СІңАр"
  filters-end

  filters( constraint-20-mix-person )
    flag-1sg.br flag-set
      form" 1sg.mix м"
    flag-1sg.br flag-clear
    form" 2sg.mix СІң"

    flag-1.pl flag-set
      form" 1pl.mix ПІс"
    flag-1.pl flag-clear
    form" 2pl.mix САр"
  filters-end

  filters( constraint-20-short-person )
    flag-Person.br flag-set
      flag-1sg.br flag-set
        form" 1sg.br м"
      flag-1sg.br flag-clear
      form" 2sg.br ң"
      flag-1.pl flag-set
        form" 1pl.br ПІс"
      flag-1.pl flag-clear
      form" 2pl.br (І)ңАр"
    flag-Person.br flag-clear
  filters-end

  filters( constraint-19 )
    flag-Imp flag-set
      form" Imp.1sg им"
      form" Imp.1pl ибІс"
      form" Imp.1.Incl Аң"
      form" Imp.1pl.Incl АңАр"
      form" Imp.1pl.Incl.dial АлАр"
      form" Imp.2pl (І)ңАр"
      form" Imp.3 СІн"
    flag-Imp flag-clear
  filters-end
  ; slot-add

slot: <PredPl>  \ 20
  20 slot-empty!
  form" -nopredpl "

  20 slot-full!
  filters( constraint-21 )
    form" PredPl ЛАр"
  filters-end
  ; slot-add

slot: <Ptcl₃>  \ 21
  21 slot-empty!
  form" -noptcl3 "

  21 slot-full!

  form" Ass₃ ОQ"

  filters( constraint-23 )
    form" Foc ТІр"
  filters-end

  filters( constraint-22 )
    form" Perm ТАQ"
  filters-end
  ; slot-add

slot-stack-here @ slot-stack - CELL / CONSTANT /slot-stack
