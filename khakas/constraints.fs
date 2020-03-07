require khakas/slotnames.fs

0  S" пар апар кил "  strlist-parse-alloc  CONSTANT пар|кил
: is-пар/кил?  ( -- f )
  paradigm-stems @  пар|кил  strlists-intersect? ;

0  S" мин син ол піс сірер олар " strlist-parse-alloc CONSTANT personal-pronouns
: is-personal-pronoun?  ( -- f )
  paradigm-stem 2@  personal-pronouns  strlist-in? ;

\ 0. Слова с пометой INVAR (не Nomen и не Verbum) никаких
\ показателей не присоединяют! Слова с пометой Invar1
\ присоединяют Ptcl3 ОК.
: constraint-0  ( -- f )
  invar1? NOT                                         ||
  slots[ 1 <Ptcl₃> )-empty?  flag Ass₃  flag-is? AND  ||
  slot-all-empty?
  ;

\ 1. Позиции 1–8 могут заполняться только у слов с пометой
\ Verbum.
: constraint-1  ( -- f )
  verb? ||
  slots[ <Distr> <Tense/Mood/Conv2> ]-empty?
  ;

\ 2. Заполнение позиций 9, 11-17 (падежно-посессивный блок)
\ может происходить: 1) либо сразу после позиции 0, если
\ слово имеет помету Nomen, 2) либо непосредственно после
\ заполненной позиции 7 одним из причастных показателей, если
\ слово имеет помету Verbum; 3) (для позиций 11-17) либо
\ непосредственно после заполненной позиции 9 (Comit), 4) (для
\ позиций 11-17) либо непосредственно после заполненной позиции
\ 10 (Affirm).
: constraint-2  ( -- f )
  <Transp> slot-empty?  slots[ <Pl₁> <Case₂> ]-empty?  AND  ||
  nomen?  slots[ 1 <Transp> )-empty? AND                    ||
  flag participles  flag-is?  slots( <Tense/Mood/Conv2> <Transp> )-empty?  AND ||
  <Transp> slot-full?                 \ Comit, Affirm
  ;

\ 3. Показатели позиции 2 (NF) и показатель Perf (I)бIС
\ (4) в пределах одной словоформы встречаются только в
\ случае заполнения позиции 2 кумулятивным показателем
\ NF.Neg или если заполнена позиция 3 [тооз-ып-таа-быс-ты-лар
\ – (кончить-NF-Add-Perf-RPast-Pl) ‘почти закончили’].
: constraint-3  ( -- f )
  <NF> slot-empty?       ||
  flag NF.Neg  flag-is?  ||
  <Ptcl1> slot-full?
  ;

\ 4. Показатели позиции 2 (NF) могут встретиться только в
\ словоформе, где есть один или несколько из показателей: 1)
\ позиции 3 (Ptcl1), 2) Dur чАТ, 3) Perf (Ы)бЫс, Perf0 (Ы)c,
\ 4) Pres2 чАдЫр, чады(р) 5) Pres (чА, ча, чАр(Ы), ту(р)),
\ 6) Indir ТЫр, 7) PresPt чАн - или на конце словоформы.
\ Показатель NF.Neg.sh ПААн допустим при сочетании с Pres ЧА,
\ чА, чАр(Ы) и Indir ТЫр.
: constraint-4  ( -- f )
  <Ptcl1> slot-full?             ||
  flags( Dur
         Perf Perf0
         Pres2 Pres2.dial.kac
         Pres Pres.dial Pres.dial.kyz Pres.dial.sh
         Indir
         PresPt.dial ) flag-is?  ||
  slots( <NF> <Ptcl₃> ]-empty?
  ;
: constraint-4sh  ( -- f )
  flags( Pres Pres.dial Pres.dial.sh Indir ) flag-is? ;

\ 4.1. NF выбирает алломорф (I)П, если:
\ 1) он непосредственно следует за основой, которая оканчивается
\    на (выпадающие, см. ниже) п, г, ғ или ӊ;
\ 2) он непосредственно следует за основой или аффиксом,
\    оканчивающимися на гласную;
\ 3) непосредственно за ним следует Ass ОК или Cont LA.
\ NF выбирает алломорф 0 после основы или аффикса на
\ невыпадающую согласную, если после него также стоит любой
\ аффикс, кроме ОК, или конец словоформы. [Таким образом, после
\ основы на невыпадающую согласную в конце словоформы для NF
\ возможны оба алломорфа.]
: constraint-4.1ₚ  ( -- f )
  <Distr> slot-empty?  <Distr> form-slot-xc-at-left fallout-short?  AND  ||
  <NF> form-slot-vowel-at-left?                                          ||
  flags( Ass₁ Cont ) flag-is?
  ;
