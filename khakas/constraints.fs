0  S" пар кил "  strlist-parse-alloc  CONSTANT пар|кил
: is-пар/кил?  ( -- f )
  paradigm-stems @  пар|кил  strlists-intersect? ;

0  S" мин син ол піс сірер олар " strlist-parse-alloc CONSTANT personal-pronouns
: is-personal-pronoun?  ( -- f )
  paradigm-stem 2@  personal-pronouns  strlist-in? ;

\ 0. Слова с пометой INVAR (не Nomen и не Verbum) никаких
\ показателей не присоединяют! Слова с пометой Invar1
\ присоединяют Ptcl3 ОК.
: constraint-0  ( -- f )
  invar1? NOT
  1 20 slot-range-empty?  flag-Ass₃ flag-is? AND
  1 21 slot-range-empty?
  OR OR ;

\ 1. Позиции 1–8 могут заполняться только у слов с пометой
\ Verbum; позиция 9 - только у слов с пометой Nomen.
: constraint-1  ( -- f )
  1 8 slot-range-empty?  verb?  OR
  9 slot-empty?  nomen?  OR
  AND ;

\ 2. Заполнение позиций 9, 11-17 (падежно-посессивный блок)
\ может происходить: 1) либо сразу после позиции 0, если
\ слово имеет помету Nomen, 2) либо непосредственно после
\ заполненной позиции 7 одним из причастных показателей, если
\ слово имеет помету Verbum; 3) (для позиций 11-17) либо
\ непосредственно после заполненной позиции 9 (Comit), 4) (для
\ позиций 11-17) либо непосредственно после заполненной позиции
\ 10 (Affirm).
: verb-with-Tense-non-RPast-Cond-Pres-Conv2?  ( -- f )
  7 slot-full?   8 10 slot-range-empty?  AND IF
    flag-RPast-or-Cond-or-Pres-or-Conv2 flag-is? NOT
  ELSE FALSE THEN
  ;
: constraint-2  ( -- f )
  9 slot-empty?  11 17 slot-range-empty?  AND
  nomen?
  verb-with-Tense-non-RPast-Cond-Pres-Conv2?  \ причастия
  9 10 slot-range-full?                       \ Comit, Affirm
  OR OR OR ;

\ 3. Показатели позиции 2 (Conv1) и показатель Perf (I)бIС
\ (4) в пределах одной словоформы встречаются только в
\ случае заполнения позиции 2 кумулятивным показателем
\ Conv.Neg или если заполнена позиция 3 [тооз-ып-таа-быс-ты-лар
\ – (кончить-Convп-Add-Perf-RPast-Pl) ‘почти закончили’].
: constraint-3  ( -- f )
  2 slot-empty?
  flag-Conv.Neg flag-is?
  3 slot-full?
  OR OR ;

\ 4. Показатели позиции 2 (Conv1) могут встретиться только в
\ словоформе, где есть один или несколько из показателей: 1)
\ позиции 3 (Ptcl1), 2) Dur чАТ, 3) Perf  (I)бIС, 4) Pres чА,
\ 5) Indir ТIр, 6) Dur.Iter чАдIр.
: constraint-4  ( -- f )
  3 slot-full?
  flag-Dur-or-Pres flag-is?
  flag-Perf flag-is?
  8 slot-full?               \ Indir
  flag-Dur.Iter flag-is?
  OR OR OR OR ;

\ 5. Показатели поз. 3 (внутренние частицы) допускаются
\ только при заполненной позиции 7 (время) или 8 (Indir) или
\ при дуративе (Dur чАт, Dur.Iter чАдIр.).
: constraint-5  ( -- f )
  7 8 slot-range-full?
  flag-Dur flag-is?
  flag-Dur.Iter flag-is?
  OR OR ;

\ 6. Показатель позиции 4 Prosp АК встречается только перед
\ морфемами Pres чА и Dur чАТ.
: constraint-6  ( -- f )
  flag-Dur-or-Pres flag-is? ;

