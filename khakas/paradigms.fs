\ after entering new data, repeat this command until no matches:
\ :3,$s/"\(.*\)\([aceiopxyöÿ]\)/\="\"" . submatch(1) . tr(submatch(2),"aceiopxyöÿ", "асеіорхуӧӱ")/gI

: flag  ( u "name" -- )
  debug-mode? IF VALUE ELSE CONSTANT THEN
  ; IMMEDIATE

\ flags in different slots must be distinct!
%000000000000000000000000001 flag flag-Cond
%000000000000000000000000010 flag flag-Conv.Neg
%000000000000000000000000100 flag flag-Conv2
%000000000000000000000001000 flag flag-Cunc
%000000000000000000000010000 flag flag-Dur
%000000000000000000000100000 flag flag-Dur1.i
%000000000000000000001000000 flag flag-Dur1.ir
%000000000000000000010000000 flag flag-Fut.a
%000000000000000000100000000 flag flag-Fut.ar
%000000000000000001000000000 flag flag-Hab
%000000000000000010000000000 flag flag-Hab.ca
%000000000000000100000000000 flag flag-Hab.cang
%000000000000001000000000000 flag flag-Imp
%000000000000010000000000000 flag flag-Imp.3
%000000000000100000000000000 flag flag-Iter
%000000000001000000000000000 flag flag-Neg6
%000000000010000000000000000 flag flag-Neg7
%000000000100000000000000000 flag flag-Opt-or-Assum
%000000001000000000000000000 flag flag-Past
%000000010000000000000000000 flag flag-Perf
%000000100000000000000000000 flag flag-Person.br
%000001000000000000000000000 flag flag-Poss1.nonpl
%000010000000000000000000000 flag flag-Poss2.nonpl
%000100000000000000000000000 flag flag-Pres
%001000000000000000000000000 flag flag-RPast
%010000000000000000000000000 flag flag-1sg.br
%100000000000000000000000000 flag flag-1.pl
flag-Dur flag-Pres OR                            flag flag-Dur-or-Pres
flag-RPast flag-Cond OR                          flag flag-RPast-or-Cond
flag-RPast-or-Cond flag-Pres OR flag-Conv2 OR    flag flag-RPast-or-Cond-or-Pres-or-Conv2
flag-Past flag-Hab OR                            flag flag-Past-or-Hab
flag-Iter flag-Opt-or-Assum OR flag-Cunc OR
flag-Dur1.ir OR flag-Hab.cang OR flag-Fut.ar OR  flag flag-Iter-or-Opt-or-Assum-or-Cunc-or-Dur1.ir-or-Hab.cang-or-Fut.ar
flag-Pres flag-Dur1.i OR flag-Past OR
flag-Hab.ca OR flag-Fut.a OR                     flag flag-Pres-or-Dur1.i-or-Past-or-Hab.ca-or-Fut.a

0  S" пар" strlist-prepend-alloc  S" кил"  strlist-prepend-alloc  CONSTANT пар|кил
: is-пар/кил?  ( -- f )
  paradigm-stems @  пар|кил  strlists-intersect? ;

: form-slot-vowel-at-left? ( -- f )
  form-slot-xc-at-left vowel? ;

: nomen-or-verb-with-Tense-non-RPast-Cond-Pres-Conv2?  ( -- f )
  verb? IF
    9 10 slot-range-full? IF
      TRUE
    ELSE
      7 slot-full?   8 10 slot-range-empty?  AND IF
        flag-RPast-or-Cond-or-Pres-or-Conv2 flag-is? 0=
      ELSE FALSE THEN
    THEN
  ELSE TRUE THEN ;

