REQUIRE rules-common.fs

2 CONSTANT cl-labial

: class-fbl  ( u -- class )
  DUP labial-vowel? IF cl-labial ELSE 0 THEN >R
  class-fb R> + ;

: rule-fbl  ( addr u -- addr u table-index )
  2DUP last-char-vowel class-fbl ;

: rule-cv-fbl  ( addr u -- addr u table-index )
  2DUP last-sound class-cv >R 2DUP last-char-vowel class-fbl 2* R> + ;