: constraint-4.1₀  ( -- f )
  <NF> form-slot-vowel-at-left? NOT  &&
  <NF> form-slot-xc-at-left fallout-short? NOT  &&
  flag Ass₁  flag-empty?
  ;

\ 5. Показатели поз. 3 (внутренние частицы) допускаются только
\ при заполненной позиции 8 (время) или при обычном дуративе
\ (Dur чАт). Примеров с Dur1 не попадалось.
: constraint-5  ( -- f )
  <Tense/Mood/Conv2> slot-full?  ||
  flag Dur  flag-is?
  ;

\ 5.1. Показатель Perf0 Ыс возможен [пока встретился] только
\ при наличии Ptcl1.
: constraint-5.1  ( -- f )
  <Ptcl1> slot-full?
  ;

\ 6. Показатель позиции 5 Prosp АК встречается только перед
\ морфемами Pres чА, PresPt чАн и Dur чАТ. Показатель NF в
\ этом случае в словоформе отсутствует.
: constraint-6  ( -- f )
  flags( Dur Pres PresPt.dial ) flag-is?
  <NF> slot-empty?
  AND ;

\ 7. Показатели Dur1 и(р), Dur₁.dial.kac Ат, Dur₁.dial.sag ит
\ заполняются только, если позиции 1, 3, 4 не заполнены, а в
\ позиции 0 стоит пар- или кил- (но при этих основах может
\ выбираться с тем же успехом и показатель Dur чАТ, свободное
\ варьирование). [Кажется, между париган (Dur1) и парчатхан
\ (Dur) есть семантическая разница, но объяснение в грамматике
\ непонятное.]
: constraint-7  ( -- f )
  <Distr> slot-empty?  &&
  slots( <NF> <Prosp,Dur1> )-empty?  &&
  is-пар/кил?
  ;

\ 8. Показатели Dur1 и(р) и Dur₁.dial.kac Ат могут стоять:
\ 1) либо непосредственно перед пробелом,
\ 2) либо непосредственно перед Person (позиция 19) или PredPl
\ (позиция 20),
\ 3) либо непосредственно перед показателями Past ГА(н), Convп,
\ Cond СА (позиция 7).
\ Показатель Dur₁.dial.sag ит может стоять перед Dur чАт,
\ Past ГА(н), Cond СА и PresPt.dial чАн.
: constraint-8  ( -- f )
  slots( <Prosp,Dur1> <Ptcl₃> ]-empty?                             ||
  slots( <Prosp,Dur1> <Person> )-empty?  <Person> slot-full?  AND  ||
  slots( <Prosp,Dur1> <PredPl> )-empty?  <PredPl> slot-full?  AND  ||
  flags( Past Cond Convₚ ) flag-is?  <Neg/Iter> slot-empty?  AND
  ;
: constraint-8sag  ( -- f )
  <Dur> slot-full?  ||
  slots( <Prosp,Dur1> <Tense/Mood/Conv2> )-empty?  flags( Past Cond PresPt.dial ) flag-is? ;

\ 8.1. Dur1 в роли видового показателя морфонологически
\ распределен: перед Past ГА(н) и перед Cond СА может
\ принять только форму и, перед Convп - только форму ир;
\ в роли показателя времени (т.е. перед Person, PredPl и
\ концом словоформы) варианты и и ир находятся в
\ свободном (точнее, диалектном) варьировании.
: constraint-8.1ᵢ  ( -- f )
  flag Convₚ  flag-empty? ;
: constraint-8.1ᵢᵣ
  flags( Past Cond ) flag-empty? ;

\ 9.1. Past ГА(н) всегда принимает форму ГА непосредственно
\ перед всеми Person: пар-га-м 'я шел', тiк-ке-зер 'вы шили'.
\ В остальных случаях принимается форма ГАн
: constraint-9.1  ( -- f )
  slots( <Tense/Mood/Conv2> <Person> )-empty? &&
  <Person> slot-full?
  ;

