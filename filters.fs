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

: filter-start(
  POSTPONE :[ ; IMMEDIATE

: )
  POSTPONE ]; FALSE POSTPONE LITERAL POSTPONE >filters ; IMMEDIATE

: filter-else  ( -- )
  filters> NOT >filters ;

: filter-end  ( -- )
  POSTPONE filters-drop ; IMMEDIATE

: filters-check  ( stem -- stem f )
  \." flags: " paradigm-flags @ 2 base ! . ."  slots:" paradigm-slot-bitmap @ . decimal cr
  filters-top-ptr BEGIN DUP filters > WHILE
    DUP 2@ >R
    \." checking filter: " dup >name ?dup-if .id else dup xt-see then r@ if ."  negated" then cr
    EXECUTE
    R> NOT IF NOT THEN
    IF
      \." filter check failed" cr
      DROP FALSE EXIT
    THEN
    2 CELLS -
  REPEAT DROP TRUE ;
