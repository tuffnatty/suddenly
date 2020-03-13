REQUIRE dictionary.fs
REQUIRE dstack.fs
REQUIRE slot-stack.fs
REQUIRE grammar.fs
REQUIRE strings.fs

0 VALUE n-slots

:noname
  \stack-mark \\." slot-prolog " depth-stack-depth . .s cr
  ; IS (slot-prolog)

:noname
  \stack-check
  ; IS (slot-epilog)

\ Buffers for delimited form names and affixes
dstack: formname
dstack: formform
dstack: formform-morphonemic
dstack: formflag

: formstack-slot  ( n dstack -- addr u )
  ]] >O n-slots dstack-depth 1+ - - 1- O dstack-pick O> [[
  ; IMMEDIATE

: form-first-full-slot  { dstack -- n }
  n-slots  dstack >O dstack-depth O> 1+ -  1+  { first-existing }
  n-slots first-existing BEGIN 2DUP >= WHILE
    DUP dstack formstack-slot  IF DROP NIP EXIT THEN
    DROP 1+
  REPEAT 2DROP 0 ;

: stem-polysyllabic?  ( -- f )
  ]] guessed-stem polysyllabic? [[ ; IMMEDIATE

: stem-prev-sound  ( -- xc )
  ]] guessed-stem prev-sound [[ ; IMMEDIATE

: stem-prev-sound-ptr  ( -- addr u )
  ]] guessed-stem prev-sound-ptr [[ ; IMMEDIATE

: stem-last-sound  ( -- xc )
  ]] guessed-stem last-sound [[ ; IMMEDIATE

: stem-last-char-vowel  ( -- xc )
  ]] guessed-stem last-char-vowel [[ ; IMMEDIATE

: form-slot  ( n -- addr u )
  ]] formform formstack-slot [[ ; IMMEDIATE

: form-slot-xc-at-left  ( n -- xc )
  1- BEGIN DUP WHILE
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
  \\." check-stem " >R 2DUP TYPE R> DUP .stem-single CR
  >R \stack-mark 2DUP paradigm-stem 2! R>
  DUP stem-p-o-s paradigm-p-o-s !
  DUP stem-dict @ dict-stems @  paradigm-stems !
  DUP stem-dict @ dict-flags @  paradigm-dict-flags !
  \." " CR
  indecl? IF
    slot-all-empty? IF
      yield-stem  ( addr u )
    ELSE
      \." indeclinate stem but there are affixes: " DUP .stem-single CR
      DROP  ( addr u )
    THEN
  ELSE
    filters-check IF
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

:+ after-fallout  { pairlist rule n-rule -- }
  \." " pairlist ?DUP-IF indent ." Pairlist: " .pairlist
  \."  while processing " formname .dstack ."  " formform .dstack cr THEN
  BEGIN pairlist WHILE
    pairlist pair-1  ( ... addr' u' )
    \." " indent rule if ." Pair " pairlist .pairlist-node ." harmony variant: " rule execute . ." left, " n-rule . ." right" cr then
    pairlist pair-flags @ { slot-flag }
    n-rule rule rule-check { harmony-ok? }
    harmony-ok? NOT IF
      slot-flag 0= IF
        2DUP last-sound-except-ь voiced? IF
          PAD OVER { D: buffer }
          OVER buffer MOVE
          buffer last-sound-except-ь unvoice  buffer last-sound-except-ь-ptr XC!
          buffer n-rule rule rule-check TO harmony-ok? 2DROP
          harmony-ok? IF
            harmony-vu-broken TO slot-flag
          THEN
        THEN
      THEN
    THEN
    harmony-ok? IF
      slot-flag S>D <<# #s #> formflag >dstack
      \\." " indent ." Trying " 2DUP TYPE ." +" pairlist pair-2 TYPE ."  formflags " formflag .dstack CR
      parse-try  ( ... )
      \\." " indent ." out of parse-try" cr
      formflag dstack> slot-flag IF #>> THEN
    ELSE
      2DROP  ( ... )
    THEN
    pairlist list-swallow  TO pairlist
  REPEAT
  ;

:+ process-single-representation  ( addr u affix affix-len rule n-rule -- addr u )
  { D: affix rule n-rule }                 ( addr u )

  \stack-mark
  \\." " indent ." <Singlerep> " 2DUP type ." +" affix type rule HEX. n-rule . .s CR

  affix  formform >dstack
  0 { pairlist }
  affix string-length IF
    \ \." BEFORE:" .s CR
    2DUP affix untransform-fallout2 TO pairlist
    \ \." AFTER:" .s BL EMIT DUP .strlist CR
  THEN
  \\." " pairlist IF indent ." Remaining list: " pairlist .pairlist .s CR THEN
  pairlist rule n-rule after-fallout
  DUP  affix string-length > IF
    \\." " indent ." Unchanged guess: " 2DUP TYPE .s CR
    2DUP affix untransform-envoice  ( addr u pairlist )
    rule n-rule after-fallout                ( addr u )
    \\." " indent ." After after-fallout: " .s CR
  THEN
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
  parse-form
  debug-bye ;