\ 20. К словам с пометой NOMEN присоединяются полные
\ лично-числовые показатели (список внутри Person), которые
\ могут следовать после любых морфем. У глаголов заполнение
\ Person возможно только при незаполненных позициях с 15 по 18 и
\ при наличии показателей: для полных форм: Iter дIр, Affirm ЧIК,
\ Opt ГАй, Assum ГАдАГ, Indir ТIр, Cunc ГАлАК, Dur1 в форме ир,
\ Hab в форме ҶАң, Fut в форме Ар; для смешанных форм: Pres ЧА,
\ Dur1 в форме и, Past ГА(н), Hab в форме ҶА, Fut в форме А; для
\ кратких форм: Cond СА, Rpast ТI.
: full-person-allowed?  ( -- f )
  verb? IF
    15 18 slot-range-empty? IF
      8 slot-full?  10 slot-full?  OR
      flag-Iter-or-Opt-or-Assum-or-Cunc-or-Dur1.ir-or-Hab.cang-or-Fut.ar flag-is?  OR
    ELSE FALSE THEN
  ELSE TRUE THEN ;
: mix-person-allowed?  ( -- f )
  verb?
  15 18 slot-range-empty?  AND
  flag-Pres-or-Dur1.i-or-Past-or-Hab.ca-or-Fut.a flag-is? AND ;
: short-person-allowed?  ( -- f )
  verb?
  15 18 slot-range-empty?  AND
  flag-RPast-or-Cond flag-is?  AND ;


slot: <Distr>  \ 1
  \ Навесим глобальные фильтры на 1-ю позицию с любым
  \ (в том числе нулевым!) аффиксом

  \ 1. Позиции 1–8 могут заполняться только у слов с пометой Verbum
  filter-start( 1 8 slot-range-empty?  verb?  OR )
    \ 2. Заполнение позиций 11-19 (падежно-посессивный блок) может
    \ происходить либо сразу после позиции 0 (если слово имеет
    \ помету Nomen), либо непосредственно после заполненной позиции
    \ 7 (если слово имеет помету Verbum); но не в случае, если
    \ позиция 7 заполнена аффиксами RPast ТI, Cond СА, Pres чА или
    \ любым из Conv (т.е. только в случаях Past, Fut, Neg.Fut, Hab,
    \ Cunc, Assum, Opt); либо непосредственно после заполненной
    \ позиции 9 (Comit), либо непосредственно после заполненной
    \ позиции 10 (Affirm).
    filter-start( 11 19 slot-range-empty?
                  nomen-or-verb-with-Tense-non-RPast-Cond-Pres-Conv2?
                  OR )
      \ 27. Позиции Conv1, Ptcl1, Pl1, Poss1, Case1, Ptcl2 не
      \ могут быть последними заполненными позициями в
      \ словоформe
      filter-start( 2 slot-empty?  3 22 slot-range-full?  OR
                    3 slot-empty?  4 22 slot-range-full?  OR
                    11 slot-empty?  12 22 slot-range-full?  OR
                    12 slot-empty?  13 22 slot-range-full?  OR
                    13 slot-empty?  14 22 slot-range-full?  OR
                    19 slot-empty?  20 22 slot-range-full?  OR
                    OR OR OR OR OR )
        1 slot-empty!
        form" -nodistr "

        1 slot-full!
        form" Distr КлА"
      filter-end
    filter-end
  filter-end
  ;

slot: <Conv1>  \ 2
  2 slot-empty!
  form" -noconv1 "

  \ 4. Показатели позиции 2 (Conv1/Conv.Neg) могут встретиться
  \ только в словоформе, где есть также показатель позиции 3
  \ (частицы), либо Dur чАТ, либо Pres чА, либо Indir ТIр,
  \ либо комбинации этих морфем.
  filter-start( 3 slot-full?
                flag-Dur-or-Pres flag-is? OR
                8 slot-full? OR )
    \ 11. Показатели Neg.Fut ПАС, Neg.Conv Пин, Neg.Сonv.Abl
    \ Пин.Аң исключают заполнение поз.2 и 6.
    filter-start( flag-Neg7 flag-empty? )
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

  \ 5. Показатели поз. 3 (внутренние частицы) допускаются
  \ только при заполненной позиции 7 (время).
  filter-start( 7 slot-full? )
    3 slot-full!
    form" Add ТАА"
    form" Cont LА"
    form" Ass1 ОК"
  filter-end
  ;

