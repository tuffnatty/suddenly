require khakas/slotnames.fs

: =>  ( slot-pos "name" )
  ]] (<this>) = IF [(')] EXIT THEN [[ ; IMMEDIATE

: is-пар/кил?  ( -- f )
  paradigm-stems @  [: strlist-get t~/ пар|апар|кил ;] list-any? ;

: is-personal-pronoun?  ( -- f )
  paradigm-stem 2@  [: strlist-get t~/ мин|син|ол|піс|сірер|олар ;] list-any? ;

\ 0. Слова с пометой INVAR (не Nomen и не Verbum) никаких
\ показателей не присоединяют! Слова с пометой Invar1
\ присоединяют Ptcl3 ОК.
: constraint-0  ( -- f )
  invar1? NOT                                         ||
  slots[ 1 <Ptcl₃> )-empty?  flag Ass₃  flag-is? AND  ||
  slot-all-empty?
  ;

\ 1. Позиции с <Distr> до <Tense/Mood/Conv> могут заполняться
\ только у слов с пометой Verbum.
: constraint-1  ( -- f )
  verb? ||
  slots[ <Distr> <Tense/Mood/Conv> ]-empty?
  ;

\ 2. Заполнение позиций 10-16 (с <Pl₁> до <Case₂>)
\ может происходить: 1) либо сразу после основы, если слово
\ имеет помету Nomen, 2) либо непосредственно после позиции
\ <Tense/Mood>, заполненной одним из причастных показателей,
\ если слово имеет помету Verbum;
: constraint-2  ( -- f )
  slots[ <Pl₁> <Case₂> ]-empty?
  || nomen?  slots[ 1 <Pl₁> )-empty? AND
  || flag participles  flag-is?  slots( <Tense/Mood/Conv> <Pl₁> )-empty?  AND
  ;

\ 3. Показатели позиции 2 (NF) и показатель Perf (Ы)бЫС
\ в пределах одной словоформы встречаются только в
\ случае заполнения позиции 2 кумулятивным показателем
\ Neg.NF или если заполнена позиция 3 [тооз-ып-таа-быс-ты-лар
\ – (кончить-NF-Add-Perf-RPast-Pl) ‘почти закончили’].
: constraint-3  ( -- f )
  <NF,Dur1> slot-empty?
  || flag Neg.NF  flag-is?
  || <Ptcl1> slot-full?
  ;

\ 4. Показатели NF, Neg.NF могут встретиться только в
\ словоформе, где есть один или несколько из показателей:
\ 1) позиции 3 <Ptcl1>,
\ 2) позиции 6 <Dur> Dur чАТ, DurDial чаТ,
\ 3) Perf (Ы)бЫс, Perf1 Ыc,
\ 4) Pres1 чАдЫр, чады(р), PresKyz ту(р),
\ 5) прочие Pres (чА, ча, чАр(Ы)),
\ 6) Indir ТЫр,
\ 7) PresPt чАн
\ - или на конце словоформы.
\ Показатель Neg.NFSh ПААн допустим в тех же случаях,
\ кроме 4), 7) и конца словоформы
: constraint-4sh  ( -- f )
  <Ptcl1> slot-full?
  || <Dur> slot-full?
  || flags( Perf Perf1
            Pres PresDial PresSh
            Indir ) flag-is?
  ;
: constraint-4  ( -- f )
  constraint-4sh
  || slots( <NF,Dur1> <Ptcl₃> ]-empty?
  || flags( Pres1 Pres1Kac PresKyz
            PresPtDial ) flag-is? ;

\ 4.1. NF выбирает алломорф (Ы)п, если:
\ 1) он непосредственно следует за основой, которая оканчивается
\    на (выпадающие, см. ниже) п, г, ғ или ӊ;
\ 2) он непосредственно следует за основой или аффиксом,
\    оканчивающимися на гласную;
\ 3) непосредственно за ним следует Ass ОК или Cont LA
\    или Add ТАА (см. пример в п. 3).
\ 4) основа или предшествующий аффикс оканчивается на согласную
\    (действует опционально для качинского диалекта, но проникло
\    и в литературные тексты).
\ NF выбирает алломорф 0 после основы или аффикса на
\ невыпадающую согласную, если после него не стоит аффикс ОК.
\ [Таким образом, после основы на невыпадающую согласную в конце
\ словоформы для NF возможны оба алломорфа.]
: constraint-4.1ₚ  ( -- f )
  <Distr> slot-empty?  <Distr> form-slot-xc-at-left fallout-short?  AND
  || <NF,Dur1> form-slot-vowel-at-left?
  || flags( Add Cont Ass₁ ) flag-is?
  || <NF,Dur1> form-slot-xc-at-left consonant?
  ;
