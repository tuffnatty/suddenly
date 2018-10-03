REQUIRE dictionary.fs
REQUIRE slot-stack.fs
REQUIRE grammar.fs
REQUIRE strings.fs

:noname
  \stack-mark
  ; IS (slot-prolog)

:noname
  \stack-check
  ; IS (slot-epilog)

\ Buffers for delimited form names and affixes
CREATE formname bstr% %ALLOT bstr-init
CREATE formform bstr% %ALLOT bstr-init
CREATE formflag bstr% %ALLOT bstr-init
: form-prepend  ( addr u bstr | 0 bstr -- )
  >R ?DUP-IF R@ bstr-prepend THEN  ( R: bstr )
  S" -" R> bstr-prepend
  ;

: form-get-slot  ( n bstr -- addr u )
  SWAP >R  ( bstr  R: n )
  cstr-get BOUNDS BEGIN  ( end ptr )
    2DUP > WHILE
      DUP C@ [CHAR] - = IF R> 1- >R THEN  ( end ptr R: n' )
      1+                                  ( end ptr' )
    R@ WHILE
  REPEAT THEN
  RDROP DUP -ROT -  ( addr u  R: )
  ;

: form-first-full-slot  ( bstr -- n )
  cstr-get OVER >R  BOUNDS BEGIN  ( end ptr  R: start )
    2DUP > WHILE
    DUP C@ [CHAR] - = WHILE
    1+
  REPEAT THEN
  2DUP > IF NIP R> - ELSE RDROP 2DROP 0 THEN ;

: stem-polysyllabic?  ( -- f )
  guessed-stem polysyllabic? ;

: stem-prev-sound  ( -- xc )
  guessed-stem prev-sound ;

: stem-prev-sound-ptr  ( -- addr u )
  guessed-stem prev-sound-ptr ;

: stem-last-sound  ( -- xc )
  guessed-stem last-sound ;

: form-slot  ( n -- addr u )
  formform form-get-slot ;

: form-slot-xc-at-left  ( n -- xc )
  formform cstr-ptr @ { form-start }
  formform form-get-slot string-addr XCHAR- BEGIN  ( addr' )
    DUP form-start > WHILE
    DUP C@ [CHAR] - = WHILE
      XCHAR-
  REPEAT THEN
  DUP form-start <= IF
    DROP stem-last-sound                       ( xc )
  ELSE XC@ THEN                                ( xc )
  ;

: first-affix  ( -- addr u )
  formform form-first-full-slot ?DUP-IF  ( n )
    formform form-get-slot
  ELSE $0. THEN ;
  
: first-affix-starts-with?  ( xc -- f )
  first-affix first-sound = ;

: form-get-flags  ( addr u -- flags )
  OVER C@ [CHAR] - = IF
    2DROP 0  ( flags )
  ELSE $0. 2SWAP >NUMBER 2DROP D>S THEN ;

: form-slot-flags  ( n -- flags )
  formflag form-get-slot form-get-flags ;

: first-form-flag  ( -- f )
  formflag form-first-full-slot form-slot-flags ;

: form-flag-is?  ( n flag -- f )
  >R form-slot-flags R> AND ;

: any-form-flag-is?  { flag -- f }
  formflag cstr-get BOUNDS BEGIN 2DUP > WHILE  ( end ptr )
    2DUP form-get-flags flag AND IF 2DROP TRUE EXIT THEN
  1+ REPEAT 2DROP FALSE ;

: form-slot-vowel-at-left?  ( n_slot -- f )
  form-slot-xc-at-left vowel? ;

: current-form-is-poss?  ( -- f )
  formname cstr-get  2 /STRING  S" pos" STRING-PREFIX? ;

REQUIRE filters.fs

language-require transforms.fs

\ Trace output
REQUIRE debugging.fs
0 VALUE parse-depth
: indent  ( -- )
  parse-depth spaces ;

: affix-name-clean  ( affix-name len -- affix-name len | 0 )
  OVER C@  [CHAR] -  = IF  2DROP 0  THEN ;

:noname  ( [addr u] affix-name len -- [addr u] )
  \\." " indent ." form-prolog: " 2over type ." +" 2dup type \.s
  affix-name-clean formname form-prepend  ( )
  ; IS (form-prolog)

language-require rules.fs
REQUIRE loaddefs.fs

DEFER yield-stem  ( stem -- )
:noname  ( stem -- )
  ." FOUND STEM: " formname .bstr SPACE formform .bstr CR .stem-single CR  ( )
  1 n-forms +! ; IS yield-stem

: check-stem  ( addr u stem -- addr u )
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

: parse-try  ( addr u -- )
  \ ." stackin " .s 2dup type cr
  slot-stack-pop ?DUP-IF  ( addr u xt )
    \ ." stacknext! "
    EXECUTE  ( should go through affix-position trying to strip off affix and calling parse-try on success )
    2DROP
  ELSE  ( addr u )
    \ ." stackdone!" .s 2dup type cr
    2DUP stem-find ?DUP-IF  ( addr u stem )
      ['] check-stem  list-map  ( addr u )
      \." ~~~~~~~~~" cr
    ELSE
      \." STEM " 2dup type ."  not in dictionary! was trying " 2dup type formform .bstr bl emit formname .bstr cr
    THEN
    2DROP
  THEN
  slot-stack-push
  ;

: rule-check  ( i xt | 0 -- f )
  ?DUP-IF SWAP >R EXECUTE ( rule-result ) R> =
  ELSE DROP TRUE THEN ;

: after-fallout  { pairlist rule n-rule -- }
  \." " pairlist ?DUP-IF indent ." Pairlist: " .pairlist
  \."  while processing " formname cstr-get type ."  " formform cstr-get type cr THEN
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
      slot-flag S>D <<# #s #> formflag form-prepend #>>
      \\." " indent ." Trying " 2DUP TYPE ." +" pairlist pair-2 TYPE ."  formflags " formflag cstr-get type CR
      parse-try  ( ... )
      \\." " indent ." out of parse-try" cr
      formflag bstr-pop
    ELSE
      2DROP  ( ... )
    THEN
    pairlist list-swallow  TO pairlist
  REPEAT
  ;

: process-single-representation  ( addr u affix affix-len rule n-rule -- addr u )
  { D: affix rule n-rule }                 ( addr u )
  \\." " indent ." <Singlerep> " 2DUP type ." +" affix type rule HEX. n-rule . .s CR
  affix formform form-prepend
  0 { pairlist }
  affix string-length IF
    \ \." BEFORE:" .s CR
    2DUP affix untransform-fallout2 TO pairlist
    \ \." AFTER:" .s BL EMIT DUP .strlist CR
  THEN
  \\." " pairlist IF indent ." Remaining list: " pairlist .pairlist .s CR THEN
  pairlist rule n-rule after-fallout
  \\." " indent ." Unchanged guess: " 2DUP TYPE .s CR
  2DUP affix untransform-envoice  ( addr u pairlist )
  rule n-rule after-fallout                ( addr u )
  \\." " indent ." After after-fallout: " .s CR
  formform bstr-pop
  ;

: process-representations  ( addr u rule sstr -- )
  { rule sstr }                         ( addr u )
  \\." " indent ." <All-reps>" 2DUP TYPE ." +" sstr .sstr rule HEX. .s CR
  sstr sstr-count @ 0 DO
    I sstr sstr-select { D: affix }
    affix string-length IF
      affix rule I process-single-representation
    THEN
  LOOP
  \\." " cr
  ;

\ an awful big word
:noname  ( addr u rule sstr -- addr u )
  \." " parse-depth 1+ TO parse-depth
  \\." " indent ." form-epilog " 2>r 2dup type ." +" 2r> 2dup .sstr .rule \.s cr
  { rule sstr }  ( addr u )
  sstr IF  \ Non-empty affix
    rule sstr process-representations
  ELSE
    \\." " indent ." Trying 0 " 2dup type ." +0" CR
    0 formform form-prepend
    0 formflag form-prepend
    2DUP parse-try
    formform bstr-pop
    formflag bstr-pop
    \\." " indent ." out of parse-try" CR
  THEN
  \." " parse-depth 1- TO parse-depth
  \\." " indent ." form-epilog ending:" over HEX. dup . 2dup type \.s
  formname bstr-pop
  ; IS (form-epilog)

: parse-yield  ( cs1 cs2 -- )
  COUNT TYPE  9 EMIT  COUNT TYPE  1 n-forms +! ;

: parse-form  ( cs -- )
  \ untransform2  ( cs1..csN n )  0 ?DO parse-one LOOP
  COUNT parse-try ;

: parse-khak  ( cs -- )
  debug-init
  slot-stack /slot-stack slot-stack-set
  slot-stack-reverse
  parse-form
  debug-bye ;