slot: <Perf/Prosp>  \ 4
  4 slot-empty!
  form" -noperf "

  4 slot-full!

  \ 3. Показатели позиции 2 (Conv1) и показатель Perf (I)бIС
  \ (4) в пределах одной словоформы встречаются только в
  \ случае заполнения позиции 2 кумулятивным показателем
  \ Conv.Neg или если заполнена позиция 3 [тооз-ып-таа-быс-ты-лар
  \ – (кончить-Convп-Add-Perf-RPast-Pl) ‘почти закончили’].
  filter-start( 2 slot-empty?  flag-Conv.Neg flag-is? OR  3 slot-full? OR )
    flag-Perf flag-set
      form" Perf (І)бІс"
    flag-Perf flag-clear
  filter-end

  \ 6. Показатель позиции 4 Prosp АК встречается только перед
  \ формами на чА (Dur, Pres)
  filter-start( flag-Dur-or-Pres flag-is? )
    form" Prosp АК"
  filter-end
  ;

slot: <Dur>  \ 5
  5 slot-empty!
  form" -nodur "

  5 slot-full!
  \ 7. Показатель Dur1 и(р) заполняется только, если позиции
  \ 1, 3, 4 не заполнены, а в позиции 0 стоит пар- или кил-
  \ (но при этих основах может выбираться с тем же успехом и
  \ показатель Dur чАТ, свободное варьирование). [Кажется,
  \ между париган (Dur1) и парчатхан (Dur) есть семантическая
  \ разница, но объяснение в грамматике непонятное.]
  filter-start( 1 slot-empty?  3 4 slot-range-empty?  AND  is-пар/кил? AND )
    \ 8. Показатель Dur1 может стоять либо непосредственно перед
    \ пробелом, либо непосредственно перед Person (позиция 20),
    \ либо непосредственно перед показателем Past ГА(н) (позиция 7).
    filter-start( 6 22 slot-range-empty?
                  6 19 slot-range-empty?  20 slot-full?  AND
                  6 slot-empty?  flag-Past flag-is?  AND
                  OR OR )
      flag-Dur1.i flag-set
        form" Dur1₁ и"
      flag-Dur1.i flag-clear
      \ 8.1. Dur1 перед Past ГА(н) может принять только форму и;
      \ во всех прочих случаях (т.е. перед Person и концом
      \ словоформы) варианты и и ир находятся в свободном
      \ варьировании.
      filter-start( flag-Past flag-empty? )
        flag-Dur1.ir flag-set
          form" Dur1₂ ир"
        flag-Dur1.ir flag-clear
      filter-end
    filter-end
  filter-end

  \ 26. Для каждого из следующих показателей: Dur чАТ, Pres чА, Indir
  \ тIр верно следующее: они не могут быть заполнены, если при этом
  \ непосредственно перед ними обнаруживается морфема, оканчивающаяся
  \ на гласную - кроме морфем из позиции Ptcl1.
  filter-start( 3 slot-full?  4 slot-empty?  AND
                5 form-slot-vowel-at-left?  NOT
                OR )
    flag-Dur flag-set
      form" Dur чАт"
    flag-Dur flag-clear
  filter-end
  ;

slot: <Neg/Iter>  \ 6
  6 slot-empty!
  form" -noneg/iter "

  \ 11. Показатели Neg.Fut ПАС, Neg.Conv Пин, Neg.Сonv.Abl
  \ Пин.Аң исключают заполнение поз.2 и 6.
  filter-start( flag-Neg7 flag-empty? )
    6 slot-full!

    \ 11.1. Показатель Conv.Neg исключает заполнение Neg в поз. 6.
    filter-start( flag-Conv.Neg flag-empty? )
      flag-Neg6 flag-set
      form" Neg ПА"
      flag-Neg6 flag-clear
    filter-end

    \ 14. Непосредственно после Iter возможны только: Past
    \ ГА(н), Person, PredPl, Ptcl3 или конец словоформы.
    filter-start( flag-Past flag-is?
                  7 19 slot-range-empty?
                  OR )
      flag-Iter flag-set
        form" Iter АдІр"
      flag-Iter flag-clear
    filter-end

    form" Dur.Iter чАдІр"
  filter-end
  ;

