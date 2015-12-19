0 CONSTANT cl-voiced
1 CONSTANT cl-unvoiced
2 CONSTANT cl-nasal
: class-nvu  ( u -- class )
  DUP nasal? IF DROP cl-nasal
  ELSE unvoiced? IF cl-unvoiced
  ELSE cl-voiced THEN THEN ;
: class-vu  ( u -- class )
  unvoiced? IF cl-unvoiced ELSE cl-voiced THEN ;

0 CONSTANT cl-back
1 CONSTANT cl-front
: class-fb  ( u -- class )
  back-vowel? IF cl-back ELSE cl-front THEN ;

0 CONSTANT cl-consonant
1 CONSTANT cl-vowel
: class-cv  ( u -- class )
  vowel? IF cl-vowel ELSE cl-consonant THEN ;

