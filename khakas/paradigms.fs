\ after entering new data, repeat this command until no matches:
\ :3,$s/"\(.*\)\([aceiopxyöÿ]\)/\="\"" . submatch(1) . tr(submatch(2),"aceiopxyöÿ", "асеіорхуӧӱ")/gI

%0000000001 CONSTANT flag-Cond
%0000000010 CONSTANT flag-Dur
%0000000100 CONSTANT flag-Form.Neg
%0000001000 CONSTANT flag-Form2
%0000010000 CONSTANT flag-Hab
%0000100000 CONSTANT flag-Opt
%0001000000 CONSTANT flag-Past
%0010000000 CONSTANT flag-Perf
%0100000000 CONSTANT flag-RPast
%1000000000 CONSTANT flag-3pos
flag-RPast flag-Cond OR         CONSTANT flag-RPast-or-Cond
flag-RPast-or-Cond flag-Opt OR  CONSTANT flag-RPast-or-Cond-or-Opt
flag-Past flag-Hab OR           CONSTANT flag-Past-or-Hab

0  S" пар" strlist-prepend-alloc  S" кил"  strlist-prepend-alloc  CONSTANT пар|кил
: is-пар/кил?  ( -- f )
  paradigm-stems @  пар|кил  strlists-intersect? ;

\ 2. Заполнение позиций 11–16 может происходить либо сразу после
\ позиции 0 (если слово имеет помету Nomen, частица, наречие),
\ либо непосредственно после заполненной позиции 8 (если слово
\ имеет помету Verbum); но не в случае, если позиция 8 заполнена
\ аффиксами RPast ТI, Cond СА, Opt КАй.
: nomen-or-verb-with-Tense-non-RPast-Cond-Opt?  ( -- f )
  verb? IF
    8 slot-empty? IF FALSE EXIT THEN
    flag-RPast-or-Cond-or-Opt flag-is? IF FALSE EXIT THEN
    9 10 slot-range-empty?
  ELSE TRUE THEN ;

\ 21. Полные лично-числовые показатели (Pers) могут следовать
\ после любых морфем, за исключением показателя недавно
\ прошедшего времени (RPast) и показателя условного наклонения
\ (Cond) непосредственно перед ними; краткие разрешены только
\ у глаголов при незаполненных позициях с 11 по 17.
: full-person-allowed?  ( -- f )
  flag-RPast-or-Cond flag-empty?  9 17 slot-range-full?  OR ;
: short-person-allowed?  ( -- f )
  verb?  11 17 slot-range-empty?  AND ;

slot: <Distr>  \ 1
  1 slot-empty!
  form" -nodistr "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    1 slot-full!
    form" Distr КлА"
  filter-end
  ;

slot: <Form1>  \ 2
  2 slot-empty!
  form" -noform1 "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    2 slot-full!

    form" Form1 (Б)"

    flag-Form.Neg flag-set
      form" Form.Neg Бин"
    flag-Form.Neg flag-clear
  filter-end
  ;

slot: <Ptcl1>  \ 3
  3 slot-empty!
  form" -noemph1 "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    3 slot-full!
    form" Emph ТАА"
    form" Delim ЛА"
    form" Ass1 ОК"
  filter-end
  ;

slot: <Perf/Prosp>  \ 4
  4 slot-empty!
  form" -noperf "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    4 slot-full!

    \ 3. Показатели позиции 2 (Form1) и показатель Perf (I)бIС
    \ (4) в пределах одной словоформы встречаются только в
    \ случае заполнения позиции 2 кумулятивным показателем
    \ Form.Neg
    filter-start( 2 slot-empty?  flag-Form.Neg flag-is? OR )
      flag-Perf flag-set
        form" Perf (І)бІс"
      flag-Perf flag-clear
    filter-end

    \ 5. Показатель позиции 4 Prosp встречается только перед
    \ формами на чА (Dur)
    filter-start( flag-Dur flag-is? )
      form" Prosp (А)К"
    filter-end
  filter-end
  ;