slot: <Tense/Mood/Conv2>  \ 7
  7 slot-empty!
  form" -notense "

  7 slot-full!

  \ 26. Для каждого из следующих показателей: Dur чАТ, Pres чА, Indir
  \ тIр верно следующее: они не могут быть заполнены, если при этом
  \ непосредственно перед ними обнаруживается морфема, оканчивающаяся
  \ на гласную - кроме морфем из позиции Ptcl1.
  filter-start( 3 slot-full?  4 6 slot-range-empty?  AND
                7 form-slot-vowel-at-left?  NOT
                OR )
    flag-Pres flag-set
      form" Pres чА"
    flag-Pres flag-clear
  filter-end

  flag-Neg7 flag-set
    form" Neg.Fut ПАс"
  flag-Neg7 flag-clear

  flag-Past flag-set
    \ 9.1. Past ГА(н) всегда принимает форму ГА непосредственно
    \ перед всеми Person: пар-га-м 'я шел', тiк-ке-зер 'вы шили'.
    \ В остальных случаях принимается форма ГАн
    filter-start( 8 19 slot-range-empty?  20 slot-full?  AND )
      form" Past₁ ГА"
    filter-else
      form" Past₂ ГАн"
    filter-end
  flag-Past flag-clear

  \ 9.3. Показатель Fut А(р) принимает форму А непосредственно
  \ перед 1.sg.br: пар-а-м ‘я пойду’, ади-м ‘я буду называть’,
  \ кил-е-м ‘я прийду’. [В диалектах Fut может также принимать
  \ форму А непосредственно перед показателем Dat ГА: пар-ар-ға
  \ > пар-а-ға 'идти'.] В прочих случаях фигурирует только форма Ар.
  filter-start( 8 19 slot-range-empty?  flag-1sg.br flag-is?  AND )
    flag-Fut.a flag-set
      form" Fut₁ А"
    flag-Fut.a flag-clear
  filter-else
    flag-Fut.ar flag-set
      form" Fut₂ Ар"
    flag-Fut.ar flag-clear
  filter-end

  flag-Hab flag-set
    \ 9.2. Показатель Hab ҶА(ң) может принимать форму ҶА
    \ непосредственно перед всеми Person: ырла-ӌаң-мын //
    \ ырла-ча-м ‘я пел (обычно)’, кил-бе-ҷең-міс //
    \ кил-бе-ҷе-біс ‘мы не приходили (обычно)’. А может и
    \ принимать форму ҶАң. В остальных случаях принимается
    \ форма ҶАң
    filter-start( 8 19 slot-range-empty?  20 slot-full?  AND )
      flag-Hab.ca flag-set
        form" Hab₁ ЧА"
      flag-Hab.ca flag-clear
    filter-end
    flag-Hab.cang flag-set
      form" Hab₂ ЧАң"
    flag-Hab.cang flag-clear
  flag-Hab flag-clear

  flag-RPast flag-set
    \ 12. Непосредственно после показателя недавно прошедшего
    \ времени (RPast) может следовать только краткий
    \ лично-числовой показатель (Person) или показатель
    \ ирреалиса (Irr);
    filter-start( 8 22 slot-range-empty?
                  8 19 slot-range-empty?  flag-Person.br flag-is?  AND  OR
                  8 slot-empty?  9 slot-full?  AND  OR )
      form" RPast ТІ"
    filter-end
  flag-RPast flag-clear

  \ 10. Показатель Cunc не встречается в одной
  \ словоформе с показателями Perf или отрицательными
  \ (всеми, в названия которых входит элемент Neg);
  filter-start( flag-Perf flag-empty?
                flag-Conv.Neg flag-empty? AND
                flag-Neg6 flag-empty? AND
                flag-Neg7 flag-empty? AND )
    form" Cunc ГАлАК"
  filter-end

  flag-Cond flag-set
    \ 13. Непосредственно после показателя условного
    \ наклонения (Cond) может следовать только краткий
    \ лично-числовой показатель (Person);
    filter-start( 8 22 slot-range-empty?
                  8 19 slot-range-empty?  flag-Person.br flag-is?  AND  OR )
      form" Cond СА"
    filter-end
  flag-Cond flag-clear
  flag-Opt-or-Assum flag-set
    form" Opt ГАй"
    form" Assum ГАдАG"
  flag-Opt-or-Assum flag-clear

  \ 25. Показатели Lim ГАли, Convп (I)П, Convа; Convпас;
  \ Neg.Conv и Neg.Conv.Abl, находясь в позиции 7, могут
  \ только заканчивать словоформу
  filter-start( 8 22 slot-range-empty? )
    form" Lim ГАли"

    flag-Neg7 flag-set
      flag-Conv2 flag-set
        form" Neg.Conv Пин"
        form" Neg.Conv.Abl ПинАң"
      flag-Conv2 flag-clear
    flag-Neg7 flag-clear

    flag-Conv2 flag-set
      form" Conv.p (І)п"
      form" Conv.pas АбАс"
      form" Conv.a А"
    flag-Conv2 flag-clear
  filter-end
  ;