\ 9.2. Показатель Hab ҶА(ң) может принимать форму ҶА
\ непосредственно перед всеми Person: ырла-ӌаң-мын // ырла-ча-м
\ ‘я пел (обычно)’, кил-бе-ҷең-міс // кил-бе-ҷе-біс ‘мы не
\ приходили (обычно)’. А может и принимать форму ҶАң. В
\ остальных случаях принимается форма ҶАң
: constraint-9.2  ( -- f )
  slots( <Tense/Mood/Conv2> <Person> )-empty? &&
  <Person> slot-full?
  ;

\ 9.3. Показатель Fut А(р) может принимать форму А
\ непосредственно перед 1.sg.br и 1.pl: пар-а-м ‘я пойду’,
\ ади-быс ‘мы будем называть’. [В диалектах Fut может также
\ принимать форму А непосредственно перед показателем Dat ГА,
\ при этом Г не выпадает: пар-ар-ға > пар-а-ға 'идти'.] В прочих
\ случаях фигурирует только форма Ар. В первом лице бывает также
\ и форма Ар: пар-ар-бын ‘я пойду’, адир-быс ‘мы будем называть’.
: constraint-9.3  ( -- f )
  slots( <Tense/Mood/Conv2> <Person> )-empty? &&
  flags( 1sg.br 1.pl ) flag-is?
  ;

\ 9.4 Pres.dial.kyz ту(р) может принимать форму ту
\ непосредственно перед Person или PredPl. Форма тур возможна в
\ любых контекстах.
: constraint-9.4  ( -- f )
  slots( <Tense/Mood/Conv2> <Person> )-empty? &&
  slots[ <Person> <PredPl> ]-full?
  ;

\ 9.5. Pres.dial.sh чАр(Ы) перед показателями 1sg и 2sg
\ принимает форму чАрЫ. (Это нужно, чтобы не было омонимичных
\ разборов чар-ым / чары-м). В остальных контекстах показатели
\ не распределены.
: constraint-9.5  ( -- f )
  slots( <Tense/Mood/Conv2> <Person> )-full?  ||
  flags( 1sg.br 2sg.br ) flag-empty? ;


\ 10. Показатель Cunc не встречается в одной
\ словоформе с показателями Perf или отрицательными
\ (всеми, в названия которых входит элемент Neg);
: constraint-10  ( -- f )
  flags( Perf NF.Neg NF.Neg.sh Neg Neg7 ) flag-empty? ;

\ 10.1. Показатель Fut А(р) не встречается в одной словоформе с
\ отрицательными показателями (Neg, NF.Neg[, Neg.Conv,
\ Neg.Сonv.Abl - в этом же слоте]).
: constraint-10.1  ( -- f )
  flags( Neg NF.Neg NF.Neg.sh ) flag-empty? ;

\ 11. Показатели Neg.Fut ПАС, Neg.Conv Пин, Neg.Сonv.Abl
\ Пин.Аң исключают заполнение поз. 6.
: constraint-11  ( -- f )
  flag Neg7  flag-empty? ;

\ 11.1. Показатель NF.Neg исключает заполнение Neg в поз. 6.
: constraint-11.1  ( -- f )
  flag NF.Neg  flag-empty? ;

\ 12. Непосредственно после показателя недавно прошедшего
\ времени (RPast) может следовать только краткий
\ лично-числовой показатель (Person), число предиката
\ (PredPl) или показатель аффирматива (Affirm) или Ptcl3
: constraint-12  ( -- f )
  slots( <Tense/Mood/Conv2> <PredPl> )-empty?                                 ||
  slots( <Tense/Mood/Conv2> <Person> )-empty?  flag Person.br  flag-is?  AND  ||
  slots( <Tense/Mood/Conv2> <Affirm> )-empty?  &&
    <Affirm> slot-full?
  ;

\ 13. Непосредственно после показателя условного наклонения
\ (Cond) может следовать только краткий лично-числовой
\ показатель (Person) или число предиката (PredPl) или Ptcl3
: constraint-13  ( -- f )
  slots( <Tense/Mood/Conv2> <PredPl> )-empty?  ||
  slots( <Tense/Mood/Conv2> <Person> )-empty?  &&
    flag Person.br  flag-is?
  ;

\ 14. Непосредственно после Iter возможны только: Past
\ ГА(н), Person, PredPl, Ptcl3 или конец словоформы.
: constraint-14  ( -- f )
  flag Past  flag-is?  ||
  slots( <Neg/Iter> <Person> )-empty?
  ;

