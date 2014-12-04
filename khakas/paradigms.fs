\ after entering new data, repeat this command until no matches:
\ :3,$s/"\(.*\)\([aceiopxyöÿ]\)/\="\"" . submatch(1) . tr(submatch(2),"aceiopxyöÿ", "асеіорхуӧӱ")/gI

   1 CONSTANT flag-Perf
   2 CONSTANT flag-Dur
   4 CONSTANT flag-Cond
   8 CONSTANT flag-RPast
  16 CONSTANT flag-1/2pos.sg
  32 CONSTANT flag-3pos
  64 CONSTANT flag-Hab
 128 CONSTANT flag-Past
 256 CONSTANT flag-Pres/Indir
 512 CONSTANT flag-Form1
1024 CONSTANT flag-Iter
2048 CONSTANT flag-Neg.cumul
flag-RPast flag-Cond OR   CONSTANT flag-RPast-or-Cond
flag-Past flag-Hab OR     CONSTANT flag-Past-or-Hab
flag-1/2pos.sg flag-3pos OR CONSTANT flag-1/2pos.sg-or-3pos

: is-пар/кел? 0 ;
: nomen-or-verb-with-Tense-without-Evid?  ( -- f )
  verb? IF
    7 slot-empty? IF
      flag-Neg.cumul flag-empty? IF FALSE EXIT THEN
    THEN
    flag-Pres/Indir flag-is? IF FALSE EXIT THEN
    8 slot-full? IF FALSE EXIT THEN
  THEN
  TRUE ;
: full-person-allowed? ( -- f )
  flag-RPast-or-Cond flag-is?
  8 15 slot-range-empty? AND NOT ;

slot: <Distr>  \ 1
  1 slot-empty!
  [form] -nodistr 0 0 |

  filter-start( verb? )
    1 slot-full!
    [form] Distr rule-vu-fb " ғла хла "
                           +" гле кле" |
  filter-end
  ;

slot: <Form>  \ 2
  2 slot-empty!
  [form] -noform 0 0 |

  filter-start( verb? )
    2 slot-full!
    [form] Form     rule-cv-fb " ып п "
                              +" іп п" | \ м диал.
    flag-Form1 flag-set
      [form] Form1  rule-fb    " а е" |
    flag-Form1 flag-clear
    [form] Form.Neg rule-nvu   " бин пин мин" |
  filter-end
  ;

slot: <Ptcl1>  \ 3
  3 slot-empty!
  [form] -noemph1 0 0 |

  filter-start( verb? )
    filter-start( flag-Form1 flag-empty? )
      3 slot-full!
      [form] Emph  rule-cv-fb  " даа таа дее тее" |
      [form] Delim rule-nvu-fb " ла ла на "
                              +" ле ле не" |
      [form] Ass1  rule-fb     " ох ӧк" |
  filter-end filter-end
  ;

slot: <Perf/Prosp>  \ 4
  4 slot-empty!
  [form] -noperf 0 0 |

  filter-start( verb? )
    filter-start( 2 slot-empty? )
      4 slot-full!
      flag-Perf flag-set
      [form] Perf  rule-cv-fb " ыбыс быс "
                             +" ібіс біс" |
      flag-Perf flag-clear
      [form] Prosp rule-cv-fb " ах х "
                             +" ек к" |
  filter-end filter-end
  ;

slot: <Dur/Iter>  \ 5
  5 slot-empty!
  [form] -nodur 0 0 |

  filter-start( verb? )
    5 slot-full!

    filter-start( flag-Form1 flag-empty? )
      flag-Dur flag-set
        filter-start( is-пар/кел? ) \ только с глаголами пар и кел!
          [form] Dur  0       " ир" |
          [form] Dur1 0       " и" |
        filter-else
          [form] Dur  rule-fb " чат чет" |
          [form] Dur1 rule-fb " ча че" |
        filter-end
      flag-Dur flag-clear
    filter-end
    flag-Iter flag-set
      [form] Iter  rule-vu-fb " дыр тыр "
                             +" дір тір" |
    flag-Iter flag-clear
  filter-end
  ;

