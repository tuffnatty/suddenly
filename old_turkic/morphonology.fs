REQUIRE rules.fs

: stem-fugitive?  ( cs -- f )
  DUP polysyllabic? IF
    DUP last-sound nasal-or-glide-or-z? IF
      prev-sound narrow-vowel?
    ELSE 0 THEN
  ELSE 0 THEN ;
