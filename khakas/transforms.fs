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

: c/[ае]/  ( xc -- f )
  DUP [CHAR] а = SWAP [CHAR] е = OR ;

: c/[лң]/  ( xc -- f )
  DUP [CHAR] л = SWAP [CHAR] ң = OR ;

: c/[оӧ]/  ( xc -- f )
  DUP [CHAR] о = SWAP [CHAR] ӧ = OR ;

: c/[бркх]/  ( xc -- f )
  CASE
    [CHAR] б OF TRUE ENDOF
    [CHAR] р OF TRUE ENDOF
    [CHAR] к OF TRUE ENDOF
    [CHAR] х OF TRUE ENDOF
    FALSE SWAP
  ENDCASE ;

: c/[хк]/  ( xc -- f )
  DUP [CHAR] х = SWAP [CHAR] к = OR ;

: /[ае][лң]/  ( D: s -- f )
  cyr > IF
    XC@+ c/[ае]/ IF
      XC@ c/[лң]/ EXIT
    THEN
  THEN DROP FALSE ;

: /[оӧ][хк]/  ( D: s -- f )
  cyr > IF
    XC@+ c/[оӧ]/ IF
      XC@ c/[хк]/ EXIT
    THEN
  THEN DROP FALSE ;

: /[ае]($|[бркх])/  ( D: s -- f )
  DUP cyr > IF
    DROP XC@+ c/[ае]/ IF
      XC@ c/[бркх]/ EXIT
    THEN
  ELSE
    cyr = IF XC@ c/[ае]/ EXIT THEN
  THEN DROP FALSE
  ;

: /(аа|ее)/  ( D: s -- f )
  cyr > IF
    XC@+ >R XC@ R@ = IF  ( R: xc )
      R> c/[ае]/ EXIT
    THEN RDROP
  THEN FALSE ;

: /и/  ( D: s -- f )
  cyr >= IF
    XC@ [CHAR] и = EXIT
  THEN DROP FALSE ;

: /ии/  ( D: s -- f )
  cyr > IF
    XC@+ [CHAR] и = IF
      XC@ [CHAR] и = EXIT
    THEN
  THEN DROP FALSE ;

