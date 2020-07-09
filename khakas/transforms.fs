REQUIRE ./../minire.fs
REQUIRE ./phonetics.fs
REQUIRE util.fs
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

%00000000000001 CONSTANT untransformed-cluster-envoice
%00000000000010 CONSTANT untransformed-left-envoice
%00000000000100 CONSTANT untransformed-left-envoice-missing
%00000000001000 CONSTANT untransformed-fallout
%00000000010000 CONSTANT untransformed-fallout-CCC
%00000000100000 CONSTANT untransformed-fallout-VГV
%00000001000000 CONSTANT untransformed-fallout-VVГV
%00000010000000 CONSTANT untransformed-fallout-V[кх]V
%00000100000000 CONSTANT untransformed-fallout-VңV
%00001000000000 CONSTANT untransformed-fallout-confluence
%00010000000000 CONSTANT untransformed-fallout-(СА|ТЫ)ңАр
%00100000000000 CONSTANT untransformed-fallout-OK
%01000000000000 CONSTANT harmony-fb-broken
%10000000000000 CONSTANT harmony-vu-broken


: /[ае]($|[бдркх])/  ( D: s -- f )
  2DUP t~/ а|е IF
    cyr /STRING
    ?DUP-0=-IF EXIT THEN
    t~/ б|д|р|к|х
  ELSE 2DROP FALSE THEN
  ;

: s/.(.*).$/$1$2\1/  { str len s1 s1-len s2 s2-len }
  s1-len  str @xc-size -  s2-len + { ofs }
  ofs IF
    str  str len ofs /STRING  CMOVE>
  THEN
  s1  str          s1-len CMOVE
  s2  str s1-len + s2-len CMOVE ;

: s/(.*)$/$1$2\1/  { s1 s1-len s2 s2-len str len -- }
  str  str len  s1-len s2-len + /STRING  CMOVE>
  s1  str          s1-len CMOVE
  s2  str s1-len + s2-len CMOVE ;

: s/(.*)$/$1\1/  { c1 str len -- }
  str  str len c1 XC-SIZE /STRING  CMOVE>
  c1 str XC! ;

: s/..(.*).$/$1$2$3\1/  { s1 s1-len s2 s2-len s3 s3-len str len -- }
  s1-len s2-len s3-len + +  str XCHAR+ XCHAR+ str -  - { ofs }
  ofs IF
    str  str len ofs /STRING  CMOVE>
  THEN
  str                        ( str )
  s1 OVER s1-len CMOVE  s1-len +
  s2 OVER s2-len CMOVE  s2-len +
  s3 SWAP s3-len CMOVE           ( )
  ;


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
      DUP last-sound-ptr cyr t~/ {envoiceable} IF
        DUP prev-sound-ptr cyr t~/ {vowel} IF
          DUP last-sound-ptr envoice-str  ( addr len cs c-addr c-u )
          2 PICK  last-sound-change    ( addr len cs )
          tr-envoice transform-flag-set
    THEN THEN THEN
    -ROT                              ( cs addr len )
  \ THEN
  ;

: transform-envoice-for-consonant-affix  ( cs addr len -- cs addr len )
  DUP 2cyrs >= IF
    2DUP t~/ {envoiceable}{vowel} IF
      2 PICK last-sound-ptr cyr t~/ {vowel} IF
        OVER cyr envoice-str     ( cs addr len c-addr c-u )
        4 PICK last-sound-add  ( cs addr len )
        +X/STRING            ( cs addr' len' )
        tr-envoice transform-flag-set
  THEN THEN THEN ;

: possibly-fugitive?  ( addr u -- f)
  last-sound-ptr  ( last-addr )
  DUP cyr "н" STR= IF
    XCHAR- cyr t~/ {glide}
  ELSE DROP FALSE THEN ;


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
"уу" fallout-repl-uu SWAP MOVE
TABLE CONSTANT fallout-table
TABLE CONSTANT fallout-reverse-table
0  "уғы" DROP  ptrlist-prepend
   "уға" DROP  ptrlist-prepend
   "уғо" DROP  ptrlist-prepend  CONSTANT fallout-reverse-uu

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


