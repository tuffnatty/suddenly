\ :3,$s/\(sound \)\([aceiopxyöÿ]\)/\=submatch(1) . tr(submatch(2),"aceiopxyöÿ", "асеіорхуӧӱ")/gI

REQUIRE sounds.fs

sound-class vowel
  sound и
  sound а sound о sound ы sound у
  sound е sound і sound ӧ sound ӱ
sound-class;

sound-class unchar-vowel
  sound и
sound-class;

sound-class back-vowel
  sound а sound о sound ы sound у
sound-class;

sound-class front-vowel
  sound е sound і sound ӧ sound ӱ
sound-class;

sound-class narrow-vowel
  sound и sound і sound у sound ӱ sound ы
sound-class;

sound-class very-narrow-vowel
  sound і sound ы
sound-class;

sound-class nasal
  sound м sound н sound ӊ
sound-class;

sound-class unvoiced
  sound п sound ф sound к sound х sound т sound ш sound с sound ц sound ч sound щ
sound-class;

sound-class voiced
  sound б sound в sound г sound ғ sound д sound ж sound з sound з sound ӌ sound щ
sound-class;

sound-class glide
  sound й sound л sound р
sound-class;

sound-class fallout
  sound г sound ғ sound к sound ң sound п sound х
sound-class;

sound-class gh-g-ng
  sound ғ sound г sound ң
sound-class;

sound-class k-kh
  sound к sound х
sound-class;

sound-class o-yo
  sound о sound ӧ
sound-class;


REQUIRE phonetics-common.fs
