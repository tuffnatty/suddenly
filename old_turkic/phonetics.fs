REQUIRE sounds.fs

sound-class vowel
  sound a sound ā sound o sound ï sound u
  sound ä sound ǟ sound ö sound i sound ü sound e sound ẹ
sound-class;

sound-class unchar-vowel
sound-class;

sound-class back-vowel
  sound a sound o sound ï sound u
sound-class;

sound-class front-vowel
  sound ä sound ö sound i sound ü sound e sound ẹ
sound-class;

sound-class narrow-vowel
  sound ï sound u sound i sound ü
sound-class;

sound-class labial-vowel
  sound ö sound ü sound o sound u
sound-class;

sound-class nasal
  sound m sound n sound ñ sound ŋ
sound-class;

sound-class nasal-or-glide-or-z
  sound m sound n sound ñ sound ŋ
  sound j sound l sound r
  sound z
sound-class;

sound-class unvoiced
  sound p sound k sound h sound t sound š sound s sound č sound ꭓ sound f
sound-class;

sound-class voiced
  sound b sound g sound ɣ sound d sound ž sound z sound ǯ sound ẟ
sound-class;

sound-class glide
  sound j sound l sound r
sound-class;

\ sound-class fallout
\   sound г sound ғ sound к sound ң sound п sound х
\ sound-class;

sound-class n-ng
  sound n sound ŋ sound ñ
sound-class;