\ 7. Показатель Dur1 и(р) заполняется только, если позиции
\ 1, 3, 4 не заполнены, а в позиции 0 стоит пар- или кил-
\ (но при этих основах может выбираться с тем же успехом и
\ показатель Dur чАТ, свободное варьирование). [Кажется,
\ между париган (Dur1) и парчатхан (Dur) есть семантическая
\ разница, но объяснение в грамматике непонятное.]
: constraint-7  ( -- f )
  1 slot-empty?
  3 4 slot-range-empty?
  is-пар/кил?
  AND AND ;

\ 8. Показатель Dur1 может стоять: 1) либо непосредственно
\ перед пробелом, 2) либо непосредственно перед Person
\ (позиция 19) или PredPl (позиция 20), 3) либо
\ непосредственно перед показателями Past ГА(н), Convп,
\ Cond СА (позиция 7).
: constraint-8  ( -- f )
  6 21 slot-range-empty?
  6 18 slot-range-empty?  19 slot-full?  AND
  6 19 slot-range-empty?  20 slot-full?  AND
  flag-Past flag-is?  flag-Cond flag-is?  flag-Convₚ flag-is?  OR OR  6 slot-empty?  AND
  OR OR OR ;

\ 8.1. Dur1 в роли видового показателя морфонологически
\ распределен: перед Past ГА(н) и перед Cond СА может
\ принять только форму и, перед Convп - только форму ир;
\ в роли показателя времени (т.е. перед Person, PredPl и
\ концом словоформы) варианты и и ир находятся в
\ свободном (точнее, диалектном) варьировании.
: constraint-8.1ᵢ  ( -- f )
  flag-Convₚ flag-empty? ;
: constraint-8.1ᵢᵣ
  flag-Past flag-empty?
  flag-Cond flag-empty?
  AND ;

\ 9.1. Past ГА(н) всегда принимает форму ГА непосредственно
\ перед всеми Person: пар-га-м 'я шел', тiк-ке-зер 'вы шили'.
\ В остальных случаях принимается форма ГАн
: constraint-9.1  ( -- f )
  8 18 slot-range-empty?
  19 slot-full?
  AND ;

\ 9.2. Показатель Hab ҶА(ң) может принимать форму ҶА
\ непосредственно перед всеми Person: ырла-ӌаң-мын // ырла-ча-м
\ ‘я пел (обычно)’, кил-бе-ҷең-міс // кил-бе-ҷе-біс ‘мы не
\ приходили (обычно)’. А может и принимать форму ҶАң. В
\ остальных случаях принимается форма ҶАң
: constraint-9.2  ( -- f )
  8 18 slot-range-empty?
  19 slot-full?
  AND ;

\ 9.3. Показатель Fut А(р) может принимать форму А
\ непосредственно перед 1.sg.br и 1.pl: пар-а-м ‘я пойду’,
\ ади-быс ‘мы будем называть’. [В диалектах Fut может также
\ принимать форму А непосредственно перед показателем Dat ГА,
\ при этом Г не выпадает: пар-ар-ға > пар-а-ға 'идти'.] В прочих
\ случаях фигурирует только форма Ар. В первом лице бывает также
\ и форма Ар: пар-ар-бын ‘я пойду’, адир-быс ‘мы будем называть’.
: constraint-9.3  ( -- f )
  8 18 slot-range-empty?
  flag-1sg.br flag-is?  flag-1.pl flag-is?  OR
  AND ;

\ 10. Показатель Cunc не встречается в одной
\ словоформе с показателями Perf или отрицательными
\ (всеми, в названия которых входит элемент Neg);
: constraint-10  ( -- f )
  flag-Perf flag-empty?
  flag-Conv.Neg flag-empty?
  flag-Neg flag-empty?
  flag-Neg7 flag-empty?
  AND AND AND ;

\ 10.1. Показатель Fut А(р) не встречается в одной словоформе с
\ отрицательными показателями (Neg, Conv.Neg[, Neg.Conv,
\ Neg.Сonv.Abl - в этом же слоте]).
: constraint-10.1  ( -- f )
  flag-Neg flag-empty?
  flag-Conv.Neg flag-empty?
  AND ;