: constraint-4.1₀  ( -- f )
  <NF,Dur1> form-slot-xc-at-left consonant?
  && <NF,Dur1> form-slot-xc-at-left fallout-short? NOT
  ;
: constraint-4.1₀-right  ( -- f )
  flag Ass₁  flag-empty? ;

\ 5. Показатели Ptcl₁ (внутренние частицы) допускаются только
\ при наличии показателя NF (NF, NF₀, Neg.NF. Neg.NFSh) и
\ любых из следующих показателей:
\ а) Perf Ыс для всех частиц (чіплеізеді ‘всё время съедает’,
\ сығыбоғысчых ‘так же вылезла’) и Perf ЫбЫс для Add ТАА
\ (тоозыптаабыстылар ‘почти закончили’),
\ б) Dur чАт, чат (хараплачатхан ‘все время всматривался)’,
\ в) Indir ТЫр и презенсы на ч (Pres чА, PresDial ча, Pres1
\ чАдЫ(р), PresPt чАн, PresSh чАр(Ы), Pres1Kac
\ чады(р)): муханначадарзың ‘всё время мучаешься’.
: constraint-5  ( -- f )
  flags( NF NF₀ Neg.NF Neg.NFSh ) flag-is?
  && flag Perf1  flag-is?
     || flag Perf flag-is?  flag Add flag-is?  AND
     || <Dur> slot-full?
     || flags( Indir Pres PresDial PresSh Pres1
                     Pres1Kac PresPtDial ) flag-is?
  ;

\ 5.1. Показатель Perf1 Ыс возможен [пока встретился]:
\ a) при наличии Ptcl1 [зачеркнуто: и любого пок-ля времени (позиции
\ <Tense/Mood/Conv> + Vis ЧЫК + Gener AдЫр + Dur1 и(р) +
\ Dur1Kac Ат)]:
\ б) при диалектном показателе Neg.NFSh ПААн.
: constraint-5.1  ( -- f )
  flag Neg.NFSh  flag-is?
  || <Ptcl1> slot-full?
     \ && <Tense/Mood/Conv> slot-full?
     \    || <Vis> slot-full?
     \    || flags( Gener Dur1 Dur1Kac ) flag-is?
  ;

\ 6. Показатель Prosp АК встречается только перед
\ морфемами Pres чА, PresPt чАн и Dur чАТ. Показатель NF в
\ этом случае в словоформе отсутствует.
: constraint-6  ( -- f )
  <NF,Dur1> slot-empty? ;
: constraint-6-right  ( -- f )
  flags( Dur Pres PresPtDial ) flag-is? ;

\ 7. Показатели Dur1 и(р), Dur1Kac Ат, Dur1Sag ит
\ заполняются только, если основой является лемма пар-, апар-
\ или кил- (но при этих основах может выбираться с тем же
\ успехом и показатель Dur чАТ, свободное варьирование).
: constraint-7  ( -- f )
  is-пар/кил?
  ;

\ 8. Показатели Dur1 и(р) и Dur1Kac Ат могут стоять:
\ 1) либо непосредственно в конце словоформы,
\ 2) либо непосредственно перед <Person> или <PredPl>,
\ 3) либо непосредственно перед показателями Past ГА(н), CvP,
\    Cond СА (позиция <Tense/Mood>).
\ Показатель Dur1Sag ит может стоять перед Dur чАт,
\ Past ГА(н), Cond СА и PresPtDial чАн.
: constraint-8  ( -- f )
  slots( <NF,Dur1> <Ptcl₃> ]-empty?
  || slots( <NF,Dur1> <Person> )-empty?  <Person> slot-full?  AND
  || slots( <NF,Dur1> <PredPl> )-empty?  <PredPl> slot-full?  AND
  || flags( Past Cond CvP ) flag-is?  <Neg/Gener> slot-empty?  AND
  ;
: constraint-8sag  ( -- f )
  slots( <NF,Dur1> <Dur> )-empty?  <Dur> slot-full?  AND
  || slots( <NF,Dur1> <Tense/Mood/Conv> )-empty?  flags( Past Cond PresPtDial ) flag-is? AND
  ;

\ 8.1. Dur1 в роли видового показателя морфонологически
\ распределен: перед Past ГА(н) и перед Cond СА может
\ принять только форму и, перед CvP - только форму ир;
\ в роли показателя времени (т.е. перед Person, PredPl и
\ концом словоформы) варианты и и ир находятся в
\ свободном (точнее, диалектном) варьировании.
: constraint-8.1ᵢ  ( -- f )
  flag CvP  flag-empty? ;
: constraint-8.1ᵢᵣ
  flags( Past Cond ) flag-empty? ;

