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
  right-context( constraint-(СА|ТЫ)ңАр-fallout
                 constraint-VңV-fallout )
    filters( constraint-0
             constraint-1
             constraint-2
             constraint-cluster-envoice
             constraint-non-envoiceable-stem
             constraint-non-envoiced-rus
             constraint-V[кх]V-fallout
             constraint-VVГV-fallout
             constraint-CCC-fallout
             constraint-broken-harmony )
      <this> slot-empty!
      form" -nodistr "

      <this> slot-full!
      form" Distr (К)лА"
      form" Distr.dial лАGлА"
    filters-end
  right-context-end
  ; slot-add

<Voice> slot:  \ 2
  <this> slot-empty!
  form" -novoice "

  <this> slot-full!
  form" Pass (Ы)л"
  form" Refl (Ы)н"
  form" Rec (Ы)с"
  form" Caus ТЫр"
  ; slot-add

<NF,Dur1> slot:  \ 3
  <this> slot-empty!
  form" -noconv1 "

  right-context( constraint-4 )
    <this> slot-full!

    filters( constraint-4.1ₚ )
      form" NF (Ы)п"
    filters-end
    right-context( constraint-4.1₀-right )
      filters( constraint-4.1₀ )
        flag-with NF₀ form" NF 0̸"
      filters-end
    right-context-end
    right-context( constraint-11.1 constraint-10 )
      form" NF.Neg Пин"
    right-context-end
  right-context-end
  right-context( constraint-4sh constraint-10 )
    form" NF.Neg.sh ПААн"
  right-context-end

  filters( constraint-7 )
    right-context( constraint-8 )
      right-context( constraint-8.1ᵢ )
        flag-with Dur1@short  form" Dur1 и"
      right-context-end
      right-context( constraint-8.1ᵢᵣ )
        flag-with Dur1@full   form" Dur1 ир"
      right-context-end
      form" Dur1.kac Ат"
    right-context-end
    right-context( constraint-8sag )
      form" Dur1.sag ит"
    right-context-end
  filters-end

  ; slot-add

<Ptcl1> slot:  \ 4
  <this> slot-empty!
  form" -noptcl1 "

  right-context( constraint-27 )
    filters( constraint-5 )
      <this> slot-full!
      flag-with Add   form" Add₁ ТАА"
      flag-with Cont  form" Cont₁ LА"
      form" Ass₁ ОК"
    filters-end
  right-context-end
  ; slot-add

<Perf> slot:  \ 5
  <this> slot-empty!
  form" -noperf "

  <this> slot-full!

  right-context( constraint-10 )
    filters( constraint-3 )
      form" Perf (Ы)бЫс"
    filters-end
  right-context-end

  filters( constraint-5.1 )
    form" Perf0 Ыс"
  filters-end
  ; slot-add

<Prosp> slot:  \ 6
  <this> slot-empty!
  form" -noprosp "

  <this> slot-full!
  right-context( constraint-6-right )
    filters( constraint-6 constraint-VA>и-fallout )
      form" Prosp.dial АК"
    filters-end
  right-context-end
  ; slot-add

<Dur> slot:  \ 7
  <this> slot-empty!
  form" -nodur "

  <this> slot-full!
  filters( constraint-26+paan )
    form" Dur чАт"
    form" Dur.dial чат"
  filters-end
  ; slot-add

<Neg/Gener> slot:  \ 8
  <this> slot-empty!
  form" -noneg/gener "

  right-context( constraint-11 )
    <this> slot-full!

    right-context( constraint-10 )
      form" Neg ПА"
    right-context-end
  right-context-end

  right-context( constraint-14 )
    filters( constraint-VA>и-fallout )
      flag-with Gener@full     form" Gener АдЫр"
      right-context( constraint-14.1 )
        flag-with Gener@short  form" Gener АдЫ"
      right-context-end
    filters-end
  right-context-end
  ; slot-add