\ 11. Показатели Neg.Fut ПАС, Neg.Conv Пин, Neg.Сonv.Abl
\ Пин.Аң исключают заполнение поз.2 и 6.
: constraint-11  ( -- f )
  flag-Neg7 flag-empty? ;

\ 11.1. Показатель Conv.Neg исключает заполнение Neg в поз. 6.
: constraint-11.1  ( -- f )
  flag-Conv.Neg flag-empty? ;

\ 12. Непосредственно после показателя недавно прошедшего
\ времени (RPast) может следовать только краткий
\ лично-числовой показатель (Person), число предиката
\ (PredPl) или показатель аффирматива (Affirm) или Ptcl3
: constraint-12  ( -- f )
  8 19 slot-range-empty?  \ PredPl or Ptcl3 or end-of-wordform
  8 18 slot-range-empty?  flag-Person.br flag-is?  AND  \ Person
  8 9 slot-range-empty?  10 slot-full?  AND  \ Affirm
  OR OR ;

\ 13. Непосредственно после показателя условного наклонения
\ (Cond) может следовать только краткий лично-числовой
\ показатель (Person) или число предиката (PredPl) или Ptcl3
: constraint-13  ( -- f )
  8 19 slot-range-empty?  \ PredPl or Ptcl3 or end-of-wordform
  8 18 slot-range-empty?  flag-Person.br flag-is?  AND  \ Person
  OR ;

\ 14. Непосредственно после Iter возможны только: Past
\ ГА(н), Person, PredPl, Ptcl3 или конец словоформы.
: constraint-14  ( -- f )
  flag-Past flag-is?
  7 18 slot-range-empty?
  OR  ;

\ 15. Показатель поз. 8 Indir TIр бывает либо при
\ незаполненных позициях 6, 7, либо при одновременном
\ заполнении позиций 6 и 7 показателями Neg (ПА) и Past (ГАн).
: constraint-15  ( -- f )
  6 7 slot-range-empty?
  flag-Neg flag-is?  flag-Past flag-is?  AND
  OR ;

\ 16. Показатели позиций 11, 12, 13 могут присутствовать в
\ словоформе: 1) вместе с пок. поз. 14 Attr КI – и
\ наоборот, для вычленения Attr в словоформе нужно
\ присутствие одного или нескольких из этих полей. Сочетание
\ Poss1+Attr KI возможно только при наличии Case1.
\ 2) Также возможны следующие сочетания:
\ а) Gen1+Pl2 для личных местоимений (мин, син, ол, пiс, сiрер, олар)
\ б) (Pl1+) (pos1+) Loc1+pos2 для Nomen
\ в) выраженный 3pos1+pos2 для Nomen
\ г) 3pos1+Pl2+pos2 для Nomen
\ д) Pl1/Pos1+Gen.3pos для всех
\ е) (Pl1+) (pos1+) All1+Abl2: столзартын ‘со стороны стола’
: constraint-16₁₁  ( -- f )
  14 slot-full?  \ 16.1
  13 slot-full?  \ 16.2б
  flag-Gen.3pos flag-is?  \ 16.2д
  flag-All1 flag-is?  flag-Abl₂ flag-is?  AND  \ 16.2е
  OR OR OR ;
: constraint-16₁₂  ( -- f )
  13 slot-full?  \ 16.1, 16.2б
  flag-3pos₁ flag-is?  12 form-slot-flags 0=  13 14 slot-range-empty?  15 16 slot-range-full? AND AND AND \ 16.2в
  flag-3pos₁ flag-is?  13 14 slot-range-empty?  15 16 slot-range-full? AND AND  \ 16.2г
  flag-Gen.3pos flag-is? \ 16.2д
  flag-All1 flag-is?  flag-Abl₂ flag-is?  AND  \ 16.2е
  OR OR OR OR ;