\ 9.1. Past ГА(н) всегда принимает форму ГА непосредственно
\ перед всеми Person: пар-га-м 'я шел', тiк-ке-зер 'вы шили'.
\ В остальных случаях принимается форма ГАн
: constraint-9.1  ( -- f )
  slots( <Tense/Mood/Conv> <Person> )-empty? &&
  <Person> slot-full?
  ;

\ 9.2. Показатель Hab ҶА(ң) может принимать форму ҶА
\ непосредственно перед всеми Person: ырла-ӌаң-мын // ырла-ча-м
\ ‘я пел (обычно)’, кил-бе-ҷең-міс // кил-бе-ҷе-біс ‘мы не
\ приходили (обычно)’. А может и принимать форму ҶАң. В
\ остальных случаях принимается форма ҶАң
: constraint-9.2  ( -- f )
  slots( <Tense/Mood/Conv> <Person> )-empty? &&
  <Person> slot-full?
  ;

\ 9.3. Показатель Fut А(р) может принимать форму А
\ непосредственно перед личными показателями: пар-а-м ‘я пойду’,
\ ади-быс ‘мы будем называть’, пол-а-зар ‘вы будете’, кил-е-зiң
\ ‘ты придешь’. [В диалектах Fut может также
\ принимать форму А непосредственно перед показателем Dat ГА,
\ при этом Г не выпадает: пар-ар-ға > пар-а-ға 'идти'.] В прочих
\ случаях фигурирует только форма Ар. В первом лице бывает также
\ и форма Ар: пар-ар-бын ‘я пойду’, адир-быс ‘мы будем называть’.
: constraint-9.3  ( -- f )
  slots( <Tense/Mood/Conv> <Person> )-empty? &&
  <Person>  slot-full?
  ;

\ 9.4 PresKyz ту(р) может принимать форму ту
\ непосредственно перед Person или PredPl. Форма тур возможна в
\ любых контекстах.
: constraint-9.4  ( -- f )
  slots( <Tense/Mood/Conv> <Person> )-empty? &&
  slots[ <Person> <PredPl> ]-full?
  ;

\ 9.5. PresSh чАр(Ы) перед показателями 1sg и 2sg
\ принимает форму чАрЫ. (Это нужно, чтобы не было омонимичных
\ разборов чар-ым / чары-м). В остальных контекстах показатели
\ не распределены.
: constraint-9.5  ( -- f )
  slots( <Tense/Mood/Conv> <Person> )-full?  ||
  flags( 1sg.br 2sg.br ) flag-empty? ;

\ 10. Показатель Cunc не встречается в одной
\ словоформе с показателями Perf или отрицательными
\ (всеми, в названия которых входит элемент Neg);
: constraint-10  ( -- f )
  flag Cunc  flag-empty? ;

\ 11. Показатели Fut А(р), Neg.Fut ПАС, Neg.Conv Пин,
\ Neg.Сonv.Abl Пин.Аң, Foc ТЫр (чтобы формы типа полбаандыр не
\ получали паразитический разбор через Foc)
\ не могут быть в одной словоформе с пок-лем Neg ПА.
: constraint-11  ( -- f )
  flags( Fut Neg7|Foc )  flag-empty? ;

\ 11.1. Показатель Neg.NF исключает заполнение Neg в поз. 6.
: constraint-11.1  ( -- f )
  flag Neg  flag-empty? ;

\ 12. Непосредственно после показателя недавно прошедшего
\ времени (RPast) может следовать только краткий
\ лично-числовой показатель (Person), число предиката
\ (PredPl), показатель аффирматива (Vis), <Ptcl₂> или <Ptcl₃>
: constraint-12  ( -- f )
  slots( <Tense/Mood/Conv> <PredPl> )-empty?
  || slots( <Tense/Mood/Conv> <Ptcl₂> )-empty?  <Ptcl₂> slot-full?  AND
  || slots( <Tense/Mood/Conv> <Person> )-empty?  flag Person.br  flag-is?  AND
  || slots( <Tense/Mood/Conv> <Vis> )-empty?  <Vis> slot-full?  AND
  ;

\ 13. Непосредственно после показателя условного наклонения
\ (Cond) может следовать только краткий лично-числовой
\ показатель (Person) или число предиката (PredPl) или Ptcl3
: constraint-13  ( -- f )
  slots( <Tense/Mood/Conv> <PredPl> )-empty?  ||
  slots( <Tense/Mood/Conv> <Person> )-empty?  &&
    flag Person.br  flag-is?
  ;

\ 14. Непосредственно после Gener возможны только: конец
\ словоформы [(обрабатывается правилом 27),] Past ГА(н),
\ CvA, CvP, <Ptcl₂>, <Person>, <PredPl> или <Ptcl₃>.
: constraint-14  ( -- f )
  flags( Past CvP CvA ) flag-is?
  || slots( <Neg/Gener> <Ptcl₂> )-empty?  <Ptcl₂> slot-full?  AND
  || slots( <Neg/Gener> <Person> )-empty?
  ;