\ 14.1. Iter: Перед Past ГА(н) и Ptcl3 возможна только форма АдЫр, в остальных случаях - и АдЫ, и АдЫр.
: constraint-14.1  ( -- f )
  flag Past  flag-empty?  &&
  slots( <Neg/Iter> <Ptcl₃> )-empty?  <Ptcl₃> slot-full?  AND NOT
  ;

\ 16.1. Показатели позиций 10-12 (Pl1, Poss1, Case1) возможны
\ только при наличии в словоформе одного или нескольких аффиксов
\ из позиций 13-16.
: constraint-16.1  ( -- f )
  slots[ <Attr> <Case₂> ]-full? ;

\ 16.2. Показатель позиции 13 Attr КI может присутствовать в
\ словоформе только при наличии Case1. [В такой словоформе могут
\ также присутствовать морфемы из позиций Pl1 и Poss1.]
\ Морфонология у показателей поз. 10/14, 11/15, 12/16 одинаковая.
: constraint-16.2  ( -- f )
  <Case₁> slot-full? ;

\ 16.3. Pl2 может следовать непосредственно за Case1, только
\ если Case1 выражен генитивом: пістіңнер ‘наши’ (см. также
\ 1.25). В прочих случаях перед нами не Pl2, а PredPl: ибделер
\ ‘они дома’.
: constraint-16.3  ( -- f )
  <Case₁> slot-empty?  ||
  <Attr> slot-full?    ||
  flag Gen₁  flag-is?
  ;

\ ₁6.4. Pl₂ может следовать непосредственно за Poss₁ только при
\ наличии Poss₂: чӱс-паз-ы-лар-ы-ның ікізін ‘двоих из сотников’.
\ В прочих случаях это не Pl₂, а PredPl: олар хызыбыстар ‘они –
\ наши дочери’.
: constraint-16.4  ( -- f )
  <Poss₁> slot-empty?            ||
  slots( <Poss₁> <Pl₂> )-empty?  ||
  <Poss₂> slot-full?
  ;

\ 16.5. Case₂ не может следовать непосредственно за Pl₁ или Poss₁.
\ Poss₂ не может следовать непосредственно за Pl₁.
: constraint-16.5-<Poss₂>  ( -- f )
  <Pl₁> slot-empty?  ||
  slots( <Pl₁> <Poss₂> )-full?
  ;
: constraint-16.5-<Case₂>  ( -- f )
  <Pl₁> slot-empty?  slots( <Pl₁> <Case₂> )-full?  OR
  <Poss₁> slot-empty?  slots( <Poss₁> <Case₂> )-full?  OR
  AND ;

\ 17. В поз. 12/16 Case набор аффиксов посессивного
\ склонения выбирается: а) в случае заполнения позиций
\ 11/15 (Poss) соответственно аффиксами не-множественного
\ числа (т.е. 1pos.sg, 2pos.sg и 3pos); в) непосредственно после
\ Nomen-основ, у которых в поле FORM есть помета poss
\ (неотделяемая принадлежность)
: constraint-17-<Case₁>  ( -- f )
  flag Poss1.nonpl  flag-empty?  &&
  slots[ 1 <Case₁> )-empty?  dictflag-poss dictflag-is?  AND NOT
  ;
: constraint-17-<Case₂>
  flag Poss2.nonpl  flag-empty?  &&
  slots[ 1 <Case₂> )-empty?  dictflag-poss dictflag-is?  AND NOT
  ;

\ 18. Аффикс Adv Ли может присоединяться к словам с пометой
\ Verbum, у которых есть причастные показатели из поз. 7, и к
\ словам с пометой Nomen. После него возможен только Ptcl3 OK.
: constraint-18  ( -- f )
  slots( <Transp> <Ptcl₃> )-full?  &&
    verb?  flag participles  flag-is?  AND  ||
    nomen?
  ;

\ 19. Показатели поз. 19 (Person) с пометой Imp могут быть
\ только у слов, имеющих помету Verbum; они следуют
\ непосредственно после заполнителя позиции с номером меньше
\ или равно 7
: constraint-19  ( -- f )
  verb?  &&
  slots( <Neg/Iter> <Person> )-empty?
  ;

