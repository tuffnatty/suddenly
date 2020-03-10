language-require rules.fs

: morphonemic-get-rules  ( addr u -- rule )
  0 { rule-sum }
  BEGIN DUP 0> WHILE                              ( addr' u' )
    \ ." At: " 2dup type ."  rule-sum: " rule-sum >NAME ?DUP-IF .ID ELSE ."  0" THEN CR
    2DUP morphoneme-find IF                    ( addr' u' xt )
      \ ." FOUND " dup >NAME .ID ~~ CR
      morphoneme-rule rule-sum rule+ TO rule-sum
      \ ." ADVANCING " ~~
      2DUP morphoneme-width /STRING
      \ ." TO " 2dup type CR
    ELSE
      +X/STRING
    THEN
  REPEAT 2DROP rule-sum ;

: morphonemic-sstr-prepare  { capacity len -- sstr }
  len 1+ 1 INVERT AND TO len
  capacity  len 1+  * 1- { buf-len }
  buf-len sstr-create { sstr }
  sstr cstr-get BLANK
  capacity  sstr sstr-count  !
  sstr sstr-allocate-arr
  sstr sstr-arr @  sstr cstr-ptr @  ( arr-ptr start )
  capacity 0 DO { arr-ptr start }                 ( )
    start  arr-ptr cstr-ptr !
    0  arr-ptr cstr-len !
    arr-ptr  cstr% %SIZE  +              ( arr-ptr' )
    start len + 1+                ( arr-ptr' start' )
  LOOP 2DROP sstr ;

: morphonemic-to-sstr-and-rule  { D: morphonemic -- sstr rule }
  \ ." morphonemic-to-sstr-and-rule:" morphonemic type cr
  morphonemic string-length 0= IF 0 0 EXIT THEN
  morphonemic morphonemic-get-rules { rule-sum }
  \ ." rule-sum: " rule-sum >NAME ?DUP-IF .NAME THEN
  rule-sum rule-capacity  { capacity }
  capacity  morphonemic string-length  morphonemic-sstr-prepare { sstr }
  morphonemic string-create  sstr sstr-morphonemic 2!
  morphonemic BEGIN DUP 0> WHILE                    ( addr' u' )
    \ ." at " 2dup type ." >"
    2DUP morphoneme-find IF { xt }
      \ ." FOUND " xt >NAME .ID CR
      sstr sstr-arr @                       ( addr' u' arr-ptr )
      capacity 0 DO { arr-ptr }                     ( addr' u' )
        OVER  morphonemic string-addr  > IF
          2DUP arr-ptr morphoneme-get TO xt
        THEN
        xt morphoneme-rule { rule }
        \ ." RULE " rule >NAME .ID CR
        \ ." VARIANT " I . ." arr-ptr: " arr-ptr HEX. CR
        xt  I rule-sum rule rule-index-convert  morphoneme-choose-variant { xc }
        \ ." XC " xc HEX. xc xemit CR
        xc [CHAR] 0 <> IF xc arr-ptr cstr-append-xc THEN
        \ ." arr-ptr" arr-ptr .cstr cr
        arr-ptr  cstr% %SIZE +             ( addr' u' arr-ptr' )
      LOOP DROP                                     ( addr' u' )
      morphoneme-skip                             ( addr'' u'' )
    ELSE
      OVER XC@ { xc }
      sstr sstr-arr @                       ( addr' u' arr-ptr )
      capacity 0 DO { arr-ptr }                     ( addr' u' )
        xc arr-ptr cstr-append-xc
        arr-ptr  cstr% %SIZE +                      ( arr-ptr' )
      LOOP DROP                                     ( addr' u' )
      xc XC-SIZE /STRING                          ( addr'' u'' )
    THEN
  REPEAT
  2DROP sstr rule-sum
  \ ." morphonemic-to-sstr:out " over .sstr dup if dup >name .id else 0 . then cr
  ;