\ 14.1. Gener: Перед Past ГА(н), CvP, CvA и Ptcl3 возможна
\ только форма АдЫр, в остальных случаях - и АдЫ, и АдЫр.
: constraint-14.1  ( -- f )
  flag Past  flag-empty?  &&
  flag CvP   flag-empty?  &&
  flag CvA   flag-empty?  &&
  slots( <Neg/Gener> <Ptcl₃> )-empty?  <Ptcl₃> slot-full?  AND NOT
  ;

\ 16.1. Показатели позиций <Pl₁>, <Poss₁>, <Case₁> возможны
\ только при наличии в словоформе одного или нескольких аффиксов
\ из позиций <Transp> (Attr, Comit, All₁.Attr), <Pl₂>, <Poss₂>, <Case₂>.
: constraint-16.1  ( -- f )
  slots( <Transp> <Case₂> ]-full?
  || <Transp> slot-full? && flags( Adv ) flag-empty?
  ;

\ 16.2. Показатель Attr КЫ может присутствовать в
\ словоформе только при наличии <Case₁>. [В такой словоформе могут
\ также присутствовать морфемы из позиций <Pl₁> и <Poss₁>.]
\ Морфонология у показателей поз. <Pl₁>/<Pl₂>, <Poss₁>/<Poss₂>,
\ <Case₁>/<Case₂> одинаковая.
: constraint-16.2  ( -- f )
  <Case₁> slot-full? ;

\ 16.3. Pl2 не может следовать непосредственно за Case1.
: constraint-16.3  ( -- f )
  <Case₁> slot-empty?
  || <Transp> slot-full?
  ;

\ 16.4. Pl₂ может следовать непосредственно за Poss₁ только при
\ наличии Poss₂: чӱс-паз-ы-лар-ы-ның ікізін ‘двоих из сотников’.
\ В прочих случаях это не Pl₂, а PredPl: олар хызыбыстар ‘они –
\ наши дочери’.
: constraint-16.4  ( -- f )
  <Poss₁> slot-empty?           ||
  slots( <Poss₁> <Pl₂> )-full?  ||
  <Poss₂> slot-full?
  ;

\ 16.5. <Case₂> не может следовать непосредственно за <Pl₁> или
\ <Poss₁>. <Case₂> не может следовать непосредственно за
\ <Case₁>, за исключением последовательностей All1+AblArch тЫн,
\ Loc1+Loc2 (пока не нашлось убедительных примеров, см.
\ рекламацию #187).
: constraint-16.5-<Pl₁>  ( -- f )
  slots( <Pl₁> <Case₂> )-full?
  || <Case₂> slot-empty?
  ;
: constraint-16.5-<Poss₁>  ( -- f )
  slots( <Poss₁> <Case₂> )-full?
  || <Case₂> slot-empty?
  ;
: constraint-16.5-<Case₁>  ( -- f )
  slots( <Case₁> <Case₂> )-full?
  || <Case₂> slot-empty?
  || [: flag All₁ flag-is? && flag AblArch flag-is? ;] EXECUTE
  || flag Loc₁ flag-is? && flag Loc flag-is?
  ;
: constraint-16.5  ( -- f )
  <Pl₁> => constraint-16.5-<Pl₁>      \ right-context
  <Poss₁> => constraint-16.5-<Poss₁>  \ right-context
  <Case₁> => constraint-16.5-<Case₁>  \ can't work as right context
  TRUE ABORT" Invalid slot for constraint-16.5!"
  ; IMMEDIATE

\ 16.6. Показатель Gen.3pos(Dial) не может присутствовать в
\ словоформе одновременно с Case1.
: constraint-16.6  ( -- f )
  flag Gen.3pos  flag-empty? ;

\ 16.7. <Poss₂>, исключая Gen.3pos, не может следовать непосредственно
\ за <Pl₁>, All₁ и за <Poss₁>.
: constraint-16.7-<Pl₁>  ( -- f )
  slots( <Pl₁> <Poss₂> )-full?
  || <Poss₂> slot-empty?
  ;
: constraint-16.7-<Poss₁>  ( -- f )
  slots( <Poss₁> <Poss₂> )-full?
  || <Poss₂> slot-empty?
  || flag Gen.3pos flag-is?
  ;
: constraint-16.7-<Case₁>  ( -- f )
  slots( <Case₁> <Poss₂> )-full?
  || <Poss₂> slot-empty? ;
: constraint-16.7  ( -- f )
  <Pl₁> => constraint-16.7-<Pl₁>      \ right-context
  <Poss₁> => constraint-16.7-<Poss₁>  \ can't work as right-context
  <Case₁> => constraint-16.7-<Case₁>  \ right-context
  TRUE ABORT" Invalid slot for constraint-16.7!"
  ; IMMEDIATE

\ 16.8. Pl2 не может непосредственно следовать за Pl1 (ислючаем
\ паразитические разборы форм типа истерлер ‘будут слышать’).
: constraint-16.8  ( -- f )
  <Pl₂> slot-empty?
  || slots( <Pl₁> <Pl₂> )-full?
  ;

\ 16.9. Показатель All1.Attr Сархы не может употребляться в одной словоформе с Case1.
: constraint-16.9  ( -- f )
  flag All₁.Attr  flag-empty? ;

\ 17. В поз. <Case₁>/<Case₂> набор аффиксов посессивного
\ склонения выбирается: а) в случае заполнения позиций
\ <Poss₁>/<Poss₂> соответственно аффиксами не-множественного
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
  verb?  flag participles  flag-is?  AND  ||
  nomen?
  ;
