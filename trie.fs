REQUIRE debugging.fs
REQUIRE memregion.fs

FALSE CONSTANT optimize-tries
FALSE CONSTANT optimize-compact-tries

STRUCT
  CELL% 256 * FIELD trie-children
  CELL%       FIELD trie-data
END-STRUCT trie%


CREATE tries-region  96000 trie% %SIZE *  region-make


: trie-new  ( -- trie )
  trie% %SIZE  tries-region  region-allot ;

: trie-forget  ( trie -- )
  bi[  tries-region region-here @  OVER - dup 0< abort" here is less than trie!" 0 FILL
   ][  tries-region region-here ! ]; ;

: trie-[]child  ( n trie -- subtrie )
  trie-children SWAP CELLS + @ ;

: trie-get-ref-ptr  ( addr u trie -- subtrie-ref-ptr )
  >R  ( addr u  R: trie )
  ?DUP-IF BEGIN
    OVER C@ CELLS  R> trie-children  +  ( addr u child-ref-ptr )
    >R 1 /STRING R>                   ( addr' u' child-ref-ptr )
    OVER 0= IF NIP NIP EXIT THEN
    DUP @ ?DUP-0=-IF                    ( addr u child-ref-ptr )
      trie-new  DUP >R  SWAP !              ( addr u  R: trie' )
    ELSE                           ( addr u child-ref-ptr trie )
      >R DROP
    THEN                                    ( addr u  R: trie' )
  AGAIN THEN ;

: trie-get  ( addr u trie -- trie' )
  >R  ( addr u  R: trie )
  BEGIN ?DUP WHILE
    OVER C@ CELLS  R> trie-children  +  ( addr u child-ref-ptr  R: )
    DUP @ ?DUP-0=-IF  ( addr u child-ref-ptr )
      trie-new  DUP >R  SWAP !  ( addr u  R: trie' )
    ELSE  ( addr u child-ref-ptr trie )
      >R DROP
    THEN  ( addr u  R: trie' )
    1 /STRING
  REPEAT DROP R> ;

: trie-get-data  ( addr u trie -- data )
  ]] trie-get trie-data @ [[ ; IMMEDIATE

: trie-find-prefix-full  ( addr u trie -- data prefix-len|0 )
  0 { trie prefix-len }
  BEGIN ?DUP WHILE
    OVER C@ CELLS  trie trie-children  +
    DUP @ ?DUP-0=-IF
      DROP 2DROP  trie trie-data @  prefix-len EXIT
    ELSE TO trie DROP THEN
    1 /STRING
    prefix-len 1+ TO prefix-len
  REPEAT DROP  trie trie-data @  prefix-len ;

optimize-tries 0= [IF]
: trie-find-prefix  ( addr u trie -- data )
  { trie }  ( addr u )
  BEGIN ?DUP WHILE
    OVER C@ trie trie-[]child ?DUP-0=-IF
      2DROP  trie trie-data @  EXIT
    ELSE TO trie THEN
    1 /STRING
  REPEAT DROP  trie trie-data @  ;

: trie-find  ( addr u trie -- data|0 )
  { trie }
  BEGIN ?DUP WHILE
    OVER C@ trie trie-[]child ?DUP-0=-IF
      2DROP 0 EXIT
    ELSE TO trie THEN
    1 /STRING
  REPEAT DROP  trie trie-data @ ;
[THEN]

: trie-put  ( mask addr u trie -- )
  trie-get trie-data  SWAP OVER @ OR  SWAP ! ;

: (.trie) { trie prefix-len -- }
  trie trie-data @ IF
    PAD prefix-len TYPE ." |"
  THEN
  256 0 DO
    I trie trie-[]child ?DUP-IF  ( subtrie )
      I  PAD prefix-len + C!
      prefix-len 1+ RECURSE  ( )
    THEN
  LOOP ;

: .trie  ( trie -- )
  DUP HEX. ." : "   0 (.trie) CR ;


STRUCT
  CELL%       FIELD compact-trie-end
  CELL%       FIELD compact-trie-start
  CELL%       FIELD compact-trie-data
END-STRUCT compact-trie%

CREATE compact-tries-region  170000 compact-trie% %SIZE *  region-make

: .compact-trie-raw  ( compact-trie -- )
  DUP HEX.
  ." : data " DUP compact-trie-data @ .
  ."  start " DUP compact-trie-start @ .
  ."  end " compact-trie-end @ . ;

: compact-trie-bounds  ( compact-trie -- end start )
  ]] compact-trie-end 2@ SWAP [[ ; IMMEDIATE COMPILE-ONLY

: compact-trie-bounds-asc  ( compact-trie -- start end)
  ]] compact-trie-end 2@ [[ ; IMMEDIATE COMPILE-ONLY

: compact-trie-[]child-ref  ( c compact-trie -- addr|0 )
  2DUP compact-trie-bounds-asc WITHIN IF
    SWAP OVER  compact-trie-start @  -  CELLS  ( compact-trie offset )
    [ compact-trie% %SIZE ]L + +  ( addr )
  ELSE 2DROP 0 THEN ;

: compact-trie-[]child  ( c compact-trie -- subtrie|0 )
  \\." getting child " over . ." from " dup .compact-trie-raw cr
  compact-trie-[]child-ref ?DUP-IF @ ELSE 0 THEN ;

: (.compact-trie)  { compact-trie prefix-len -- }
  compact-trie compact-trie-data @ IF
    PAD prefix-len TYPE ." |"
  THEN
  compact-trie compact-trie-bounds +DO
    I compact-trie compact-trie-[]child ?DUP-IF  ( subtrie )
      I  PAD prefix-len + C!
      prefix-len 1+ RECURSE  ( )
    THEN
  LOOP ;

: .compact-trie  ( compact-trie -- )
  DUP HEX. ." : "
  0 (.compact-trie) CR ;

: compact-trie-[]child!  ( x c compact-trie -- )
  compact-trie-[]child-ref ! ;

: compact-trie-collect-bounds  { trie compact-trie -- }
  256 0 DO
    I trie trie-[]child IF
      compact-trie compact-trie-start @  0= IF
        I  compact-trie compact-trie-start !
      THEN
      I 1+  compact-trie compact-trie-end !
    THEN
  LOOP ;

optimize-compact-tries 0= [IF]
: compact-trie-find-prefix  ( addr u compact-trie -- data )
  { compact-trie }  ( addr u )
  BEGIN ?DUP WHILE
    \stack-mark
    \." searching " 2dup type ." in " compact-trie .compact-trie
    OVER C@ compact-trie compact-trie-[]child ?DUP-0=-IF
      \stack-check 2DROP  compact-trie compact-trie-data @  EXIT
    ELSE  TO compact-trie  THEN
    1 /STRING
    \stack-check
  REPEAT DROP  compact-trie compact-trie-data @ ;

: compact-trie-find  ( addr u compact-trie -- data )
  { compact-trie }  ( addr u )
  BEGIN ?DUP WHILE
    \stack-mark
    \." searching " 2dup type ." in " compact-trie .compact-trie
    OVER C@ compact-trie compact-trie-[]child ?DUP-0=-IF
      \stack-check 2DROP  0 EXIT
    ELSE  TO compact-trie  THEN
    1 /STRING
    \stack-check
  REPEAT DROP  compact-trie compact-trie-data @ ;

[ELSE]

c-library trie_lib
\c #include <stdint.h>
optimize-tries [IF]
\c typedef struct Trie {
\c    struct Trie *children[256];
\c    int data;
\c } Trie;
\c int trie_find(uint8_t *addr, int len, Trie *trie) {
\c    Trie *child;
\c    while (len && (child = trie->children[*addr])) {
\c       trie = child;
\c       addr++, len--;
\c    }
\c    return len ? 0 : trie->data;
\c }
\c int trie_find_prefix(uint8_t *addr, int len, Trie *trie) {
\c    Trie *child;
\c    while (len && (child = trie->children[*addr])) {
\c       trie = child;
\c       addr++, len--;
\c    }
\c    return trie->data;
\c }
[THEN]
\c typedef struct CompactTrie {
\c    int end, start, data;
\c } CompactTrie;
\c int compact_trie_find(uint8_t *addr, int len, CompactTrie *trie) {
\c    #pragma GCC unroll 4
\c    while (len && trie->start != trie->end) {
\c       uint8_t c = *addr;
\c       if (c < trie->start || c >= trie->end) {
\c          break;
\c       }
\c       CompactTrie *child = ((CompactTrie **)&trie[1])[c - trie->start];
\c       if (!child) {
\c          break;
\c       }
\c       trie = child;
\c       addr++, len--;
\c    }
\c    return len ? 0 : trie->data;
\c }
\c int compact_trie_find_prefix(uint8_t *addr, int len, CompactTrie *trie) {
\c    #pragma GCC unroll 4
\c    while (len && trie->start != trie->end) {
\c       uint8_t c = *addr;
\c       if (c < trie->start || c >= trie->end) {
\c          break;
\c       }
\c       CompactTrie *child = ((CompactTrie **)&trie[1])[c - trie->start];
\c       if (!child) {
\c          break;
\c       }
\c       trie = child;
\c       addr++, len--;
\c    }
\c    return trie->data;
\c }
optimize-tries [IF]
c-function trie-find trie_find a n a -- n
c-function trie-find-prefix trie_find_prefix a n a -- n
[THEN]
c-function compact-trie-find compact_trie_find a n a -- n
c-function compact-trie-find-prefix compact_trie_find_prefix a n a -- n
end-c-library
[THEN]

: trie-compact  { trie -- compact-trie }
  \stack-mark
  \\." trie-compact: " trie .trie compact-tries-region .region
  compact-tries-region region-here @ { compact-trie }
  trie trie-data @  compact-trie compact-trie-data !
  trie compact-trie compact-trie-collect-bounds
  compact-trie compact-trie-bounds  ( end start )
  \\." bounds" 2dup . . cr
  2DUP - CELLS  compact-trie% %SIZE  +  compact-tries-region region-allot DROP
  2DUP <> IF
    DO   ( )
      I trie trie-[]child ?DUP-IF  ( subtrie )
        \\." subtrie " I . ." : " dup .trie
        RECURSE  I compact-trie compact-trie-[]child! ( )
      THEN
    LOOP
  ELSE 2DROP THEN
  \stack-check
  compact-trie ;