: constraint-16_1б  ( -- f )
  11 13 slot-range-full? ;
: constraint-16_2а  ( -- f )
  14 slot-full?  \ Attr
  is-personal-pronoun?  15 slot-full?  AND  \ Pl2
  OR ;
: constraint-16_2б  ( -- f )
  14 slot-full?  \ Attr
  nomen?  16 slot-full?  AND  \ Poss2
  OR ;
: constraint-16_2е  ( -- f )
  14 slot-full?  \ Attr
  flag-Abl₂ flag-is?
  OR ;

\ 17. В поз. 13/17 Case набор аффиксов посессивного
\ склонения выбирается: а) в случае заполнения позиций
\ 12/16 (Poss) соответственно аффиксами не-множественного
\ числа (т.е. 1pos.sg, 2pos.sg и 3pos); в) после
\ Nomen-основ, у которых заполнено в словарных статьях поле
\ FORM [там приведены стяженные формы 3pos].
: constraint-17₁₃  ( -- f )
  flag-Poss1.nonpl flag-empty?
  dictflag-poss dictflag-empty?
  AND ;
: constraint-17₁₇
  flag-Poss2.nonpl flag-empty?
  dictflag-poss dictflag-empty?
  AND ;

\ 18. Аффиксу Adv из поз. 18 непосредственно предшествует Attr
\ КI из поз. 14, Case2 (один из показателей поз. 17) или один из
\ причастных показателей поз. 7.
: constraint-18  ( -- f )
  14 slot-full?  15 17 slot-range-empty?  AND
  17 slot-full?
  7 slot-full?  8 17 slot-range-empty?  AND
  OR OR ;

\ 19. Показатели поз. 19 (Person) с пометой Imp могут быть
\ только у слов, имеющих помету Verbum; они следуют
\ непосредственно после заполнителя позиции с номером меньше
\ или равно 6
: constraint-19  ( -- f )
  verb?
  7 18 slot-range-empty?
  AND ;

\ 20. К словам с пометой NOMEN присоединяются полные
\ лично-числовые показатели (список внутри Person), которые
\ могут следовать после любых морфем. У глаголов заполнение
\ Person возможно только при незаполненных позициях с 11 по 17 и
\ при наличии показателей:
\ для полных форм: Iter A.дIр, Dur.Iter чАдIр, Affirm ЧIК, Opt
\ ГАй, Assum ГАдАГ, Indir ТIр, Cunc ГАлАК, Neg.Fut ПАс, Dur1 в
\ форме ир, Hab в форме ҶАң, Fut в форме Ар;
\ для смешанных форм: Pres ЧА, Dur1 в форме и, Past ГА(н), Hab в
\ форме ҶА, Fut в форме А;
\ для кратких форм: Cond СА, Rpast ТI.
: constraint-20-full-person  ( -- f )
  verb? IF
    11 17 slot-range-empty? IF
      8 slot-full?  10 slot-full?  OR
      flag-Iter-or-Opt-or-Assum-or-Cunc-or-Neg.Fut-or-Dur1ᵢᵣ-or-Hab₂-or-Futₐᵣ flag-is?  OR
    ELSE FALSE THEN
  ELSE nomen? THEN ;
: constraint-20-mix-person  ( -- f )
  verb?
  15 17 slot-range-empty?  AND
  flag-Pres-or-Dur1ᵢ-or-Past-or-Hab₁-or-Futₐ flag-is? AND ;
: constraint-20-short-person  ( -- f )
  verb?
  15 17 slot-range-empty?  AND
  flag-RPast-or-Cond flag-is?  AND ;

