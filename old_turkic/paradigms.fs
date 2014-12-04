slot: <Neg>  \ 1
  1 slot-empty!
  [form] -noneg 0 0 |

  filter-start( verb? )
    1 slot-full!
    [form] Neg rule-fb " ma mä" |
  filter-end
  ;

slot: <Tense/Mood>  \ 2
  2 slot-empty!
  [form] -notense 0 0 |

  filter-start( verb? )
    2 slot-full!
    filter-start( 1 slot-empty? )
      [form] Neg.Indir   rule-fb     " maduk mädük" |
      [form] Neg.Sequ➀   rule-fb     " matï mäti" |
      [form] Neg.Sequ➁   rule-fb     " matïn mätin" |
      [form] Neg.Praes➀  rule-fb     " maz mäz" |
      [form] Neg.Praes➁  rule-fb     " mas mäs" |
      [form] Neg.Fut₂    rule-fb     " mačï mäči" |
      [form] Indir       rule-fb     " mïš miš" |
      [form] Sequ➀       rule-cv-fbl " ïp p ip p up p üp p" |
      [form] Sequ➁       rule-cv-fbl " ïpan pan ipän pän upan pan üpän pän" |
      [form] Praes➀      rule-cv-fb  " ur jur ür jür" |
      [form] Praes➁      rule-cv-fb  " ïr r ir r" |
      [form] Praes➂      rule-fb     " ar är" |
      [form] Fut₂➀       rule-fb     " dačï däči" |
      [form] Fut₂➁       rule-fb     " tačï täči" |
    filter-end
    [form] Perf➀      rule-fb     " duk dük" |
    [form] Perf➁      rule-fb     " tuk tük" |
    [form] Res        rule-fb     " juk jük" |
    [form] Fut₁➀      rule-fb     " gaj gäj" |
    [form] Fut₁➁      rule-fb     " kaj käj" |
    [form] FutIm➀     rule-fb     " galïr gälir" |
    [form] FutIm➁     rule-fb     " kalïr kälir" |
    [form] Inf        rule-fb     " mak mäk" |
    [form] PrtImpf    rule-cv-fbl " ïgma gma igmä gmä ugma gma ügmä gmä" |
    [form] PrtAct     rule-cv-fbl " ïglï glï igli gli uglï glï ügli gli" |
    [form] PrtHab➀    rule-fb     " gan gän" |
    [form] PrtHab➁    rule-fb     " kan kän" |
    [form] PrtAuct    rule-fb     " gučï güči" |
    [form] PrtProsp   rule-fbl    " sïk sik suk sük" |
    [form] PrtProj➀   rule-fb     " gu gü" |
    [form] PrtProj➁   rule-fb     " ku kü" |
    [form] PrtNecess➀ rule-fb     " guluk gülük" |
    [form] PrtNecess➁ rule-fb     " kuluk külük" |
    [form] Conv₁➀     rule-cv-fb  " u ju ü jü" |
    [form] Conv₁➁     rule-fb     " a ä" |
    [form] Conv₁➂     rule-fb     " ï i" |
    [form] Conv₂      rule-cv-fbl " ïjïn jïn ijin jin ujïn jïn üjin jin" |
    [form] ConvFin    rule-fb     " galï gäli" |
    [form] ConvDelim  rule-fb     " gïnča ginčä" |
    [form] Cond➀      rule-fb     " sar sär" |
    [form] Cond➁      rule-fb     " sa sä" |
  filter-end
  ;

slot: <Num₁>  \ 3
  3 slot-empty!
  [form] -nonum₁ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR )
    3 slot-full!

    [form] Pl rule-fb " lar lär" |
  filter-end
  ;

slot: <Poss₁>  \ 4
  4 slot-empty!
  [form] -noposs₁ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR )
    4 slot-full!
    [form] Poss1.sg      rule-cv-fbl   " ïm m im m um m üm m" |
    [form] Poss2.sg➀     rule-cv-fbl   " ïŋ ŋ iŋ ŋ uŋ ŋ üŋ ŋ" |
    [form] Poss2.sg➁     rule-cv-fbl   " ïg g ig g ug g üg g" |
    filter-start( 5 15 slot-range-empty? )
      [form] Poss3         rule-cv-fb " ï sï i si" |
    filter-end
    filter-start( 5 15 slot-range-full? )
      [form] Poss3         rule-cv-fb " ïn sïn in sin" |
    filter-end
    [form] Poss1.pl➀     rule-cv-fbl   " ïmïz mïz imiz miz umuz muz ümüz müz" |
    [form] Poss1.pl➁     rule-cv-fb    " umuz muz ümüz müz" |
    [form] Poss2.pl➀     rule-cv-fbl   " ïŋïz ŋïz iŋiz ŋiz uŋuz ŋuz üŋüz ŋüz" |
    [form] Poss2.pl➁     rule-cv-fbl   " ïgïz gïz igiz giz uguz guz ügüz güz" |
    [form] Poss2.pl.Pol➀ rule-cv-fbl   " ïŋïzlar ŋïzlar iŋizlär ŋizlär uŋuzlar ŋuzlar üŋüzlär ŋüzlär" |
    [form] Poss2.pl.Pol➁ rule-cv-fbl   " ïgïzlar gïzlar igizlär gizlär uguzlar guzlar ügüzlär güzlär" |
  filter-end
  ;