: constraint-18-right  ( -- f )
  slots( <Transp> <Ptcl₃> )-empty? ;

\ 19. Показатели поз. <Person> с пометой Imp могут быть
\ только у слов, имеющих помету Verbum; они следуют
\ непосредственно после основы или аффиксов позиций,
\ предшествующих <Tense/Mood>
: constraint-19  ( -- f )
  verb?  &&
  slots[ <Tense/Mood/Conv> <Person> )-empty?
  ;

\ 20. К словам с пометой Nomen присоединяются полные
\ лично-числовые показатели (список внутри Person), которые
\ могут следовать после любых морфем. У глаголов заполнение
\ Person возможно только при незаполненных позициях с 11 по 17
\ (за исключением Comp ТАГ) и при наличии показателей:
\ для полных форм: Dur1Kac Ат, Irr ЧЫК, Opt ГАй, Assum
\ ГАдАГ, Indir ТЫр, Cunc ГАлАК, Neg.Fut ПАс, полные формы
\ аффиксов Gener А.дЫр, Pres1 чАдЫр, Pres1Kac чадыр,
\ PresKyz тур, Dur1 ир, Hab ҶАң, Fut Ар, в качинском также Pres
\ чА (тоғынчабын ‘я работаю’) и Past ГА(н) (парғабын ‘я шел’,
\ чоохтаанохпын ‘я тоже говорил’);
\ для кратких форм: Cond СА, Rpast ТЫ.
\ Cмешанный набор форм (1sg.br + др.лица в полных формах)
\ присоединяется к аффиксам: Pres чА, PresDial ча, PresSh
\ чАр(Ы), Past ГА(н), кратким формам аффиксов Gener А.дЫ, Pres1
\ чАдЫ, Pres1Kac чады, PresKyz ту,  Dur1 и, Hab ҶА, Fut А (+ в
\ диалектах также Ар: килерім ‘я приду’, тур: сағынтурым ‘я
\ думаю’, чАдЫр: полчадырым ‘я существую’, АдЫр: нанадырым ‘я
\ возвращаюсь’).

: constraint-20-full-person  ( -- f )
  nomen?
  || verb?
     && flag Comp  flag-is?
        || slots[ <Pl₁> <Case₂> ]-empty?
           && flags( Assum|Neg.Fut|Opt Indir Cunc
                     Gener@full Pres1@full Pres1Kac@full
                     PresKyz@full Dur1@full Dur1Kac
                     Hab@full Fut@full
                     Past Pres ) flag-is?
              || <Vis> slot-full?
  ;
: constraint-20-mix-person  ( -- f )
  verb?  &&
    flag Comp  flag-is? ||
    slots[ <Pl₂> <Case₂> ]-empty?  &&
      flags( Pres PresDial PresSh Past
             Gener
             Pres1 Pres1Kac@short
             PresKyz
             Dur1@short Hab@short Fut ) flag-is?
  ;
: constraint-20-short-person  ( -- f )
  verb?  &&
    flag Comp  flag-is? ||
    slots[ <Pl₂> <Case₂> ]-empty?  &&
      flags( RPast Cond ) flag-is? ;
: constraint-20-full-or-mix-person  ( -- f )
  constraint-20-full-person
  || constraint-20-mix-person
  ;

