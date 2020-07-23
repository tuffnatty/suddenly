REQUIRE debugging.fs
REQUIRE minire.fs
REQUIRE strings.fs
REQUIRE ./phonetics.fs

: last-vowel ( addr u -- u | 0 )
  OVER >R                      ( addr u -- R: cs )
  0 -ROT last-sound-ptr BEGIN ( vowel cs-cur  R: cs )
    DUP XC@ DUP vowel? IF  ( vowel cs-cur cs-cur@ )
      -ROT NIP 1
    ELSE DROP XCHAR- DUP R@ <     ( vowel cs-next f )
    THEN
  UNTIL
  DROP RDROP ;

: last-char-vowel ( addr u -- u | 0 )
  OVER >R                      ( addr u -- R: cs )
  0 -ROT last-sound-ptr BEGIN ( vowel cs-cur  R: cs )
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

:+ last-char-vowel-row  { addr u -- wid }
  addr u last-sound-ptr BEGIN  ( addr' )
    DUP cyr t~/ {front-vowel} IF DROP front-vowel EXIT THEN
    DUP cyr t~/ {back-vowel}  IF DROP back-vowel  EXIT THEN
  XCHAR-  DUP addr <  UNTIL
  DROP front-vowel ;

REQUIRE ./../rules-common.fs