<Tense/Mood/Conv> slot:  \ 9
  <this> slot-empty!
  form" -notense "

  <this> slot-full!

  filters( constraint-26+paan )
    form" Pres чА"

    filters( constraint-frontstem )
      form" Pres.dial ча"
    filters-end

    right-context( constraint-9.5 )
      ( flag-with Pres.sh@short )  form" Pres.sh чар"
    right-context-end
    ( flag-with Pres.sh@full )     form" Pres.sh чары"
  filters-end

  filters( constraint-26 )
    flag-with Pres.kyz@full     form" Pres.kyz тур"
    right-context( constraint-9.4 )
      flag-with Pres.kyz@short  form" Pres.kyz ту"
    right-context-end

    flag-with Pres2@full   form" Pres2 чАдЫр"
    flag-with Pres2@short  form" Pres2 чАдЫ"

    filters( constraint-frontstem )
      flag-with Pres2.kac@full  form" Pres2.kac чадыр"
    filters-end
    flag-with Pres2.kac@short   form" Pres2.kac чады"
  filters-end

  flag participles  flag-set  \ причастные показатели
    right-context( constraint-9.1 )
      form" Past ГА"
    right-context-else
      form" Past ГАн"
    right-context-end

    filters( constraint-VA>и-fallout )
      right-context( constraint-9.3 )
        flag-with Fut@short  form" Fut А"
      right-context-end
      flag-with Fut@full     form" Fut Ар"
    filters-end

    flag Neg7  flag-set
      flag-with Assum|Neg.Fut|Opt  form" Neg.Fut ПАс"
    flag Neg7  flag-clear

    right-context( constraint-9.2 )
      flag-with Hab@short  form" Hab ЧА"
    right-context-end
    flag-with Hab@full     form" Hab ЧАң"

    flag-with Assum|Neg.Fut|Opt  form" Assum ГАдАГ"

    form" Cunc ГАлАК"

    form" Simul ААчЫК"

    filters( constraint-26 )
      form" PresPt.dial чАн"
    filters-end

    flag-with Assum|Neg.Fut|Opt  form" Opt ГАй"
  flag participles  flag-clear

  right-context( constraint-12 )
    form" RPast ТЫ"
  right-context-end

  right-context( constraint-13 )
    form" Cond СА"
  right-context-end

  right-context( constraint-25 )
    flag converbs  flag-set \ деепричастия
      form" Lim ГАли"

      flag Neg7  flag-set
        form" Neg.Conv Пин"
        form" Neg.Conv.Abl ПинАң"
      flag Neg7  flag-clear

      form" Cv.p (Ы)п"
      filters( constraint-VA>и-fallout )
        form" Cv.kac АбАс"
        form" Cv.a А"
      filters-end
    flag converbs flag-clear
  right-context-end

  filters( constraint-26+paan )
    filters( constraint-voicedstem+Indir )
      form" Indir тЫр"
    filter-else
      form" Indir ТЫр"
    filters-end
  filters-end
  ; slot-add

<Pl₁> slot:  \ 10
  <this> slot-empty!
  form" -nopl1 "

  right-context( constraint-16.1 constraint-16.5 constraint-16.7 constraint-16.8 constraint-27 )
    <this> slot-full!
    form" Pl₁ ЛАр"
  right-context-end
  ; slot-add

<Poss₁> slot:  \ 11
  <this> slot-empty!
  form" -noposs1 "

  right-context( constraint-16.1 constraint-16.5 constraint-27 )
    filters( constraint-16.7 )
      <this> slot-full!

      flag Poss1.nonpl  flag-set
        form" 1pos.sg₁ (Ы)м"
        form" 2pos.sg₁ (Ы)ң"
        filters( constraint-OK-fallout-<Poss₁> )
          form" 3pos₁ (з)Ы"
        filters-end
      flag Poss1.nonpl  flag-clear
      form" 1pos.pl₁ (Ы)бЫс"
      form" 2pos.pl₁ (Ы)ңАр"
    filters-end
  right-context-end
  ; slot-add