slot: <Apos₁>  \ 5
  5 slot-empty!
  [form] -noapos₁ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR )
    5 slot-full!
    [form] Apos rule-fb " lï li" |
  filter-end
;

slot: <Case₁>  \ 6
  6 slot-empty!
  [form] -nocase₁ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR )
    6 slot-full!
    [form] Gen➀   rule-cv-fbl " ïŋ nïŋ iŋ niŋ uŋ nuŋ üŋ nüŋ" |
    [form] Acc➁   rule-fb     " nï ni" |
    [form] Dat➀   rule-fb     " ka kä" |
    [form] Dat➁   rule-fb     " ga gä" |
    [form] Loc➀   rule-fb     " ta tä" |
    [form] Loc➁   rule-fb     " da dä" |
    [form] Abl➀   rule-fb     " dïn din" |
    [form] Abl➁   rule-fb     " tïn tin" |
    [form] Abl➂   rule-fb     " dan dän" |
    [form] Abl➃   rule-fb     " tan tän" |
    [form] Instr  rule-cv-fbl " ïn n in n un n ün n" |
    [form] Equ    rule-fb     " ča čä" |
    [form] Dir    rule-fb     " garu gärü" |
    [form] Part   rule-fb     " ra rä" |
    [form] Simil➀ rule-fb     " laju läjü" |
    [form] Simil➁ rule-fb     " čulaju čüläjü" |
    [form] Com    rule-fbl    " lïgu ligü lugu lügü" |
    \ filter-start( 4 slot-empty? )
      [form] Gen➁   rule-fbl    " nïŋ niŋ nuŋ nüŋ" |
      [form] Gen➂   rule-fbl    " nïg nig nug nüg" |
      [form] Gen➃   rule-fb     " nuŋ nüŋ" |
      [form] Acc➀   rule-cv-fbl " ïg g ig g ug g üg g" |
    \ filter-end
    filter-start( 4 slot-full? )
      [form] Acc➀p  rule-fb     " ïn in" |
      [form] Dat➂p  rule-fb     " a ä" |
      [form] Dir➁p  rule-fb     " aru ärü" |
    filter-end
  filter-end
  ;

slot: <Attr₁>  \ 7
  7 slot-empty!
  [form] -noattr₁ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR )
    7 slot-full!
    [form] Attr  rule-fb " kï ki" |
  filter-end
  ;

slot: <Comit₁>  \ 8
  8 slot-empty!
  [form] -nocomit₁ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR )
    8 slot-full!

    [form] Comit  rule-fbl " lïg lig lug lüg" |
    [form] Car    rule-fbl " sïz siz suz süz" |
    [form] Comp   rule-fb  " dag däg" |
    [form] Dimin➀ rule-fb  " kja kjä" |
    [form] Dimin➁ rule-fb  " kïña kiñä" |
  filter-end
  ;

slot: <Num₂>  \ 9
  9 slot-empty!
  [form] -nonum₂ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR  3 8 slot-range-full?  AND )
    9 slot-full!

    [form] Pl rule-fb " lar lär" |
  filter-end
  ;