slot: <Neg>  \ 6
  6 slot-empty!
  [form] -noneg 0 0 |

  filter-start( verb? )
    filter-start( 2 slot-empty? )
      6 slot-full!
      [form] Neg          rule-nvu-fb " ба па ма "
                                     +" бе пе ме" |
      flag-Neg.cumul flag-set
        [form] Neg.Fut      rule-nvu-fb " бас пас мас "
                                       +" бес пес мес" |
        [form] Neg.Conv     rule-nvu    " бин пин мин" |
        [form] Neg.Conv.Abl rule-vu-fb  " бинаң пинаң "
                                       +" бинең пинең" |
      flag-Neg.cumul flag-clear
  filter-end filter-end
  ;

slot: <Tense/Mood>  \ 7
  7 slot-empty!
  [form] -notense 0 0 |

  filter-start( verb? )
    7 slot-full!
    filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
      flag-Pres/Indir flag-set
        filter-start( flag-Dur flag-empty? )
          [form] Pres  rule-fb    " чадыр чедір" |
          [form] Pres1 rule-fb    " чададыр чедедір" |
        filter-end
        [form] Indir   rule-vu-fb " дыр тыр "
                                 +" дір тір" |
      flag-Pres/Indir flag-clear
      flag-Hab flag-set
        [form] Hab     rule-vu-fb " ӌаң чаң "
                                 +" ӌең чең" |
      flag-Hab flag-clear

      filter-start( 2 slot-empty?  4 6 slot-range-full?  OR  )
        [form] Fut     rule-fb    " ар ер" |
        flag-RPast flag-set
          [form] RPast rule-vu-fb " ды ты "
                                 +" ді ті" |
        flag-RPast flag-clear
        flag-Past flag-set
          [form] Past  rule-vu-fb " ған хан "
                                 +" ген кен" |
        flag-Past flag-clear
        filter-start( 1 5 slot-range-empty? )
          flag-Cond flag-set
            [form] Cond  rule-vu-fb " за са "
                                   +" зе се" |
          flag-Cond flag-clear
        filter-end
      filter-end
      filter-start( flag-Perf flag-empty? )
        [form] Cunc    rule-vu-fb " ғалах халах "
                                 +" гелек келек" |
      filter-end
      [form] Conv1     rule-cv-fb " ып п "
                                 +" іп п" |
      [form] Conv1dial rule-cv-fb " абас м "
                                 +" ебес м" |
      [form] Conv2     rule-fb    " а е" |
      [form] Lim       rule-vu-fb " ғали хали "
                                 +" гели кели" |
      [form] Opt       rule-vu-fb " ғай хай "
                                 +" гей кей" |
      [form] Debit     rule-vu-fb " ғадағ хадағ "
                                 +" гедег кедег" |
  filter-end filter-end
  ;

slot: <Irr/Evid>  \ 8
  8 slot-empty!
  [form] -noirr 0 0 |

  filter-start( flag-Cond flag-empty? )
    filter-start( verb? )
      filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
        8 slot-full!
        [form] Irr/Evid rule-vu-fb " ӌых чых "
                                  +" ӌіх чіх" |
  filter-end filter-end filter-end
  ;

slot: <Comit>  \ 9
  9 slot-empty!
  [form] -nocomit 0 0 |

  filter-start( nomen-or-verb-with-Tense-without-Evid? )
    filter-start( flag-RPast-or-Cond flag-empty? )
      filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
        9 slot-full!

        [form] Comit rule-nvu-fb " лығ тығ нығ "
                                +" ліг тіг ніг" |
  filter-end filter-end filter-end
  ;

slot: <Num>  \ 10
  10 slot-empty!
  [form] -nonum 0 0 |

  filter-start( nomen-or-verb-with-Tense-without-Evid? )
    filter-start( flag-RPast-or-Cond flag-empty? )
      filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
        10 slot-full!

        [form] Pl rule-nvu-fb " лар тар нар "
                             +" лер тер нер" |
  filter-end filter-end filter-end
  ;

