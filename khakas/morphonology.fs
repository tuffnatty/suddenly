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
S" ыі" rule-fb morphoneme І
S" ы0і0" rule-cv-fb morphoneme (І)
S" оӧ" rule-fb morphoneme О
S" ғхгк" rule-vu-fb morphoneme Г
S" ғг" rule-fb morphoneme G
S" бпм" rule-nvu morphoneme П
S" 0б0п0м" rule-cv-nvu morphoneme (П)
S" дтн" rule-nvu morphoneme Д
S" 0з" rule-cv morphoneme (з)
\ S" хк" rule-fb morphoneme К
S" хк" rule-fb morphoneme Q
S" ғгхкхк00" rule-cv-vu-fb morphoneme К
S" лтн" rule-nvu morphoneme Л
S" ллн" rule-nvu morphoneme L
S" нт" rule-vu morphoneme Н
S" 0н" rule-cv morphoneme (н)
S" зс" rule-vu morphoneme С
S" дт" rule-vu morphoneme Т
S" тт" rule-cv morphoneme D
S" ӌч" rule-vu morphoneme Ч