\ 20. К словам с пометой NOMEN присоединяются полные
\ лично-числовые показатели (список внутри Person), которые
\ могут следовать после любых морфем. У глаголов заполнение
\ Person возможно только при незаполненных позициях с 11 по 17 и
\ при наличии показателей:
\ для полных форм: Dur₁.dial.kac Ат, Irr ЧЫК, Opt ГАй, Assum
\ ГАдАГ, Indir ТЫр, Cunc ГАлАК, Neg.Fut ПАс, полные формы
\ аффиксов Iter А.дЫр, Pres2 чАдЫр, Pres2.dial.kac чадыр,
\ Pres.dial.kyz тур, Dur1 ир, Dur₁.dial.kac Ат, Hab ҶАң, Fut Ар,
\ в качинском также Past в форме ГА (парғабын ‘я шел’);
\ для смешанных форм: Pres ЧА, Pres.dial ча, Pres.dial.sh
\ чАр(Ы), Past ГА(н), краткие формы аффиксов Iter А.дI, Pres2
\ чАдЫ, Pres2.dial.kac чады, Pres.dial.kyz ту, Dur1 и, Hab ҶА,
\ Fut А (+ в диалектах также Ар: килерім ‘я приду’);
\ для кратких форм: Cond СА, Rpast ТI.
: constraint-20-full-person  ( -- f )
  nomen? ||
  verb? &&
    slots[ <Pl₁> <Case₂> ]-empty? &&
      flags( Affirm Opt Assum Indir Cunc Neg.Fut
             Iter@full Pres2@full Pres2.dial.kac@full
             Pres.dial.kyz@full Dur1@full Dur₁.dial.kac
             Hab@full Fut@full
             Past@short ) flag-is?
  ;
: constraint-20-mix-person  ( -- f )
  verb?  &&
    slots[ <Pl₂> <Case₂> ]-empty?  &&
      flags( Pres Pres.dial Pres.dial.sh Past
             Iter@short
             Pres2@short Pres2.dial.kac@short
             Pres.dial.kyz@short
             Dur1@short Hab@short Fut ) flag-is?
  ;
: constraint-20-short-person  ( -- f )
  verb?  &&
    slots[ <Pl₂> <Case₂> ]-empty?  &&
      flags( RPast Cond ) flag-is? ;


\ 21. Показатель поз. 20 PredPl может стоять после:
\ а) пок-ля времени (позиция 8 [за исключением ConvA, ConvP,
\    Neg.Conv (.Abl), Lim и PresPt чАн, согласно пр.25] + Indir
\    TIр + Affirm ЧIК + Iter AдIр + Dur1 и(р) + Dur₁.dial.kac Ат),
\ б) пок-ля падежа (п. 16) или посессивности (п. 15),
\ в) некоторых полей Person (1pl, Imp.3),
\ г) чистой именной основы,
\ д) пок-ля Ptcl2 (поз.17): Хай пірее одыртхан ағастар парохтар
\    мында - Здесь имеется и несколько посаженных деревьев (ГХЯ)
: constraint-21  ( -- f )
  flag Iter flag-is?  slots( <Neg/Iter> <PredPl> )-empty?  AND                     ||
  <Tense/Mood/Conv2> slot-full?  slots( <Tense/Mood/Conv2> <PredPl> )-empty?  AND  ||
  <Affirm> slot-full?  slots( <Affirm> <PredPl> )-empty?  AND                      ||
  <Poss₂> slot-full?  slots( <Poss₂> <PredPl> )-empty?  AND                        ||
  <Case₂> slot-full?  slots( <Case₂> <PredPl> )-empty?  AND                        ||
  <Ptcl₂> slot-full?  slots( <Ptcl₂> <PredPl> )-empty?  AND                        ||
  flags( Dur1 Dur₁.dial.kac 1.pl Imp.3 ) flag-is?                                  ||
  slots[ 1 <PredPl> )-empty?  nomen?  AND
  ;

\ 22. Показатель Perm присоединяется только к императивным
\ показателям из поз. 19 Person, к отрицанию ПА или к чистой
\ основе слов категории Verbum (имеющей значение Imp.2sg).
: constraint-22  ( -- f )
  slots( <Person> <Ptcl₃> )-empty?  flag Imp  flag-is?  AND    ||
  slots( <Neg/Iter> <Ptcl₃> )-empty?  flag Neg  flag-is?  AND  ||
  slots[ 1 <Ptcl₃> )-empty?  verb?  AND
  ;