slot: <Poss>  \ 11
  11 slot-empty!
  [form] -noposs 0 0 |

  filter-start( nomen-or-verb-with-Tense-without-Evid? )
    filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
      filter-start( flag-RPast-or-Cond flag-empty? )
        11 slot-full!

        flag-1/2pos.sg flag-set
          [form] 1pos.sg rule-cv-fb " ым м "
                                   +" ім м" |
          [form] 2pos.sg rule-cv-fb " ың ң "
                                   +" ің ң" |
        flag-1/2pos.sg flag-clear
        flag-3pos flag-set
          [form] 3pos    rule-cv-fb " ы зы "
                                   +" і зі" |
        flag-3pos flag-clear
        [form] 1pos.pl   rule-cv-fb " ыбыс быс "
                                   +" ібіс біс" |
        [form] 2pos.pl   rule-cv-fb " ыңар ңар "
                                   +" іңер ңер" |
  filter-end filter-end filter-end
  ;

slot: <Apos>  \ 12
  12 slot-empty!
  [form] -noapos 0 0 |

  filter-start( nomen-or-verb-with-Tense-without-Evid? )
    filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
      filter-start( flag-RPast-or-Cond flag-empty? )
        12 slot-full!

        [form] Apos  rule-vu  " ни ти" |
  filter-end filter-end filter-end
;

slot: <Case>  \ 13
  13 slot-empty!
  [form] -nocase 0 0 |

  filter-start( nomen-or-verb-with-Tense-without-Evid? )
    filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
      filter-start( flag-RPast-or-Cond flag-empty? )
        13 slot-full!

        [form] Gen    rule-vu-fb  " ның тың "
                                 +" нің тің" |

        filter-start( flag-1/2pos.sg-or-3pos flag-is? )
          [form] Dat  rule-cv-fb  " а на "
                                 +" е не" |
        filter-else
          [form] Dat  rule-vu-fb  " ға ха "
                                 +" ге ке" |
        filter-end

        filter-start( flag-3pos flag-is? )
          [form] Acc  0           " н" |
          [form] Loc  rule-fb     " нда нде" |
        filter-else
          [form] Acc  rule-vu-fb  " ны ты "
                                 +" ні ті" |
          [form] Loc  rule-vu-fb  " да та "
                                 +" де те" |
        filter-end

        [form] Abl    rule-nvu-fb " даң таң наң "
                                 +" дең тең нең" |

        filter-start( flag-3pos flag-is? )
          [form] All  rule-fb     " нзар нзер" |
        filter-else
          [form] All  rule-vu-fb  " зар сар "
                                 +" зер сер" |
        filter-end

        [form] Instr  rule-fb     " наң нең" |
        [form] Prol   rule-vu-fb  " ӌа ча "
                                 +" ӌе че" |
        [form] Delib  rule-nvu-fb " даңар таңар наңар "
                                 +" деңер теңер неңер" |

        filter-start( flag-3pos flag-is? )
          [form] Comp rule-fb     " ндағ ндег" |
        filter-else
          [form] Comp rule-vu-fb  " дағ тағ "
                                 +" дег тег" |
        filter-end

        filter-start( 12 slot-full? )
          [form] Temp rule-cv-fb  " ын н "
                                 +" ін н" |
        filter-end
  filter-end filter-end filter-end
  ;

slot: <Attr>  \ 14
  14 slot-empty!
  [form] -noattr 0 0 |

  filter-start( nomen-or-verb-with-Tense-without-Evid? )
    filter-start( flag-Form1 flag-empty?  flag-Iter flag-is? OR )
      filter-start( flag-RPast-or-Cond flag-empty? )
        14 slot-full!

        [form] Attr  rule-vu-fb " ғы хы "
                               +" гі кі" |
  filter-end filter-end filter-end
  ;

slot: <Ptcl2>  \ 15
  15 slot-empty!
  [form] -noemph 0 0 |

  filter-start( nomen-or-verb-with-Tense-without-Evid? )
    filter-start( flag-RPast-or-Cond flag-empty? )
      15 slot-full!

      [form] Ass2  rule-fb  " ох ӧк" |
  filter-end filter-end
  ;

