REQUIRE minire.fs
REQUIRE phonetics.fs
REQUIRE warp.fs

%000001 CONSTANT tr-envoice
%000010 CONSTANT tr-fugitive
%000100 CONSTANT tr-hiatus-imp
%001000 CONSTANT tr-hiatus-emph
%010000 CONSTANT tr-confluence
%100000 CONSTANT tr-fallout
VARIABLE transform-flags
: transform-flag-set  ( flag -- )
  transform-flags @ OR transform-flags ! ;
: transform-flags-clear  ( -- )
  0 transform-flags ! ;
: transform-performed?  ( flag -- f? )
  transform-flags @ AND ;

%00000000001 CONSTANT untransformed-left-envoice
%00000000010 CONSTANT untransformed-left-envoice-missing
%00000000100 CONSTANT untransformed-fallout
%00000001000 CONSTANT untransformed-fallout-CCC
%00000010000 CONSTANT untransformed-fallout-VГV
%00000100000 CONSTANT untransformed-fallout-VVГV
%00001000000 CONSTANT untransformed-fallout-V[кх]V
%00010000000 CONSTANT untransformed-fallout-VңV
%00100000000 CONSTANT untransformed-fallout-confluence
%01000000000 CONSTANT untransformed-fallout-(СА|ТІ)ңАр
%10000000000 CONSTANT harmony-vu-broken


: /[ае]($|[бдркх])/  ( D: s -- f )
  2DUP ~/ [ае]/ IF
    cyr /STRING
    DUP 0= IF 2DROP TRUE EXIT THEN
    ~/ [бдркх]/ EXIT
  ELSE 2DROP FALSE THEN
  ;

: /([ая]а|ее)/  ( D: s -- f )
  cyr > IF
    XC@+ DUP [CHAR] я = IF DROP [CHAR] а THEN >R XC@ R@ = IF  ( R: xc )
      R> DUP [CHAR] а = IF DROP TRUE ELSE [CHAR] е = THEN EXIT
    THEN RDROP
  THEN FALSE ;
0 [IF]
  2DUP S" аа" string-prefix? IF 2DROP TRUE EXIT THEN
  S" ее" string-prefix? IF TRUE EXIT THEN
  FALSE ;
[ENDIF]

: s/(.*).$/г\1/  { str len -- }
  str  str cyr+  len cyr -  MOVE
  [CHAR] г str XC! ;

: s/(.*).$/ғ\1/  { str len -- }
  str  str cyr+  len cyr -  MOVE
  [CHAR] ғ str XC! ;

: s/.(.*).$/$1$2\1/  { str len $1 $2 -- }
  $2 IF
    str  str cyr+  len cyr -  MOVE
  THEN
  $1 str XC!
  $2 IF
    $2 str XCHAR+ XC!
  THEN ;

: s/(.*)$/$1$2\1/  { str len $1 $2 -- }
  str  str 2cyrs +  len 2cyrs -  MOVE
  $1 str XC!
  $2 str XCHAR+ XC! ;

: s/(.*)$/$1\1/  { str len $1 -- }
  str  str cyr+  len cyr -  MOVE
  $1 str XC! ;

: s/..(.*).$/$1$2$3\1/  { str len $1 $2 $3 -- }
  str  str cyr+  len cyr -  MOVE
  $1 str XC!
  $2 str XCHAR+ XC!
  $3 str XCHAR+ XCHAR+ XC! ;


\ 1. Озвончение глухого согласного в интервокальной позиции
\ [действует всегда]. Если основа заканчивается на сочетание
\ «гласный + глухой согласный» (п, ф, т, ш, с, к, х), то при
\ прибавлении аффикса на гласный звук согласный в интервокальной
\ позиции озвончается и переходит в парный звонкий (б, в, д, ж,
\ з, г, ғ). На фонему ч озвончение в интервокале не действует.
\ 2) Корневая конечная т у некоторых основ (в основном
\ глагольных) в литературном языке не озвончается: атар (ат)
\ ‘он будет стрелять’, хатым (хат) ‘моя баба’. У подобных основ
\ допустимы также и диалектные варианты с озвончением.


: transform-envoice-for-vowel-affix  ( cs addr len -- cs addr len )
  \ OVER XC@ vowel? IF  \ moved vowel test to caller
    ROT                               ( addr len cs )
    DUP C@ cyr > IF
      DUP last-sound unvoiced? IF
        DUP last-sound [CHAR] ч <> IF
          DUP prev-sound vowel? IF
            DUP last-sound envoice  ( addr len cs u )
            OVER last-sound-change    ( addr len cs )
            tr-envoice transform-flag-set
    THEN THEN THEN THEN
    -ROT                              ( cs addr len )
  \ THEN
  ;

