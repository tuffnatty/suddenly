REQUIRE dictionary.fs
REQUIRE dstack.fs
REQUIRE slot-stack.fs
REQUIRE grammar.fs
REQUIRE strings.fs

0 VALUE n-slots

:noname
  \\." slot-prolog " depth-stack-depth . .s cr
  ; IS (slot-prolog)

:noname
  ; IS (slot-epilog)

\ Buffers for delimited form names and affixes
dstack: formname
dstack: formform
dstack: formform-morphonemic
dstack: formflag

: formstack-slot  ( n dstack -- addr u )
  ]] >O n-slots dstack-depth 1+ - - 1- O dstack-pick O> [[
  ; IMMEDIATE

: form-next-full-slot  { start-slot dstack -- n }
  n-slots start-slot 1+ BEGIN 2DUP >= WHILE
    DUP dstack formstack-slot  IF DROP NIP EXIT THEN
    DROP 1+
  REPEAT 2DROP 0 ;

: form-first-full-slot  { dstack -- n }
  n-slots  dstack >O dstack-depth O> 1+ -  { before-first-existing }
  before-first-existing dstack form-next-full-slot ;

: stem-polysyllabic?  ( -- f )
  ]] guessed-stem polysyllabic? [[ ; IMMEDIATE

: stem-prev-sound  ( -- xc )
  ]] guessed-stem prev-sound [[ ; IMMEDIATE

: stem-prev-sound-ptr  ( -- addr u )
  ]] guessed-stem prev-sound-ptr [[ ; IMMEDIATE

: stem-last-sound  ( -- xc )
  ]] guessed-stem last-sound [[ ; IMMEDIATE

: stem-last-sound-ptr  ( -- addr )
  ]] guessed-stem last-sound-ptr [[ ; IMMEDIATE

: stem-last-char-vowel  ( -- xc )
  ]] guessed-stem last-char-vowel [[ ; IMMEDIATE

: stem-last-char-vowel-row  ( -- wid )
  ]] guessed-stem last-char-vowel-row [[ ; IMMEDIATE

: form-slot  ( n -- addr u )
  ]] formform formstack-slot [[ ; IMMEDIATE

: form-slot-xc-at-left  ( n -- xc )
  BEGIN DUP WHILE
    DUP 1- form-slot ?DUP-IF last-sound NIP EXIT THEN
    DROP 1-
  REPEAT
  DROP stem-last-sound ;

: first-affix  ( -- addr u )
  formform form-first-full-slot ?DUP-IF  ( n )
    formform formstack-slot
  ELSE $0. THEN ;

: first-affix-starts-with?  ( xc -- f )
  first-affix first-sound = ;

: form-slot-flags  ( n -- flags )
  formflag formstack-slot  ?DUP-0=-IF  DROP 0  ( flags )
  ELSE $0. 2SWAP >NUMBER 2DROP D>S THEN ;

: first-form-flag  ( -- f )
  formform form-first-full-slot form-slot-flags ;

: form-flag-is?  ( n flag -- f )
  >R form-slot-flags R> AND ;

: next-form-flag-is?  ( n flag -- f )
  >R formform form-next-full-slot R> form-flag-is? ;

: any-form-flag-is?  { flag -- f }
  formflag >O dstack-depth O> 1+ BEGIN DUP WHILE
    DUP flag form-flag-is? IF EXIT THEN
    1-
  REPEAT ;

: form-slot-vowel-at-left?  ( n_slot -- f )
  form-slot-xc-at-left vowel? ;

: current-form-is-poss?  ( -- f )
  formname dstack@  1 /STRING  S" pos" STRING-PREFIX? ;

REQUIRE filters.fs

language-require transforms.fs

\ Trace output
REQUIRE debugging.fs
0 VALUE parse-depth
: indent  ( -- )
  parse-depth spaces ;

: affix-name-clean  ( affix-name len -- affix-name len | affix-name 0 )
  OVER C@  [CHAR] -  = IF  DROP 0  THEN ;

TIMER: +form-prolog
:noname  ( [addr u] affix-name len -- [addr u] )  +form-prolog
  \\." " indent ." form-prolog: " depth-stack-depth . 2over type ." +" 2dup type \.s
  affix-name-clean formname >dstack  ( )
  +record ; IS (form-prolog)

language-require rules.fs
REQUIRE loaddefs.fs

DEFER yield-stem  ( stem -- )
:noname  ( stem -- )
  ." FOUND STEM: " formname .dstack SPACE formform-morphonemic .dstack CR .stem-single CR  ( )
  1 n-forms +! ; IS yield-stem

: check-stem  ( addr u stem -- addr u )
  \." check-stem " >R 2DUP TYPE SPACE R> DUP .stem-single CR
  { stem }  ( addr u )
  \stack-mark
  2DUP paradigm-stem 2!
  stem stem-dict @  paradigm-dict !
  stem stem-p-o-s paradigm-p-o-s !
  stem stem-dict @ dict-stems @  paradigm-stems !
  stem stem-dict @ dict-flags @  paradigm-dict-flags !
  \." " CR
  indecl? IF
    slot-all-empty? IF
      stem yield-stem
    ELSE
      \." indeclinate stem but there are affixes: " stem .stem-single CR
    THEN
  ELSE
    stem filters-check IF  ( stem )
      \\." yielding" CR
      yield-stem  ( addr u )
    ELSE DROP THEN  ( addr u )
  THEN
  \stack-check
  ;

:+ parse-try-slot  { D: s xt -- }
  \stack-mark
  \\." " indent ." stack.parse-try-slot " depth-stack-depth . ~~ s type xt hex. cr
  s xt EXECUTE (slot-epilog) ( should go through affix-position trying to strip off affix and calling parse-try on success ) 2DROP
  \\." " indent ." stack.parse-try-slot.checking! " depth-stack-depth . .s cr
  \stack-check
  \\." " indent ." stack.parse-try-slot.checked! " depth-stack-depth . .s cr
  ;

:+ parse-try-stem  { D: s -- }
  \stack-mark
    \\." " indent ." stack.done!" .s s type cr
    s string-addr 0= IF ABORT" Corrupt stem?" THEN
    s stem-find ?DUP-IF  { stem }
      s  stem  ['] check-stem  list-map  2DROP
      \." ~~~~~~~~~" cr
    ELSE
      \." STEM " s type ."  not in dictionary! was trying " s type formform-morphonemic .dstack bl emit formname .dstack cr
    THEN
  \stack-check
  \\." " indent ." stack.done.checked! " .s cr
  ;

:+ parse-try  { D: s -- }
  \stack-mark
  \\." " indent ." stack.in " depth-stack-depth . s type s . . cr
  slot-stack-pop ?DUP-IF  { xt }
    s xt parse-try-slot
  ELSE
    s parse-try-stem
  THEN
  \\." " indent ." stack.in.checking! " depth-stack-depth . .s cr
  \stack-check
  slot-stack-push
  \\." " indent ." stack.in.checked! " depth-stack-depth . .s cr
  ;

: rule-check  ( addr u i xt | 0 -- addr u f )
  ?DUP-IF SWAP >R EXECUTE ( rule-result ) R> =
  ELSE DROP TRUE THEN ;

:+ after-fallout-pair  { D: left-part  D: affix  slot-flag rule n-rule -- }
  \." " indent rule if ." Pair " left-part TYPE ." +" affix TYPE ."  harmony variant: " rule execute . ." left, " n-rule . ." right" cr then
  left-part  n-rule rule rule-check { harmony-ok? }  2DROP
  harmony-ok? NOT IF
    slot-flag 0= IF
      PAD left-part string-length { D: buffer }
      left-part last-sound-except-ь-ptr cyr t~/ {voiced} IF
        left-part string-addr buffer CMOVE
        buffer last-sound-except-ь-ptr cyr  2DUP unvoice-str DROP -ROT CMOVE
        \." " indent ." Correcting VU harmony to " buffer TYPE CR
        buffer n-rule rule rule-check TO harmony-ok? 2DROP
        harmony-ok? IF
          slot-flag harmony-vu-broken OR TO slot-flag
        THEN
      THEN
      left-part last-vowel [CHAR] и = IF
        left-part string-addr buffer CMOVE
        buffer string-end { ptr }
        BEGIN ptr buffer string-addr > WHILE
          ptr XCHAR- TO ptr
          ptr XC@ [CHAR] и = IF
            [CHAR] і ptr XC!
            FALSE
          ELSE TRUE THEN WHILE
        REPEAT THEN
        \." " indent ." Correcting FB harmony to " buffer TYPE CR
        buffer n-rule rule rule-check TO harmony-ok? 2DROP
        harmony-ok? IF
          slot-flag harmony-fb-broken OR TO slot-flag
        THEN
      THEN
    THEN
  THEN
  harmony-ok? IF
    \stack-mark \\." ENTERING PARSE-TRY" .s cr
    slot-flag ?DUP-IF S>D <<# #s #> ELSE $0. THEN formflag >dstack
    \\." " indent ." Trying " left-part TYPE ." +" affix TYPE ."  formflags " formflag .dstack CR
    left-part parse-try
    \\." " indent ." after-fallout-pair: out of parse-try" .s cr
    formflag dstack> slot-flag IF #>> THEN
    \stack-check
    \\." " indent ." out of harmony-ok cr
  THEN
  ;

[: HERE 128 0 DO Untransformer new DROP LOOP ;] STATIC-A WITH-ALLOCATER CONSTANT untransformer-stack
0 VALUE untransformer-stack-depth
: get-untransformer  ( -- )
  untransformer-stack-depth 1+ TO untransformer-stack-depth ;
: top-untransformer  ( -- o )
  untransformer-stack-depth 1- [ Untransformer >OSIZE @ CELL+ ]L * untransformer-stack + ;
: dispose-untransformer  ( -- )
  untransformer-stack-depth 1- TO untransformer-stack-depth ;

:+ process-single-representation  ( addr u affix affix-len rule n-rule -- addr u )
  { D: affix rule n-rule }                 ( addr u )

  \stack-mark
  \\." " indent ." <Singlerep> " 2DUP type ." +" affix type rule HEX. n-rule . .s CR

  affix  formform >dstack
  get-untransformer  top-untransformer >O

  TRY
    2DUP  affix  rule  n-rule  ['] after-fallout-pair  configure

    unfallout

    DUP  affix string-length > IF
      \\." " indent ." Unchanged guess: " 2DUP TYPE .s CR
      unjoin
      \\." " indent ." After after-fallout: " .s CR
    THEN

  ENDTRY-IFERROR
    O> dispose-untransformer THROW
  THEN
  O> dispose-untransformer
  formform dstack>

  \stack-check
  ;

:+ process-representations  ( addr u rule sstr -- addr u )
  { rule sstr }                         ( addr u )
  \stack-mark
  \\." " indent ." <All-reps>" 2DUP TYPE ." +" sstr .sstr rule HEX. .s CR
  sstr sstr-morphonemic 2@  formform-morphonemic >dstack
  sstr sstr-count @ 0 DO
    I sstr sstr-select { D: affix }
    affix string-length IF
      DUP affix string-length >= IF
        affix rule I process-single-representation
      THEN
    THEN
  LOOP
  formform-morphonemic dstack>
  \stack-check
  \\." " cr
  ;

\ an awful big word
TIMER: +form-epilog
:noname +form-epilog ( addr u rule sstr -- addr u )
  \." " parse-depth 1+ TO parse-depth
  \\." " indent ." form-epilog " 2>r 2dup type ." +" 2r> 2dup .sstr .rule \.s cr
  ?DUP-IF  \ Non-empty affix
    \\." " indent ." non-empty" CR
    process-representations
  ELSE
    DROP  ( addr u )
    \\." " indent ." Trying 0 " 2dup type ." +0" CR
    $0. formform >dstack
    $0. formform-morphonemic >dstack
    $0. formflag >dstack
    2DUP parse-try
    formform dstack>
    formform-morphonemic dstack>
    formflag dstack>
    \\." " indent ." out of parse-try" CR
  THEN
  \\." " indent ." form-epilog ending:" over HEX. dup . 2dup type \.s
  \." " parse-depth 1- TO parse-depth
  formname dstack>
  +record ; IS (form-epilog)

: parse-yield  ( cs1 cs2 -- )
  COUNT TYPE  9 EMIT  COUNT TYPE  1 n-forms +! ;

: parse-form  ( cs -- )
  \ untransform2  ( cs1..csN n )  0 ?DO parse-one LOOP
  COUNT parse-try ;

: parse-khak  ( cs -- )
  debug-init
  slot-stack /slot-stack slot-stack-set
  /slot-stack TO n-slots
  slot-stack-reverse
  0 paradigm-slot-bitmap !
  0 n-filters !
  flag-none paradigm-flags!
  formform dstack-clear
  formname dstack-clear
  formform-morphonemic dstack-clear
  formflag dstack-clear
  0 TO untransformer-stack-depth

  parse-form

  debug-bye ;
