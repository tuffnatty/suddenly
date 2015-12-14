REQUIRE rules.fs
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
\ последнего согласного основы на -ғ, -г, -ӊ и любого аффикса на
\ -ғ, -г, -ӊ, соответствующий согласный аффикса выпадает:
\ суғ+-ға > суғ-а ‘воде’, кöг+-ге > кöг-е ‘песне’.

: transform-confluence  ( cs addr len -- cs addr len )
  OVER XC@ gh-g-ng? IF
    2 PICK last-sound gh-g-ng? IF
      +X/STRING                      ( cs addr' len' )
      tr-confluence transform-flag-set
  THEN THEN ;


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
fallout-rule-src
\ уғы > уу

CREATE fallout-repl-uu 2 cyrs ALLOT
S" уу" fallout-repl-uu SWAP MOVE
TABLE CONSTANT fallout-table
TABLE CONSTANT fallout-reverse-table
0  S" уғы" DROP  ptrlist-prepend
   S" уға" DROP  ptrlist-prepend
   S" уғо" DROP  ptrlist-prepend  CONSTANT fallout-reverse-uu

: fallout-reverse-find  ( ptr -- 0 | ptrlist )
  2cyrs fallout-reverse-table SEARCH-WORDLIST IF EXECUTE
  ELSE 0 THEN ;

: fallout-rule-prepare  ( -- )
  GET-CURRENT
  fallout-rule-src OVER + SWAP BEGIN 2DUP > WHILE   ( end ptr )
    DUP C@ 128 AND IF
      fallout-table SET-CURRENT
      DUP >R                                 ( end ptr R: src )
      DUP 3cyrs NEXTNAME
      3cyrs + BEGIN DUP C@ 128 AND 0= WHILE 1+ REPEAT
      DUP CONSTANT
      DUP fallout-reverse-find ?DUP-IF  ( end ptr list R: src )
        R> ptrlist-append                           ( end ptr )
      ELSE
        fallout-reverse-table SET-CURRENT
        DUP 2cyrs NEXTNAME
        0 R> ptrlist-prepend CONSTANT
      THEN                                          ( end ptr )
      2cyrs +
    ELSE 1+ THEN
  REPEAT 2DROP
  SET-CURRENT ;

fallout-rule-prepare

