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
           constraint-cluster-envoice
           constraint-non-envoiceable-stem
           constraint-non-envoiced-rus
           constraint-(СА|ТЫ)ңАр-fallout
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
    ( flag-with Distr@full )   form" Distr КлА"
    ( flag-with Distr@short )  form" Distr лА"
  filters-end
  ; slot-add

slot: <NF>  \ 2
  2 slot-empty!
  form" -noconv1 "

  filters( constraint-4 )
    2 slot-full!

    filters( constraint-4.1ₚ )
      form" NF (Ы)п"
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
    form" Ass₁ ОК"
  filters-end
  ; slot-add

slot: <Perf/Prosp>  \ 4
  4 slot-empty!
  form" -noperf "

  4 slot-full!

  filters( constraint-3 )
    form" Perf (Ы)бЫс"
  filters-end

  filters( constraint-5.1 )
    form" Perf0 (Ы)с"
  filters-end
  ; slot-add

slot: <Dur>  \ 5
  5 slot-empty!
  form" -nodur "

  5 slot-full!
  filters( constraint-7 constraint-8 )
    filters( constraint-8.1ᵢ )
      flag-with Dur1@short  form" Dur1ᵢ и"
    filters-end
    filters( constraint-8.1ᵢᵣ )
      flag-with Dur1@full   form" Dur1ᵢᵣ ир"
    filters-end
    form" Dur.dial.kac Ат"
  filters-end

  filters( constraint-26₅ )
    form" Dur чАт"
  filters-end

  filters( constraint-6 )
    form" Prosp.dial АК"
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
      flag-with Iter@full     form" Iter₁ АдЫр"
      filters( constraint-14.1 )
        flag-with Iter@short  form" Iter₂ АдЫ"
      filters-end
    filters-end
  filters-end
  ; slot-add

slot: <Tense/Mood/Conv2>  \ 7
  7 slot-empty!
  form" -notense "

  7 slot-full!

  filters( constraint-26₇ )
    form" Pres чА"

    filters( constraint-frontstem )
      form" Pres.dial ча"
    filters-end

    flag-with Pres.dial.kyz@full     form" Pres.dial.kyz тур"
    filters( constraint-9.4 )
      flag-with Pres.dial.kyz@short  form" Pres.dial.kyz ту"
    filters-end

    filters( constraint-9.5 )
      ( flag-with Pres.dial.sh@short )  form" Pres.dial.sh чАр"
    filters-end
    ( flag-with Pres.dial.sh@full )     form" Pres.dial.sh чАрЫ"

    flag-with Pres2@full   form" Pres2 чАдЫр"
    flag-with Pres2@short  form" Pres2 чАдЫ"

    filters( constraint-frontstem )
      flag-with Pres2.dial.kac@full  form" Pres2.dial.kac чадыр"
    filters-end
    flag-with Pres2.dial.kac@short   form" Pres2.dial.kac чады"
  filters-end

  flag participles  flag-set  \ причастные показатели
    filters( constraint-9.1 )
      flag-with Past@short     form" Past ГА"
    filter-else
      ( flag-with Past@full )  form" Past ГАн"
    filters-end

    filters( constraint-10.1 )
      filters( constraint-9.3 )
        flag-with Fut@short  form" Fut А"
      filters-end
      flag-with Fut@full     form" Fut Ар"
    filters-end

    flag Neg7  flag-set
      form" Neg.Fut ПАс"
    flag Neg7  flag-clear

    filters( constraint-9.2 )
      flag-with Hab@short  form" Hab ЧА"
    filters-end
    flag-with Hab@full     form" Hab ЧАң"

    form" Assum ГАдАГ"

    filters( constraint-10 )
      form" Cunc ГАлАК"
    filters-end

    form" Simul ААчЫК"

    filters( constraint-26₇ )
      form" PresPt.dial чАн"
    filters-end
  flag participles  flag-clear

  filters( constraint-12 )
    form" RPast ТЫ"
  filters-end

  filters( constraint-13 )
    form" Cond СА"
  filters-end

  form" Opt ГАй"

  filters( constraint-25 )
    form" Lim ГАли"

    flag Neg7  flag-set
      flag Conv2  flag-set
        form" Neg.Conv Пин"
        form" Neg.Conv.Abl ПинАң"
      flag Conv2  flag-clear
    flag Neg7  flag-clear

    flag Conv2  flag-set
      form" Convₚ (Ы)п"
      form" Conv.pas.dial АбАс"
      form" Conv.a А"
    flag Conv2  flag-clear
  filters-end
  ; slot-add

slot: <Indir>  \ 8
  8 slot-empty!
  form" -noindir "

  filters( constraint-15 constraint-26₈ )
    8 slot-full!
    filters( constraint-voicedstem+Indir )
      form" Indir тЫр"
    filter-else
      form" Indir ТЫр"
    filters-end
  filters-end
  ; slot-add

slot: <Transp>  \ 9
  9 slot-empty!
  form" -nocomit "

  9 slot-full!

  form" Comit ЛЫГ"

  filters( constraint-18 )
    form" Adv Ли"
  filters-end
  ; slot-add

slot: <Affirm>  \ 10
  10 slot-empty!
  form" -noaffirm "

  10 slot-full!
  form" Affirm ЧЫК"
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

    flag Poss1.nonpl  flag-set
      form" 1pos.sg₁ (Ы)м"
      form" 2pos.sg₁ (Ы)ң"
      filters( constraint-OK-fallout₁₂ )
        form" 3pos₁ (з)Ы"
      filters-end
    flag Poss1.nonpl  flag-clear
    form" 1pos.pl₁ (Ы)бЫс"
    form" 2pos.pl₁ (Ы)ңАр"
  filters-end
  ; slot-add