\ 21. Показатель поз. 20 PredPl может стоять после: а) пок-ля
\ времени (позиция 7 (за исключением ConvA, ConvP, Neg.Conv
\ (.Abl), Lim, согласно правилу 25) + Indir TIр + Affirm ЧIК +
\ Iter AдIр + Dur.Iter чАдIр + Dur1 и(р)), б) пок-ля падежа
\ (п. 17) или посессивности (п. 16), в) некоторых полей Person
\ (1pl, Imp.3), г) чистой именной основы, д) пок-ля Ptcl2
\ (поз.18): Хай пірее одыртхан ағастар парохтар мында - Здесь
\ имеется и несколько посаженных деревьев (ГХЯ).
: constraint-21  ( -- f )
  6 slot-full?  7 19 slot-range-empty?  AND  flag-Neg flag-empty? AND  \ Iter
  7 slot-full?  8 19 slot-range-empty?  AND  \ Tense
  8 slot-full?  9 19 slot-range-empty?  AND  \ Indir
  10 slot-full?  11 19 slot-range-empty?  AND  \ Affirm
  16 slot-full?  17 19 slot-range-empty?  AND  \ Poss
  17 slot-full?  18 19 slot-range-empty?  AND  \ Case
  18 slot-full?  19 slot-empty?  AND  \ Ptcl2
  flag-Dur1ᵢᵣ flag-Dur1ᵢ flag-OR  flag-is?
  flag-1.pl flag-Imp.3 flag-OR  flag-is?
  1 19 slot-range-empty?  nomen?  AND
  OR OR OR OR OR OR OR OR OR ;

\ 22. Показатель Perm присоединяется только к императивным
\ показателям из поз. 19 Person, к отрицанию ПА или к чистой
\ основе слов категории Verbum (имеющей значение Imp.2sg).
: constraint-22  ( -- f )
  20 slot-empty?  flag-Imp flag-is?  AND
  7 20 slot-range-empty?  flag-Neg flag-is?  AND
  1 20 slot-range-empty?  verb?  AND
  OR OR ;

\ 23. Пок-тель Foc может встретиться: а) в словоформах с
\ пометой Nomen; б) в глаголах, у которых есть временные
\ показатели Past ГАн и Hab ЧАӊ.
: constraint-23  ( -- f )
  nomen?
  flag-Past-or-Hab flag-is?
  OR ;

\ 25. После деепричастных показателей позиции 7 (Lim ГАли,
\ Convп (I)П, Convа; Convпас; Neg.Conv и Neg.Conv.Abl)
\ может стоять только показатель Ass ОК из позиции Ptcl3.
: constraint-25  ( -- f )
  8 21 slot-range-empty?
  flag-Ass₃ flag-is?
  OR ;

\ 26. Для каждого из следующих показателей: Dur чАТ, Dur.Iter
\ чАдIр, Pres чА, Indir тIр верно следующее: они не могут быть
\ заполнены, если при этом непосредственно перед ними
\ обнаруживается морфема, оканчивающаяся на гласную - кроме
\ морфем из позиции Ptcl1.
: constraint-26₅  ( -- f )
  3 slot-full?  4 slot-empty?  AND
  5 form-slot-vowel-at-left?  NOT
  OR ;
: constraint-26₆  ( -- f )
  3 slot-full?  4 5 slot-range-empty?  AND
  6 form-slot-vowel-at-left?  NOT
  OR ;
: constraint-26₇  ( -- f )
  3 slot-full?  4 6 slot-range-empty?  AND
  7 form-slot-vowel-at-left?  NOT
  OR ;
: constraint-26₈  ( -- f )
  3 slot-full?  4 7 slot-range-empty?  AND
  8 form-slot-vowel-at-left?  NOT
  OR ;

\ 27. Позиции Conv1, Ptcl1, Pl1, Poss1, Case1 не могут быть
\ последними заполненными позициями в словоформe
: constraint-27  ( -- f )
  2 slot-empty?  3 21 slot-range-full?  OR
  3 slot-empty?  4 21 slot-range-full?  OR
  11 slot-empty?  12 21 slot-range-full?  OR
  12 slot-empty?  13 21 slot-range-full?  OR
  13 slot-empty?  14 21 slot-range-full?  OR
  AND AND AND AND ;

