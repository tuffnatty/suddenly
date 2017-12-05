\ filters stack. Each element is 2 cells: xt negation
STRUCT
  CELL% FIELD filter-xt
  CELL% FIELD filter-negated
  CELL% FIELD filter-nt
END-STRUCT filter%

VARIABLE n-filters
CREATE filters filter% 64 * %ALLOT

: filters-top-ptr  ( -- ptr )
  n-filters @ 1- filter% %size * filters + ;

: .filter  ( -- )
  filters-top-ptr DUP filter-negated @ IF ." negated: " THEN  ( top )
  filter-xt @ >BODY SEE-THREADED ;

: >filters  ( nt xt f -- )
  1 n-filters +!
  filters-top-ptr { top }
  top filter-negated !  top filter-xt !  top filter-nt !  ( )
  \\."  ADDED FILTER: " .filter cr
  ;

: filters-drop  ( -- )
  \\."  DROPPING FILTER: " .filter cr
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
      s FIND-NAME ?DUP-IF       ( nt )
        DUP POSTPONE LITERAL
        NAME>INT POSTPONE LITERAL  ( )
      ELSE 1 ABORT"  word not found!" THEN
      FALSE POSTPONE LITERAL
      POSTPONE >filters
      count 1+ TO count
    ELSE
      REFILL 0= ABORT"  no closing parenthesis"
    THEN
  REPEAT count ; IMMEDIATE

: filters-end  ( compile: filters-sys -- )
  NEGATE ]]L n-filters +! [[ ; IMMEDIATE

: filters-check  ( stem -- stem f )
  \." hypothesis:  " DUP .stem-single
  \."  affixes:    " formform .bstr cr
  \."  affix names:" formname .bstr cr
  \."  slot flags: " formflag .bstr cr
  \."  slots:      " .slots cr
  \."  flags:      " paradigm-flags flags. cr
  \."  filters:    "
  filters-top-ptr BEGIN DUP filters >= WHILE  { filter }
    \stack-mark
    \." " filter filter-nt @ .ID  filter filter-negated @ if ."  [negated]" then
    filter filter-xt @ EXECUTE                ( stem f )
    filter filter-negated @ NOT IF NOT THEN       ( f' )
    IF                                               ( )
      \." FAILED" cr
      \stack-check
      FALSE EXIT
    THEN                                             ( )
    \." OK, "
    \stack-check
    filter filter% %size -
    \\." stem is now " over .stem-single cr
  REPEAT DROP TRUE
  \." " cr
  ;
