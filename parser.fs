REQUIRE slot-stack.fs
REQUIRE dictionary.fs
REQUIRE grammar.fs
REQUIRE strings.fs
REQUIRE filters.fs

:noname
  ; IS (slot-prolog)

:noname ; IS (slot-epilog)

\ Buffers for delimited form names and affixes
CREATE formname bstr% %ALLOT bstr-init
CREATE formform bstr% %ALLOT bstr-init
: form-prepend  ( addr u bstr | 0 bstr -- )
  >R ?DUP-IF R@ bstr-prepend THEN  ( R: bstr )
  S" -" R> bstr-prepend
  ;

: form-get-slot  ( n bstr -- addr u )
  SWAP >R  ( bstr  R: n )
  cstr-get BOUNDS BEGIN  ( end ptr )
    2DUP > WHILE
      DUP C@ [CHAR] - = IF R> 1- >R THEN  ( end ptr R: n' )
    R@ WHILE
    1+                                    ( end ptr' )
  REPEAT THEN
  RDROP DUP -ROT -  ( addr u  R: )
  ;

: form-slot-xc-at-left  ( n -- xc )
  formform form-get-slot DROP XCHAR- BEGIN  ( addr' )
    DUP formform cstr-ptr @ > WHILE
    DUP C@ [CHAR] - = WHILE
      XCHAR-
  REPEAT THEN
  DUP formform cstr-ptr @ <= IF
    DROP paradigm-stem 2@ last-sound           ( xc )
  ELSE XC@ THEN                                ( xc )
  ;

: current-form-is-poss?  ( -- f )
  formname cstr-get  2 /STRING  S" pos" STRING-PREFIX? ;

REQUIRE transforms.fs

\ Trace output
REQUIRE debug.fs
0 VALUE parse-depth
: indent
  parse-depth spaces ;

: affix-name-clean ( affix-name len -- affix-name len | 0 )
  OVER C@  [CHAR] -  = IF  2DROP 0  THEN ;

:noname  ( [addr u] affix-name len -- [addr u] )
  \." " indent ." form-prolog: " 2>R 2dup type bl emit 2R> 2dup type cr
  affix-name-clean formname form-prepend  ( )
  ; IS (form-prolog)

REQUIRE rules.fs
REQUIRE loaddefs.fs

: yield-stem  ( stem -- )
  ." FOUND STEM: " formname .bstr SPACE formform .bstr CR .stem-single CR
  1 n-forms +! ;

: check-stem  ( addr u stem -- addr u )
  >R 2DUP paradigm-stem 2! R>
  DUP stem-p-o-s paradigm-p-o-s !
  DUP stem-dict @ dict-stems @  paradigm-stems !
  indecl? IF
    slot-all-empty? IF
      yield-stem
    ELSE
      \." indeclinate stem but there are affixes: " DUP .stem-single CR
      DROP
    THEN
  ELSE
    \." about to check filters for: " formname .bstr CR DUP .stem-single CR
    filters-check IF
      yield-stem
    ELSE DROP THEN
  THEN ;

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
      \." STEM " 2dup type ."  not in dictionary! was trying " formname .bstr cr
    THEN
    2DROP
  THEN
  slot-stack-push ;

: rule-check  ( i xt | 0 -- f )
  DUP IF SWAP >R EXECUTE R> =
  ELSE 2DROP TRUE THEN ;

: process-single-representation  ( addr u affix affix-len rule n-rule -- addr u )
  { D: affix rule n-rule }  ( addr u )
  affix formform form-prepend
  2DUP affix untransform-fallout  ( addr u strlist )
  BEGIN DUP WHILE
    DUP list-next @ 0=  { unchanged? }
    DUP strlist-get  { D: left-part }
    unchanged? IF
      left-part affix untransform-envoice  ( ... pairlist )
    ELSE
      \." " indent ." After fallout check with affix " affix TYPE ." : " left-part TYPE CR
      0  left-part  affix string-length  string-strip  affix pairlist-prepend  ( ... pairlist )
      \." " indent ." Pairlist: " dup pair-1 type ." +" dup pair-2 type cr
    THEN
    BEGIN DUP WHILE
      DUP pair-1  ( ... pairlist addr' u' )
      \." " rule if rule execute . ." expected, " n-rule . ." actual" cr then
      n-rule rule rule-check IF
        \." " indent ." Trying " 2DUP TYPE ." +" affix TYPE CR
        parse-try  ( ... pairlist )
        \." " indent ." out of parse-try" cr
      ELSE 2DROP THEN  ( ... pairlist )
      list-swallow  ( ... pairlist' )
    REPEAT DROP  ( ... )
    list-swallow  ( addr u strlist' )
  REPEAT DROP  ( addr u )
  formform bstr-pop ;

: process-representations  ( addr u rule sstr -- )
  { rule sstr }                         ( addr u )
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
  \." " indent ." form-epilog " 2>r 2dup type bl emit 2r> 2dup .sstr .rule cr
  { rule sstr }  ( addr u )
  sstr IF  \ Non-empty affix
    rule sstr process-representations
  ELSE
    \." " indent ." Trying 0 " 2dup type ." +0" CR
    0 formform form-prepend
    2DUP parse-try
    formform bstr-pop
    \\." " indent ." out of parse-try" CR
  THEN
  \." " parse-depth 1- TO parse-depth
  \\." " indent ." form-epilog ending:" 2dup type cr
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