: transform-envoice-for-consonant-affix  ( cs addr len -- cs addr len )
  DUP 2cyrs >= IF
    OVER XC@ unvoiced? IF
      OVER XC@ [CHAR] ч <> IF
        OVER cyr+ XC@ vowel? IF
          2 PICK last-sound vowel? IF
            OVER XC@ envoice     ( cs addr len u )
            3 PICK last-sound-add  ( cs addr len )
            +X/STRING            ( cs addr' len' )
            tr-envoice transform-flag-set
  THEN THEN THEN THEN THEN ;

: possibly-fugitive?  ( addr u -- f)
  2DUP last-sound [CHAR] н = IF
    prev-sound glide?
  ELSE 2DROP 0 THEN ;

: untransform-left-envoice  { list  D: left-part  D: affix -- list' }
  list left-part affix pairlist-prepend TO list
  left-part last-sound { xc }
  xc unvoiced? IF
    untransformed-left-envoice-missing  list pair-flags !
  ELSE xc voiced? IF
    xc  [CHAR] ӌ <> IF
      list left-part affix pairlist-prepend TO list
      xc unvoice  list pair-1 last-sound-ptr  XC!
      untransformed-left-envoice  list pair-flags !
  THEN THEN THEN
  list ;

: untransform-fugitive  { list  D: left-part  D: affix -- list }
  list left-part affix pairlist-prepend TO list
  list pair-1  last-sound  list pair-1  string-append-char NIP  list pair-1-len !
  [CHAR] у  list pair-1  prev-sound-ptr  XC!

  list  list pair-1  affix  pairlist-prepend TO list
  [CHAR] ы  list pair-1  prev-sound-ptr  XC!

  list  list pair-1  affix  pairlist-prepend TO list
  [CHAR] і  list pair-1  prev-sound-ptr  XC!

  list ;

: untransform-envoice  { D: s D: affix -- pairlist }
  \\." unjoining " affix type ."  from " s type ." ..."
  0 { list }
  s  affix string-length  - { D: left-part }
  affix first-sound { c }
  c vowel? IF
    s affix string-ends IF
      left-part prev-sound vowel? IF
        list left-part affix untransform-left-envoice TO list
      ELSE
        \ check for fugitive
        current-form-is-poss? IF
          affix first-sound  very-narrow-vowel? IF
            left-part possibly-fugitive? IF
              list left-part affix untransform-fugitive TO list
        THEN THEN THEN
        list left-part affix pairlist-prepend TO list
      THEN
    THEN
  ELSE
    c unvoiced?  c [CHAR] ч <>  AND IF
      s affix string-ends IF
        left-part last-sound vowel?  affix string-length cyr >  AND NOT IF
          list left-part affix pairlist-prepend TO list
        THEN
      ELSE
        c envoice  affix string-addr  XC!
        s affix string-ends IF
          left-part last-sound vowel? IF
            list left-part affix pairlist-prepend TO list
        THEN THEN
        c  affix string-addr  XC!
      THEN
    ELSE
      s affix string-ends IF
        list left-part affix pairlist-prepend TO list
      THEN
    THEN
  THEN
  \\." " list IF ." SUCCESS" ELSE ." NO SUCCESS" THEN CR
  list ;

\ 3. Беглые гласные [не всегда]. В многосложных основах узкие
\ гласные (у, ы, i), находящихся между согл. р–н, л–н, й–н),
\ выпадают при присоед. афф. принадл. -ы, -i: пурун/пурны ‘нос’,
\ харын/харны ‘живот’, орын/орны ‘место’, чарын/чарны ‘спина’,
\ мойын/мойны ‘шея’, ирiн/ирнi ‘губа’, килiн/килнi ‘невестка’,
\ чилiн/чилнi ‘грива’; Для i, ы правило действует всегда. Для у
\ факультативно: хулун ‘жеребёнок-сосунок’ (тибе хулуны
\ ‘верблюжонок’).

: transform-fugitive  ( cs addr len -- cs addr len )
  2DUP is-1-char? IF
    OVER XC@ very-narrow-vowel? IF   ( cs addr len )
      ROT                            ( addr len cs )
      \ DUP polysyllabic? IF
      \   DUP last-sound [CHAR] н = IF
      \     DUP prev-sound very-narrow-vowel? IF
      \       DUP third-sound glide? IF
              DUP prev-sound-cut
              tr-fugitive transform-flag-set
      \ THEN THEN THEN THEN
      -ROT                           ( cs addr len )
  THEN THEN ;


: -trans-cut-last-vowel  ( cs addr len -- cs addr len )
  ROT DUP last-sound vowel? IF DUP last-sound-cut THEN -ROT ;


\ 4. Выпадение последнего гласного глагольной основы (любого
\ качества) при присоединении личных афф. императива на -и (-им,
\ -ибыс, -ибiс, -имдах, -имдек, ибыстах, ибiстек) [всегда]:
\ ырла- ‘петь’ + -им > ырлим ‘пусть я спою’; сине- ‘мерить’ +
\ -им > синим ‘пусть я измерю’; ырла- + -ибыс > ырлибыс ‘споём’;
\ сине- + -ибiс > синибiс ‘измерим’.
\ Реализация: выпадают конечные краткие гласные при
\ присоединении аффикса на -и-!

