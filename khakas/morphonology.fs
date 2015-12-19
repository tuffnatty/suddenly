REQUIRE rules.fs
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
S" ӧо" rule-fb morphoneme О
S" гғ" rule-fb morphoneme Г
S" бпм" rule-nvu morphoneme Б
S" 0б0п0м" rule-cv-nvu morphoneme (Б)
S" дтн" rule-nvu morphoneme Д
S" 0з" rule-cv morphoneme (з)
S" ғгхк" rule-vu-fb morphoneme К
S" 00гғ00кх" rule-cv-vu-fb morphoneme (К)
S" лтн" rule-nvu morphoneme Л
S" нт" rule-vu morphoneme Н
S" 0н" rule-cv morphoneme (н)
S" зс" rule-vu morphoneme С
S" дт" rule-vu morphoneme Т
S" ӌч" rule-vu morphoneme Ч
