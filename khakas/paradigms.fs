\ after entering new data, repeat this command until no matches:
\ :3,$s/"\(.*\)\([aceiopxyöÿ]\)/\="\"" . submatch(1) . tr(submatch(2),"aceiopxyöÿ", "асеіорхуӧӱ")/gI

require khakas/flags.fs
require khakas/constraints.fs
require khakas/slotnames.fs

CREATE slot-stack 32 CELLS ALLOT
VARIABLE slot-stack-here  slot-stack slot-stack-here !
: slot-add  ( slot-position xt -- )
  slot-stack-here @ !  ( slot-position )
  CELL slot-stack-here +!
  slot-stack-here @ slot-stack - CELL / <> ABORT" wrong slot order!"  ( )
  0  slot-stack-here @ ! ;

<Distr> slot:  \ 1
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
    <this> slot-empty!
    form" -nodistr "

    <this> slot-full!
    ( flag-with Distr@full )   form" Distr КлА"
    ( flag-with Distr@short )  form" Distr лА"
  filters-end
  ; slot-add

<NF> slot:  \ 2
  <this> slot-empty!
  form" -noconv1 "

  filters( constraint-4 )
    <this> slot-full!

    filters( constraint-4.1ₚ )
      form" NF (Ы)п"
    filters-end
    filters( constraint-4.1₀ )
      form" NF₀ 0̸"
    filters-end
    form" NF.Neg Пин"
    form" NF.Neg.sh ПААн"
  filters-end
  ; slot-add

<Ptcl1> slot:  \ 3
  <this> slot-empty!
  form" -noptcl1 "

  filters( constraint-5 )
    <this> slot-full!
    form" Add ТАА"
    form" Cont LА"
    form" Ass₁ ОК"
  filters-end
  ; slot-add

<Perf> slot:  \ 4
  <this> slot-empty!
  form" -noperf "

  <this> slot-full!

  filters( constraint-3 )
    form" Perf (Ы)бЫс"
  filters-end

  filters( constraint-5.1 )
    form" Perf0 (Ы)с"
  filters-end
  ; slot-add

<Prosp,Dur1> slot:  \ 5
  <this> slot-empty!
  form" -noprosp "

  <this> slot-full!
  filters( constraint-7 constraint-8 )
    filters( constraint-8.1ᵢ )
      flag-with Dur1@short  form" Dur1ᵢ и"
    filters-end
    filters( constraint-8.1ᵢᵣ )
      flag-with Dur1@full   form" Dur1ᵢᵣ ир"
    filters-end
    form" Dur.dial.kac Ат"
  filters-end

  filters( constraint-6 )
    form" Prosp.dial АК"
  filters-end
  ; slot-add

<Dur> slot:  \ 6
  <this> slot-empty!
  form" -nodur "

  <this> slot-full!
  filters( constraint-26₅ )
    form" Dur чАт"
  filters-end
  ; slot-add

<Neg/Iter> slot:  \ 7
  <this> slot-empty!
  form" -noneg/iter "

  filters( constraint-11 )
    <this> slot-full!

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

<Tense/Mood/Conv2> slot:  \ 8
  <this> slot-empty!
  form" -notense "

  <this> slot-full!

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

  filters( constraint-26₈ )
    <this> slot-full!
    filters( constraint-voicedstem+Indir )
      form" Indir тЫр"
    filter-else
      form" Indir ТЫр"
    filters-end
  filters-end
  ; slot-add

<Transp> slot:  \ 9
  <this> slot-empty!
  form" -nocomit "

  <this> slot-full!

  form" Comit ЛЫГ"

  filters( constraint-18 )
    form" Adv Ли"
  filters-end
  ; slot-add

<Pl₁> slot:  \ 10
  <this> slot-empty!
  form" -nopl1 "

  filters( constraint-16.1 )
    <this> slot-full!
    form" Pl₁ ЛАр"
  filters-end
  ; slot-add

<Poss₁> slot:  \ 11
  <this> slot-empty!
  form" -noposs1 "

  filters( constraint-16.1 )
    <this> slot-full!

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

<Case₁> slot:  \ 12
  <this> slot-empty!
  form" -nocase1 "

  filters( constraint-16.1 )
    <this> slot-full!

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

<Attr> slot:  \ 13
  <this> slot-empty!
  form" -noattr "

  filters( constraint-16.2₁₄ )
    <this> slot-full!
    form" Attr КЫ"
  filters-end
  ; slot-add

<Pl₂> slot:  \ 14
  <this> slot-empty!
  form" -nopl2 "

  filters( constraint-16.3 constraint-16.4 )
    <this> slot-full!
    form" Pl₂ ЛАр"
  filters-end
  ; slot-add

<Poss₂> slot:  \ 15
  <this> slot-empty!
  form" -noposs2 "

  filters( constraint-16.5₁₆ )
    <this> slot-full!

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

<Case₂> slot:  \ 16
  <this> slot-empty!
  form" -nocase2 "

  <this> slot-full!

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

<Ptcl₂> slot:  \ 17
  <this> slot-empty!
  form" -noptcl2 "

  <this> slot-full!

  form" Ass₂ ОК"
  ; slot-add

<Affirm> slot:  \ 18
  <this> slot-empty!
  form" -noaffirm "

  <this> slot-full!
  form" Affirm ЧЫК"
  ; slot-add

<Person> slot:  \ 19
  <this> slot-empty!
  form" -3prs.sg "

  <this> slot-full!

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

<PredPl> slot:  \ 20
  <this> slot-empty!
  form" -nopredpl "

  <this> slot-full!
  filters( constraint-21 )
    form" PredPl ЛАр"
  filters-end
  ; slot-add

<Ptcl₃> slot:  \ 21
  <this> slot-empty!
  form" -noptcl3 "

  <this> slot-full!

  form" Ass₃ ОК"

  filters( constraint-23 )
    form" Foc ТЫр"
  filters-end

  filters( constraint-22 )
    form" Perm ТАК"
  filters-end
  ; slot-add

slot-stack-here @ slot-stack - CELL / CONSTANT /slot-stack