: fallout-rule-find  ( ptr -- 0 | ptr' )
  3cyrs fallout-table SEARCH-WORDLIST IF EXECUTE
  ELSE 0 THEN ;

CREATE fallout-buf 3 cyrs ALLOT
: transform-fallout  ( cs addr len -- cs addr len )
  \." transform-fallout: " 2 pick count type ." +"  2dup type ." :"
  DUP 0= IF EXIT THEN  \ do nothing with empty affix
  tr-envoice tr-confluence OR  transform-performed? IF EXIT THEN
  ROT DUP C@ 2cyrs < IF -ROT EXIT THEN            ( addr len cs )
  DUP COUNT + XCHAR- XCHAR- { start }
  start XC@  DUP vowel? IF                     ( cs addr len xc )
    [CHAR] у =  IF                                ( cs addr len )
      DUP polysyllabic? 0=  ELSE  0  THEN { uu? }
    start fallout-buf 2cyrs MOVE
    ROT DUP fallout-buf 2cyrs + cyr MOVE ROT      ( cs addr len )
    fallout-buf fallout-rule-find DUP IF     ( cs addr len repl )
      uu? IF DROP fallout-repl-uu THEN
    THEN
  ELSE DROP -ROT 0 ENDIF 
  DUP 0= IF                                     ( cs addr len 0 )
    DROP DUP 2cyrs < IF EXIT THEN                 ( cs addr len )
    start cyr+ DUP XC@ vowel? IF           ( cs addr len start' )
      TO start                                    ( cs addr len )
      start fallout-buf cyr MOVE
      OVER fallout-buf cyr+ 2cyrs MOVE
      fallout-buf fallout-rule-find DUP IF   ( cs addr len repl )
        >R +X/STRING R>                    ( cs addr' len' repl )
        3 PICK DUP C@ cyr+ SWAP C!
      THEN
    ELSE DROP EXIT THEN
  THEN
  ?DUP-IF                                    ( cs addr len repl )
    start 2cyrs MOVE +X/STRING                  ( cs addr' len' )
    tr-fallout transform-flag-set
  THEN ;


GET-CURRENT  ( wid )
VOCABULARY fallout-untransformer ALSO fallout-untransformer DEFINITIONS

0 VALUE addr
0 VALUE u
0 VALUE fallout-start
0 VALUE ofs-into-affix
0 VALUE affix-ptr
0 VALUE affix-len
0 VALUE list

: untransform-fallout-add  ( ptrlist-node -- )
  ptrlist-ptr @  { src }
  \ ." ofs is " ofs . ." affix is " affix-ptr len type cr
  \ ." ComparingA " affix-ptr 3cyrs ofs - TYPE ."  and " src ofs +  3cyrs ofs - TYPE CR
  \ affix-ptr  3cyrs ofs -  src ofs +  OVER  STR= IF
    \ ." ComparingB " addr' 2cyrs +  u OVER addr - TYPE ."  and " affix-ptr ofs +  len ofs -  TYPE CR
    \ addr' 2cyrs +  u OVER addr -  affix-ptr ofs +  len ofs -  STR= IF
      list  addr  u cyr+  strlist-prepend-alloc TO list
      fallout-start addr -  list strlist-str 1+  +  ( cs-ptr )
      DUP  DUP cyr+  u fallout-start addr - - MOVE
      src  OVER  3cyrs MOVE
      \ ." constructed " list strlist-str count type cr
      \ ." should compare " dup ofs +  list strlist-str count + over - type ."  with " affix-ptr len type cr
      ofs-into-affix +  list strlist-str count + over -  ( cs-ptr' cs-len' )
      affix-ptr affix-len COMPARE IF ( )
        list list-swallow TO list
      THEN ( )
    \ THEN
  \ THEN
  ;

: g/gh+vowel?  ( addr u -- f )
  2cyrs >= IF                                          ( addr )
    DUP XC@  DUP [CHAR] г =  SWAP [CHAR] ғ = OR  ( addr g/gh? )
    SWAP cyr+ XC@ vowel? AND                              ( f )
  ELSE DROP FALSE THEN ;

: уу?  ( addr -- f )
  DUP XC@ [CHAR] у = IF
    cyr+ XC@ [CHAR] у =  ( f )
  ELSE DROP FALSE THEN ;

: untransform-get-fallout-start-and-ofs-into-affix  ( -- f )
  \ if u >= affix.len - 1 and affix ~= /[гғ]V/, e.g. суу = су+ға
  u  affix-len cyr -  >=  affix-ptr affix-len g/gh+vowel? AND IF
    \ (end of form - affix.len (c|уу), 1)
    addr u + affix-len -  TO fallout-start
    cyr TO ofs-into-affix
  ELSE
    \ if u > affix.len and affix ~= /V/, e.g. суғ+ы > суу
    u affix-len >  affix-ptr XC@ vowel?  AND IF
      \ (end of form - affix.len - 1 (c|уу), 2)
      addr u + affix-len - cyr - TO fallout-start
      2cyrs TO ofs-into-affix
    ELSE FALSE EXIT THEN
  THEN TRUE ;

( wid ) SET-CURRENT  \ public words follow

: untransform-fallout  ( D: str D: affix -- strlist )
  TO affix-len TO affix-ptr TO u TO addr                     ( )
  0 addr u strlist-prepend-alloc TO list  \ add original
  untransform-get-fallout-start-and-ofs-into-affix IF
    fallout-start addr -  2cyrs <=  fallout-start уу?  AND IF
      fallout-reverse-uu                    ( srclist )
    ELSE fallout-start fallout-reverse-find THEN ( srclist | 0 )
    ?DUP-IF ['] untransform-fallout-add  list-map THEN       ( )
  THEN
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