slot: <Poss₂>  \ 10
  10 slot-empty!
  [form] -noposs₂ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR  4 8 slot-range-full?  AND )
    10 slot-full!
    [form] Poss1.sg      rule-cv-fbl   " ïm m im m um m üm m" |
    [form] Poss2.sg➀     rule-cv-fbl   " ïŋ ŋ iŋ ŋ uŋ ŋ üŋ ŋ" |
    [form] Poss2.sg➁     rule-cv-fbl   " ïg g ig g ug g üg g" |
    filter-start( 11 15 slot-range-empty? )
      [form] Poss3         rule-cv-fb " ï sï i si" |
    filter-end
    filter-start( 11 15 slot-range-full? )
      [form] Poss3         rule-cv-fb " ïn sïn in sin" |
    filter-end
    [form] Poss1.pl➀     rule-cv-fbl   " ïmïz mïz imiz miz umuz muz ümüz müz" |
    [form] Poss1.pl➁     rule-cv-fb    " umuz muz ümüz müz" |
    [form] Poss2.pl➀     rule-cv-fbl   " ïŋïz ŋïz iŋiz ŋiz uŋuz ŋuz üŋüz ŋüz" |
    [form] Poss2.pl➁     rule-cv-fbl   " ïgïz gïz igiz giz uguz guz ügüz güz" |
    [form] Poss2.pl.Pol➀ rule-cv-fbl   " ïŋïzlar ŋïzlar iŋizlär ŋizlär uŋuzlar ŋuzlar üŋüzlär ŋüzlär" |
    [form] Poss2.pl.Pol➁ rule-cv-fbl   " ïgïzlar gïzlar igizlär gizlär uguzlar guzlar ügüzlär güzlär" |
  filter-end
  ;

slot: <Apos₂>  \ 11
  11 slot-empty!
  [form] -noapos₂ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR  5 8 slot-range-full?  AND )
    11 slot-full!
    [form] Apos rule-fb " lï li" |
  filter-end
;

slot: <Case₂>  \ 12
  12 slot-empty!
  [form] -nocase₂ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR  6 8 slot-range-full?  AND )
    12 slot-full!
    [form] Gen➀   rule-cv-fbl " ïŋ nïŋ iŋ niŋ uŋ nuŋ üŋ nüŋ" |
    [form] Acc➁   rule-fb     " nï ni" |
    [form] Dat➀   rule-fb     " ka kä" |
    [form] Dat➁   rule-fb     " ga gä" |
    [form] Loc➀   rule-fb     " ta tä" |
    [form] Loc➁   rule-fb     " da dä" |
    [form] Abl➀   rule-fb     " dïn din" |
    [form] Abl➁   rule-fb     " tïn tin" |
    [form] Abl➂   rule-fb     " dan dän" |
    [form] Abl➃   rule-fb     " tan tän" |
    [form] Instr  rule-cv-fbl " ïn n in n un n ün n" |
    [form] Equ    rule-fb     " ča čä" |
    [form] Dir    rule-fb     " garu gärü" |
    [form] Part   rule-fb     " ra rä" |
    [form] Simil➀ rule-fb     " laju läjü" |
    [form] Simil➁ rule-fb     " čulaju čüläjü" |
    [form] Com    rule-fbl    " lïgu ligü lugu lügü" |
    \ filter-start( 4 slot-empty? )
      [form] Gen➁   rule-fbl    " nïŋ niŋ nuŋ nüŋ" |
      [form] Gen➂   rule-fbl    " nïg nig nug nüg" |
      [form] Gen➃   rule-fb     " nuŋ nüŋ" |
      [form] Acc➀   rule-cv-fbl " ïg g ig g ug g üg g" |
    \ filter-end
    filter-start( 4 slot-full? )
      [form] Acc➀p  rule-fb     " ïn in" |
      [form] Dat➂p  rule-fb     " a ä" |
      [form] Dir➁p  rule-fb     " aru ärü" |
    filter-end
  filter-end
  ;

slot: <Attr₂>  \ 13
  13 slot-empty!
  [form] -noattr₂ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR  7 8 slot-range-full?  AND )
    13 slot-full!
    [form] Attr  rule-fb " kï ki" |
  filter-end
  ;

slot: <Comit₂>  \ 14
  14 slot-empty!
  [form] -nocomit₂ 0 0 |

  filter-start( verb? NOT  2 slot-full?  OR  8 8 slot-range-full?  AND )
    14 slot-full!

    [form] Comit  rule-fbl " lïg lig lug lüg" |
    [form] Car    rule-fbl " sïz siz suz süz" |
    [form] Comp   rule-fb  " dag däg" |
    [form] Dimin➀ rule-fb  " kja kjä" |
    [form] Dimin➁ rule-fb  " kïña kiñä" |
  filter-end
  ;