: transform-hiatus-imp  ( cs addr len -- cs addr len )
  OVER XC@ [CHAR] и = IF
    ROT DUP last-sound vowel? IF       ( addr len cs )
      DUP prev-sound vowel? 0= IF
        DUP last-sound-cut
        tr-hiatus-imp transform-flag-set
    THEN THEN -ROT                     ( cs addr len )
  THEN ;


\ 5. Выпадение конечного гласного основы или конечного гласного
\ любого аффикса перед аффиксом -ох, -öк (производным от одно-
\ именной усилительной частицы) [всегда]: любой гласный, будь то
\ гласный основы или аффикса, предшествующий аффиксу -ох, -öк,
\ выпадает: андох ‘там же’ < анды ‘там’ + ох ‘же’.

: transform-hiatus-emph  ( cs addr len -- cs addr len )
  2DUP is-2-char? IF
    OVER XC@ o-yo? IF
      OVER cyr+ XC@ k-kh? IF
        ROT DUP last-sound vowel? IF    ( addr len cs )
          DUP last-sound-cut
          tr-hiatus-emph transform-flag-set
        THEN -ROT                       ( cs addr len )
  THEN THEN THEN ;


\ 6. Стечение согласных основы и аффикса. При стечении
\ последнего согласного основы на -ғ, -г, -ң и любого аффикса на
\ -ғ, -г, -ң, соответствующий согласный аффикса выпадает:
\ суғ+-ға > суғ-а ‘воде’, кöг+-ге > кöг-е ‘песне’.

