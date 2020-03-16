REQUIRE debugging.fs
REQUIRE strings.fs
REQUIRE trie.fs

GET-CURRENT  ( wid )
VOCABULARY triere ALSO triere DEFINITIONS

\ Trie-based pattern matching:

DEFER (t~/)

: string-parse  ( addr u c -- addr+u' u-u' addr u' )
  { D: orig  c }                            ( )
  orig BEGIN DUP WHILE             ( addr' u' )
    OVER C@  c  <>  WHILE
      1 /STRING
  REPEAT THEN
  OVER  orig string-addr -  { len }
  DUP IF  1 /STRING  THEN  \ skip delimiter
  orig string-addr len ;

: (t~/)-doclass  { D: rest  wid  trie -- }
  \ Build subtrie for the first sound in wid.
  \ Refer to that subtrie for every sound in wid.
  \ \." (t~/)-doclass " rest type cr
  0 { child }
  wid sound-each-str  ( c-addr c-u )
    child IF
      trie trie-get-ref-ptr  child  SWAP !
    ELSE
      trie trie-get TO child      ( )
      rest child (t~/)
    THEN
  sound-next
  \ \." doclass-done" cr
  ;

: parse-class  ( addr u -- wid )
  [CHAR] } string-parse   FIND-NAME NAME>INT EXECUTE ;

0 VALUE (t~/)-data

:noname  ( addr u trie -- )
  \ \." (t~/) " >r 2dup type ." ," r> dup .trie ." ." cr
  { trie }  ( addr u )
  DUP IF
    OVER C@ CASE
      [CHAR] { OF   1 /STRING   parse-class trie (t~/)-doclass   ENDOF
      ( c )  >R                               ( addr u  R: c )
        OVER 1 trie trie-get   >R 1 /STRING R>   RECURSE   ( )
        R>                                               ( c )
    ENDCASE
  ELSE
    2DROP
    (t~/)-data trie trie-data !
  THEN
  ; IS (t~/)

( wid ) SET-CURRENT  \ public words follow

: (t~/-union-trie-put)  ( addr u trie -- )
  { trie }  ( addr u )
  BEGIN DUP WHILE
    [CHAR] | string-parse trie (t~/)
  REPEAT 2DROP ;

: (t~/-union)  ( addr u -- compact-trie )
  trie-new { trie }
  trie (t~/-union-trie-put)  ( )
  trie trie-compact  ( compact-trie )
  trie trie-forget
  ;

: twolevel-trie-put  ( addr u trie -- )
  { trie }       ( addr u )
  \\." twolevel-trie-put " 2DUP TYPE trie .trie CR
  TRUE TO (t~/)-data
  2DUP (t~/-union) TO (t~/)-data
  \\." made compact-trie " (t~/)-data .compact-trie CR
  trie (t~/-union-trie-put) ;

: t~/  ( addr u "regex" -- f )
  \stack-mark
  TRUE TO (t~/)-data
  BL PARSE  (t~/-union) ]]L compact-trie-find-prefix [[
  \stack-check
  ; IMMEDIATE COMPILE-ONLY

PREVIOUS
