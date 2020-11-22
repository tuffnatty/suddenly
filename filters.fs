REQUIRE cleave.fs
REQUIRE dstack.fs

\ filters stack. Each element is 2 cells: xt negation
STRUCT
  CELL% FIELD filter-xt
  CELL% FIELD filter-negated
  CELL% FIELD filter-nt
END-STRUCT filter%

VARIABLE n-filters
CREATE filters filter% 64 * %ALLOT

: filters-top-ptr  ( -- ptr )
  n-filters @ 1- [ filter% %size ]L * filters + ;

: .filter  ( -- )
  filters-top-ptr bi[ filter-negated @ IF ." negated: " THEN
                   ][ filter-xt @ >BODY SEE-THREADED ]; ;

: >filters  ( nt xt f -- )
  1 n-filters +!
  filters-top-ptr { top }
  top filter-negated !  top filter-xt !  top filter-nt !  ( )
  \ \."  ADDED FILTER: " .filter cr
  ;

: filters-drop  ( -- )
  \ \."  DROPPING FILTER: " .filter cr
  ASSERT( n-filters @ 0> )  \ filter stack underflow
  -1 n-filters +! ;

: filters>  ( -- nt xt f )
  filters-top-ptr { top }
  top filter-nt @  top filter-xt @  top filter-negated @  ( nt xt f )
  filters-drop ;

: filter-else  ( -- )
  filters> NOT >filters ;

: filters(  ( <name>... -- | compile: filters-sys )
  0 { count }
  BEGIN
    PARSE-NAME { D: s }
  s S" )" COMPARE WHILE
    s string-length IF
      s FIND-NAME ?DUP-IF  ( nt )
        DUP IMMEDIATE?  IF  NAME?INT EXECUTE  THEN
        ( nt )  bi[ POSTPONE LITERAL
                 ][ NAME>INT POSTPONE LITERAL ];
      ELSE  1 ABORT"  word not found!"  THEN
      FALSE POSTPONE LITERAL
      POSTPONE >filters
      count 1+ TO count
    ELSE
      REFILL 0= ABORT"  no closing parenthesis"
    THEN
  REPEAT count ; IMMEDIATE

: filters-end  ( compile: filters-sys -- )
  NEGATE ]]L n-filters +! [[ ; IMMEDIATE

debug-mode? [IF]

  ' filters( ALIAS right-context( IMMEDIATE
  ' filter-else ALIAS right-context-else
  ' filters-end ALIAS right-context-end IMMEDIATE

[ELSE]

: right-context(  ( <name>... -- rhc-sys )
  DEPTH 0 { depth0 count }
  BEGIN
    PARSE-NAME { D: s }
  s S" )" COMPARE WHILE
    s string-length IF
      s FIND-NAME ?DUP-IF  ( nt )
        DUP IMMEDIATE? IF NAME?INT EXECUTE ( nt' ) THEN
        NAME>INT COMPILE, ]] IF [[
      ELSE 1 ABORT"  word not found!" THEN
      count 1+ TO count
    ELSE
      REFILL 0= ABORT"  no closing parenthesis"
    THEN
  REPEAT count ; IMMEDIATE
: right-context-else  ( if-sys rhc-sys -- if-sys rhc-sys )
  >R ]] ELSE [[ R> ; IMMEDIATE
: right-context-end  ( rhc-sys -- )
  0 DO ]] THEN [[ LOOP ; IMMEDIATE

[THEN]


: filters-check  ( stem -- stem f )
  \." hypothesis:  " DUP .stem-single
  \."  affixes:    " formform .dstack-enum cr
  \."  affixes(MP):" formform-morphonemic .dstack-enum cr
  \."  affix names:" formname .dstack-enum cr
  \."  slot flags: " formflag .dstack-enum cr
  \."  slots:      " .slots cr
  \."  flags:      " paradigm-flags flags. cr
  \."  filters:    "
  filters-top-ptr BEGIN DUP filters >= WHILE  { filter }
    \." " filter filter-nt @ .ID  filter filter-negated @ if ."  [negated]" then
    filter filter-xt @ checked-execute(0-1)   ( stem f )
    filter filter-negated @ NOT IF NOT THEN       ( f' )
    IF                                               ( )
      \." FAILED" cr
      FALSE EXIT
    THEN                                             ( )
    \." OK, "
    filter filter% %size -
    \ \." stem is now " over .stem-single cr
  REPEAT DROP TRUE
  \." " cr ." Filters check complete." cr
  ;