: transform-confluence  { cs  D: affix -- cs  D: affix' }
  affix ~/ [гғ]/ IF
    cs COUNT ~/ [ғгң]$/ IF
      tr-confluence transform-flag-set
      cs  affix +X/STRING  EXIT         ( cs  D: affix' )
  THEN THEN cs affix ;


\ 2.2. Выпадение звонкого заднеязычного (ғ, г) в интервокальной
\ позиции и стяжение двух обрамлявших его гласных в один долгий
\ [не всегда]. (Ғ, г может принадлежать основе, аффиксу датива
\ (-ға, -ге), афф. прич. прош.вр. -ған, -ген, афф. желательного
\ накл. -ғай, -гей, афф. прич. -ғалах, -гелек и т.п.) Ғ, г может
\ выпадать после краткого гласного и никогда – после долгого.

REQUIRE heredoc.fs
>> fallout-rule-src
аға > аа        ыға > аа        уғы > ии        оғы > оо        еге > ее        ігі > ии        ӧге > ӧӧ
ағы > аа        ығы > ии        ********        оға > оо        еги > ее        іги* > ии       ӧги* > ӧӧ
ағу* > аа       ығу* > ыы*      уға > аа        оғу > оо        егі > ее        іге > ее        ӧгі > ӧӧ
ағо* > аа       ығо* > оо       уғу* > уу       оғо > оо        егӧ* > ее       ігӧ* > ӧӧ       ӧгӧ > ӧӧ
                                уғо* > оо                       егӱ* > ее       ігӱ* > ии       ӧгӱ* > ӧӧ
ӱге > ее    игі > ии
ӱгі > ии    иги > ии
ӱги > ии
ӱгӧ* > ӧӧ
ӱгӱ* > ӱӱ

гг > г
ғғ > ғ
ңг > ң
ңғ > ң
fallout-rule-src
\ уғы > уу

CREATE fallout-repl-uu 2 cyrs ALLOT
S" уу" fallout-repl-uu SWAP MOVE
TABLE CONSTANT fallout-table
TABLE CONSTANT fallout-reverse-table
0  S" уғы" DROP  ptrlist-prepend
   S" уға" DROP  ptrlist-prepend
   S" уғо" DROP  ptrlist-prepend  CONSTANT fallout-reverse-uu

: fallout-reverse-find  ( addr u -- 0 | ptrlist )
  fallout-reverse-table SEARCH-WORDLIST IF EXECUTE
  ELSE 0 THEN ;

: fallout-rule-prepare  ( -- )
  GET-CURRENT
  fallout-rule-src OVER + SWAP BEGIN 2DUP > WHILE   ( end ptr )
    DUP cyr? IF
      fallout-table SET-CURRENT
      DUP >R                                 ( end ptr R: src )
      DUP DUP count-cyrs DUP >R NEXTNAME R> +      ( end ptr' )
      skip-latin
      DUP CONSTANT
      DUP DUP count-cyrs fallout-reverse-find ?DUP-IF  ( end ptr list R: src )
        R> ptrlist-append                           ( end ptr )
      ELSE
        fallout-reverse-table SET-CURRENT
        DUP DUP count-cyrs NEXTNAME
        0 R> ptrlist-prepend CONSTANT
      THEN                                          ( end ptr )
      skip-cyrs
    ELSE 1+ THEN
  REPEAT 2DROP
  SET-CURRENT ;

fallout-rule-prepare

: fallout-rule-find  { ptr -- 0 | ptr' }
  ptr 3cyrs fallout-table SEARCH-WORDLIST IF
    EXECUTE
  ELSE ptr 2cyrs fallout-table SEARCH-WORDLIST IF
    EXECUTE
  ELSE 0 THEN THEN ;

CREATE fallout-buf 3 cyrs ALLOT
: transform-fallout  ( cs addr len -- cs addr len )
  \." transform-fallout: " 2 pick count type ." +"  2dup type ." :"
  DUP 0= IF EXIT THEN  \ do nothing with empty affix
  tr-envoice tr-confluence OR  transform-performed? IF EXIT THEN
  ROT DUP C@ 2cyrs < IF -ROT EXIT THEN { addr len cs }        ( )
  cs COUNT + XCHAR- XCHAR- { start }
  0 { repl }
  start XC@ { xc }
  xc vowel? IF
    xc [CHAR] у = IF cs polysyllabic-cs? 0= ELSE 0 THEN { uu? }
    start fallout-buf 2cyrs MOVE
    addr  fallout-buf 2cyrs +  cyr MOVE
    fallout-buf fallout-rule-find TO repl
    repl uu? AND IF  fallout-repl-uu TO repl  THEN
  THEN
  repl 0= IF
    len 2cyrs < IF cs addr len EXIT THEN
    start cyr+ DUP XC@ vowel? IF                       ( start' )
      TO start                                                ( )
      start  fallout-buf  cyr  MOVE
      addr  fallout-buf cyr+  2cyrs  MOVE
      fallout-buf fallout-rule-find TO repl
      repl IF
        addr len +X/STRING TO len To addr
        cs C@  cyr+  cs C!
      THEN
    ELSE DROP cs addr len EXIT THEN
  THEN
  repl IF
    repl start 2cyrs MOVE
    tr-fallout transform-flag-set
    cs addr len +X/STRING  ( cs addr' len' )
  ELSE cs addr len THEN  ( cs addr' len' )
  ;


GET-CURRENT  ( wid )
VOCABULARY fallout-untransformer ALSO fallout-untransformer DEFINITIONS

0 VALUE ofs-into-affix
0 VALUE list
0 VALUE fallout-flags

: untransform-check-fallout-start  { fallout-start D: s D: affix -- }
  fallout-start  s string-addr < IF
    ." NEGATIVE fallout-ofs " fallout-start s string-addr - . ."  s:" s TYPE ."  affix:" affix TYPE CR BYE
  ELSE fallout-start  s string-addr  -  200 > IF
    ." HUGE fallout-ofs " fallout-start s string-addr - . ."  s:" s TYPE ."  affix:" affix TYPE CR BYE
  THEN THEN ;

: untransform-get-fallout-coord  { D: s  D: affix -- 0 | fallout-start ofs-into-affix TRUE }
  \ if u >= affix.len and affix ~= /[гғ]V/, e.g. суу = су+ға
  s string-length  affix string-length  >= IF
    affix ~/ [гғ][V]/ IF
      \ (end of form - affix.len (c|уу), 1cyr)
      s string-end  affix string-length  -  cyr TRUE EXIT
    THEN
  THEN
  \ if u > affix.len
  s string-length  affix string-length > IF
    affix ~/ [V]/ IF  \ e.g. суғ+ы > суу
      \ (end of form - affix.len - 1 (c|уу), 2)
      s string-end  affix string-length  -  cyr -  2cyrs TRUE EXIT
    THEN
    affix ~/ ң[ае]р/ IF  \ e.g. пас+са+ңар > пассар
      s string-end  2cyrs -  cyr TRUE EXIT
    THEN
    affix ~/ [C]/ IF  \ e.g. финн+нең > финнең
      s string-end  affix string-length -  cyr -  cyr TRUE EXIT
    THEN
  THEN
  FALSE ;

0 VALUE guess-size


: guess-make  { D: s fallout-ofs -- D: guess-fallout }
  s  cyr+ TO guess-size  PAD guess-size MOVE
  PAD guess-size fallout-ofs /STRING ;

: guess-yield  { D: affix -- }
  list  PAD guess-size  affix  pairlist-prepend  TO list
  fallout-flags list pair-flags ! ;

: guess-check  { D: guess-fallout  D: affix -- }
  \\." check:" guess-fallout type ."  affix:" affix type ."  ofs-into:" ofs-into-affix . cr
  guess-fallout ofs-into-affix /STRING  affix STR= IF
    \ unjoin affix
    guess-size  affix string-length - TO guess-size
    \\." YIELD" CR
    affix guess-yield
  THEN ;


: untransform-fallout2-confluence  { D: s  D: affix  fallout-ofs -- }
  \ 2. Выпадение Г после Г, ң без стяжения. При стечении по-
  \ следней согласной основы на -ғ, -г, -ң и любого аффикса с
  \ начальной морфонемой Г первая согласная аффикса (то есть
  \ Г) выпадает, согласная основы остается: суғ + -ға > суғ-а
  \ ‘воде’, эг + -гей > эгей ‘пусть гнет’, тоң + -ған > тоңан
  \ ‘замерз’. На месте стыка аффиксов правило не действует!
  \stack-mark
  untransformed-fallout-confluence TO fallout-flags
  s fallout-ofs /STRING { D: fallout }
  fallout ~/ [ғгң]/ IF
    affix ~/ [гғ]/ IF
      fallout first-sound { c1 }
      s string-addr fallout-ofs last-char-vowel front-vowel? IF [CHAR] г ELSE [CHAR] ғ THEN { c2 }
      s fallout-ofs guess-make { D: guess-fallout }
      guess-fallout c1 c2 s/.(.*).$/$1$2\1/
      guess-fallout affix guess-check
    THEN
  THEN
  untransformed-fallout TO fallout-flags
  \stack-check
  ;

: untransform-fallout-add-vcv  { D: s  D: affix  fallout-ofs v1 c v2 -- }
  s fallout-ofs guess-make { D: guess-fallout }
  guess-fallout v1 c v2 s/..(.*).$/$1$2$3\1/
  guess-fallout affix guess-check
  ;

: untransform-fallout-add-vv  { D: s  D: affix  fallout-ofs v1 v2 -- }
  \\." trying " v1 XEMIT v2 XEMIT CR
  s fallout-ofs guess-make { D: guess-fallout }
  guess-fallout v1 v2 s/.(.*).$/$1$2\1/
  v2 0= IF
    guess-size cyr - TO guess-size
    guess-fallout cyr - TO guess-fallout
  THEN
  \\." got " guess-fallout TYPE CR
  guess-fallout affix guess-check
  ;

: untransform-fallout2-aa/ee { D: s  D: affix  fallout-ofs v1 c vowels }
  vowels sound-each { v2 }
    s affix fallout-ofs v1 c v2 untransform-fallout-add-vcv
    v1 v2 <> IF
      s affix fallout-ofs v2 c v1 untransform-fallout-add-vcv
    THEN
  sound-next
  ;

: untransform-fallout2-vgv-non-first  { D: s  D: affix  fallout-ofs -- }
  \ 3.1 Выпадение морфонемы Г в интервокальной позиции и
  \ стяжение двух обрамлявших ее гласных в одну долгую
  \ [не всегда].
  \ В непервом слоге: выпадение этой согласной происходит в
  \ позиции только после краткой гласной и перед другой
  \ краткой гласной (а, е, ы, i, у, ӱ). Невозможно выпадение
  \ согласной после или перед долгой гласной. На месте
  \ стяжения образуется долгая аа/ее, если хотя бы одна из
  \ гласных была а/е; в иных случаях образуется ии.
  \ Начальная Г у аффиксов (Past ГАн, Cunc ГАлАх, Assum
  \ ГАдАГ, Opt ГАй, Dat ГА) выпадает в интервокальной
  \ позиции всегда.
  \stack-mark
  s string-addr  fallout-ofs cyr+  polysyllabic? IF
    s fallout-ofs /STRING { D: fallout }
    [CHAR] г { c }
    fallout /([ая]а|ее)/ IF
      fallout first-sound { v1 }
      s string-addr fallout-ofs + { fallout-start }
      short-front-vowels { vowels }
      long-front-vowels { long-vowels }
      v1 back-vowel? IF
        [CHAR] ғ TO c
        short-back-vowels TO vowels
        long-back-vowels TO long-vowels
      THEN
      untransformed-fallout-VГV TO fallout-flags
      s affix fallout-ofs v1 c        vowels untransform-fallout2-aa/ee

      untransformed-fallout-VңV TO fallout-flags
      s affix fallout-ofs v1 [CHAR] ң vowels untransform-fallout2-aa/ee

      untransformed-fallout-VVГV TO fallout-flags
      s affix fallout-ofs v1 c        long-vowels untransform-fallout2-aa/ee

      untransformed-fallout-V[кх]V TO fallout-flags
      v1 back-vowel? IF  [CHAR] х  ELSE  [CHAR] к  THEN  TO c
      s affix fallout-ofs v1 c        vowels untransform-fallout2-aa/ee

      untransformed-fallout TO fallout-flags
    ELSE fallout ~/ ии/ IF
      0 0 { v1 v2 }
      s string-addr  fallout-ofs  { D: left-part }
      left-part last-char-vowel back-vowel? IF
        [CHAR] ғ TO c
        [CHAR] ы TO v1  [CHAR] у TO v2
      ELSE
        [CHAR] і TO v1  [CHAR] ӱ TO v2
      THEN
      untransformed-fallout-VГV TO fallout-flags
      s affix fallout-ofs v1 c        v1 untransform-fallout-add-vcv
      s affix fallout-ofs v1 c        v2 untransform-fallout-add-vcv
      s affix fallout-ofs v2 c        v1 untransform-fallout-add-vcv
      s affix fallout-ofs v2 c        v2 untransform-fallout-add-vcv

      untransformed-fallout-VңV TO fallout-flags
      s affix fallout-ofs v1 [CHAR] ң v1 untransform-fallout-add-vcv
      s affix fallout-ofs v1 [CHAR] ң v2 untransform-fallout-add-vcv
      s affix fallout-ofs v2 [CHAR] ң v1 untransform-fallout-add-vcv
      s affix fallout-ofs v2 [CHAR] ң v2 untransform-fallout-add-vcv

      untransformed-fallout-V[кх]V TO fallout-flags
      left-part last-char-vowel back-vowel? IF  [CHAR] х  ELSE  [CHAR] к  THEN TO c
      s affix fallout-ofs v1 c        v1 untransform-fallout-add-vcv
      s affix fallout-ofs v1 c        v2 untransform-fallout-add-vcv
      s affix fallout-ofs v2 c        v1 untransform-fallout-add-vcv
      s affix fallout-ofs v2 c        v2 untransform-fallout-add-vcv

      untransformed-fallout TO fallout-flags
    THEN THEN
  THEN
  \stack-check
  ;

: untransform-fallout2-vgv-first  { D: s  D: affix  fallout-ofs -- }
  \ В односложных основах правило действует и на долгие, и на
  \ краткие гласные основы (в аффиксе во всех случаях действует
  \ только для кратких гласных).
  \stack-mark
  s string-addr  fallout-ofs cyr+  polysyllabic? 0= IF
    s string-addr fallout-ofs + { fallout-start }
    fallout-start vowel-long? IF
      fallout-start XC@ { v1 }
      v1 back-vowel? { back? }
      back? IF [CHAR] ғ ELSE [CHAR] г THEN { c }

      \ Односложные глаголы, оканчивающиеся на
      \ краткую гласную, стягиваются с аффиксами так же, как
      \ многосложные: теен ‘сказал’ < тi ‘сказать’ + ған Past,
      \ чеелек ‘еще не поел’ < чi ‘есть’ + гелек Cunc.
      affix ~/ [гғ]/ IF
        s affix fallout-ofs [CHAR] і c v1 untransform-fallout-add-vcv
      THEN

      \ В прочих случаях на месте стяжения образуется длинная
      \ гласная, идентичная корневой.
      back? IF short-back-vowels ELSE short-front-vowels THEN sound-each { v2 }
        s affix fallout-ofs v1 c        v2 untransform-fallout-add-vcv
        s affix fallout-ofs v1 [CHAR] ң v2 untransform-fallout-add-vcv

        \ 3.3. Выпадение конечной губной -п односложной
        \ глагольной основы при прибавлении афф. Convп -ып
        \ [всегда]: тап ‘находить’ + -ып > таап ‘найдя’, сап
        \ ‘ударять’ + -ып > саап ‘ударяя’, теп ‘толкать’ + -іп >
        \ тееп ‘толкая’.
        v2 [CHAR] ы =  v2 [CHAR] і =  OR  affix ~/ [ыі]п/  AND IF
          s affix fallout-ofs v1 [CHAR] п v2 untransform-fallout-add-vcv
        THEN
      sound-next
    THEN
  THEN
  \stack-check
  ;

: untransform-fallout2-vv-Imp.1  { D: s  D: affix  fallout-ofs -- }
  \stack-mark
  affix ~/ и[мб]/ IF
    s fallout-ofs /STRING { D: fallout }
    fallout ~/ и/ IF
      s last-char-vowel back-vowel? IF back-vowels ELSE front-vowels THEN sound-each { v1 }
        s affix fallout-ofs v1 [CHAR] и untransform-fallout-add-vv
      sound-next
    THEN
  THEN
  \stack-check
  ;

: untransform-fallout2-vv-Imp.1.Incl  { D: s  D: affix  fallout-ofs -- }
  \stack-mark
  affix ~/ [ае][лң]/ IF
    s fallout-ofs /STRING { D: fallout }
    fallout /([ая]а|ее)/ IF
      fallout second-sound { v2 }
      v2 back-vowel? IF back-vowels ELSE front-vowels THEN sound-each { v1 }
        s affix fallout-ofs v1 0 untransform-fallout-add-vv
      sound-next
    THEN
  THEN
  \stack-check
  ;

: untransform-fallout2-VА>и  { D: s  D: affix  fallout-ofs -- }
  \stack-mark
  affix /[ае]($|[бдркх])/ IF
    s fallout-ofs /STRING { D: fallout }
    fallout ~/ и/ IF
      [CHAR] е  front-vowels  { v2 vowels }
      s last-char-vowel back-vowel? IF
        [CHAR] а TO v2  back-vowels TO vowels
      THEN
      vowels sound-each { v1 }
        s affix fallout-ofs v1 v2 untransform-fallout-add-vv
      sound-next
    THEN
  THEN
  \stack-check
  ;

: untransform-fallout2-OK  { D: s  D: affix  fallout-ofs -- }
  \stack-mark
  affix ~/ [оӧ][хк]/ IF
    s fallout-ofs /STRING { D: fallout }
    s string-addr  fallout-ofs cyr+  { D: s[:fallout+1] }
    s[:fallout+1] polysyllabic? IF
      s string-addr fallout-ofs + cyr - XC@ consonant? IF
        short-unrounded-front-vowels { vowels }
        fallout first-sound { v2 }
        v2 back-vowel? IF short-unrounded-back-vowels TO vowels THEN
        vowels sound-each { v1 }
          s affix fallout-ofs v1 v2 untransform-fallout-add-vv
        sound-next
      THEN
    THEN
  THEN
  \stack-check
  ;

: untransform-fallout-add-vc  { D: s  D: affix  fallout-ofs v c -- }
  s fallout-ofs guess-make cyr+ { D: guess-fallout }
  guess-size cyr+ TO guess-size
  guess-fallout v c s/(.*)$/$1$2\1/
  guess-fallout affix guess-check
  ;

: untransform-fallout-add-c  { D: s  D: affix  fallout-ofs c -- }
  s fallout-ofs guess-make cyr /STRING { D: guess-fallout }
  guess-fallout c s/(.*)$/$1\1/
  guess-fallout affix guess-check
  ;

: untransform-fallout-(СА|ТІ)ңАр  { D: s  D: affix  fallout-ofs -- }
  \stack-mark
  affix ~/ ң[ае]р/ IF
    affix cyr /STRING  { D: affix[1:] }
    s fallout-ofs /STRING  { D: fallout }
    fallout affix[1:] STR= IF
      fallout first-sound { v }
      s string-addr fallout-ofs { D: left-part }
      left-part last-sound { c }
      c v = { long? }
      long? IF
        fallout-ofs cyr - TO fallout-ofs
        left-part prev-sound TO c
      THEN
      c [CHAR] т = IF
        v front-vowel? IF [CHAR] і ELSE [CHAR] ы THEN
      ELSE v THEN { v1 }
      c [CHAR] с =  c [CHAR] з =  c [CHAR] т =  OR OR IF
        s affix fallout-ofs v1 [CHAR] ң
        long? IF
          v untransform-fallout-add-vcv
        ELSE untransform-fallout-add-vc THEN
      THEN
    THEN
  THEN
  \stack-check
  ;

: untransform-fallout-CCC  { D: s  D: affix  fallout-ofs -- }
  \stack-mark
  affix first-sound { c }
  s fallout-ofs /STRING  { D: fallout }
  fallout first-sound  c = IF
    s affix fallout-ofs c untransform-fallout-add-c
  THEN
  \stack-check
  ;


( wid ) SET-CURRENT  \ public words follow


: untransform-fallout2  { D: s  D: affix -- pairlist }
  \stack-mark
  \\." untransform-fallout2: " s TYPE ." +" affix TYPE \.s CR
  0 TO list
  untransformed-fallout TO fallout-flags
  \\." coords? " s TYPE ." +" affix TYPE \.s CR
  s affix untransform-get-fallout-coord IF TO ofs-into-affix { fallout-start }
    fallout-start s affix untransform-check-fallout-start
    fallout-start  s string-addr  -  { fallout-ofs }

    \\." confluence? " s TYPE ." +" affix TYPE ."  fallout-ofs:" fallout-ofs . ."  ofs-into:" ofs-into-affix . \.s CR
    s affix fallout-ofs untransform-fallout2-confluence

    \ 3. Выпадения со стяжением гласных в интервокале
    \ 3.2 Выпадение сонанта ң в интервокальной позиции и
    \ последующее стяжение двух обрамлявших его гласных в одну
    \ долгую [не всегда]. Правила выпадения такия же, как у
    \ морфонемы Г. В аффиксах бывает конечная ң, которая может
    \ попасть в интервокал: Hab ЧАң и 2pos.sg (I)ӊ. Правило
    \ действует на Hab и не действует на посессив
    \ 3.4. Выпадение конечных заднеязычных глухих в основах на
    \ -х, -к. Это происходит в многосложных (!) основах в
    \ интервокальной позиции и сопровождается последующим
    \ стяжением двух обрамлявших его гласных в одну долгую (см.
    \ правило 2.2) [не всегда].
    \\." vgv-non-first? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
    s affix fallout-ofs untransform-fallout2-vgv-non-first
    \\." vgv-first? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
    s affix fallout-ofs untransform-fallout2-vgv-first

    ofs-into-affix 2cyrs = IF  \ that is, affix starts with a vowel
      ofs-into-affix cyr - TO ofs-into-affix

      \ II.2. императив инклюзивный. Аффиксы инклюзивного
      \ императива Imp.1.Incl Аң, Imp.1.Incl.Pl АңАр/Алар при
      \ присодинении к основам на гласную поглощают гласную
      \ основы, на месте стяжения образуется долгая аа/ее
      \\." vv-Imp.1.Incl? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
      s affix fallout-ofs untransform-fallout2-vv-Imp.1.Incl

      fallout-ofs cyr+ TO fallout-ofs

      \ II.1. императив 1 числа: В глаголе поглощение первой
      \ гласной аффикса императива предыдущей любой гласной
      \ основы (другими словами: выпадение любой последней гласной
      \ основы при присоединении личных афф. императива 1 лица
      \ Imp.1 на -и (-им, -ибыс, -ибiс)) [всегда]:
      \\." vv-Imp.1? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
      s affix fallout-ofs untransform-fallout2-vv-Imp.1

      \ II.3. А с аллофоном и: В глаголах правила слияния с
      \ фонетическими преобразованиями для афф. Fut -Ар, Convа
      \ -А, Convпас A.бАс (диал.), Prosp АК, Iter АдIр. Эти
      \ аффиксы не имеют вариантов, начинающихся на согласную.
      \ При присоединении их к основе на гласную происходит
      \ стяжение двух кратких гласных в нейтральную и без
      \ долготы [всегда]
      \\." VА>и? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
      s affix fallout-ofs untransform-fallout2-VА>и

      \ II.5. поглощение гласных перед -ох: Выпадение конечной
      \ краткой гласной основы или аффикса перед аффиксом -ох,
      \ -öк (производным от одноименной усилительной частицы)
      \ [всегда]: краткая неогубленная гласная не первого слога,
      \ будь то гласная основы или аффикса, предшествующая
      \ аффиксу -ох, -öк, выпадает: турох < тура ‘дом’ + ох;
      \ андох ‘там же’ < анда ‘там’ + ох ‘же’. С долгой гласной
      \ этого не происходит: мағаа ‘Dat от мин’ + ох > мағааох.
      \ В первом слоге гласная любой длины не стягивается: пу
      \ ‘этот’ +ох > пуох 'этот же'.
      \\." OK? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
      s affix fallout-ofs untransform-fallout2-OK
    ELSE  \ that is, affix starts with a consonant
      \ II.4. стяжение 2pl: Выпадение сонанта ң из личного афф. 2pl
      \ ңАр в интервокальной позиции после Cond СА и RPast ТI
      \ [не всегда]. При выпадении ң гласная аффикса также
      \ выпадает: пас-са-ңар > пас-сар ‘если будем писать’,
      \ пас-па-за-ңар > пас-па-зар ‘если не будем писать’,
      \ пас-ты-ңар > пастар 'вы недавно написали'. В лит. языке
      \ допускаются как стяженный, так и полный (без выпадений)
      \ варианты данных форм. У Чебодаевой есть отражение
      \ долготы на письме: пассаар, пастаар.
      \\." (СА|ТІ)ңАр? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
      untransformed-fallout-(СА|ТІ)ңАр TO fallout-flags
      s affix fallout-ofs untransform-fallout-(СА|ТІ)ңАр

      \ I.4. Упрощение группы из трех согласных.
      \ Если русское заимствование заканчивается на двойную
      \ согласную (финн, класс, ватт) и после нее идет аффикс,
      \ начинающийся с той же согласной, на стыке пишется не
      \ три согласных, а две: финнең, классар, ватты.
      \\." CCC? " s TYPE ." +" affix TYPE ." |" list .pairlist \.s CR
      untransformed-fallout-CCC TO fallout-flags
      s affix fallout-ofs untransform-fallout-CCC
    THEN

    \\." " list IF ." final list: " list .pairlist \.s CR THEN
  THEN
  \stack-check
  list
  ;

PREVIOUS

2VARIABLE trans-timer
: transform2  ( warp -- warp )
  \ utime 2>R
  { warp }
  transform-flags-clear
  warp warp-result                          ( result )
  warp warp-cached-result-seg         ( result affix )
  warp warp-segs# @  warp warp-result-segs# @  ?DO
    warp warp-seg-len  I + C@     ( result affix len )
    OVER XC@ vowel? IF
      transform-envoice-for-vowel-affix
      transform-hiatus-emph
      transform-hiatus-imp
      warp warp-fugitive? IF
        transform-fugitive
      THEN
    ELSE
      transform-envoice-for-consonant-affix
      transform-confluence
    THEN
    transform-fallout
    \ 2DUP TYPE CR
    2DUP + >R  cs+  warp warp-result R>  ( result affix )
  LOOP 2DROP warp  ( warp )
  warp warp-segs# @  warp warp-result-segs# !
  \ utime 2R> D- trans-timer 2@ D+ trans-timer 2!
  ;