\ 21. Показатель PredPl ЛАр может стоять после:
\ а) пок-ля времени (позиции <Tense/Mood> [за исключением CvA, CvP,
\    Neg.Conv (.Abl), Lim и PresPt чАн, согласно пр.25] + Indir
\    TЫр + Vis ЧЫК + Gener AдЫр + Dur1 и(р) + Dur1Kac Ат),
\ б) пок-ля <Case2> или <Poss2>
\ в) некоторых полей Person (1pl, Imp.3),
\ г) чистой именной основы,
\ д) пок-ля Ptcl₂: Хай пірее одыртхан ағастар парохтар
\    мында - Здесь имеется и несколько посаженных деревьев (ГХЯ)
\ е) показателя Comit ЛЫГ.
: constraint-21  ( -- f )
  flag Gener flag-is?  slots( <Neg/Gener> <PredPl> )-empty?  AND
  || <Tense/Mood/Conv> slot-full?  slots( <Tense/Mood/Conv> <PredPl> )-empty?  AND
  || <Vis> slot-full?  slots( <Vis> <PredPl> )-empty?  AND
  || <Poss₂> slot-full?  slots( <Poss₂> <PredPl> )-empty?  AND
  || <Case₂> slot-full?  slots( <Case₂> <PredPl> )-empty?  AND
  || <Ptcl₂> slot-full?  slots( <Ptcl₂> <PredPl> )-empty?  AND
  || flags( Dur1 Dur1Kac 1.pl Imp.3 ) flag-is?
  || slots[ 1 <PredPl> )-empty?  nomen?  AND
  || flag Comit  flag-is?  slots( <Transp> <PredPl> )-empty?  AND
  ;

\ 22. Показатель Perm присоединяется только к императивным
\ показателям из поз. <Person>, к отрицанию Neg ПА, Dist (К)лА,
\ залоговым показателям (Pass, Rec, Refl, Caus), Perf (Ы)бЫс,
\ Dur чАт, чат или к чистой основе слов категории Verbum
\ (имеющей значение Imp.2sg).
: constraint-22  ( -- f )
  slots( <Person> <Ptcl₃> )-empty?  flag Imp  flag-is?  AND    ||
  slots( <Neg/Gener> <Ptcl₃> )-empty?  flag Neg  flag-is?  AND  ||
  slots( <Distr> <Ptcl₃> )-empty?  <Distr> slot-full?  AND  ||
  slots( <Voice> <Ptcl₃> )-empty?  <Voice> slot-full?  AND  ||
  slots( <Perf> <Ptcl₃> )-empty?  flag Perf  flag-is?  AND  ||
  slots( <Dur> <Ptcl₃> )-empty?      <Dur> slot-full?  AND  ||
  slots[ 1 <Ptcl₃> )-empty?  verb?  AND
  ;

\ 23. Пок-тель Foc может встретиться: а) в словоформах с
\ пометой Nomen; б) в глаголах, у которых есть временные
\ показатели Past ГАн, Fut А(р) и Hab ЧАӊ.
: constraint-23  ( -- f )
  nomen?  ||
  flags( Past Fut Hab ) flag-is?
  ;

\ 25. После деепричастных показателей позиции <Tense/Mood> (Lim ГАли,
\ CvP (Ы)П, CvA; CvKac; Neg.Conv и Neg.Conv.Abl) может
\ стоять только показатель Ass ОК из позиции Ptcl3.
: constraint-25  ( -- f )
  slots( <Tense/Mood/Conv> <Ptcl₃> ]-empty?  ||
  flag Ass₃  flag-is?
  ;

\ 26. Показатели Pres чА, PresPt чАн, Dur чАТ, DurDial чаТ,
\ Pres1 чАдЫр, Pres1Kac чадыр, PresKyz тур,
\ PresDial ча, PresSh чАр(Ы), Indir тЫр возможны только
\ при наличии показателей NF / Neg.NF / Perf / Prosp / Dur₁.
\ Pres чА, PresDial ча, PresSh чАр(Ы), Dur чАТ, DurDial
\ чаТ, Indir тЫр возможны также при Neg.NFSh ПААн
: constraint-26  ( -- f )
  <NF,Dur1> slot-full?  flag Neg.NFSh flag-empty?  AND
  || flags( Perf ProspDial ) flag-is?
  ;
: constraint-26+paan  ( -- f )
  <NF,Dur1> slot-full?
  || flags( Perf ProspDial ) flag-is?
  ;

\ 27. Позиции Ptcl1, Pl1, Poss1, Case1, Ptcl2 не могут быть
\ последними заполненными позициями в словоформe.
: constraint-27-<Ptcl1>  ( -- f )  slots( <Ptcl1> <Ptcl₃> ]-full? ;
: constraint-27-<Pl₁>    ( -- f )  slots( <Pl₁>   <Ptcl₃> ]-full? ;
: constraint-27-<Poss₁>  ( -- f )  slots( <Poss₁> <Ptcl₃> ]-full? ;
: constraint-27-<Case₁>  ( -- f )  slots( <Case₁> <Ptcl₃> ]-full? ;
: constraint-27-<Ptcl₂>  ( -- f )  slots( <Ptcl₂> <Ptcl₃> ]-full? ;
: constraint-27  ( -- f )
  <Ptcl1> => constraint-27-<Ptcl1>
  <Pl₁>   => constraint-27-<Pl₁>
  <Poss₁> => constraint-27-<Poss₁>
  <Case₁> => constraint-27-<Case₁>
  <Ptcl₂> => constraint-27-<Ptcl₂>
  TRUE ABORT" Invalid slot for constraint-27!"
  ; IMMEDIATE