REQUIRE mini-oof2.fs
GET-CURRENT  ( wid )
VOCABULARY fallout-untransformer ALSO fallout-untransformer DEFINITIONS
: end-public-class  ( public-wid class-sys "name" -- public-wid class-sys )
  3 PICK SET-CURRENT
  END-CLASS
  DEFINITIONS ;

object CLASS
  2VALUE: s
  2VALUE: affix
  2VALUE: fallout-rslice
  VALUE: wordform-row
  VALUE: stem-polysyllabic?
  VALUE: flags
  VALUE: ofs-into-affix
  VALUE: fallout-start
  VALUE: fallout-pos
  VALUE: rule
  VALUE: n-rule
  VALUE: receiver-xt
  VALUE: guess-size
  ALIGNED 128 CHARS +FIELD guess-buffer
end-public-class Untransformer

: affix-len  ]] affix string-length [[ ; IMMEDIATE

:+ predict-fallout-coord  ( -- 0 | fallout-start ofs-into-affix TRUE )
  s string-length affix-len - { affix-pos }
  s string-addr  affix-pos + { fallout-start }

  \ if u >= affix.len and affix ~= /[гғ]V/, e.g. суу = су+ға
  affix-pos 0>= IF
    fallout-start affix-len  S" -" SEARCH NIP NIP IF FALSE EXIT THEN

    affix t~/ г{vowel}|ғ{vowel} IF
      \ (end of form - affix.len (c|уу), 1cyr)
      fallout-start              cyr   TRUE   EXIT
    THEN
  THEN

  \ if u > affix.len
  affix-pos 0> IF
    affix t~/ {vowel}{vowel} IF  \ e.g улуғ+ла+аачых > улуғлаачых
      \ (end of form - affix.len (улуғл|аачых), 0)
      fallout-start              0     TRUE   EXIT
    THEN

    affix t~/ {vowel} IF  \ e.g. суғ+ы > суу
      \ (end of form - affix.len - 1 (c|уу), 2)
      fallout-start XCHAR-      2cyrs  TRUE   EXIT
    THEN

    affix t~/ ңар|ңер IF  \ e.g. пас+са+ңар > пассар
      s  affix +X/STRING  string-ends IF
        fallout-start XCHAR+     cyr   TRUE   EXIT
      THEN
    THEN

    affix t~/ {consonant} IF  \ e.g. финн+нең > финнең
      s affix string-ends IF
        fallout-start cyr  fallout-start XCHAR- cyr STR= IF
          fallout-start XCHAR-   cyr   TRUE   EXIT
      THEN THEN
    THEN
  THEN
  FALSE ;

: guess-make  ( D: s -- )
  bi[ TO guess-size
   ][ guess-buffer SWAP MOVE ]; ;

: guess-yield  ( -- )
  \stack-mark
  \\." YIELDing guess: " guess-buffer guess-size TYPE ." +" affix TYPE .s CR
  guess-buffer guess-size  affix  flags  rule   n-rule   receiver-xt  EXECUTE
  \\." BACK FROM YIELD: " guess-buffer guess-size TYPE ." +" affix TYPE .s CR
  \stack-check
  ;

: guess-check  { D: guess-fallout  -- }
  \\." check:" guess-fallout type ."  affix:" affix type ."  ofs-into:" ofs-into-affix . cr
  guess-fallout ofs-into-affix /STRING  affix STR= IF
    \ unjoin affix
    guess-size affix-len - TO guess-size
    \\." YIELD" CR
    guess-yield
  THEN ;

: unfallout-guess-make  ( -- D: guess-fallout )
  s  cyr+ guess-make  guess-buffer guess-size  fallout-pos /STRING ;

:+ unfallout-confluence  ( -- )
  \ 2. Выпадение Г после Г, ң без стяжения. При стечении по-
  \ следней согласной основы на -ғ, -г, -ң и любого аффикса с
  \ начальной морфонемой Г первая согласная аффикса (то есть
  \ Г) выпадает, согласная основы остается: суғ + -ға > суғ-а
  \ ‘воде’, эг + -гей > эгей ‘пусть гнет’, тоң + -ған > тоңан
  \ ‘замерз’. На месте стыка аффиксов правило не действует!
  \stack-mark
  untransformed-fallout-confluence TO flags
  fallout-rslice t~/ ғ|г|ң IF
    affix t~/ г|ғ IF
      fallout-rslice first-sound-ptr  cyr { D: C1 }
      wordform-row front-vowel = IF "г" ELSE "ғ" THEN { D: C2 }
      unfallout-guess-make { D: guess-fallout }
      guess-fallout C1 C2 s/.(.*).$/$1$2\1/
      guess-fallout guess-check
    THEN
  THEN
  untransformed-fallout TO flags
  \stack-check
  ;

: unfallout-add-vcv  ( D: v1  D: c  D: v2 -- )
  unfallout-guess-make ['] s/..(.*).$/$1$2$3\1/
                       ['] guess-check
                       2bi
  ;

:+ init-unfallout  ( -- f )
  \ \." coords? " s TYPE ." +" affix TYPE \.s
  predict-fallout-coord IF  ( fallout-start ofs-into-affix )
    TO ofs-into-affix  ( fallout-start )
    s string-addr { wordform-addr }
    DUP wordform-addr -  ( fallout-start fallout-pos )
    DUP 0< IF
      ." NEGATIVE fallout-pos " . ."  s:" s TYPE ."  affix:" affix TYPE CR BYE
    ELSE DUP 200 > IF
      ." HUGE fallout-pos " . ."  s:" s TYPE ."  affix:" affix TYPE CR BYE
    THEN THEN
    >R  s R@ /STRING  TO fallout-rslice  R>
    wordform-addr OVER  ( fallout-start fallout-pos  D: fallout-lslice )
    2DUP last-char-vowel-row  TO wordform-row
    2DUP string-end @xc-size +  polysyllabic?  TO stem-polysyllabic?  ( fallout-start fallout-pos )
    TO fallout-pos   TO fallout-start  TRUE  ( -- f )
  ELSE FALSE THEN ;

: unfallout-add-vv  { D: v1  D: v2 -- }
  \\." unfallout-add-vv: trying " v1 TYPE v2 TYPE CR
  unfallout-guess-make { D: guess-fallout }
  guess-fallout v1 v2 s/.(.*).$/$1$2\1/
  v2 string-length 0= IF
    guess-size cyr - TO guess-size
    guess-fallout cyr - TO guess-fallout
  THEN
  \\." got " guess-fallout TYPE CR
  guess-fallout guess-check
  ;

: unfallout-add-vvv  { D: v1  D: v2 -- }
  \\." unfallout-add-vvv: trying " v1 TYPE v2 TYPE CR
  unfallout-guess-make { D: guess-fallout }
  guess-fallout v1 v2 s/.(.*).$/$1$2\1/
  v2 string-length 0= IF
    guess-size cyr - TO guess-size
  ELSE
    guess-fallout +X/STRING TO guess-fallout
  THEN
  \\." got " guess-fallout TYPE CR
  guess-fallout guess-check
  ;

: unfallout-aa/ee  { D: v1  D: c  vowels -- }
  vowels sound-each-str { D: v2 }
    v1 c v2 unfallout-add-vcv
    v1 v2 COMPARE IF
      v2 c v1 unfallout-add-vcv
    THEN
  sound-next
  ;

:+ unfallout-vgv-non-first  ( -- )
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
  stem-polysyllabic? IF
    fallout-rslice t~/ аа|яа|ее IF
      fallout-rslice cyr left-slice { D: V1 }
      fallout-rslice t~/ {back-vowel} IF
        "ғ"  short-back-vowel   long-back-vowel
      ELSE
        "г"  short-front-vowel  long-front-vowel
      THEN { D: C  vowels  long-vowels }

      untransformed-fallout-VГV TO flags
      V1  C              vowels unfallout-aa/ee

      untransformed-fallout-VңV TO flags
      V1  "ң"            vowels unfallout-aa/ee

      untransformed-fallout-VVГV TO flags
      V1  C              long-vowels unfallout-aa/ee

      untransformed-fallout-V[кх]V TO flags
      V1  C unvoice-str  vowels unfallout-aa/ee

      untransformed-fallout TO flags
    ELSE fallout-rslice t~/ ии IF
      wordform-row back-vowel = IF
           "ғ" "ы" "у"
      ELSE "г" "і" "ӱ" THEN { D: C  D: V1  D: V2 }

      untransformed-fallout-VГV TO flags
      V1 C     V1 unfallout-add-vcv
      V1 C     V2 unfallout-add-vcv
      V2 C     V1 unfallout-add-vcv
      V2 C     V2 unfallout-add-vcv

      untransformed-fallout-VңV TO flags
      V1 "ң"   V1 unfallout-add-vcv
      V1 "ң"   V2 unfallout-add-vcv
      V2 "ң"   V1 unfallout-add-vcv
      V2 "ң"   V2 unfallout-add-vcv

      untransformed-fallout-V[кх]V TO flags
      C unvoice-str { D: C-unv }
      V1 C-unv V1 unfallout-add-vcv
      V1 C-unv V2 unfallout-add-vcv
      V2 C-unv V1 unfallout-add-vcv
      V2 C-unv V2 unfallout-add-vcv

      untransformed-fallout TO flags
    THEN THEN
  THEN
  \stack-check
  ;

:+ unfallout-vgv-first  ( -- )
  \ 3.1 Выпадение морфонемы Г в интервокальной позиции и
  \ стяжение двух обрамлявших ее гласных в одну долгую
  \ [не всегда].
  \ В односложных основах правило действует и на долгие, и на
  \ краткие гласные основы (в аффиксе во всех случаях действует
  \ только для кратких гласных).
  \stack-mark
  stem-polysyllabic? NOT IF
    fallout-rslice vowel-long? IF
      fallout-rslice cyr left-slice { D: V1 }
      fallout-rslice t~/ {back-vowel} IF  "ғ"    short-back-vowel
                                    ELSE  "г"    short-front-vowel
                                    THEN { D: C  vowels }

      \ Односложные глаголы, оканчивающиеся на
      \ краткую гласную, стягиваются с аффиксами так же, как
      \ многосложные: теен ‘сказал’ < тi ‘сказать’ + ған Past,
      \ чеелек ‘еще не поел’ < чi ‘есть’ + гелек Cunc.
      affix t~/ г|ғ IF
        "і" C   V1 unfallout-add-vcv
      THEN

      \ В прочих случаях на месте стяжения образуется длинная
      \ гласная, идентичная корневой.
      vowels sound-each-str { D: V2 }
        V1  C   V2 unfallout-add-vcv
        V1  "ң" V2 unfallout-add-vcv

        \ 3.3. Выпадение конечной губной -п односложной
        \ глагольной основы при прибавлении афф. NF, Convп (I)П
        \ и Perf IбIС Convп -ып [всегда]: тап ‘находить’ + -ып >
        \ таап ‘найдя’, сап ‘ударять’ + -ып > саап ‘ударяя’, теп
        \ ‘толкать’ + -іп > тееп ‘толкая’, сап ‘косить’ + -ыбыс
        \ > саабыс ‘скоси!’, теп ‘толкать’ + -ібіс > теебіс
        \ ‘толкни!’.
        V2 t~/ ы|і IF
          affix t~/ 0̸|ып|ыб|іп|іб  IF
            V1 "п" V2 unfallout-add-vcv
        THEN THEN
      sound-next
    THEN
  THEN
  \stack-check
  ;

:+ unfallout-vv-Imp.1  ( -- )
  \stack-mark
  affix t~/ им|иб IF
    fallout-rslice t~/ и IF
      wordform-row sound-each-str { D: V1 }
        V1 "и" unfallout-add-vv
      sound-next
    THEN
  THEN
  \stack-check
  ;

:+ unfallout-vv-Imp.1.Incl  ( -- )
  \stack-mark
  affix t~/ ал|ел|аң|ең  IF
    fallout-rslice t~/ аа|яа|ее  IF
      affix t~/ {back-vowel} IF back-vowel ELSE front-vowel THEN sound-each-str \ { D: V1 }
        ( D: V1 ) "" unfallout-add-vv
      sound-next
    THEN
  THEN
  \stack-check
  ;

:+ unfallout-vv-Simul  ( -- )
  \stack-mark
  affix t~/ аачых|еечік IF
    fallout-rslice { D: fallout }
    fallout t~/ аа|яа|ее IF
      fallout second-sound-ptr  cyr { D: V2 }
      V2 t~/ {back-vowel} IF back-vowel ELSE front-vowel THEN sound-each-str { D: V1 }
        V1 V2 unfallout-add-vvv
      sound-next
    THEN
  THEN
  \stack-check
  ;

:+ unfallout-VА>и  ( -- )
  \stack-mark
  affix /[ае]($|[бдркх])/ IF
    fallout-rslice t~/ и IF
      wordform-row back-vowel = IF "а" ELSE "е" THEN  { D: V2 }
      wordform-row sound-each-str { D: V1 }
        V1 V2 unfallout-add-vv
      sound-next
    THEN
  THEN
  \stack-check
  ;

:+ unfallout-OK  ( -- )
  \stack-mark
  affix t~/ ох|ӧк IF
    stem-polysyllabic? IF
      s fallout-pos left-slice  last-sound-ptr cyr t~/ {consonant} IF
        untransformed-fallout-OK TO flags
        fallout-rslice cyr left-slice { D: V2 }
        V2 t~/ {back-vowel} IF
             short-unrounded-back-vowel
        ELSE short-unrounded-front-vowel THEN sound-each-str { D: V1 }
          V1 V2 unfallout-add-vv
        sound-next
        untransformed-fallout TO flags
      THEN
    THEN
  THEN
  \stack-check
  ;

: unfallout-add-vc  ( D: V  D: C -- )
  unfallout-guess-make cyr+ ['] s/(.*)$/$1$2\1/
                            [: guess-size cyr+ TO guess-size
                               guess-check ;]
                            2bi
  ;

: unfallout-add-c  ( C -- )
  unfallout-guess-make cyr /STRING  ['] s/(.*)$/$1\1/
                                    ['] guess-check
                                    2bi
  ;

:+ unfallout-(СА|ТЫ)ңАр  ( -- )
  \stack-mark
  affix t~/ ңар|ңер IF
    fallout-rslice  { D: fallout }
    fallout  affix +X/STRING  STR= IF
      fallout cyr left-slice { D: V }
      s fallout-pos left-slice { D: left-part }
      left-part last-sound-ptr { c-ptr }
      fallout string-addr vowel-long-middle? { long? }
      long? IF c-ptr XCHAR- TO c-ptr THEN
      c-ptr cyr t~/ т|д IF
           fallout t~/ {front-vowel} IF "і" ELSE "ы" THEN
      ELSE V                                              THEN { D: V1 }
      c-ptr cyr t~/ с|з|т|д IF
        long? IF
          fallout-pos cyr - TO fallout-pos
          V1  "ң"  V  unfallout-add-vcv
          fallout-pos cyr+ TO fallout-pos
        ELSE
          V1  "ң"  unfallout-add-vc
        THEN
      THEN
    THEN
  THEN
  \stack-check
  ;

: unfallout-CCC  ( -- )
  \stack-mark
  affix first-sound { C }
  fallout-rslice first-sound  C = IF
    C unfallout-add-c
  THEN
  \stack-check
  ;

:+ unfallout-vowelaffix  ( -- )
  \G Reconstruct possible fallout kinds for affixes starting with a vowel
  \stack-mark
    ofs-into-affix ?DUP-IF  cyr -  TO ofs-into-affix  THEN

    \ II.2. императив инклюзивный. Аффиксы инклюзивного
    \ императива Imp.1.Incl Аң, Imp.1.Incl.Pl АңАр/Алар при
    \ присодинении к основам на гласную поглощают гласную
    \ основы, на месте стяжения образуется долгая аа/ее
    \ Так же себя ведет симулятив ААчЫК.
    \\." vv-Imp.1.Incl? " s TYPE ." +" affix TYPE ." |" \.s
    unfallout-vv-Imp.1.Incl
    \\." vv-Simul? " s TYPE ." +" affix TYPE ." |" \.s
    unfallout-vv-Simul

    fallout-pos cyr+  TO fallout-pos
    fallout-rslice cyr /STRING  ( D: fallout-rslice )
    stem-polysyllabic? NOT IF 2DUP t~/ {vowel} TO stem-polysyllabic? THEN
    TO fallout-rslice                             ( )

    \ II.1. императив 1 числа: В глаголе поглощение первой
    \ гласной аффикса императива предыдущей любой гласной
    \ основы (другими словами: выпадение любой последней гласной
    \ основы при присоединении личных афф. императива 1 лица
    \ Imp.1 на -и (-им, -ибыс, -ибiс)) [всегда]:
    \\." vv-Imp.1? " s TYPE ." +" affix TYPE ." |" \.s
    unfallout-vv-Imp.1

    \ II.3. А с аллофоном и: В глаголах правила слияния с
    \ фонетическими преобразованиями для афф. Fut -Ар, Convа
    \ -А, Convпас A.бАс (диал.), Prosp АК, Iter АдIр. Эти
    \ аффиксы не имеют вариантов, начинающихся на согласную.
    \ При присоединении их к основе на гласную происходит
    \ стяжение двух кратких гласных в нейтральную и без
    \ долготы [всегда]
    \\." VА>и? " s TYPE ." +" affix TYPE ." |" \.s
    unfallout-VА>и

    \ II.5. поглощение гласных перед -ох: Выпадение конечной
    \ краткой гласной основы или аффикса перед аффиксом -ох,
    \ -öк (производным от одноименной усилительной частицы)
    \ [не всегда]: краткая неогубленная гласная не первого
    \ слога, будь то гласная основы или аффикса,
    \ предшествующая аффиксу -ох, -öк, выпадает: турох < тура
    \ ‘дом’ + ох; андох ‘там же’ < анда ‘там’ + ох ‘же’. С
    \ долгой гласной этого не происходит: мағаа ‘Dat от мин’
    \ + ох > мағааох. В первом слоге гласная любой длины не
    \ стягивается: пу ‘этот’ + ох > пуох 'этот же'. 3pos в
    \ виде алломорфов -ы/-i не стягивается: хызох < хыс+ох,
    \ но не < хыс-ы-ох. Гласная дательного падежа не
    \ стягивается: суғ+ға+ох > суғаох ‘в воду же’.
    \\." OK? " s TYPE ." +" affix TYPE ." |" \.s
    unfallout-OK
  \stack-check ;

:+ unfallout-consonantaffix  ( -- )
  \G Reconstruct possible fallout kinds for affixes starting with a consonant
  \stack-mark
    \ II.4. стяжение 2pl: Выпадение сонанта ң из личного афф. 2pl
    \ ңАр в интервокальной позиции после Cond СА и RPast ТI
    \ [не всегда]. При выпадении ң гласная аффикса также
    \ выпадает: пас-са-ңар > пас-сар ‘если будем писать’,
    \ пас-па-за-ңар > пас-па-зар ‘если не будем писать’,
    \ пас-ты-ңар > пастар 'вы недавно написали'. В лит. языке
    \ допускаются как стяженный, так и полный (без выпадений)
    \ варианты данных форм. У Чебодаевой есть отражение
    \ долготы на письме: пассаар, пастаар.
    \\." (СА|ТЫ)ңАр? " s TYPE ." +" affix TYPE ." |" \.s
    untransformed-fallout-(СА|ТЫ)ңАр TO flags
    unfallout-(СА|ТЫ)ңАр

    \ I.4. Упрощение группы из трех согласных.
    \ Если русское заимствование заканчивается на двойную
    \ согласную (финн, класс, ватт) и после нее идет аффикс,
    \ начинающийся с той же согласной, на стыке пишется не
    \ три согласных, а две: финнең, классар, ватты.
    \\." CCC? " s TYPE ." +" affix TYPE ." |" \.s
    untransformed-fallout-CCC TO flags
    unfallout-CCC
  \stack-check ;

:+ unfallout-all  ( -- )
  \stack-mark
  \\." confluence? " s TYPE ." +" affix TYPE ."  fallout-pos:" fallout-pos . ."  ofs-into:" ofs-into-affix . \.s
  unfallout-confluence

  \ 3. Выпадения со стяжением гласных в интервокале
  \ 3.2 Выпадение сонанта ң в интервокальной позиции и
  \ последующее стяжение двух обрамлявших его гласных в одну
  \ долгую [не всегда]. Правила выпадения такие же, как у
  \ морфонемы Г. В аффиксах бывает конечная ң, которая может
  \ попасть в интервокал: Hab ЧАң и 2pos.sg (I)ӊ. Правило
  \ действует на Hab и не действует на посессив
  \ 3.4. Выпадение конечных заднеязычных глухих в основах на
  \ -х, -к. Это происходит в многосложных (!) основах в
  \ интервокальной позиции и сопровождается последующим
  \ стяжением двух обрамлявших его гласных в одну долгую (см.
  \ правило 2.2) [не всегда].
  \\." vgv-non-first? " s TYPE ." +" affix TYPE ." |" \.s
  unfallout-vgv-non-first
  \\." vgv-first? " s TYPE ." +" affix TYPE ." |" \.s
  unfallout-vgv-first

  ofs-into-affix  DUP 2cyrs =  SWAP 0=  OR IF  \ that is, affix starts with a vowel
    unfallout-vowelaffix
  ELSE  \ that is, affix starts with a consonant
    unfallout-consonantaffix
  THEN

  \stack-check ;

: untransform-left-envoice  { D: left-part -- }
  \\." untransform-left-envoice " left-part TYPE CR
  left-part guess-make
  left-part last-sound-ptr cyr { D: last }
  last t~/ {unvoiced} IF
    last "ч" COMPARE IF
      untransformed-left-envoice-missing  TO flags
    THEN
    guess-yield
  ELSE last t~/ {voiced} IF
    guess-yield
    last "ӌ" COMPARE IF
      last unvoice-str  guess-buffer guess-size  last-sound-ptr  SWAP CMOVE
      untransformed-left-envoice  TO flags
      guess-yield
    THEN
  ELSE
    guess-yield
  THEN THEN
  \\." untransform-left-envoice " left-part TYPE ."  done" CR
  ;

: untransform-fugitive  { D: left-part -- }
  left-part guess-make
  guess-buffer guess-size  last-sound-ptr { prev-ptr }
  prev-ptr cyr  OVER  2cyrs INSERT
  guess-size cyr+ TO guess-size

  [CHAR] у  prev-ptr XC!   guess-yield
  [CHAR] ы  prev-ptr XC!   guess-yield
  [CHAR] і  prev-ptr XC!   guess-yield
  ;

( wid ) SET-CURRENT  \ public words follow


:+ configure  ( D: s  D: affix  rule  n-rule  receiver-xt -- )
  TO receiver-xt  TO n-rule  TO rule       ( D: s  D: affix )
  TO affix  TO s      ( )
  ;

:+ unfallout  ( -- )
  untransformed-fallout TO flags  ( )
  \stack-mark
  \ \." unfallout: " s TYPE ." +" affix TYPE \.s
  init-unfallout IF
    unfallout-all
  THEN
  \stack-check ;

TIMER: +unenvoice-affix

:+ unjoin  ( -- )
  0 TO flags  ( )
  
  \\." unjoining " affix type ."  from " s type ." ..."
  s affix-len - { D: left-part }
  affix t~/ {vowel} IF
    s affix string-ends IF
      left-part prev-sound-ptr cyr { D: prev }
      prev t~/ {vowel} IF
        left-part untransform-left-envoice
      ELSE
        \ check for fugitive
        current-form-is-poss? IF
          affix t~/ {very-narrow-vowel} IF
            left-part possibly-fugitive? IF
              left-part untransform-fugitive
        THEN THEN THEN

        \ 4) В заимствованиях возможен переход с>з не только в
        \ интервокале, но и в кластерах с сонорными: курс -
        \ курзы, но баланс - балансы (по словарю).
        prev t~/ {sonorant} IF
          left-part last-sound [CHAR] з = IF
            left-part guess-make
            [CHAR] с  guess-buffer guess-size last-sound-ptr  XC!
            untransformed-cluster-envoice TO flags
            guess-yield
        THEN THEN

        left-part guess-make
        guess-yield
      THEN
    THEN
  ELSE
    +unenvoice-affix
    affix t~/ {envoiceable} IF
      s affix string-ends IF
        affix-len cyr <=  ?DUP-0=-IF left-part last-sound-ptr cyr t~/ {vowel} NOT THEN  IF
          left-part guess-make
          guess-yield
        THEN
      ELSE
        s  affix cyr /STRING  string-ends IF
          left-part last-sound-ptr cyr t~/ {vowel} IF
            affix-len cyr > IF
              s string-end affix-len -  cyr  affix cyr left-slice envoice-str STR= IF
                left-part guess-make
                guess-yield
        THEN THEN THEN THEN
      THEN
    ELSE
      s affix string-ends IF
        left-part guess-make
        guess-yield
      ELSE
        affix t~/ 0 IF
          s guess-make
          guess-yield
        THEN
      THEN
    THEN
    +record
  THEN
  \ \." " ." Unjoin attempt completed" CR
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
    OVER @vowel? IF
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