<Case₁> slot:  \ 12
  <this> slot-empty!
  form" -nocase1 "

  right-context( constraint-16.1 constraint-27 )
    <this> slot-full!

    filters( constraint-16.5 )
      right-context( constraint-16.7 )
        filters( constraint-17-<Case₁> )
          \ form" Gen₁ НЫң"
          form" All₁ САр"
        filter-else
          \ form" Gen₁ нЫң"
          form" All₁ (н)САр"
        filters-end
      right-context-end

      right-context( constraint-16.6 )
        filters( constraint-17-<Case₁> )
          form" Loc₁ ТА"
        filter-else
          form" Loc₁ (н)ТА"
        filters-end
      right-context-end
    filters-end

  right-context-end
  ; slot-add

<Transp> slot:  \ 13
  <this> slot-empty!
  form" -notransp "

  <this> slot-full!

  filters( constraint-16.2 )
    <this> slot-full!
    form" Attr КЫ"
  filters-end

  form" Comit ЛЫГ"

  right-context( constraint-18-right )
    filters( constraint-18 )
      form" Adv Ли"
    filters-end
  right-context-end
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

  <this> slot-full!

  flag Poss2.nonpl  flag-set
    form" 1pos.sg (Ы)м"
    form" 2pos.sg (Ы)ң"
    filters( constraint-OK-fallout-<Poss₂> )
      form" 3pos (з)Ы"
    filters-end
    form" Gen.3pos Ни"
    flag-with Gen.3pos  form" Gen.3pos.dial Ди"
  flag Poss2.nonpl  flag-clear
  form" 1pos.pl (Ы)бЫс"
  form" 2pos.pl (Ы)ңАр"
  ; slot-add

<Case₂> slot:  \ 16
  <this> slot-empty!
  form" -nocase2 "

  <this> slot-full!

  right-context( constraint-29 )
    form" Gen.dial ТЫң"
    form" Instr нАң"
    form" Instr.dial мАң"
    form" Instr.dial мнаң"
    form" Instr.dial ПЫнАң"
  right-context-end

  filters( constraint-17-<Case₂> )
    \ Simple declension
    right-context( constraint-29 )
      form" Gen НЫң"
      right-context( constraint-OK-fallout-<Case₂> )
        form" Dat ГА"
      right-context-end
      form" Acc НЫ"
      form" Acc.dial ДЫ"
      form" Instr.dial ПАң"
      form" Instr.dial ПлАң"
    right-context-end
    form" Loc ТА"
    form" Abl ДАң"
    form" Abl.dial нАң"
    form" All САр"
    form" All.dial САрЫ"
    form" All.dial СА"
    form" Prol ЧА"
    form" Delib ДАңАр"
    form" Delib.dial ДАңАрЫ"
    form" Comp ТАГ"
    filters( constraint-30 )
      form" Abl.arch тЫн"
    filters-end
  filter-else
    \ Possessive declension
    right-context( constraint-29 )
      form" Gen нЫң"
      right-context( constraint-OK-fallout-<Case₂> )
        form" Dat (н)А"
        form" Dat.dial (н)ГА"
      right-context-end
      filters( constraint-V+Acc )
        form" Acc н"
      filter-else
        form" Acc нЫ"
      filters-end
      form" Acc.dial дЫ"
      form" Instr.dial (н)мАң"
      form" Instr.dial (н)млАң"
      form" Instr.dial бАң"
    right-context-end
    form" Loc (н)ТА"
    form" Abl нАң"
    form" All (н)САр"
    form" All.dial1 (н)САрЫ"
    form" All.dial2 (н)СА"
    form" Prol (н)ЧА"
    form" Delib нАңАр"
    form" Delib.dial нАңАрЫ"
    form" Comp (н)ТАГ"
    right-context( constraint-33 )
      form" Voc Ай"
    right-context-end
  filters-end
  ; slot-add

<Ptcl₂> slot:  \ 17
  <this> slot-empty!
  form" -noptcl2 "

  <this> slot-full!

  right-context( constraint-27 )
    filters( constraint-34 )
      form" Ass₂ ОК"
      flag-with Cont  form" Cont₂ LА"
      flag-with Add   form" Add₂ ТАА"
      filters( constraint-32 )
        form" Part ни"
      filters-end
    filters-end
  right-context-end
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
      form" Imp.1pl.dial АлЫс"
      form" Imp.1.Incl Аң"
      form" Imp.1.Incl.dial АК"
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