slot: <Indir>  \ 8
  8 slot-empty!
  form" -noindir "

  \ 15. Показатель поз. 8 Indir TIр бывает либо при
  \ незаполненных позициях 6, 7, либо при одновременном
  \ заполнении позиций 6 и 7 показателями Neg (ПА) и Past
  \ (ГАн).
  filter-start( 6 7 slot-range-empty?
                flag-Neg6 flag-is?  flag-Past flag-is?  AND  OR )
    \ 26. Для каждого из следующих показателей: Dur чАТ, Pres чА, Indir
    \ тIр верно следующее: они не могут быть заполнены, если при этом
    \ непосредственно перед ними обнаруживается морфема, оканчивающаяся
    \ на гласную - кроме морфем из позиции Ptcl1.
    filter-start( 3 slot-full?  4 7 slot-range-empty?  AND
                  8 form-slot-vowel-at-left?  NOT
                  OR )
      8 slot-full!
      form" Indir ТІр"
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
  form" Affirm ЧІК"
  ;

slot: <Pl₁>  \ 11
  11 slot-empty!
  form" -nopl1 "

  \ 16. Показатели позиций 11, 12, 13 могут присутствовать в
  \ словоформе только вместе с пок. поз. 14 Attr КI – и
  \ наоборот, для вычленения Attr в словоформе нужно
  \ присутствие одного или нескольких из этих полей.
  filter-start( 14 slot-full? )
    11 slot-full!
    form" Pl₁ ЛАр"
  filter-end
  ;

slot: <Poss₁>  \ 12
  12 slot-empty!
  form" -noposs1 "

  \ 16. Показатели позиций 11, 12, 13 могут присутствовать в
  \ словоформе только вместе с пок. поз. 14 Attr КI – и
  \ наоборот, для вычленения Attr в словоформе нужно
  \ присутствие одного или нескольких из этих полей.
  filter-start( 14 slot-full? )
    12 slot-full!

    flag-Poss1.nonpl flag-set
      form" 1pos.sg (І)м"
      form" 2pos.sg (І)ң"
      form" 3pos (з)І"
    flag-Poss1.nonpl flag-clear
    form" 1pos.pl (І)бІс"
    form" 2pos.pl (І)ңАр"
  filter-end
  ;