slot: <Person>  \ 16
  16 slot-empty!
  [form] -3prs.sg 0 0 |

  16 slot-full!

  filter-start( nomen-or-verb-with-Tense-without-Evid? )

    filter-start( full-person-allowed? )
      [form] 1prs.sg         rule-nvu-fb " бын пын мын "
                                        +" бін пін мін" |
    filter-end

    filter-start( 9 15 slot-range-empty? )
      [form] 1prs.sg.br      rule-cv-fb  " ым м "
                                        +" ім м" |
    filter-end

    filter-start( full-person-allowed? )
      [form] 2prs.sg         rule-vu-fb  " зың сың "
                                        +" зің сің" |
    filter-end

    filter-start( 9 15 slot-range-empty? )
      [form] 2prs.sg.br      rule-cv-fb  " ың ң "
                                        +" ің ң" |
    filter-end

    filter-start( flag-Past-or-Hab flag-is? )
      [form] 3prs            rule-fb     " дыр дір" |
    filter-end

    [form] 1prs.pl           rule-nvu-fb " быс пыс мыс "
                                        +" біс піс міс" |
    filter-start( full-person-allowed? )
      [form] 2prs.pl         rule-vu-fb  " зар сар "
                                        +" зер сер" |
    filter-end

    filter-start( 9 15 slot-range-empty? )
      [form] 2prs.pl.br      rule-cv-fb  " ыңар ңар "
                                        +" іңер ңер" |
    filter-end

    [form] 3prs.pl           rule-nvu-fb " лар тар нар "
                                        +" лер тер нер" |

    filter-start( 7 15 slot-range-empty?  flag-Neg.cumul flag-empty? AND )
      [form] Imp1prs.sg      0           " им" |
      [form] Imp3prs.sg      rule-vu-fb  " зын сын "
                                        +" зін сін" |
      [form] Imp1prs.dual    rule-fb     " аң ең" |
      [form] Imp1prs.pl      rule-fb     " ибыс ибіс" |
      [form] Imp1prs.plIncl  rule-fb     " аңар еңер" |
      [form] Imp2prs.pl      rule-cv-fb  " ыңар ңар "
                                        +" іңер ңер" |
      [form] Imp3prs.pl      rule-vu-fb  " зыннар сыннар "
                                        +" зіннер сіннер" |
      [form] Prec1prs.sg     rule-fb     " имдах имдек" |
      [form] Prec2prs.sg     rule-vu-fb  " дах тах "
                                        +" дек тек" |
      [form] Prec3prs.sg     rule-vu-fb  " зындах сындах "
                                        +" зіндек сіндек" |
      [form] Prec1prs.dual   rule-fb     " аңдах еңдек" |
      [form] Prec1prs.pl     rule-fb     " ибыстах ибістек" |
      [form] Prec1prs.plIncl rule-fb     " аңардах еңердек" |
      [form] Prec2prs.pl     rule-cv-fb  " ыңардах ңардах "
                                        +" іңердек ңердек" |
      [form] Prec3prs.pl     rule-vu-fb  " зыннардах сыннардах "
                                        +" зіннердек сіннердек" |
      [form] Past1prs.sg     rule-vu-fb  " ғам хам "
                                        +" гем кем" |
      [form] Past2prs.sg     rule-vu-fb  " ғаӊ хаӊ "
                                        +" геӊ кеӊ" |
      [form] Past1prs.pl     rule-vu-fb  " ғабыс хабыс "
                                        +" гебіс кебіс" |
      [form] Past2prs.pl     rule-vu-fb  " ғазар хазар "
                                        +" гезер кезер" |
    filter-end

  filter-end
  ;

slot: <Manner>  \ 17
  17 slot-empty!
  [form] -noadv 0 0 |

  17 slot-full!

  [form] Manner rule-nvu " ли ти ни" |
;

CREATE slot-stack
 ' <Distr> , ' <Form> , ' <Ptcl1> , ' <Perf/Prosp> ,
 ' <Dur/Iter> , ' <Neg> , ' <Tense/Mood> , ' <Irr/Evid> ,
 ' <Comit> , ' <Num> , ' <Poss> , ' <Apos> ,
 ' <Case> , ' <Attr> , ' <Ptcl2> , ' <Person> , ' <Manner> , 0 ,
17 CONSTANT /slot-stack