slot: <Dur>  \ 5
  5 slot-empty!
  form" -nodur "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой
  \ Verbum
  filter-start( verb? )
    5 slot-full!
    \ 14. Показатель Dur1 и(р) заполняется только, если позиции
    \ 1, 3, 4 не заполнены, а в позиции 0 стоит пар- или кил-
    \ (но при этих основах может выбираться с тем же успехом и
    \ показатель Dur чА(Т), свободное варьирование).
    filter-start( 1 slot-empty?  3 4 slot-range-empty?  AND  is-пар/кил? AND )
      form" Dur1₁ и"
      \ 15.2. Только вариант Dur1 и выбирается перед показате-
      \ лем Past КАн (8), во всех прочих случаях варианты и и
      \ ир находятся в свободном варьировании.
      filter-start( flag-Past flag-empty? )
        form" Dur1₂ ир"
      filter-end
    filter-end

    \ 12. Показатель Dur чА(Т) заполняется только, если
    \ заполнена хотя бы одна из позиций 2, 4, (т.е. Form1 или
    \ Perf).
    filter-start( 2 slot-full?  4 slot-full?  OR )
      flag-Dur flag-set
        \ 15. Вариант Dur ча выбирается только непосредственно
        \ перед Iter дIр, личными окончаниями или концом слова,
        \ вариант чат – в прочих случаях и в конце слова (2
        \ встреченных примера, правда, были не презенсом, а 2 л.
        \ императивом; но, возможно,надо ставить свободное
        \ варьирование).
        filter-start( 6 slot-empty?  7 slot-full?  AND
                      6 17 slot-range-empty?  18 slot-full?  AND  OR
                      6 19 slot-range-empty?  OR )
          form" Dur₁ чА"
        filter-end
        filter-start( 6 slot-empty?  7 slot-full?  AND
                      6 17 slot-range-empty?  18 slot-full?  AND
                      OR  NOT )
          form" Dur₂ чАт"
        filter-end
      flag-Dur flag-clear
    filter-end
  filter-end
  ;

slot: <Neg/Form2>  \ 6
  6 slot-empty!
  form" -noneg "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    6 slot-full!
    \ 7. Если в поз. 2 есть показатель Form.Neg, то в поз. 6 не
    \ может быть отр. показателей Neg, Neg.Fut, Neg.Conv,
    \ Neg.Сonv.Abl.
    filter-start( flag-Form.Neg flag-empty? )
      form" Neg БА"
      form" Neg.Fut БАс"
      form" Neg.Conv Бин"
      form" Neg.Conv.Abl БинАң"
    filter-end
    flag-Form2 flag-set
      form" Form2 А"
    flag-Form2 flag-clear
  filter-end
  ;

slot: <Iter>  \ 7
  7 slot-empty!
  form" -noiter "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    7 slot-full!
    form" Iter дІр"
  filter-end
  ;

slot: <Tense/Mood>  \ 8
  8 slot-empty!
  form" -notense "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    \ 4. Показатели позиции 2 (Form1) и все показатели поз. 8
    \ (Fut, Past, RPast, Hab, Cond и др) непосредственно рядом
    \ не встречаются.
    filter-start( 2 slot-empty?  3 7 slot-range-full?  OR  )
      \ 6. Непосредственно после показателя позиции 6 Form2 А
      \ может следовать только показатель Iter дIр.
      filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
        8 slot-full!
        flag-Past flag-set
          form" Past КАн"
        flag-Past flag-clear
        form" Fut Ар"
        flag-Hab flag-set
          form" Hab ЧАң"
        flag-Hab flag-clear
        flag-RPast flag-set
          form" RPast ТІ"
        flag-RPast flag-clear
        \ 8. Показатель Cond заполняется только, если  позиции 1- 7 незаполнены
        \ 9. Показатель Opt заполняется только, если  позиции 1-7 незаполнены
        \ 10. Показатель Assum заполняется только, если  позиции 1-7 незаполнены
        \ 11. Показатель Cunc заполняется только, если  позиции 1-7 незаполнены
        filter-start( 1 7 slot-range-empty? )
          \ 16. Показатель Perf и показатель Cunc в пределах
          \ одной словоформы не встречаются
          filter-start( flag-Perf flag-empty? )
            form" Cunc КАлАК"
          filter-end
          flag-Cond flag-set
            form" Cond СА"
          flag-Cond flag-clear
          flag-Opt flag-set
            form" Opt КАй"
          flag-Opt flag-clear
          form" Assum КАдаК"
        filter-end
        form" Conv1 (І)п"
        form" Conv1dial АбАс"
        form" Conv2 А"
        form" Lim КАли"
  filter-end filter-end filter-end
  ;

