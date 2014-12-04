REQUIRE rules.fs


: possibly-fugitive?  ( addr u -- f)
  last-sound nasal-or-glide-or-z? ;

: untransform-envoice  ( addr u affix len -- pairlist )
  \\." unjoining " 2dup type
  2DUP { affix-ptr len  D: affix }               ( addr u )
  \\."  from " 2dup type ." ..."
  0 { list }
  affix-ptr XC@                             ( addr u aff1 )
  DUP vowel? IF
    DROP                                         ( addr u )
    \ \." checking if " 2dup type ."  ends in " affix type cr
    2DUP affix string-ends IF
      \ \." yes it does" cr
      len -                                     ( addr u' )
      \ \." checking if " 2dup type ." [-2] is vowel" cr
      2DUP prev-sound vowel? IF
        \ \." yes" cr
          2DUP list -ROT affix pairlist-prepend TO list
          2DROP                                         ( )
      ELSE
         \." no" cr
        \ check for fugitive
        \ current-form-is-poss? IF  \ any form in Old Turkic
          affix-ptr XC@  vowel? IF
            2DUP possibly-fugitive? IF
              2DUP list -ROT affix pairlist-prepend TO list
              list pair-1 last-sound-ptr >R  ( addr u'  R: endptr )
              R@ XC@  R@ CHAR+  XC!
              1 CHARS  list pair-1-len +!
              [CHAR] u  R> XC!                  ( addr u' )
              list list pair-1 affix pairlist-prepend TO list
              [CHAR] i  list pair-1 prev-sound-ptr  XC!
              list list pair-1 affix pairlist-prepend TO list
              list pair-1 last-sound-ptr >R     ( addr u'  R: endptr )
              R@ XC@  R@ XCHAR+  XC!
              1 CHARS  list pair-1-len +!
              [CHAR] ï  R> 1 CHARS - XC!
              list list pair-1 affix pairlist-prepend TO list
              [CHAR] ü  list pair-1 prev-sound-ptr  XC!
        THEN THEN \ THEN
        list -ROT affix pairlist-prepend TO list        ( )
      THEN                                              ( )
    ELSE
      \ \." no it does not" cr
      2DROP
    THEN                                                ( )
  ELSE                                      ( addr u aff1 )
    { xc }                                       ( addr u )
      2DUP affix string-ends IF
        len -
        list -ROT affix pairlist-prepend TO list         ( )
      ELSE 2DROP THEN                                    ( )
  THEN
  \\." " list IF ." SUCCESS" cr THEN
  list ;

: untransform-fallout  ( addr u affix len -- strlist )
  { affix-ptr len }                                      ( addr u )
  0 { list }
  2DUP list -ROT strlist-prepend-alloc TO list
  2DROP list
  ;
