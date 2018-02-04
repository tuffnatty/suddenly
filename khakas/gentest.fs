T{ S" аал+да+хы+лар"		S" аалдағылар"		parse-test -> TRUE }T
T{ S" абылағ+ы"			S" абылаа"		parse-test -> TRUE }T
\ искл? T{ S" ағ+ы"		S" аа"			parse-test -> FALSE }T
T{ S" ағ+ы"			S" ағы"			parse-test -> TRUE }T
T{ S" ада+а+быс"		S" адибыс"		parse-test -> TRUE }T
T{ S" ада+ар+быс"		S" адирбыс"		parse-test -> TRUE }T
T{ S" анда+ох"			S" андох"		parse-test -> TRUE }T
T{ S" аннаң+ох"			S" аннаңох"		parse-test -> TRUE }T
T{ S" аннаңар+ох"		S" аннаңарох"		parse-test -> TRUE }T
T{ S" аң+ы"			S" аңы"			parse-test -> TRUE }T
T{ S" аң+ы"			S" аа"			parse-test -> TRUE }T  \ #92
T{ S" ат+ар"			S" атар"		parse-test -> TRUE }T
T{ S" ат+ың+а"			S" адаа"		parse-test -> FALSE }T
T{ S" ватт+ты"			S" ватты"		parse-test -> TRUE }T
T{ S" ватт+ы"			S" ватты"		parse-test -> TRUE }T
T{ S" заочно+ға"		S" заочнаа"	parse-test -> TRUE }T
T{ S" заочно+ға"		S" заочноға"	parse-test -> TRUE }T
T{ S" завод+тар"		S" заводтар"		parse-test -> TRUE }T
T{ S" ирін+і"			S" ирні"		parse-test -> TRUE }T
T{ S" изерге"	headword?	S" искен"		parse-test -> TRUE }T
T{ S" истерге"	headword?	S" искен"		parse-test -> TRUE }T
T{ S" ис+кен"			S" искен"		parse-test -> TRUE }T
T{ S" изерге"	headword?	S" исткен"		parse-test -> FALSE }T
T{ S" истерге"	headword?	S" исткен"		parse-test -> TRUE }T
T{ S" ист+кен"			S" исткен"		parse-test -> TRUE }T
T{ S" ізі+гелек"		S" ізеелек"		parse-test -> TRUE }T
T{ S" ізік+ім"			S" ізиим"		parse-test -> TRUE }T
T{ S" ізік+ім"			S" ізігім"		parse-test -> TRUE }T
T{ S" істі+нде"                 S" істінде" 		parse-test -> TRUE }T
T{ S" киле+гедег"		S" килеедег"		parse-test -> TRUE }T
T{ S" кил"			S" кил"			parse-test -> TRUE }T
T{ S" кил+0̸"			S" кил"			parse-test -> TRUE }T
T{ S" килін+і"			S" килні"		parse-test -> TRUE }T
T{ S" киме+ге"			S" кимее"		parse-test -> TRUE }T
T{ S" кис+0̸+чет+кен"		S" кисчеткен"		parse-test -> TRUE }T
T{ S" кис+0̸+чет+ер+ге"		S" кисчедерге"		parse-test -> TRUE }T
T{ S" кізі+ге"			S" кізее"		parse-test -> TRUE }T
T{ S" кіріс+і+нзер+тін"		S" кірізінзертін"	parse-test -> TRUE }T
T{ S" комиссия+ға"		S" комиссияа"           parse-test -> TRUE }T
T{ S" кӧг+і"			S" кӧӧ"			parse-test -> TRUE }T
T{ S" кӧзӧ+ге"			S" кӧзӧге"		parse-test -> TRUE }T
T{ S" мағаа+ох"			S" мағааох"		parse-test -> TRUE }T
T{ S" маң+ы"			S" маа"			parse-test -> TRUE }T
T{ S" марығ+ы"			S" марии"		parse-test -> TRUE }T
T{ S" марығ+ы"			S" марығы"		parse-test -> FALSE }T
T{ S" меню+ға"			S" менюға"		parse-test -> TRUE }T
T{ S" меню+ге"			S" менюге"		parse-test -> FALSE }T
T{ S" мойын+ы"			S" мойны"		parse-test -> TRUE }T
T{ S" мында+ох"			S" мындох"		parse-test DROP expected-found -> 1 }T
T{ S" ниң+ӧк"			S" ниик"		parse-test -> FALSE }T
T{ S" нуғ+ар"			S" нуур"		parse-test -> TRUE }T
T{ S" ол+ох"			S" олох"		parse-test -> TRUE }T
T{ S" ӧң+і"			S" ӧӧ"			parse-test -> TRUE }T
T{ S" орын+ы"			S" орны"		parse-test -> TRUE }T
T{ S" орын+ы+лар+ы"		S" орнылары"		parse-test -> TRUE }T
T{ S" паба+м+ни"		S" пабамни"		parse-test -> TRUE }T
T{ S" пағ+ы"			S" паа"			parse-test -> TRUE }T
T{ S" пар+а+м"			S" парам"		parse-test -> TRUE }T
T{ S" пар+ар+бын"		S" парарбын"		parse-test -> TRUE }T
\ диал. T{ S" пар+а+ға"		S" параға"		parse-test -> TRUE }T
T{ S" пар+и+ған"		S" париған"		parse-test -> TRUE }T
T{ S" пар+и+лар"		S" парилар"		parse-test -> TRUE }T
T{ S" пар+0̸+даа+ча"		S" пардаача"		parse-test -> TRUE }T
T{ S" пар+0̸+ла+ча"		S" парлача"		parse-test -> TRUE }T
T{ S" пар+за+ох"		S" парзох"		parse-test -> TRUE }T
T{ S" пар+0̸+тыр+лар"		S" партырлар"		parse-test -> TRUE }T
T{ S" пар+0̸+чадыр"		S" парчадыр"		parse-test -> TRUE }T
T{ S" пар+0̸+чат+пас"            S" парчатпас"		parse-test -> TRUE }T
T{ S" пар+ып+ох+чат+тыр"	S" парыбохчаттыр"	parse-test -> TRUE }T
T{ S" пас+па+за+ңар"		S" паспазар"		parse-test -> TRUE }T
T{ S" пас+са+ңар"		S" пассар"		parse-test -> TRUE }T
T{ S" пас+са+ңар"		S" пассаар"		parse-test -> TRUE }T
T{ S" пас+ты+ңар"		S" пастар"		parse-test -> TRUE }T
T{ S" пас+ты+ңар"		S" пастаар"		parse-test -> TRUE }T
T{ S" пасты+ңар"		S" пастар"		parse-test -> FALSE }T
T{ S" піл+ӌең+ің"		S" пілӌеең"		parse-test -> TRUE }T
T{ S" піс+тің+нер"		S" пістіңнер"		parse-test -> TRUE }T
T{ S" пир+бе+дек"		S" пирбедек"		parse-test -> TRUE }T
T{ S" пир+дек"			S" пирдек"		parse-test -> TRUE }T
T{ S" поғ+ып"			S" пооп"		parse-test -> TRUE }T
T{ S" пол+ар+ӌығ"		S" поларӌығ"		parse-test -> FALSE }T
T{ S" пол+ар+ӌых"		S" поларӌых"		parse-test -> TRUE }T
T{ S" пол+бас+пыс"		S" полбаспыс"		parse-test -> TRUE }T
T{ S" пол+бин+ыбыс+хан"		S" полбиныбысхан"	parse-test -> TRUE }T
T{ S" пол+0̸+даа+чат"		S" полдаачат"		parse-test -> TRUE }T
T{ S" пол+0̸+даа+чат+са"		S" полдаачатса"		parse-test -> TRUE }T
T{ S" пӧрік+ім"			S" пӧриим"		parse-test -> TRUE }T
T{ S" пӧрік+ім"			S" пӧрігім"		parse-test -> TRUE }T
T{ S" пуғ+ы"			S" пуу"			parse-test -> TRUE }T
T{ S" пу+ох"			S" пуох"		parse-test -> TRUE }T
T{ S" пурун+ы"			S" пурны"		parse-test -> TRUE }T
T{ S" пызо+ға"			S" пызаа"		parse-test -> FALSE }T
T{ S" пызо+ға"			S" пызоға"		parse-test -> TRUE }T
T{ S" сағ+им"			S" сағим"		parse-test -> TRUE }T
T{ S" сағ+ы"			S" саа"			parse-test -> TRUE }T
T{ S" сап+ыбыс"			S" саабыс"		parse-test -> TRUE }T
T{ S" сап+ыбыс+хан"		S" саабысхан"		parse-test -> TRUE }T
T{ S" сағ+ып"			S" саап"		parse-test -> TRUE }T
T{ S" салаа+ға"			S" салааға"		parse-test -> TRUE }T
T{ S" сана"			S" сана"		parse-test -> TRUE }T
T{ S" сана+0̸"			S" сана"		parse-test -> FALSE }T
T{ S" сана+а"			S" сани"		parse-test -> TRUE }T
T{ S" сана+абас"		S" санибас"		parse-test -> TRUE }T
T{ S" сана+ар"			S" санир"		parse-test -> TRUE }T
T{ S" сана+адыр"		S" санидыр"		parse-test -> TRUE }T
T{ S" сана+бас"			S" санабас"		parse-test -> TRUE }T
T{ S" сана+бас+ох"		S" санабазох"		parse-test -> TRUE }T
T{ S" сана+ған"			S" санаан"		parse-test -> TRUE }T
T{ S" сана+п+ох+ча"		S" санабохча"		parse-test -> TRUE }T
T{ S" сана+п+ча"		S" санапча"		parse-test -> TRUE }T
T{ S" сана+чадыр"		S" саначадыр"		parse-test -> FALSE }T
T{ S" сап+ы"			S" сабы"		parse-test -> TRUE }T
T{ S" сап+ып"			S" саап"		parse-test -> TRUE }T
T{ S" сап+ып+чадыр"		S" саапчадыр"		parse-test -> TRUE }T
T{ S" семья+лар+ы+наңар"	S" семьяларынаңар"	parse-test DROP expected-found -> 1 }T
T{ S" сенек+і"                  S" сенее"               parse-test -> FALSE }T
T{ S" сиг+ер"			S" сиир"		parse-test -> TRUE }T
T{ S" сиг+і"			S" сии"			parse-test -> TRUE }T
T{ S" сиг+іп"			S" сиип"		parse-test -> TRUE }T
T{ S" сине+еңер"		S" синееңер"		parse-test -> TRUE }T
T{ S" сине+елер"		S" синеелер"		parse-test -> TRUE }T
T{ S" сине+ибіс"		S" синибіс"		parse-test -> TRUE }T
T{ S" сине+им"			S" синим"		parse-test -> TRUE }T
T{ S" сіліг+і"			S" сілии"		parse-test -> TRUE }T
T{ S" скандинав+тар"		S" скандинавтар"	parse-test -> TRUE }T
T{ S" соғ+ар"			S" соор"		parse-test -> TRUE }T
T{ S" соғ+ы"			S" соғы"		parse-test -> TRUE }T
T{ S" сӧле+ебес"		S" сӧлибес"		parse-test -> TRUE }T
T{ S" стол+зар+тын"		S" столзартын"		parse-test -> TRUE }T
T{ S" стол+ы+нзар+тын"		S" столынзартын"	parse-test -> TRUE }T
T{ S" суғ+ға"			S" суға"		parse-test -> TRUE }T
T{ S" суғ+ы"			S" суу"			parse-test -> TRUE }T
T{ S" сӱрдег+і"			S" сӱрдее"		parse-test -> TRUE }T
T{ S" сурығ+ы"			S" сурии"		parse-test -> TRUE }T
T{ S" сыда+ӌаң+ы"		S" сыдаӌаа"		parse-test -> TRUE }T
T{ S" сых+ар+ға"	        S" сығарға"		parse-test -> TRUE }T
T{ S" тағ+ы"			S" таа"			parse-test -> TRUE }T
T{ S" таны+ған"			S" танаан"		parse-test -> TRUE }T
T{ S" тап+ып"			S" таап"		parse-test -> TRUE }T
T{ S" тариф+ы"			S" тарифы"		parse-test -> TRUE }T
T{ S" таста+ар+ға"		S" тастирға"		parse-test -> TRUE }T
T{ S" тег+іп"			S" тееп"		parse-test -> TRUE }T
T{ S" теп+іп"			S" тееп"		parse-test -> TRUE }T
T{ S" тетрадь+таң"		S" тетрадьтаң"		parse-test -> TRUE }T
T{ S" теп+ібіс"			S" теебіс"		parse-test -> TRUE }T
T{ S" ті+ген"			S" теен"		parse-test -> TRUE }T
T{ S" ті+ер"			S" тир"			parse-test -> TRUE }T
T{ S" тік+ер"			S" тігер"		parse-test -> TRUE }T
T{ S" тоғыс+ы+наңар"		S" тоғызынаңар"		parse-test -> TRUE }T
T{ S" тӧзе+е"			S" тӧзи"		parse-test -> TRUE }T
T{ S" тӧзе+едір"		S" тӧзидір"		parse-test -> TRUE }T
T{ S" тоң+ар"			S" тоор"		parse-test -> TRUE }T
T{ S" тоң+ған"			S" тоңан"		parse-test -> TRUE }T
T{ S" тоң+ып"			S" тооп"		parse-test -> TRUE }T
T{ S" тӱк+і"			S" тӱгі"		parse-test -> TRUE }T
T{ S" тӱлгӱ+ге"			S" тӱлгее"		parse-test -> TRUE }T
T{ S" тура+ға"			S" тураа"		parse-test -> TRUE }T
T{ S" тура+ох"			S" турох"		parse-test -> TRUE }T
T{ S" тус+ы"			S" тузы"		parse-test -> TRUE }T
T{ S" тут+ар+ға"		S" тударға"		parse-test -> TRUE }T
T{ S" ӱг+ер"			S" ӱӱр"			parse-test -> TRUE }T
T{ S" ӱг+іп"			S" ӱӱп"			parse-test -> TRUE }T
T{ S" узу+аң"			S" узааң"		parse-test -> TRUE }T
T{ S" узу+ар"			S" узир"		parse-test -> TRUE }T
T{ S" узу+ах+ча"		S" узихча"		parse-test -> TRUE }T
T{ S" узу+ғай"			S" узаай"		parse-test -> TRUE }T
T{ S" уйғу+ға"			S" уйғаа"		parse-test -> TRUE }T
T{ S" улуғ+ы"			S" улии"		parse-test -> TRUE }T
T{ S" фамилия+зы"		S" фамилиязы"		parse-test -> TRUE }T
T{ S" фамилия+ы"		S" фамилияы"		parse-test -> FALSE }T
T{ S" финн+нең"			S" финнең"		parse-test -> TRUE }T
T{ S" финн+нең"			S" финннең"		parse-test -> FALSE }T
\ T{ S" хан-пиг+і"		S" хан-пигі"		parse-test -> TRUE }T
T{ S" харах+ым"			S" хараам"		parse-test -> TRUE }T
T{ S" харах+ым"			S" харағым"		parse-test -> TRUE }T
T{ S" харын+ы"			S" харны"		parse-test -> TRUE }T
T{ S" хатхыр+0̸+чадыр+бын"	S" хатхырчадырбын"	parse-test -> TRUE }T
T{ S" хат+ым"			S" хатым"		parse-test -> TRUE }T
T{ S" хах+ы"			S" хағы"		parse-test -> TRUE }T
T{ S" хол+да+хы+лар"		S" холдағылар"		parse-test -> TRUE }T
T{ S" хомай+зыбыс"		S" хомайзыбыс"		parse-test -> TRUE }T
T{ S" хузух+ым"			S" хузиим"		parse-test -> TRUE }T
T{ S" хузух+ым"			S" хузуғым"		parse-test -> TRUE }T
T{ S" хулун+ы"			S" хулуны"		parse-test -> TRUE }T
T{ S" хус+тар"			S" хустар"		parse-test -> TRUE }T
T{ S" хыс+тар+ың+ни"		S" хыстарыңни"		parse-test -> TRUE }T
T{ S" чазы+ға"			S" чазаа"		parse-test -> TRUE }T
T{ S" чайлағ+ы"			S" чайлаа"		parse-test -> TRUE }T
T{ S" чарын+ы"			S" чарны"		parse-test -> TRUE }T
T{ S" чі+гелек"			S" чеелек"		parse-test -> TRUE }T
T{ S" чилін+і"			S" чилні"		parse-test -> TRUE }T
T{ S" чиң+ер"			S" чиңер"		parse-test -> TRUE }T
T{ S" чиң+іп"			S" чиип"		parse-test -> TRUE }T
T{ S" чілің+і"			S" чілии"		parse-test -> TRUE }T
T{ S" чоохта+ды+ңар"            S" чоохтадар"           parse-test -> TRUE }T
T{ S" чӧгіг+і"			S" чӧгии"		parse-test -> TRUE }T
T{ S" чӧлег+і"			S" чӧлее"		parse-test -> TRUE }T
T{ S" чӧлег+ім"			S" чӧлеем"		parse-test -> TRUE }T
T{ S" чӧр+ӌең+і"		S" чӧрӌее"		parse-test -> TRUE }T
T{ S" чӱг+і"			S" чӱгі"		parse-test -> TRUE }T
T{ S" чӱзӱг+і"			S" чӱзии"		parse-test -> TRUE }T
T{ S" чӱк+і"			S" чӱгі"		parse-test -> TRUE }T
T{ S" чӱрек+і+біс+ке"		S" чӱреебіске"		parse-test -> TRUE }T
T{ S" чурта+п+чадыр"		S" чуртапчадыр"		parse-test -> TRUE }T
T{ S" чух+ы"			S" чуғы"		parse-test -> TRUE }T
T{ S" чығ+а"			S" чыға"		parse-test -> TRUE }T
T{ S" чығ+адыр"			S" чығадыр"		parse-test -> TRUE }T
T{ S" чығ+ар"			S" чыыр"		parse-test -> TRUE }T
T{ S" чығ+ып"			S" чыып"		parse-test -> TRUE }T
T{ S" чығ+ыбыс+хан"		S" чыыбысхан"		parse-test -> TRUE }T
\ lost from document T{ S" шаш+ы"			S" шашы"		parse-test -> TRUE }T
T{ S" ырла+ибыс"		S" ырлибыс"		parse-test -> TRUE }T
T{ S" ырла+им"			S" ырлим"		parse-test -> TRUE }T
T{ S" эг+гей"			S" эгей"		parse-test -> TRUE }T
T{ S" эг+ер"			S" ээр"			parse-test -> TRUE }T
T{ S" ээ+зі+м"			S" ээзім"		parse-test -> TRUE }T