slot: <Evid>  \ 9
  9 slot-empty!
  form" -noevid "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
      \ 19. Непосредственно после показателя недавно прошедшего
      \ времени (RPast) может следовать только лично-числовой
      \ показатель (Pers) или показатель ирреалиса (Irr)
      filter-start( flag-RPast flag-empty? )
        \ 20. Непосредственно после показателя условного
        \ наклонения (Cond) может следовать только
        \ лично-числовой показатель (Pers);
        filter-start( flag-Cond flag-empty? )
          9 slot-full!
          form" Indir ТІр"
  filter-end filter-end filter-end filter-end
  ;

slot: <Irr>  \ 10
  10 slot-empty!
  form" -noirr "

  \ 1. Позиции 1–10 могут заполняться только у слов с пометой Verbum
  filter-start( verb? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty? )
      \ 20. Непосредственно после показателя условного
      \ наклонения (Cond) может следовать только
      \ лично-числовой показатель (Pers);
      filter-start( flag-Cond flag-empty?  9 9 slot-range-full?  OR )
        10 slot-full!
        form" Irr ЧІК"
  filter-end filter-end filter-end
  ;

slot: <Comit>  \ 11
  11 slot-empty!
  form" -nocomit "

  filter-start( nomen-or-verb-with-Tense-non-RPast-Cond-Opt? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
      \ 19. Непосредственно после показателя недавно прошедшего
      \ времени (RPast) может следовать только лично-числовой
      \ показатель (Pers) или показатель ирреалиса (Irr)
      filter-start( flag-RPast flag-empty?  9 10 slot-range-full?  OR )
        \ 20. Непосредственно после показателя условного
        \ наклонения (Cond) может следовать только
        \ лично-числовой показатель (Pers);
        filter-start( flag-Cond flag-empty?  9 10 slot-range-full?  OR )
          11 slot-full!

          form" Comit ЛІГ"
  filter-end filter-end filter-end filter-end
  ;

slot: <Num>  \ 12
  12 slot-empty!
  form" -nonum "

  filter-start( nomen-or-verb-with-Tense-non-RPast-Cond-Opt? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
      \ 19. Непосредственно после показателя недавно прошедшего
      \ времени (RPast) может следовать только лично-числовой
      \ показатель (Pers) или показатель ирреалиса (Irr)
      filter-start( flag-RPast flag-empty?  9 11 slot-range-full?  OR )
        \ 20. Непосредственно после показателя условного
        \ наклонения (Cond) может следовать только
        \ лично-числовой показатель (Pers);
        filter-start( flag-Cond flag-empty?  9 11 slot-range-full?  OR )
          12 slot-full!

          form" Pl ЛАр"
  filter-end filter-end filter-end filter-end
  ;

slot: <Poss>  \ 13
  13 slot-empty!
  form" -noposs "

  filter-start( nomen-or-verb-with-Tense-non-RPast-Cond-Opt? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
      \ 19. Непосредственно после показателя недавно прошедшего
      \ времени (RPast) может следовать только лично-числовой
      \ показатель (Pers) или показатель ирреалиса (Irr)
      filter-start( flag-RPast flag-empty?  9 12 slot-range-full?  OR )
        \ 20. Непосредственно после показателя условного
        \ наклонения (Cond) может следовать только
        \ лично-числовой показатель (Pers);
        filter-start( flag-Cond flag-empty?  9 12 slot-range-full?  OR )
          13 slot-full!

          form" 1pos.sg (І)м"
          form" 2pos.sg (І)ң"
          flag-3pos flag-set
            form" 3pos (з)І"
          flag-3pos flag-clear
          form" 1pos.pl (І)бІс"
          form" 2pos.pl (І)ңАр"
  filter-end filter-end filter-end filter-end
  ;

slot: <Apos>  \ 14
  14 slot-empty!
  form" -noapos "

  filter-start( nomen-or-verb-with-Tense-non-RPast-Cond-Opt? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
      \ 19. Непосредственно после показателя недавно прошедшего
      \ времени (RPast) может следовать только лично-числовой
      \ показатель (Pers) или показатель ирреалиса (Irr)
      filter-start( flag-RPast flag-empty?  9 13 slot-range-full?  OR )
        \ 20. Непосредственно после показателя условного
        \ наклонения (Cond) может следовать только
        \ лично-числовой показатель (Pers);
        filter-start( flag-Cond flag-empty?  9 13 slot-range-full?  OR )
          14 slot-full!

          form" Apos Ни"
  filter-end filter-end filter-end filter-end
;

slot: <Case>  \ 15
  15 slot-empty!
  form" -nocase "

  filter-start( nomen-or-verb-with-Tense-non-RPast-Cond-Opt? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
      \ 19. Непосредственно после показателя недавно прошедшего
      \ времени (RPast) может следовать только лично-числовой
      \ показатель (Pers) или показатель ирреалиса (Irr)
      filter-start( flag-RPast flag-empty?  9 14 slot-range-full?  OR )
        \ 20. Непосредственно после показателя условного
        \ наклонения (Cond) может следовать только
        \ лично-числовой показатель (Pers);
        filter-start( flag-Cond flag-empty?  9 14 slot-range-full?  OR )
          15 slot-full!

          form" Gen НІң"

          \ 17. Аффиксы посессивного склонения выбираются только
          \ в случае заполнения позиции 13 (Poss) или 14 (Apos);
          filter-start( 13 14 slot-range-empty? )
            form" Dat (К)А"
            form" Acc НІ"
            form" Loc ТА"
            form" All САр"
            form" Prol ЧА"
            form" Comp ТАГ"
          filter-else
            form" Dat₁ (К)А"
            form" Dat₂ нА"
            filter-start( 14 slot-empty?  flag-3pos flag-is?  AND )
              form" Acc₂ Н"
            filter-else
              form" Acc₁ НІ"
            filter-end
            form" Loc (н)ТА"
            form" All (н)САр"
            form" Prol (н)ЧА"
            form" Comp (н)ДАГ"
          filter-end

          form" Abl ДАң"
          form" Instr нАң"
          form" Delib ДАңАр"
          form" Temp (І)н"
  filter-end filter-end filter-end filter-end
  ;

slot: <Attr>  \ 16
  16 slot-empty!
  form" -noattr "

  filter-start( nomen-or-verb-with-Tense-non-RPast-Cond-Opt? )
    \ 6. Непосредственно после показателя позиции 6 Form2 А
    \ может следовать только показатель Iter дIр.
    filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
      \ 19. Непосредственно после показателя недавно прошедшего
      \ времени (RPast) может следовать только лично-числовой
      \ показатель (Pers) или показатель ирреалиса (Irr)
      filter-start( flag-RPast flag-empty?  9 15 slot-range-full?  OR )
        \ 20. Непосредственно после показателя условного
        \ наклонения (Cond) может следовать только
        \ лично-числовой показатель (Pers);
        filter-start( flag-Cond flag-empty?  9 15 slot-range-full?  OR )
          16 slot-full!

          form" Attr КІ"
  filter-end filter-end filter-end filter-end
  ;

slot: <Ptcl2>  \ 17
  17 slot-empty!
  form" -noemph "

  \ 6. Непосредственно после показателя позиции 6 Form2 А
  \ может следовать только показатель Iter дIр.
  filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
    \ 19. Непосредственно после показателя недавно прошедшего
    \ времени (RPast) может следовать только лично-числовой
    \ показатель (Pers) или показатель ирреалиса (Irr)
    filter-start( flag-RPast flag-empty?  9 16 slot-range-full?  OR )
      \ 20. Непосредственно после показателя условного
      \ наклонения (Cond) может следовать только
      \ лично-числовой показатель (Pers);
      filter-start( flag-Cond flag-empty?  9 16 slot-range-full?  OR )
        17 slot-full!

        form" Ass2 ОК"
  filter-end filter-end filter-end
  ;

slot: <Person>  \ 18
  18 slot-empty!
  form" -3prs.sg "

  \ 6. Непосредственно после показателя позиции 6 Form2 А
  \ может следовать только показатель Iter дIр.
  filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
    18 slot-full!

    filter-start( full-person-allowed? )
      form" 1prs.sg БІн"
      form" 2prs.sg СІң"

      \ 22. Личные показатели 3prs.sg -дыр, -дiр могут следовать
      \ только непосредственно после показателей КАн и Hab ЧАӊ.
      filter-start( flag-Past-or-Hab flag-is?  9 17 slot-range-empty?  AND )
        form" 3prs дІр"
      filter-end

      form" 1prs.pl БІс"
      form" 2prs.pl САр"
      form" 3prs.pl СІңнАр"
    filter-end

    filter-start( short-person-allowed? )
      form" 1prs.sg.br м"
      form" 2prs.sg.br ң"
      form" 2prs.pl.br (І)ңАр"
      form" 3prs.pl ЛАр"
    filter-end

    \ 18. Показатели поз. 18 (Person) с пометами Imp и Prec могут
    \ быть только у слов, имеющих помету Verbum; они следуют
    \ непосредственно после заполнителя позиции с номером <= 6
    filter-start( verb?  7 17 slot-range-empty?  AND )
      form" Imp1prs.sg им"
      form" Imp3prs.sg СІн"
      form" Imp1prs.dual Аң"
      form" Imp1prs.pl ибІс"
      form" Imp1prs.plIncl АңАр"
      form" Imp2prs.pl (І)ңАр"
      form" Imp3prs.pl СІннАр"
      form" Prec1prs.sg имдАК"
      form" Prec2prs.sg ТАК"
      form" Prec3prs.sg СІндАК"
      form" Prec1prs.dual АңдАК"
      form" Prec1prs.pl ибІстАК"
      form" Prec1prs.plIncl АңАрдАК"
      form" Prec2prs.pl (І)ңАрдАК"
      form" Prec3prs.pl СІннАрдАК"
      \ [form] Past1prs.sg     rule-vu-fb  " ғам хам "
      \                                   +" гем кем" |
      \ [form] Past2prs.sg     rule-vu-fb  " ғаӊ хаӊ "
      \                                   +" геӊ кеӊ" |
      \ [form] Past1prs.pl     rule-vu-fb  " ғабыс хабыс "
      \                                   +" гебіс кебіс" |
      \ [form] Past2prs.pl     rule-vu-fb  " ғазар хазар "
      \                                   +" гезер кезер" |
    filter-end
  filter-end
  ;

slot: <Adv>  \ 19
  19 slot-empty!
  form" -noadv "

  \ 6. Непосредственно после показателя позиции 6 Form2 А
  \ может следовать только показатель Iter дIр.
  filter-start( flag-Form2 flag-empty?  7 slot-full?  OR )
    \ 19. Непосредственно после показателя недавно прошедшего
    \ времени (RPast) может следовать только лично-числовой
    \ показатель (Pers) или показатель ирреалиса (Irr)
    filter-start( flag-RPast flag-empty?  9 18 slot-range-full?  OR )
      \ 20. Непосредственно после показателя условного
      \ наклонения (Cond) может следовать только
      \ лично-числовой показатель (Pers);
      filter-start( flag-Cond flag-empty?  9 18 slot-range-full?  OR )
        19 slot-full!

        form" Adv Ли"
  filter-end filter-end filter-end
;

CREATE slot-stack
 ' <Distr> , ' <Form1> , ' <Ptcl1> , ' <Perf/Prosp> ,
 ' <Dur> , ' <Neg/Form2> , ' <Iter> , ' <Tense/Mood> ,
 ' <Evid> , ' <Irr> , ' <Comit> , ' <Num> ,
 ' <Poss> , ' <Apos> , ' <Case> , ' <Attr> ,
 ' <Ptcl2> , ' <Person> , ' <Adv> , 0 ,
HERE slot-stack - 1- CELL / CONSTANT /slot-stack