slot: <Case₁>  \ 13
  13 slot-empty!
  form" -nocase1 "

  \ 16. Показатели позиций 11, 12, 13 могут присутствовать в
  \ словоформе только вместе с пок. поз. 14 Attr КI – и
  \ наоборот, для вычленения Attr в словоформе нужно
  \ присутствие одного или нескольких из этих полей.
  filter-start( 14 slot-full? )
    13 slot-full!

    \ 17. В поз. 13/18 Case набор аффиксов посессивного
    \ склонения выбирается: а) в случае заполнения позиций
    \ 12/16 (Poss) аффиксами не-множественного числа (т.е.
    \ 1pos.sg, 2pos.sg и 3pos); б) при заполнении поз. 17
    \ (Apos); в) после основ, которые указаны в словарных
    \ статьях у Nomen в поле FORM [там приведены стяженные
    \ формы 3pos].
    filter-start( flag-Poss1.nonpl flag-empty?  flag-Poss2.nonpl flag-empty?  AND
                  17 slot-empty?  AND )
      form" Dat ГА"
      form" Loc ТА"
      form" All САр"
    filter-else
      form" Dat (н)А"
      form" Loc (н)ТА"
      form" All (н)САр"
    filter-end
  filter-end
  ;

slot: <Attr>  \ 14
  14 slot-empty!
  form" -noattr "

  \ 16. Показатели позиций 11, 12, 13 могут присутствовать в
  \ словоформе только вместе с пок. поз. 14 Attr КI – и
  \ наоборот, для вычленения Attr в словоформе нужно
  \ присутствие одного или нескольких из этих полей.
  filter-start( 11 13 slot-range-full? )
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
    form" 2pos.sg (І)ң"
    form" 3pos (з)І"
  flag-Poss2.nonpl flag-clear
  form" 1pos.pl (І)бІс"
  form" 2pos.pl (І)ңАр"
  ;

slot: <Apos>  \ 17
  17 slot-empty!
  form" -noapos "

  17 slot-full!
  form" Apos Ни"
;

slot: <Case₂>  \ 18
  18 slot-empty!
  form" -nocase2 "

  18 slot-full!

  form" Gen НІң"

  \ 17. В поз. 13/18 Case набор аффиксов посессивного
  \ склонения выбирается: а) в случае заполнения позиций
  \ 12/16 (Poss) аффиксами не-множественного числа (т.е.
  \ 1pos.sg, 2pos.sg и 3pos); б) при заполнении поз. 17
  \ (Apos); в) после основ, которые указаны в словарных
  \ статьях у Nomen в поле FORM [там приведены стяженные
  \ формы 3pos].
  filter-start( flag-Poss1.nonpl flag-empty?  flag-Poss2.nonpl flag-empty?  AND
                17 slot-empty?  AND )
    form" Dat ГА"
    form" Acc НІ"
    form" Loc ТА"
    form" Abl ДАң"
    form" All САр"
    form" Prol ЧА"
    form" Delib ДАңАр"
    form" Comp ТАG"
  filter-else
    form" Dat (н)А"
    filter-start( 18 form-slot-vowel-at-left? )
      form" Acc₂ Н"
    filter-else
      form" Acc₁ НІ"
    filter-end
    form" Loc (н)ТА"
    form" Abl нАң"
    form" All (н)САр"
    form" Prol (н)ЧА"
    form" Delib (н)нАңАр"
    form" Comp (н)ТАG"
  filter-end

  form" Instr нАң"
  ;

slot: <Ptcl₂>  \ 19
  19 slot-empty!
  form" -noptcl2 "

  19 slot-full!

  form" Ass ОК"
  \ 18. Аффиксам Adv, Adv1, Adv 2 из поз. 19 непосредственно
  \ предшествует Attr КI из поз. 14 или падеж (один из
  \ показателей поз. 18). Аффикс Adv Ли может также
  \ присоединяться к показателям поз. 7 (причастиям).
  filter-start( 14 slot-full?  16 18 slot-range-empty?  AND
                18 slot-full?  OR )
    form" Adv1 (І)зІн"
    form" Аdv2 ТІн"
  filter-end
  filter-start( 14 slot-full?  16 18 slot-range-empty?  AND
                18 slot-full?  OR
                7 slot-full?  8 18 slot-range-empty?  AND  OR )
    form" Adv Ли"
  filter-end
  ;

