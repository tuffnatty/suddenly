language-require rules.fs
REQUIRE sounds.fs

: stem-fugitive?  ( cs -- f )
  DUP polysyllabic? IF
    DUP last-sound [CHAR] н = IF
      DUP prev-sound very-narrow-vowel? IF
        third-sound glide?
      ELSE 0 THEN
    ELSE 0 THEN
  ELSE 0 THEN ;


REQUIRE phonetics-common.fs

S" ае" rule-fb morphoneme А
S" а0е0" rule-cv-fb morphoneme (А)
S" ыі" rule-fb morphoneme Ы
S" ы0і0" rule-cv-fb morphoneme (Ы)
S" оӧ" rule-fb morphoneme О
S" ғхгк" rule-vu-fb morphoneme Г
S" бпм" rule-nvu morphoneme П
S" 0б0п0м" rule-cv-nvu morphoneme (П)
S" дтн" rule-nvu morphoneme Д
S" 0з" rule-cv morphoneme (з)
\ S" хк" rule-fb morphoneme К
S" ғгхкхк00" rule-cv-vu-fb morphoneme К
S" лтн" rule-nvu morphoneme Л
S" ллн" rule-nvu morphoneme L
S" нт" rule-vu morphoneme Н
S" 0н" rule-cv morphoneme (н)
S" зс" rule-vu morphoneme С
S" дт" rule-vu morphoneme Т
S" ӌч" rule-vu morphoneme Ч

S" ғг" rule-fb morphoneme G
S" хк" rule-fb morphoneme Q

S" Г" rule-vu cl-voiced S" G" morphoneme-substitution
S" К" rule-cv cl-vowel S" Q" morphoneme-substitution
