REQUIRE ./../strings.fs
REQUIRE ./phonetics.fs

: last-char-vowel ( addr u -- u | 0 )
  OVER >R
  0 -ROT last-sound-ptr BEGIN ( vowel cs-cur  r: cs )
    DUP XC@ DUP unchar-vowel? IF  ( vowel cs-cur cs-cur@ )
      -ROT NIP XCHAR- DUP R@ <    ( vowel cs-cur 0 )
    ELSE DUP front-vowel? IF      ( vowel cs-cur cs-cur@ )
      -ROT NIP 1                  ( vowel cs-cur 1 )
    ELSE DUP back-vowel? IF
      -ROT NIP 1
    ELSE DROP XCHAR- DUP R@ <     ( vowel cs-next f )
    THEN THEN THEN
  UNTIL
  DROP RDROP ;

REQUIRE ./../rules-common.fs