\ 29. Предикативные показатели (Person, PredPl) невозможны в
\ сочетании с падежами: Gen2, Acc2, Instr2.
: constraint-29  ( -- f )
  19 20 slot-range-empty? ;

\ 30. Алломорф Abl -тІн возможен в словоформе только при наличии All1.
: constraint-30  ( -- f )
  flag-All1 flag-is? ;

\ Неозвончаемые основы
: constraint-non-envoiceable-stem  ( -- f )
  first-form-flag untransformed-left-envoice AND NOT
  dictflag-no-envoice dictflag-empty?
  OR ;
: constraint-non-envoiced-rus  ( -- f )
  first-form-flag untransformed-left-envoice-missing AND NOT
  dictflag-no-envoice dictflag-is?
  dictflag-rus dictflag-is?
  OR OR ;

\ Запрещенные контексты для выпадения VңV
: constraint-VңV-fallout  ( -- f )
  \ флаг выпадения конечного ң находится в слоте справа
  13 form-slot-flags untransformed-fallout-VңV AND NOT
  17 form-slot-flags untransformed-fallout-VңV AND NOT
  AND ;

\ Запрещенные контексты для выпадения (СА|ТІ)ңАр
: constraint-(СА|ТІ)ңАр-fallout  ( -- f )
  19 form-slot-flags untransformed-fallout-(СА|ТІ)ңАр AND
  flag-2pl.br flag-is?
  flag-RPast-or-Cond flag-empty?
  AND AND NOT
  16 form-slot-flags untransformed-fallout-(СА|ТІ)ңАр AND
  flag-2pos.pl flag-is?
  AND NOT
  12 form-slot-flags untransformed-fallout-(СА|ТІ)ңАр AND
  flag-2pos.pl flag-is?
  AND NOT
  AND AND ;

\ Запрещенные контексты для выпадения Г после Г, ң без стяжения
: constraint-[Гң]Г-fallout  ( -- f )
  first-form-flag untransformed-fallout-confluence AND
  untransformed-fallout-confluence any-form-flag-is? NOT
  OR ;

\ Запрещенные контексты для выпадения одной из трех одинаковых согласных
: constraint-CCC-fallout  ( -- f )
  stem-last-sound stem-prev-sound <>
  stem-last-sound first-affix-starts-with? NOT
  dictflag-rus dictflag-is?  first-form-flag untransformed-fallout-CCC  AND  0<> AND
  dictflag-rus dictflag-empty?  first-form-flag untransformed-fallout-CCC AND NOT  AND
  OR OR OR ;

\ Запрещенные контексты для выпадения после долгой гласной
: constraint-VVГV-fallout  ( -- f )
  first-form-flag untransformed-fallout-VVГV AND NOT
  dictflag-rus dictflag-is?
  OR ;

\ Разрешенные контексты для невыпадения VГV
: constraint-VГV-fallout  ( -- f )
  nomen? verb? OR NOT
  first-form-flag untransformed-fallout-VГV untransformed-fallout-VңV OR AND
  1 21 slot-range-empty?
  stem-polysyllabic? NOT
  stem-last-sound gh-g-ng? NOT
  stem-prev-sound vowel? NOT
  first-affix first-sound vowel? NOT
  OR OR OR OR OR OR ;

\ Запрещенные контексты для выпадения конечного к, х
: constraint-V[кх]V-fallout  ( -- f )
  first-form-flag untransformed-fallout-V[кх]V AND NOT
  dictflag-rus dictflag-empty?
  OR ;

\ После звонких в основе глухое: пар-тыр, сом-тыр
: constraint-voicedstem+Indir  ( -- f )
  1 7 slot-range-empty?
  stem-last-sound consonant?
  stem-last-sound unvoiced? NOT
  AND AND ;

: constraint-V+Acc  ( -- f )
  17 form-slot-vowel-at-left? ;

: constraint-broken-harmony  ( -- f )
  harmony-vu-broken any-form-flag-is? NOT
  dictflag-rus dictflag-is?  first-form-flag harmony-vu-broken AND 0<>  AND
  OR ;