slot: <Person>  \ 20
  20 slot-empty!
  form" -3prs.sg "

  20 slot-full!

  filter-start( full-person-allowed? )
    form" 1sg ПІн"
    form" 1sg.dial СІм"
    form" 2sg СІң"

    flag-1.pl flag-set
      form" 1pl ПІс"
    flag-1.pl flag-clear
    form" 2pl САр"
    form" 2pl.dial СІңАр"
  filter-end

  filter-start( mix-person-allowed? )
    flag-1sg.br flag-set
      form" 1sg.mix м"
    flag-1sg.br flag-clear
    form" 2sg.mix СІң"

    flag-1.pl flag-set
      form" 1pl.mix ПІс"
    flag-1.pl flag-clear
    form" 2pl.mix САр"
  filter-end

  filter-start( short-person-allowed? )
    flag-Person.br flag-set
      flag-1sg.br flag-set
        form" 1sg.br м"
      flag-1sg.br flag-clear
      form" 2sg.br ң"
      form" 1pl.br ПІс"
      form" 2pl.br (І)ңАр"
    flag-Person.br flag-clear
  filter-end

  \ 19. Показатели поз. 20 (Person) с пометой Imp могут быть
  \ только у слов, имеющих помету Verbum; они следуют
  \ непосредственно после заполнителя позиции с номером меньше
  \ или равно 6
  filter-start( verb?  7 19 slot-range-empty?  AND )
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

slot: <PredPl>  \ 21
  21 slot-empty!
  form" -nopredpl "

  21 slot-full!
  \ 21. Показатель поз. 21 PredPl может стоять после: а) пок-ля
  \ времени (позиция 7 (за исключением ConvA, ConvP, Neg.Conv
  \ (.Abl), Lim, согласно правилу 25) + Indir TIр + Affirm ЧIК +
  \ Iter AдIр + Dur.Iter чАдIр), б) пок-ля падежа (п. 18) или
  \ посессивности (п. 16), в) некоторых поклей Person (1pl,
  \ Imp.3), г) чистой именной основы.
  filter-start( 6 slot-full?  7 20 slot-range-empty?  AND  flag-Neg6 flag-empty? AND  \ Iter
                7 slot-full?  8 20 slot-range-empty?  AND  \ Tense
                8 slot-full?  9 20 slot-range-empty?  AND  \ Indir
                10 slot-full?  11 20 slot-range-empty?  AND  \ Affirm
                16 slot-full?  17 20 slot-range-empty?  AND  \ Poss
                18 slot-full?  19 20 slot-range-empty?  AND  \ Case
                flag-1.pl flag-Imp.3 OR  flag-is?
                1 20 slot-range-empty?
                OR OR OR OR OR OR OR )
    form" PredPl ЛАр"
  filter-end
  ;

slot: <Ptcl₃>  \ 22
  22 slot-empty!
  form" -noptcl3 "

  22 slot-full!

  form" Ass ОК"
  \ 23. Пок-тель Foc может встретиться: а) в словоформах с
  \ пометой Nomen; б) в глаголах, у которых есть временные
  \ показатели Past ГАн и Hab ЧАӊ.
  filter-start( verb? NOT
                flag-Past-or-Hab flag-is?  OR )
    form" Foc ТІр"
  filter-end

  \ 22. Показатель Perm присоединяется только к императивным
  \ показателям из поз. 20 Person.
  filter-start( 21 slot-empty?  flag-Imp flag-is?  AND )
    form" Perm ТАК"
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
 ' <Apos> ,
 ' <Case₂> ,
 ' <Ptcl₂> ,
 ' <Person> ,
 ' <PredPl> ,
 ' <Ptcl₃> , 0 ,
HERE slot-stack - 1- CELL / CONSTANT /slot-stack