\ 29. Предикативные показатели (Person, PredPl) невозможны в
\ сочетании с падежами: Gen2, Acc2, Instr2, Dat, All (а также их
\ диалектными вариантами Gen2Dial, AccDial, InstrDial,
\ DatDial, AllDial1, AllDial2).
: constraint-29  ( -- f )
  slots[ <Person> <PredPl> ]-empty? ;

\ 30. Алломорф Abl -тЫн возможен в словоформе только при наличии All₁.
: constraint-30  ( -- f )
  flag All₁  flag-is? ;

\ 32. Part ни встречается только при наличии RPast ТЫ (других
\ примеров пока не встретилось).
: constraint-32  ( -- f )
  flag RPast  flag-is? ;

\ 33. Voc Ай возможен только на конце словоформы.
: constraint-33  ( -- f )
  slots( <Case₂> <Ptcl₃> ]-empty? ;

\ 34. При присоединении Ptcl2 к основе глагола (Verbum)
\ обязательно также наличие показателя времени (позиции
\ Tense/Mood (за исключением деепричастий и PresPt чАн) +
\ Vis ЧЫК + Gener AдЫр + Dur1 и(р) + DurDialKac Ат).
\ Ptcl2 не может встретиться в словоформе без показателей Person
\ или PredPl, если непосредственно после нее не стоит
\ Vis ЧЫК: Аӌа іди полтырохчых. - Дядя так же был. (Чиланы).
\ Ptcl2 ТАА не присоединяется к именным основам [не встретилось
\ таких примеров, но есть много неверных разборов].
: constraint-34  ( -- f )
  slots[ <Vis> <PredPl> ]-full?
  && verb? NOT
     || <Tense/Mood/Conv> slot-full?  flags( converbs PresPtDial ) flag-empty?  AND
     || <Vis> slot-full?
     || flags( Gener Dur1 Dur1Kac ) flag-is?
  ;
: constraint-34-Add  ( -- f )
  verb? ;

\ 35. All САр (также диалектные формы СА, САрЫ) невозможен
\ непосредственно после (причастных) аффиксов из позиции
\ Tense/Mood. [Ограничение нужно для отсечения неверных
\ разборов слов типа хараанзар (глаз.3pos-All,
\ *всматриваться-Past-All), поларзар (быть-Fut-2pl,
\ *быть-Fut-All)]. Если причастие субстантивировано, то это
\ сочетание возможно: Оринаның орнына — інек сағӌаңнарзар
\ ‘Вместо Орины — к дояркам.’ (сағ-ӌаң-нар-зар –
\ доить-Hab-Pl-All) (ГХЯ, М. Кильчичакова)
: constraint-35  ( -- f )
  flag participles  flag-empty?
  || slots( <Tense/Mood/Conv> <Case₂> )-full?
  ;

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
  <Poss₁> slot-empty?  <Poss₁> untransformed-fallout-VңV next-form-flag-is? NOT  OR  &&
  <Poss₂> slot-empty?  <Poss₂> untransformed-fallout-VңV next-form-flag-is? NOT  OR
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
      AND NOT &&
        <Person> form-slot-flags untransformed-fallout-(СА|ТЫ)ңАр AND
        flag Imp  flag-is?
        AND NOT
  ;

\ Запрещенные контексты для выпадения одной из трех одинаковых согласных
: constraint-CCC-fallout  ( -- f )
  dictflag-rus dictflag-is? IF
    \ For rus. loanwords, either:
    \ the stem ends with CC, first affix starts with C and has untransformed-fallout-CCC flag;
    \ or: the stem does not end with CC or first affix does not start with C, and first affix has no untransformed-fallout-CCC flag.
    stem-last-sound-ptr cyr  stem-prev-sound-ptr cyr COMPARE
    stem-last-sound first-affix-starts-with? NOT
    OR
    first-form-flag untransformed-fallout-CCC  AND  0<>
    <>
  ELSE
    \ For non-loanwords, the untransform is disallowed.
    untransformed-fallout-CCC any-form-flag-is? NOT
  THEN
  ;

\ Запрещенные контексты для выпадения после долгой гласной
: constraint-VVГV-fallout  ( -- f )
  dictflag-rus dictflag-is?
  || untransformed-fallout-VVГV any-form-flag-is? NOT
  ;