slot: <Case₁>  \ 13
  13 slot-empty!
  form" -nocase1 "

  filters( constraint-16.1 )
    13 slot-full!

    form" Gen₁ НЫң"

    filters( constraint-17₁₃ )
      form" Loc₁ ТА"
    filter-else
      form" Loc₁ (н)ТА"
    filters-end

    flag All₁  flag-set
      filters( constraint-17₁₃ )
        form" All₁ САр"
      filter-else
        form" All₁ (н)САр"
      filters-end
    flag All₁  flag-clear
  filters-end
  ; slot-add

slot: <Attr>  \ 14
  14 slot-empty!
  form" -noattr "

  filters( constraint-16.2₁₄ )
    14 slot-full!
    form" Attr КЫ"
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

  filters( constraint-16.5₁₆ )
    16 slot-full!

    flag Poss2.nonpl  flag-set
      form" 1pos.sg (Ы)м"
      form" 2pos.sg (Ы)ң"
      filters( constraint-OK-fallout₁₆ )
        form" 3pos (з)Ы"
      filters-end
      form" Gen.3pos Ни"
      form" Gen.3pos.dial Ди"
    flag Poss2.nonpl  flag-clear
    form" 1pos.pl (Ы)бЫс"
    form" 2pos.pl (Ы)ңАр"
  filters-end
  ; slot-add

slot: <Case₂>  \ 17
  17 slot-empty!
  form" -nocase2 "

  17 slot-full!

  filters( constraint-16.5₁₇ )
    filters( constraint-29 )
      form" Gen₂ НЫң"
      form" Gen.dial ТЫң"
      form" Instr нАң"
      form" Instr.dial ПАң"
      form" Instr.dial мАң"
      form" Instr.dial ПлАң"
    filters-end

    filters( constraint-17₁₇ )
      filters( constraint-OK-fallout₁₇ )
        form" Dat ГА"
      filters-end
      filters( constraint-29 )
        form" Acc НЫ"
        form" Acc.dial ТЫ"
      filters-end
      form" Loc ТА"
      form" Abl₁ ДАң"
      form" All САр"
      form" All.dial САрЫ"
      form" All.dial СА"
      form" Prol ЧА"
      form" Delib ДАңАр"
      form" Comp ТАГ"
      filters( constraint-30 )
        form" Abl₂ тЫн"
      filters-end
    filter-else
      filters( constraint-OK-fallout₁₇ )
        form" Dat (н)А"
        form" Dat.dial (н)ГА"
      filters-end
      filters( constraint-29 )
        filters( constraint-V+Acc )
          form" Acc₂ Н"
          form" Acc.dial₂ Т"
        filter-else
          form" Acc₁ НЫ"
          form" Acc.dial₁ ТЫ"
        filters-end
      filters-end
      form" Loc (н)ТА"
      form" Abl нАң"
      form" All (н)САр"
      form" All.dial₁ (н)САрЫ"
      form" All.dial₂ (н)СА"
      form" Prol (н)ЧА"
      form" Delib нАңАр"
      form" Comp (н)ТАГ"
      form" Instr.dial₁ (н)мАң"
      form" Instr.dial₂ (н)млАң"
    filters-end
  filters-end
  ; slot-add

slot: <Ptcl₂>  \ 18
  18 slot-empty!
  form" -noptcl2 "

  18 slot-full!

  form" Ass₂ ОК"
  ; slot-add

slot: <Person>  \ 19
  19 slot-empty!
  form" -3prs.sg "

  19 slot-full!

  filters( constraint-20-full-person )
    form" 1sg ПЫн"
    form" 1sg.dial СЫм"
    form" 2sg СЫң"

    flag-with 1.pl  form" 1pl ПЫс"
    flag-with 1.pl  form" 1pl.dial СЫбЫс"

    form" 2pl САр"
    form" 2pl.dial СЫңАр"
  filters-end

  filters( constraint-20-mix-person )
    flag-with 1sg.br  form" 1sg.mix м"

    form" 2sg.mix СЫң"

    flag-with 1.pl  form" 1pl.mix ПЫс"

    form" 2pl.mix САр"
  filters-end

  filters( constraint-20-short-person )
    flag Person.br  flag-set
      form" 1sg.br м"
      form" 2sg.br ң"

      flag-with 1.pl  form" 1pl.br ПЫс"

      form" 2pl.br (Ы)ңАр"
    flag Person.br  flag-clear
  filters-end

  filters( constraint-19 )
    flag Imp  flag-set
      form" Imp.1sg им"
      form" Imp.1pl ибЫс"
      form" Imp.1.Incl Аң"
      form" Imp.1pl.Incl АңАр"
      form" Imp.1pl.Incl.dial АлАр"
      form" Imp.2pl (Ы)ңАр"
      form" Imp.3 СЫн"
    flag Imp  flag-clear
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

  form" Ass₃ ОК"

  filters( constraint-23 )
    form" Foc ТЫр"
  filters-end

  filters( constraint-22 )
    form" Perm ТАК"
  filters-end
  ; slot-add

slot-stack-here @ slot-stack - CELL / CONSTANT /slot-stack