\ 23. Пок-тель Foc может встретиться: а) в словоформах с
\ пометой Nomen; б) в глаголах, у которых есть временные
\ показатели Past ГАн и Hab ЧАӊ.
: constraint-23  ( -- f )
  nomen?  ||
  flags( Past Hab ) flag-is?
  ;

\ 25. После деепричастных показателей позиции 8 (Lim ГАли,
\ Convп (I)П, Convа; Convпас; Neg.Conv и Neg.Conv.Abl) может
\ стоять только показатель Ass ОК из позиции Ptcl3.
: constraint-25  ( -- f )
  slots( <Tense/Mood/Conv2> <Ptcl₃> ]-empty?  ||
  flag Ass₃  flag-is?
  ;

\ 26. Показатели Pres чА, PresPt чАн, Dur чАТ, Pres2 чАдIр,
\ Pres2.dial.kac чадыр, Pres.dial.kyz тур, Pres.dial ча,
\ Pres.dial.sh чАр(Ы), Indir тIр возможны только при наличии
\ показателей позиции 2 (NF или NF.Neg) или 4 (Perf) или
\ Prosp АК.
\ Pres чА, Pres.dial ча, Pres.dial.shor чАр(Ы), Indir тIр
\ возможны также при NF.Neg.sh ПААн
: constraint-26  ( -- f )
  <NF> slot-full?    ||
  <Perf> slot-full?  ||
  flag Prosp.dial  flag-is? ;
: constraint-26+paan  ( -- f )
  <NF> slot-full?    ||
  <Perf> slot-full?  ||
  flags( Prosp.dial NF.Neg.sh ) flag-is?
  ;

\ 27. Позиции Ptcl1, Pl1, Poss1, Case1, Ptcl2 не могут быть
\ последними заполненными позициями в словоформe
: constraint-27  ( -- f )
  <Ptcl1> slot-empty?  slots( <Ptcl1> <Ptcl₃> ]-full?  OR  &&
  <Pl₁> slot-empty?  slots( <Pl₁> <Ptcl₃> ]-full?  OR  &&
  <Poss₁> slot-empty?  slots( <Poss₁> <Ptcl₃> ]-full?  OR  &&
  <Case₁> slot-empty?  slots( <Case₁> <Ptcl₃> ]-full?  OR  &&
  <Ptcl₂> slot-empty?  slots( <Ptcl₂> <Ptcl₃> ]-full?  OR
  ;

\ 29. Предикативные показатели (Person, PredPl) невозможны в
\ сочетании с падежами: Gen2, Acc2, Instr2.
: constraint-29  ( -- f )
  slots[ <Person> <PredPl> ]-empty? ;

\ 30. Алломорф Abl -тЫн возможен в словоформе только при наличии All₁.
: constraint-30  ( -- f )
  flag All₁  flag-is? ;

\ Неозвончаемые основы
: constraint-non-envoiceable-stem  ( -- f )
  first-form-flag untransformed-left-envoice AND NOT  ||
  dictflag-no-envoice dictflag-empty?
  ;
: constraint-non-envoiced-rus  ( -- f )
  first-form-flag untransformed-left-envoice-missing AND NOT  ||
  dictflag-no-envoice dictflag-is?                            ||
  dictflag-rus dictflag-is?
  ;

\ Озвончение кластера
: constraint-cluster-envoice  ( -- f )
  first-form-flag untransformed-cluster-envoice AND NOT  ||
  dictflag-rus dictflag-is?
  ;

\ Запрещенные контексты для выпадения VңV
: constraint-VңV-fallout  ( -- f )
  \ флаг выпадения конечного ң находится в слоте справа
  <Poss₁> 1+ form-slot-flags untransformed-fallout-VңV AND NOT  &&
  <Poss₂> 1+ form-slot-flags untransformed-fallout-VңV AND NOT
  ;

\ Запрещенные контексты для выпадения (СА|ТЫ)ңАр
: constraint-(СА|ТЫ)ңАр-fallout  ( -- f )
  <Person> form-slot-flags untransformed-fallout-(СА|ТЫ)ңАр AND
  flag 2pl.br  flag-is?
  flags( RPast Cond ) flag-empty?
  AND AND NOT  &&
    <Poss₂> form-slot-flags untransformed-fallout-(СА|ТЫ)ңАр AND
    flag 2pos.pl  flag-is?
    AND NOT &&
      <Poss₁> form-slot-flags untransformed-fallout-(СА|ТЫ)ңАр AND
      flag 2pos.pl  flag-is?
      AND NOT
  ;

