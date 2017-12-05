\ filters stack. Each element is 2 cells: xt negation
CREATE filters 0 , 64 CELLS ALLOT

: filters-top-ptr  ( -- ptr )
  filters @ 2* CELLS filters + ;

: .filter  ( -- )
  filters-top-ptr 2@ IF ." negated: " THEN >BODY SEE-THREADED ;

: >filters  ( xt f -- )
  filters @ 1+ filters !
  filters-top-ptr 2!
  \\."  ADDED FILTER: " .filter cr
  ;

: filters-drop  ( -- )
  \\."  DROPPING FILTER: " .filter cr
  filters @  ( depth )
  ASSERT( DUP 0> )  \ filter stack underflow
  1- filters ! ;

: filters>  ( -- xt f )
  filters-top-ptr 2@ filters-drop ;

: filter-start(:  ( <name> -- )
  POSTPONE :[: ; IMMEDIATE

: ;)
  POSTPONE ]; FALSE POSTPONE LITERAL POSTPONE >filters ; IMMEDIATE

: filter-start  ( <name> -- )
  POSTPONE [']  FALSE POSTPONE LITERAL  POSTPONE >filters ; IMMEDIATE

: filter-else  ( -- )
  filters> NOT >filters ;

: filter-end  ( -- )
  POSTPONE filters-drop ; IMMEDIATE

: filters(  ( <name>... -- | compile: filters-sys )
  0 { count }
  BEGIN
    PARSE-NAME { D: s }
  s S" )" COMPARE WHILE
    s string-length IF
      s FIND-NAME ?DUP-IF NAME>INT POSTPONE LITERAL ELSE 1 ABORT"  word not found!" THEN
      FALSE POSTPONE LITERAL
      POSTPONE >filters
      count 1+ TO count
    ELSE
      REFILL 0= ABORT"  no closing parenthesis"
    THEN
  REPEAT count ; IMMEDIATE

: filters-end  ( compile: filters-sys -- )
  NEGATE ]]L filters +! [[ ; IMMEDIATE

: filters-check  ( stem -- stem f )
  \." flags: " paradigm-flags flags. ."  slots:" paradigm-slot-bitmap @ bin. ." transform-flags:" formflag cstr-get type cr
  \." checking "
  filters-top-ptr BEGIN DUP filters > WHILE  ( stem filter-ptr )
    \stack-mark
    DUP 2@ >R
    \." " dup .xt r@ if ."  [negated]" then
    EXECUTE
    R> NOT IF NOT THEN
    IF
      \."  FAIL" cr
      \stack-check
      DROP FALSE EXIT
    ELSE
      \." , "
      \stack-check
    THEN
    2 CELLS -
    \\." stem is now " over .stem-single cr
  REPEAT DROP TRUE
  \." " cr
  ;