: /и[мб]/  ( D: s -- f )
  cyr > IF XC@+ ( addr' xc )
    [CHAR] и = IF XC@ DUP [CHAR] м = SWAP [CHAR] б = OR EXIT THEN
  THEN DROP FALSE ;

: /г/  ( D: s -- f )
  IF XC@ [CHAR] г = ELSE DROP FALSE THEN ;

: /[гғ]/  ( D: s -- f )
  IF XC@ CASE
    [CHAR] г OF TRUE EXIT ENDOF
    [CHAR] ғ OF TRUE EXIT ENDOF
  ENDCASE ELSE DROP THEN FALSE ;

: /[ғгң]/  ( D: s -- f )
  IF XC@ gh-g-ng? ELSE DROP FALSE THEN ;

: /[ғгң]$/  ( D: s -- f )
  X\STRING- /[ғгң]/ ;

: /[ыі]п/  ( D: s -- f )
  2cyrs >= IF XC@+ ( addr' xc )
    DUP [CHAR] ы = SWAP [CHAR] і = OR IF ( addr' )
      XC@ [CHAR] п = EXIT
    THEN
  THEN DROP FALSE ;

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
\ з, г, ғ).

: transform-envoice-for-vowel-affix  ( cs addr len -- cs addr len )
  \ OVER XC@ vowel? IF  \ moved vowel test to caller
    ROT                             ( addr len cs )
    DUP C@ cyr > IF
      DUP last-sound unvoiced? IF
	DUP prev-sound vowel? IF
	  DUP last-sound envoice  ( addr len cs u )
	  OVER last-sound-change    ( addr len cs )
          tr-envoice transform-flag-set
    THEN THEN THEN
    -ROT                            ( cs addr len )
  \ THEN
  ;

: transform-envoice-for-consonant-affix  ( cs addr len -- cs addr len )
  DUP 2cyrs >= IF
    OVER XC@ unvoiced? IF
      OVER cyr+ XC@ vowel? IF
        2 PICK last-sound vowel? IF
          OVER XC@ envoice     ( cs addr len u )
          3 PICK last-sound-add  ( cs addr len )
          +X/STRING            ( cs addr' len' )
          tr-envoice transform-flag-set
  THEN THEN THEN THEN ;

: possibly-fugitive?  ( addr u -- f)
  2DUP last-sound [CHAR] н = IF
    prev-sound glide?
  ELSE 2DROP 0 THEN ;

: untransform-envoice  ( addr u affix len -- pairlist )
  \\." unjoining " 2dup type
  2DUP { affix-ptr len  D: affix }               ( addr u )
  \\."  from " 2dup type ." ..."
  0 { list }
  affix-ptr XC@                             ( addr u aff1 )
  DUP vowel? IF
    DROP                                         ( addr u )
    \ \." checking if " 2dup type ."  ends in " affix type cr
    2DUP affix string-ends IF
      \ \." yes it does\n"
      len -                                     ( addr u' )
      \ \." checking if " 2dup type ." [-2] is vowel" cr
      2DUP prev-sound vowel? IF
        \ \." yes" cr
        2DUP last-sound { xc }
        xc unvoiced? IF
          2DROP                                         ( )
        ELSE
          2DUP list -ROT affix pairlist-prepend TO list
          xc voiced? IF
            list -ROT affix pairlist-prepend TO list    ( )
            xc unvoice  list pair-1 last-sound-ptr  XC!
          ELSE 2DROP THEN                               ( )
        THEN                                            ( )
      ELSE
        \ \." no" cr
        \ check for fugitive
        current-form-is-poss? IF
          affix-ptr XC@  very-narrow-vowel? IF
            2DUP possibly-fugitive? IF
              2DUP list -ROT affix pairlist-prepend TO list
              list pair-1 last-sound-ptr >R  ( addr u'  R: endptr )
              R@ XC@  R@ cyr+  XC!
              cyr  list pair-1-len +!
              [CHAR] у  R> XC!                  ( addr u' )
              list list pair-1 affix pairlist-prepend TO list
              [CHAR] ы  list pair-1 prev-sound-ptr  XC!
              list list pair-1 affix pairlist-prepend TO list
              [CHAR] і  list pair-1 prev-sound-ptr  XC!
        THEN THEN THEN
        list -ROT affix pairlist-prepend TO list        ( )
      THEN                                              ( )
    ELSE
      \ \." no it does not" cr
      2DROP
    THEN                                                ( )
  ELSE                                      ( addr u aff1 )
    { xc }                                       ( addr u )
    xc unvoiced? IF
      2DUP affix string-ends IF
        len -
        2DUP last-sound vowel?  len cyr > AND IF
          2DROP                                         ( )
        ELSE
          list -ROT affix pairlist-prepend TO list      ( )
        THEN
      ELSE
        xc envoice affix-ptr XC!
        2DUP affix string-ends IF
          len -
          2DUP last-sound vowel? IF
            list -ROT affix pairlist-prepend TO list     ( )
          ELSE 2DROP THEN                                ( )
        ELSE 2DROP THEN                                  ( )
        xc affix-ptr XC!
      THEN
    ELSE
      2DUP affix string-ends IF
        len -
        list -ROT affix pairlist-prepend TO list         ( )
      ELSE 2DROP THEN                                    ( )
    THEN
  THEN
  \\." " list IF ." SUCCESS" cr THEN
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
  affix /[гғ]/ IF
    cs COUNT /[ғгң]$/ IF
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

: g/gh+vowel?  ( addr u -- f )
  2cyrs >= IF                                          ( addr )
    DUP XC@  DUP [CHAR] г =  SWAP [CHAR] ғ = OR  ( addr g/gh? )
    SWAP cyr+ XC@ vowel? AND                              ( f )
  ELSE DROP FALSE THEN ;

: check-G-affix-possible  ( D: s  D: affix -- f )
  { _ s-len affix-ptr affix-len }
  s-len affix-len >= IF
    affix-ptr affix-len g/gh+vowel?
  ELSE FALSE THEN ;

: check-V-affix-possible  ( D: s  D: affix -- f )
  { _ s-len affix-ptr affix-len }
  s-len affix-len > IF
    affix-ptr XC@ vowel?
  ELSE FALSE THEN ;

: untransform-check-fallout-start  { fallout-start D: s D: affix -- }
  fallout-start  s string-addr < IF
    ." NEGATIVE fallout-ofs " fallout-start s string-addr - . ."  s:" s TYPE ."  affix:" affix TYPE CR BYE
  ELSE fallout-start  s string-addr  -  200 > IF
    ." HUGE fallout-ofs " fallout-start s string-addr - . ."  s:" s TYPE ."  affix:" affix TYPE CR BYE
  THEN THEN ;

: untransform-get-fallout-coord  { D: s  D: affix -- 0 | fallout-start ofs-into-affix TRUE }
  \ if u >= affix.len and affix ~= /[гғ]V/, e.g. суу = су+ға
  s affix check-G-affix-possible IF
    \ (end of form - affix.len (c|уу), 1cyr)
    s string-end  affix string-length  -  cyr TRUE EXIT
  THEN
  \ if u > affix.len and affix ~= /V/, e.g. суғ+ы > суу
  s affix check-V-affix-possible IF
    \ (end of form - affix.len - 1 (c|уу), 2)
    s string-end  affix string-length  -  cyr -  2cyrs TRUE EXIT
  THEN  FALSE ;

( wid ) SET-CURRENT  \ public words follow

: strlist-get-buffer  ( list  D: src -- D: buf  list' )
  cyr+ strlist-prepend-alloc >R R@ strlist-get R> ;

: untransform-fallout2-add  { D: s  fallout-ofs -- D: cs-fallout }
  list s strlist-get-buffer TO list  ( D: buffer )
  fallout-ofs /STRING
  ;

: untransform-fallout2-check  { D: cs-fallout  D: affix  -- }
  cs-fallout ofs-into-affix /STRING  affix  COMPARE IF
    list list-swallow TO list
  THEN
  ;

: untransform-fallout2-confluence  { D: s  D: affix  fallout-ofs -- }
  \ 2. Выпадение Г после Г, ң без стяжения. При стечении по-
  \ следней согласной основы на -ғ, -г, -ң и любого аффикса с
  \ начальной морфонемой Г первая согласная аффикса (то есть
  \ Г) выпадает, согласная основы остается: суғ + -ға > суғ-а
  \ ‘воде’, эг + -гей > эгей ‘пусть гнет’, тоң + -ған > тоңан
  \ ‘замерз’.
  s fallout-ofs /STRING  /[ғгң]/ IF
    affix /[гғ]/ IF
      s string-addr fallout-ofs + XC@ { stem-consonant }
      stem-consonant [CHAR] г = IF [CHAR] г ELSE [CHAR] ғ THEN { lost-consonant }
      s fallout-ofs untransform-fallout2-add  { D: cs-fallout }
      cs-fallout stem-consonant lost-consonant s/.(.*).$/$1$2\1/
      cs-fallout affix untransform-fallout2-check
    THEN
  THEN ;

: untransform-fallout-add-vcv  { D: s  D: affix  fallout-ofs vowel1 consonant vowel2 -- }
  s fallout-ofs untransform-fallout2-add { D: cs-fallout }
  cs-fallout vowel1 consonant vowel2 s/..(.*).$/$1$2$3\1/
  cs-fallout affix untransform-fallout2-check
  ;

: untransform-fallout-add-vv  { D: s  D: affix  fallout-ofs vowel1 vowel2 -- }
  \\." trying " vowel1 XEMIT vowel2 XEMIT CR
  s fallout-ofs untransform-fallout2-add { D: cs-fallout }
  cs-fallout vowel1 vowel2 s/.(.*).$/$1$2\1/
  vowel2 0= IF
    list strlist-str DUP C@ cyr - SWAP C!
    cs-fallout cyr - TO cs-fallout
  THEN
  \\." got " cs-fallout TYPE CR
  cs-fallout affix untransform-fallout2-check
  ;

: untransform-fallout2-aa/ee { D: s  D: affix  fallout-ofs vowel1 consonant vowels }
  vowels sound-each { vowel2 }
    s affix fallout-ofs vowel1 consonant vowel2 untransform-fallout-add-vcv
    vowel1 vowel2 <> IF
      s affix fallout-ofs vowel2 consonant vowel1 untransform-fallout-add-vcv
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
  s string-addr  fallout-ofs cyr+  polysyllabic? IF
    s fallout-ofs /STRING { D: fallout }
    [CHAR] г { consonant }
    fallout /(аа|ее)/ IF
      s string-addr fallout-ofs + { fallout-start }
      fallout-start XC@ { vowel1 }
      short-front-vowels { vowels }
      vowel1 [CHAR] а = IF
        [CHAR] ғ TO consonant
        short-back-vowels TO vowels
      THEN
      s affix fallout-ofs vowel1 consonant vowels untransform-fallout2-aa/ee
      s affix fallout-ofs vowel1 [CHAR] ң  vowels untransform-fallout2-aa/ee
    ELSE fallout /ии/ IF
      0 0 { vowel1 vowel2 }
      s string-addr  fallout-ofs  last-char-vowel back-vowel? IF
        [CHAR] ғ TO consonant
        [CHAR] ы TO vowel1  [CHAR] у TO vowel2
      ELSE
        [CHAR] і TO vowel1  [CHAR] ӱ TO vowel2
      THEN
      s affix fallout-ofs vowel1 consonant vowel1 untransform-fallout-add-vcv
      s affix fallout-ofs vowel1 consonant vowel2 untransform-fallout-add-vcv
      s affix fallout-ofs vowel2 consonant vowel1 untransform-fallout-add-vcv
      s affix fallout-ofs vowel2 consonant vowel2 untransform-fallout-add-vcv
      s affix fallout-ofs vowel1 [CHAR] ң vowel1 untransform-fallout-add-vcv
      s affix fallout-ofs vowel1 [CHAR] ң vowel2 untransform-fallout-add-vcv
      s affix fallout-ofs vowel2 [CHAR] ң vowel1 untransform-fallout-add-vcv
      s affix fallout-ofs vowel2 [CHAR] ң vowel2 untransform-fallout-add-vcv
    THEN THEN
  THEN ;

: untransform-fallout2-vgv-first  { D: s  D: affix  fallout-ofs -- }
  \ В односложных основах правило действует и на долгие, и на
  \ краткие гласные. На месте стяжения образуется длинная
  \ гласная, идентичная корневой.
  s string-addr  fallout-ofs cyr+  polysyllabic? 0= IF
    s string-addr fallout-ofs + { fallout-start }
    fallout-start vowel-long? IF
      fallout-start XC@ { vowel1 }
      vowel1 back-vowel? IF
           back-vowels   [CHAR] ғ
      ELSE front-vowels  [CHAR] г  THEN { consonant } sound-each { vowel2 }
        s affix fallout-ofs vowel1 consonant vowel2 untransform-fallout-add-vcv
        s affix fallout-ofs vowel1 [CHAR] ң vowel2 untransform-fallout-add-vcv

        \ 3.3. Выпадение конечной губной -п односложной
        \ глагольной основы при прибавлении афф. Convп -ып
        \ [всегда]: тап ‘находить’ + -ып > таап ‘найдя’, сап
        \ ‘ударять’ + -ып > саап ‘ударяя’, теп ‘толкать’ + -іп >
        \ тееп ‘толкая’.
        vowel2 [CHAR] ы =  vowel2 [CHAR] і =  OR  affix /[ыі]п/  AND IF
          s affix fallout-ofs vowel1 [CHAR] п vowel2 untransform-fallout-add-vcv
        THEN
      sound-next
    THEN
  THEN ;

: untransform-fallout2-vv-Imp.1  { D: s  D: affix  fallout-ofs -- }
  affix /и[мб]/ IF
    s string-addr fallout-ofs + { fallout-start }
    fallout-start XC@ [CHAR] и = IF
      s last-char-vowel back-vowel? IF back-vowels ELSE front-vowels THEN sound-each { vowel1 }
        s affix fallout-ofs vowel1 [CHAR] и untransform-fallout-add-vv
      sound-next
    THEN
  THEN
  ;

: untransform-fallout2-vv-Imp.1.Incl  { D: s  D: affix  fallout-ofs -- }
  affix /[ае][лң]/ IF
    s fallout-ofs /STRING { D: fallout }
    fallout /(аа|ее)/ IF
      fallout string-addr XC@ { vowel2 }
      vowel2 back-vowel? IF back-vowels ELSE front-vowels THEN sound-each { vowel1 }
        s affix fallout-ofs vowel1 0 untransform-fallout-add-vv
      sound-next
    THEN
  THEN ;

: untransform-fallout2-VА>и  { D: s  D: affix  fallout-ofs -- }
  affix /[ае]($|[бркх])/ IF
    s fallout-ofs /STRING { D: fallout }
    fallout /и/ IF
      [CHAR] е  front-vowels  { vowel2 vowels }
      s last-char-vowel back-vowel? IF
        [CHAR] а TO vowel2  back-vowels TO vowels
      THEN
      vowels sound-each { vowel1 }
        s affix fallout-ofs vowel1 vowel2 untransform-fallout-add-vv
      sound-next
    THEN
  THEN
  ;

: untransform-fallout2-OK  { D: s  D: affix  fallout-ofs -- }
  affix /[оӧ][хк]/ IF
    s string-addr  fallout-ofs cyr+  { D: s[:fallout+1] }
    s[:fallout+1] polysyllabic? IF
      s string-addr fallout-ofs + cyr - XC@ consonant? IF
        short-unrounded-front-vowels { vowels }
        s fallout-ofs /STRING { D: fallout }
        fallout string-addr XC@ { vowel2 }
        vowel2 back-vowel? IF short-unrounded-back-vowels TO vowels THEN
        vowels sound-each { vowel1 }
          s affix fallout-ofs vowel1 vowel2 untransform-fallout-add-vv
        sound-next
      THEN
    THEN
  THEN
  ;
: untransform-fallout2  { D: s  D: affix -- strlist }
  \stack-mark
  \\." here? " s TYPE ." +" affix TYPE .s CR
  0 s strlist-prepend-alloc TO list  \ add original
  \\." coords? " s TYPE ." +" affix TYPE .s CR
  s affix untransform-get-fallout-coord IF TO ofs-into-affix { fallout-start }
    fallout-start s affix untransform-check-fallout-start
    fallout-start  s string-addr  -  { fallout-ofs }

    \\." confluence? " s TYPE ." +" affix TYPE ."  fallout-ofs:" fallout-ofs . ."  ofs-into:" ofs-into-affix . .s CR
    s affix fallout-ofs untransform-fallout2-confluence

    \ 3. Выпадения со стяжением гласных в интервокале
    \ 3.2 Выпадение сонанта ң в интервокальной позиции и
    \ последующее стяжение двух обрамлявших его гласных в одну
    \ долгую [не всегда]. Правила выпадения такия же, как у
    \ морфонемы Г. В аффиксах бывает конечная ң, которая может
    \ попасть в интервокал: Hab ЧАң и 2pos.sg (I)ӊ. Правило
    \ действует на Hab и не действует на посессив
    \\." vgv-non-first? " s TYPE ." +" affix TYPE .s CR
    s affix fallout-ofs untransform-fallout2-vgv-non-first
    \\." vgv-first? " s TYPE ." +" affix TYPE .s CR
    s affix fallout-ofs untransform-fallout2-vgv-first

    ofs-into-affix 2cyrs = IF  \ that is, affix starts with a vowel
      ofs-into-affix cyr - TO ofs-into-affix

      \ II.2. императив инклюзивный. Аффиксы инклюзивного
      \ императива Imp.1.Incl Аң, Imp.1.Incl.Pl АңАр/Алар при
      \ присодинении к основам на гласную поглощают гласную
      \ основы, на месте стяжения образуется долгая аа/ее
      \\." vv-Imp.1.Incl? " s TYPE ." +" affix TYPE .s CR
      s affix fallout-ofs untransform-fallout2-vv-Imp.1.Incl

      fallout-ofs cyr+ TO fallout-ofs

      \ II.1. императив 1 числа: В глаголе поглощение первой
      \ гласной аффикса императива предыдущей любой гласной
      \ основы (другими словами: выпадение любой последней гласной
      \ основы при присоединении личных афф. императива 1 лица
      \ Imp.1 на -и (-им, -ибыс, -ибiс)) [всегда]:
      \\." vv-Imp.1? " s TYPE ." +" affix TYPE .s CR
      s affix fallout-ofs untransform-fallout2-vv-Imp.1

      \ II.3. А с аллофоном и: В глаголах правила слияния с
      \ фонетическими преобразованиями для афф. Fut -Ар, Convа
      \ -А, Convпас A.бАс (диал.), Prosp АК. Эти аффиксы не
      \ имеют вариантов, начинающихся на согласную. При
      \ присоединении их к основе на гласную происходит стяжение
      \ двух кратких гласных в нейтральную и без долготы
      \ [всегда]
      \\." VА>и? " s TYPE ." +" affix TYPE .s CR
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
      \\." OK? " s TYPE ." +" affix TYPE .s CR
      s affix fallout-ofs untransform-fallout2-OK
    THEN

    \\." final list: " list .strlist .s CR
  THEN
  \stack-check
  list ;

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