\ Запрещенные контексты для выпадения Г после Г, ң без стяжения
: constraint-[Гң]Г-fallout  ( -- f )
  first-form-flag untransformed-fallout-confluence AND  ||
  untransformed-fallout-confluence any-form-flag-is? NOT
  ;

\ Запрещенные контексты для выпадения одной из трех одинаковых согласных
: constraint-CCC-fallout  ( -- f )
  stem-last-sound stem-prev-sound <>                                                  ||
  stem-last-sound first-affix-starts-with? NOT                                        ||
  dictflag-rus dictflag-is?  first-form-flag untransformed-fallout-CCC  AND  0<> AND  ||
  dictflag-rus dictflag-empty?  first-form-flag untransformed-fallout-CCC AND NOT  AND
  ;

\ Запрещенные контексты для выпадения после долгой гласной
: constraint-VVГV-fallout  ( -- f )
  first-form-flag untransformed-fallout-VVГV AND NOT  ||
  dictflag-rus dictflag-is?
  ;

\ Разрешенные контексты для невыпадения VГV
: constraint-VГV-fallout  ( -- f )
  nomen? verb? OR NOT                                                         ||
  first-form-flag untransformed-fallout-VГV untransformed-fallout-VңV OR AND  ||
  slot-all-empty?                                                             ||
  stem-polysyllabic? NOT                                                      ||
  stem-last-sound gh-g-ng? NOT                                                ||
  stem-prev-sound vowel? NOT                                                  ||
  stem-prev-sound short-vowel? NOT                                            ||
  stem-prev-sound-ptr vowel-long-middle?                                      ||
  first-affix vowel-long?                                                     ||
  first-affix first-sound short-vowel? NOT
  ;

\ Запрещенные контексты для выпадения конечного к, х
: constraint-V[кх]V-fallout  ( -- f )
  first-form-flag untransformed-fallout-V[кх]V AND NOT  ||
  dictflag-rus dictflag-empty?
  ;

\ После NF глухое: пар-тыр, сом-тыр
: constraint-voicedstem+Indir  ( -- f )
  flag NF₀  flag-is?  slots( <NF> <Tense/Mood/Conv2> )-empty?  AND
  \ 1 7 slot-range-empty?
  \ stem-last-sound consonant?
  \ stem-last-sound unvoiced? NOT
  \ AND AND
  ;

\ Pres.dial ча, Pres2.dial.kac чадыр только после переднерядных основ
: constraint-frontstem  ( -- f )
  stem-last-char-vowel back-vowel? NOT
  ;

\ поглощение гласных перед -ох: 3pos в виде алломорфов -ы/-i не
\ стягивается: хызох < хыс+ох, но не < хыс-ы-ох. Гласная
\ дательного падежа не стягивается: суғ+ға+ох > суғаох ‘в воду
\ же’.
: constraint-OK-fallout₁₂  ( -- f )
  slots( <Poss₁> <Ptcl₂> )-full?
  <Ptcl₂> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  slots( <Poss₁> <Ptcl₃> )-full?
  <Ptcl₃> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  AND                                                  ||
  <Poss₁> form-slot first-sound consonant?
  ;
: constraint-OK-fallout₁₆  ( -- f )
  slots( <Poss₂> <Ptcl₂> )-full?
  <Ptcl₂> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  slots( <Poss₂> <Ptcl₃> )-full?
  <Ptcl₃> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  AND                                                  ||
  <Poss₂> form-slot first-sound consonant?
  ;
: constraint-OK-fallout₁₇  ( -- f )
  <Ptcl₂> form-slot-flags untransformed-fallout-OK AND NOT  &&
    slots( <Case₂> <Ptcl₃> )-full?  ||
    <Ptcl₃> form-slot-flags untransformed-fallout-OK AND NOT
  ;

: constraint-V+Acc  ( -- f )
  <Case₂> form-slot-vowel-at-left? ;

: constraint-broken-harmony  ( -- f )
  harmony-vu-broken any-form-flag-is? NOT  ||
  dictflag-rus dictflag-is?  &&
    first-form-flag harmony-vu-broken AND 0<>
  ;
