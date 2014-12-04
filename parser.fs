REQUIRE slot-stack.fs
REQUIRE dictionary.fs
REQUIRE grammar.fs
REQUIRE strings.fs
REQUIRE filters.fs

:noname ; IS (slot-prolog)

:noname ; IS (slot-epilog)

\ Buffers for delimited form names and affixes
CREATE formname bstr% %ALLOT bstr-init
CREATE formform bstr% %ALLOT bstr-init
: form-prepend  ( addr u bstr | 0 bstr -- )
  >R ?DUP-IF R@ bstr-prepend THEN  ( R: bstr )
  S" -" R> bstr-prepend
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
: rule-cv-fb ['] rule-cv-fb ;
: rule-fb ['] rule-fb ;
: rule-nvu ['] rule-nvu ;
: rule-nvu-fb ['] rule-nvu-fb ;
: rule-vu ['] rule-vu ;
: rule-vu-fb ['] rule-vu-fb ;
[IFDEF] rule-fbl
  : rule-fbl ['] rule-fbl ;
  : rule-cv-fbl ['] rule-cv-fbl ;
[ENDIF]

REQUIRE loaddefs.fs

: check-stem  ( stem -- )
  DUP stem-p-o-s paradigm-p-o-s !
  \." about to check filters for: " formname .bstr CR DUP .stem-single CR
  filters-check IF
    ." FOUND STEM: " formname .bstr SPACE formform .bstr CR .stem-single CR
    1 n-forms +!
  ELSE DROP THEN ;

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

\ an awful big word
:noname  ( addr u rule sstr -- addr u )
  \." " parse-depth 1+ TO parse-depth
  \." " indent ." form-epilog " 2>R 2dup type 2R> dup .sstr ."  rule=" over .rule ." ]" cr
  { rule sstr }  ( addr u )
  sstr IF
    sstr sstr-count @ 0 DO
      I sstr sstr-select { D: affix }
      affix formform form-prepend
      2DUP affix untransform-fallout  ( addr u strlist )
      BEGIN DUP WHILE
        DUP list-next @ 0=  { unchanged? }
        DUP strlist-get  { D: left-part }
        unchanged? IF
          left-part affix untransform-envoice  ( ... pairlist )
        ELSE
          \." " indent ." After fallout check with affix " affix TYPE ." : " 2DUP TYPE CR
          0  left-part  affix string-length  string-strip  affix pairlist-prepend  ( ... pairlist )
          \." " indent ." Pairlist: " dup pair-1 type ." +" dup pair-2 type cr
        THEN
        BEGIN DUP WHILE
          DUP pair-1  ( ... pairlist addr' u' )
          \\." " rule if rule execute . ." expected, " i . ." actual" cr then
          I rule rule-check IF
            \." " indent ." Trying " 2DUP TYPE ." +" affix TYPE CR
            parse-try
            \." " indent ." out of parse-try, processing " >r 2dup type r> cr
          ELSE 2DROP THEN  ( ... pairlist )
          list-swallow  ( ... pairlist' )
        REPEAT DROP  ( ... )
        list-swallow  ( addr u strlist' )
      REPEAT DROP  ( addr u )
      formform bstr-pop
    LOOP
    \\." " cr
  ELSE
    \." " indent ." Trying 0 " 2dup type ." +0" CR
    0 formform form-prepend
    2DUP parse-try
    formform bstr-pop
    \\." " indent ." out of parse-try" CR
  THEN
  \\." " indent ." form-epilog ending:" 2dup type cr
  \." " parse-depth 1- TO parse-depth
  formname bstr-pop
  ; IS (form-epilog)

: parse-yield  ( cs1 cs2 -- )
  COUNT TYPE 9 EMIT COUNT TYPE 1 n-forms +! ;

: parse-form  ( cs -- )
  \ untransform2  ( cs1..csN n )  0 ?DO parse-one LOOP
  COUNT parse-try ;

: parse-khak  ( cs -- )
  debug-init
  slot-stack /slot-stack slot-stack-set
  slot-stack-reverse
  parse-form
  debug-bye ;
