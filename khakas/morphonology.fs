REQUIRE rules.fs

: stem-fugitive?  ( cs -- f )
  DUP polysyllabic? IF
    DUP last-sound [CHAR] н = IF
      DUP prev-sound very-narrow-vowel? IF
        third-sound glide?
      ELSE 0 THEN
    ELSE 0 THEN
  ELSE 0 THEN ;