slot: <Person>  \ 15
  15 slot-empty!
  [form] -3prs.sg 0 0 |
  filter-start( verb?  2 14 slot-range-empty?  AND )
    [form] Imp2prs.sg 0 0 |
  filter-end

  15 slot-full!

  filter-start( verb? NOT  2 slot-full?  OR )
    [form] 1prs.sg➀    rule-fb  " man män" |
    [form] 1prs.sg➁    rule-fb  " ban bän" |
    [form] 2prs.sg     rule-fb  " san sän" |
    [form] 1prs.pl➀    rule-fbl " mïz miz muz müz" |
    [form] 1prs.pl➁    rule-fbl " bïz biz buz büz" |
    [form] 2prs.pl     rule-fbl " sïz siz suz süz" |
    [form] 2prs.pl.Pol rule-fbl " sïzlar sizlär suzlar süzlär" |
    [form] 3prs.pl     rule-fb  " lar lär" |
  filter-end
  filter-start( verb?  2 14 slot-range-empty?  AND )
    [form] Imp1prs.sg        rule-cv-fb  " ajïn jïn äjin jin" |
    [form] Imp2prs.sg.Emph➀  rule-fb     " gïl gil" |
    [form] Imp2prs.sg.Emph➁  rule-fb     " ču čü" |
    [form] Imp3prs➀          rule-fb     " zun zün" |
    [form] Imp3prs➁          rule-fb     " sun sün" |
    [form] Imp3prs➂          rule-fb     " čun čün" |
    [form] Imp1prs.pl        rule-cv-fb  " alïm lïm älim lim" |
    [form] Imp2prs.pl        rule-cv-fbl " ïŋ ŋ iŋ ŋ uŋ ŋ üŋ ŋ" |
    [form] Imp2prs.pl.Pol    rule-cv-fbl " ïŋlar ŋlar iŋlär ŋlär uŋlar ŋlar üŋlär ŋlär" |
    [form] Imp3prs.pl➀       rule-fb     " zuŋlar züŋlär" |
    [form] Imp3prs.pl➁       rule-fb     " suŋlar süŋlär" |
    [form] Imp3prs.pl➂       rule-fb     " čuŋlar čüŋlär" |
    [form] Praet1prs.sg➀     rule-fbl    " dïm dim dum düm" |
    [form] Praet1prs.sg➁     rule-fbl    " tïm tim tum tüm" |
    [form] Praet2prs.sg➀     rule-fbl    " dïŋ diŋ duŋ düŋ" |
    [form] Praet2prs.sg➁     rule-fbl    " dïg dig dug düg" |
    [form] Praet2prs.sg➂     rule-fbl    " tïŋ tiŋ tuŋ tüŋ" |
    [form] Praet2prs.sg➃     rule-fbl    " tïg tig tug tüg" |
    [form] Praet3prs➀        rule-fb     " dï di" |
    [form] Praet3prs➁        rule-fb     " tï ti" |
    [form] Praet1prs.pl➀     rule-fbl    " dïmïz dimiz dumuz dümüz" |
    [form] Praet1prs.pl➁     rule-fb     " dumuz dümüz" |
    [form] Praet1prs.pl➂     rule-fbl    " tïmïz timiz tumuz tümüz" |
    [form] Praet1prs.pl➃     rule-fb     " tumuz tümüz" |
    [form] Praet1prs.pl➄     rule-fbl    " dïk dik duk dük" |
    [form] Praet1prs.pl➅     rule-fbl    " tïk tik tuk tük" |
    [form] Praet2prs.pl➀     rule-fbl    " dïŋïz diŋiz duŋuz düŋüz" |
    [form] Praet2prs.pl➁     rule-fbl    " dïgïz digiz duguz dügüz" |
    [form] Praet2prs.pl➂     rule-fbl    " tïŋïz tiŋiz tuŋuz tüŋüz" |
    [form] Praet2prs.pl➃     rule-fbl    " tïgïz tigiz tuguz tügüz" |
    [form] Praet2prs.pl.Pol➀ rule-fbl    " dïŋïzlar diŋizlär duŋuzlar düŋüzlär" |
    [form] Praet2prs.pl.Pol➁ rule-fbl    " dïgïzlar digizlär duguzlar dügüzlär" |
    [form] Praet2prs.pl.Pol➂ rule-fbl    " tïŋïzlar tiŋizlär tuŋuzlar tüŋüzlär" |
    [form] Praet2prs.pl.Pol➃ rule-fbl    " tïgïzlar tigizlär tuguzlar tügüzlär" |
    [form] Praet3prs.pl➀     rule-fb     " dïlar dilär" |
    [form] Praet3prs.pl➁     rule-fb     " tïlar tilär" |
  filter-end
  ;


CREATE slot-stack
 ' <Neg> , ' <Tense/Mood> ,
 ' <Num₁> , ' <Poss₁> , ' <Apos₁> , ' <Case₁> , ' <Attr₁> , ' <Comit₁> ,
 ' <Num₂> , ' <Poss₂> , ' <Apos₂> , ' <Case₂> , ' <Attr₂> , ' <Comit₂> ,
 ' <Person> , 0 ,
15 CONSTANT /slot-stack