\ Разрешенные контексты для невыпадения VГV
: constraint-VГV-fallout  ( -- f )
  nomen? verb? OR NOT                                                         ||
  first-form-flag untransformed-fallout-VГV untransformed-fallout-VңV OR AND  ||
  slot-all-empty?                                                             ||
  stem-polysyllabic? NOT                                                      ||
  TRUE ||
  stem-prev-sound-ptr cyr t~/ {vowel} NOT                                     ||
  stem-prev-sound-ptr cyr t~/ {short-vowel} NOT                               ||
  stem-prev-sound-ptr vowel-long-middle?                                      ||
  first-affix vowel-long?                                                     ||
  first-affix t~/ {short-vowel} NOT
  ;

\ Запрещенные контексты для выпадения конечного к, х
: constraint-V[кх]V-fallout  ( -- f )
  first-form-flag untransformed-fallout-V[кх]V AND NOT  ||
  dictflag-rus dictflag-empty?
  ;

\ После NF глухое: пар-тыр, сом-тыр
: constraint-voicedstem+Indir  ( -- f )
  flag NF₀  flag-is?  slots( <NF,Dur1> <Tense/Mood/Conv> )-empty?  AND
  \ 1 7 slot-range-empty?
  \ stem-last-sound consonant?
  \ stem-last-sound unvoiced? NOT
  \ AND AND
  ;

\ PresDial ча, Pres1Kac чадыр только после переднерядных основ
: constraint-frontstem  ( -- f )
  stem-last-char-vowel-row front-vowel =
  ;

\ поглощение гласных перед -ох: 3pos в виде алломорфов -ы/-i не
\ стягивается: хызох < хыс+ох, но не < хыс-ы-ох. Гласная
\ дательного падежа и деепричастия на гласную не стягивается:
\ суғ+ға+ох > суғаох ‘в воду же’.
: constraint-OK-fallout-<Tense/Mood/Conv>  ( -- f )
  slots( <Tense/Mood/Conv> <Ptcl₂> )-full?
  <Ptcl₂> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  slots( <Tense/Mood/Conv> <Ptcl₃> )-full?
  <Ptcl₃> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  AND
  ;
: constraint-OK-fallout-<Poss₁>  ( -- f )
  slots( <Poss₁> <Ptcl₂> )-full?
  <Ptcl₂> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  slots( <Poss₁> <Ptcl₃> )-full?
  <Ptcl₃> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  AND                                                  ||
  <Poss₁> form-slot t~/ {consonant}
  ;
: constraint-OK-fallout-<Poss₂>  ( -- f )
  slots( <Poss₂> <Ptcl₂> )-full?
  <Ptcl₂> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  slots( <Poss₂> <Ptcl₃> )-full?
  <Ptcl₃> form-slot-flags untransformed-fallout-OK AND NOT
  OR
  AND                                                  ||
  <Poss₂> form-slot t~/ {consonant}
  ;
: constraint-OK-fallout-<Case₂>  ( -- f )
  <Ptcl₂> form-slot-flags untransformed-fallout-OK AND NOT  &&
    slots( <Case₂> <Ptcl₃> )-full?  ||
    <Ptcl₃> form-slot-flags untransformed-fallout-OK AND NOT
  ;

: constraint-V+Acc  ( -- f )
  <Case₂> form-slot-vowel-at-left? ;

: constraint-DistrDial-short  ( -- f )
  <Distr> form-slot-vowel-at-left? NOT ;

: constraint-broken-harmony  ( -- f )
  [: harmony-vu-broken any-form-flag-is? NOT  ||
     dictflag-rus dictflag-is?  &&
       first-form-flag harmony-vu-broken AND 0<> ;] EXECUTE &&
  [: harmony-fb-broken any-form-flag-is? NOT  ||
     dictflag-rus dictflag-composite OR  dictflag-is?  &&
       first-form-flag harmony-fb-broken AND 0<> ;] EXECUTE
  ;

: constraint-VA>и-fallout-with-slot  { n-slot -- f }
  n-slot form-slot-vowel-at-left? NOT
  || n-slot form-slot-flags untransformed-fallout-VA>и AND
  ;
: constraint-VA>и-fallout-<Tense/Mood/Conv>  <Tense/Mood/Conv> constraint-VA>и-fallout-with-slot ;
: constraint-VA>и-fallout-<Neg/Gener>              <Neg/Gener> constraint-VA>и-fallout-with-slot ;
: constraint-VA>и-fallout-<Prosp>                      <Prosp> constraint-VA>и-fallout-with-slot ;
: constraint-VA>и-fallout  ( -- f )
  <Tense/Mood/Conv> => constraint-VA>и-fallout-<Tense/Mood/Conv>
  <Neg/Gener>       => constraint-VA>и-fallout-<Neg/Gener>
  <Prosp>           => constraint-VA>и-fallout-<Prosp>
  TRUE ABORT" Invalid slot for constraint-VA>и-fallout
  ; IMMEDIATE

: constraint-reduplication  ( -- f )
  dict-reduplication @ NOT
  || nomen?
  ;
