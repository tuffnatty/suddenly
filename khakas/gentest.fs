T{ S" аал+да+хы+лар"		S" аалдағылар"		parse-test -> TRUE }T
T{ S" аар+лығ+ы"		S" аарлии"		parse-test -> TRUE }T
T{ S" абылағ+ы"			S" абылаа"		parse-test -> TRUE }T
T{ S" ағ+ы"			S" аа"			parse-test -> TRUE }T  \ #147
T{ S" ағ+ы"			S" ағы"			parse-test -> TRUE }T
T{ S" ада+а+быс"		S" адибыс"		parse-test -> TRUE }T
T{ S" ада+ар+быс"		S" адирбыс"		parse-test -> TRUE }T
T{ S" азах+ым+ай"		S" азаамай"		parse-test -> TRUE }T
T{ S" азыра+ах+чат+ха+м"	S" азырихчатхам"	parse-test -> TRUE }T
T{ S" акел+ер+ге"		S" акелерге"		parse-test -> TRUE }T
T{ S" алкоголик+ке"		S" алкоголикке"		parse-test -> TRUE }T
T{ S" ал+алыс"			S" алалыс"		parse-test -> TRUE }T
T{ S" анда+ох"			S" андох"		parse-test -> TRUE }T
T{ S" аңмар+ли"			S" аңмарли"		parse-test -> TRUE }T
T{ S" аннаң+ох"			S" аннаңох"		parse-test -> TRUE }T
T{ S" аннаңар+ох"		S" аннаңарох"		parse-test -> TRUE }T
T{ S" аң+ы"			S" аңы"			parse-test -> TRUE }T
T{ S" аң+ы"			S" аа"			parse-test -> TRUE }T  \ #92
T{ S" ат+ар"			S" атар"		parse-test -> TRUE }T
T{ S" ат+ыбыс+ах+ча+лар"        S" атыбызахчалар"       parse-test -> TRUE }T  \ #126
T{ S" ат+ың+а"			S" адаа"		parse-test -> FALSE }T
T{ S" ватт+ты"			S" ватты"		parse-test -> TRUE }T
T{ S" ватт+ы"			S" ватты"		parse-test -> TRUE }T
T{ S" грузчик+тер"		S" грузчиктер"		parse-test -> TRUE }T
T{ S" заочно+ға"		S" заочнаа"	        parse-test -> TRUE }T
T{ S" заочно+ға"		S" заочноға"	        parse-test -> TRUE }T
T{ S" завод+тар"		S" заводтар"		parse-test -> TRUE }T
T{ S" иней+лер+і+мең"		S" инейлерімең"		parse-test -> TRUE }T
T{ S" ин+еліс"			S" инеліс"		parse-test -> TRUE }T
T{ S" ит+і+бең"			S" идібең"		parse-test -> TRUE }T
T{ S" ирке+м+ей"		S" иркемей"		parse-test -> TRUE }T
T{ S" ирін+і"			S" ирні"		parse-test -> TRUE }T
T{ S" изерге"	headword?	S" искен"		parse-test -> TRUE }T
T{ S" истерге"	headword?	S" искен"		parse-test -> TRUE }T
T{ S" ис+кен"			S" искен"		parse-test -> TRUE }T
T{ S" изерге"	headword?	S" исткен"		parse-test -> FALSE }T
T{ S" истерге"	headword?	S" исткен"		parse-test -> TRUE }T
T{ S" ис+тер+лер"		S" истерлер"		parse-test -> FALSE }T  \ #191
T{ S" ист+кен"			S" исткен"		parse-test -> TRUE }T
T{ S" ит+ті+ни+м"		S" иттіним"		parse-test -> TRUE }T
T{ S" ізі+гелек"		S" ізеелек"		parse-test -> TRUE }T
T{ S" ізік+ім"			S" ізиим"		parse-test -> TRUE }T
T{ S" ізік+ім"			S" ізігім"		parse-test -> TRUE }T
T{ S" істі+нде"                 S" істінде" 		parse-test -> TRUE }T
T{ S" іч+ер+ге"			S" ічерге"		parse-test -> TRUE }T
T{ S" іч+еліс"			S" ічеліс"		parse-test -> TRUE }T
T{ S" киле+гедег"		S" килеедег"		parse-test -> TRUE }T
T{ S" кил"			S" кил"			parse-test -> TRUE }T
T{ S" кил+0̸"			S" кил"			parse-test -> TRUE }T
T{ S" кил+ге+бін"		S" килгебін"		parse-test -> TRUE }T
T{ S" кил+ген+ни"		S" килгенни"		parse-test -> TRUE }T
T{ S" кил+ер+ім"		S" килерім"		parse-test -> TRUE }T
T{ S" килін+і"			S" килні"		parse-test -> TRUE }T
T{ S" кил+0̸+ча"			S" килча"		parse-test -> TRUE }T
T{ S" кил+0̸+ча+м"		S" килчам"		parse-test -> TRUE }T
T{ S" кил+0̸+чады"		S" килчады"		parse-test DROP expected-found -> 1 }T
T{ S" кил+0̸+чады+м"		S" килчадым"		parse-test -> TRUE }T
T{ S" кил+0̸+чадыр+бын"		S" килчадырбын"		parse-test -> TRUE }T
T{ S" кил+0̸+чен"		S" килчен"		parse-test -> TRUE }T
T{ S" киме+ге"			S" кимее"		parse-test -> TRUE }T
T{ S" кип-азах+ы"		S" кип-азаа"		parse-test -> TRUE }T
T{ S" кип-азах+ым"		S" кип-азаам"		parse-test -> TRUE }T
T{ S" кис+0̸+чет+кен"		S" кисчеткен"		parse-test -> TRUE }T
T{ S" кис+0̸+чет+ер+ге"		S" кисчедерге"		parse-test -> TRUE }T
T{ S" кізі+ге"			S" кізее"		parse-test -> TRUE }T
T{ S" кіріс+і+нзер+тін"		S" кірізінзертін"	parse-test -> TRUE }T
T{ S" кіртӧң+ге+ӧк"		S" кіртӧңеӧк"		parse-test -> TRUE }T
T{ S" кіртӧң+ӧк"		S" кіртӧңӧк"		parse-test -> TRUE }T
T{ S" кіртӧң+ге+ӧк"		S" кіртӧңӧк"		parse-test -> FALSE }T
T{ S" комиссия+ға"		S" комиссияа"           parse-test -> TRUE }T
T{ S" кӧг+і"			S" кӧӧ"			parse-test -> TRUE }T
T{ S" кӧзӧ+ге"			S" кӧзӧге"		parse-test -> TRUE }T
T{ S" кӧй+0̸+чары"		S" кӧйчары"		parse-test -> TRUE }T  \ #181
T{ S" кӧр+бе+ген+дее+бін"	S" кӧрбеендеебін"	parse-test -> TRUE }T
T{ S" кӧр+еліс"			S" кӧреліс"		parse-test -> TRUE }T
T{ S" курс+ы"			S" курзы"		parse-test -> TRUE }T
T{ S" курс+ы"			S" курсы"		parse-test -> TRUE }T
T{ S" кӱн+і+нге"		S" кӱнінге"		parse-test -> TRUE }T
T{ S" кӱн+і+нке"		S" кӱнінке"		parse-test -> FALSE }T
T{ S" мағаа+ох"			S" мағааох"		parse-test -> TRUE }T
T{ S" маң+ы"			S" маа"			parse-test -> TRUE }T
T{ S" марығ+ы"			S" марии"		parse-test -> TRUE }T
T{ S" марығ+ы"			S" марығы"		parse-test -> TRUE }T  \ #147
T{ S" марығ+ды"			S" марығды"		parse-test -> TRUE }T
T{ S" меню+ға"			S" менюға"		parse-test -> TRUE }T
T{ S" меню+ге"			S" менюге"		parse-test -> FALSE }T
T{ S" мойын+ы"			S" мойны"		parse-test -> TRUE }T
T{ S" мында+ох"			S" мындох"		parse-test DROP expected-found -> 1 }T
T{ S" нан+адыр+ым"		S" нанадырым"		parse-test -> TRUE }T  \ #207
T{ S" ник+тер+ім+ді"		S" никтерімді"		parse-test -> TRUE }T
T{ S" ниң+ӧк"			S" ниик"		parse-test -> FALSE }T
T{ S" нуғ+ар"			S" нуур"		parse-test -> TRUE }T
T{ S" ойла+а+м"			S" ойлаам"		parse-test -> FALSE }T
T{ S" ойна+а+быс"		S" ойнаабыс"		parse-test -> FALSE }T
T{ S" олар+дың"			S" олардың"		parse-test -> TRUE }T
T{ S" ол+ох"			S" олох"		parse-test -> TRUE }T
T{ S" оолғы+м+ай"		S" оолғымай"		parse-test -> TRUE }T
T{ S" орын+ы"			S" орны"		parse-test -> TRUE }T
T{ S" орын+ы+лар+ы"		S" орнылары"		parse-test -> TRUE }T
T{ S" ӧдір+бе+ек"		S" ӧдірбеек"		parse-test -> TRUE }T
T{ S" ӧзе+ӧк+іс"		S" ӧзӧгіс"		parse-test -> FALSE }T  \ #184
T{ S" ӧс+ӧк+іс"			S" ӧзӧгіс"		parse-test -> FALSE }T  \ #184
T{ S" ӧң+і"			S" ӧӧ"			parse-test -> TRUE }T
T{ S" паба+м+ни"		S" пабамни"		parse-test -> TRUE }T
T{ S" пағ+ы"			S" паа"			parse-test -> TRUE }T
T{ S" пала+баң"			S" палабаң"		parse-test -> TRUE }T
T{ S" пала+блаң"		S" палаблаң"		parse-test -> TRUE }T
T{ S" пала+маң"			S" паламаң"		parse-test -> TRUE }T
T{ S" палты+мнаң"		S" палтымнаң"		parse-test -> TRUE }T
T{ S" параан"			S" параан"		parse-test -> TRUE }T
T{ S" пара+ң+ы+н"		S" параан"		parse-test -> FALSE }T
T{ S" пар+и+ған"		S" параан"		parse-test -> FALSE }T
T{ S" пар+а+м"			S" парам"		parse-test -> TRUE }T
T{ S" пар+ар+бын"		S" парарбын"		parse-test -> TRUE }T
\ диал. T{ S" пар+а+ға"		S" параға"		parse-test -> TRUE }T
T{ S" пар+ады+м"		S" парадым"		parse-test -> TRUE }T
T{ S" пар+адыр+а"		S" парадыра"		parse-test -> TRUE }T
T{ S" пар+адыр+ып"		S" парадырып"		parse-test -> TRUE }T
T{ S" пар+ат+са+ң"		S" паратсаң"		parse-test -> TRUE }T
T{ S" пар+ат+хан"		S" паратхан"		parse-test -> TRUE }T
T{ S" пар+ах+ча"		S" парахча"		parse-test -> TRUE }T
T{ S" пар+0̸+ах+ча"		S" парахча"		parse-test -> FALSE }T
T{ S" пар+ға+бын"		S" парғабын"		parse-test -> TRUE }T
T{ S" пар+ған+дағ+зың"		S" парғандағзың"	parse-test -> TRUE }T
T{ S" пар+и+ған"		S" париған"		parse-test -> TRUE }T
T{ S" пар+и+лар"		S" парилар"		parse-test -> TRUE }T
T{ S" пар+ит+чат+ып"		S" паритчадып"		parse-test -> TRUE }T
T{ S" пар+0̸+даа+ча"		S" пардаача"		parse-test -> TRUE }T
T{ S" пар+0̸+ла+ча"		S" парлача"		parse-test -> TRUE }T
T{ S" пар+за+ох"		S" парзох"		parse-test -> TRUE }T
T{ S" пар+0̸+ту+м"		S" партум"		parse-test -> TRUE }T
T{ S" пар+0̸+тур"		S" партур"		parse-test -> TRUE }T
T{ S" пар+0̸+тур+бын"		S" партурбын"		parse-test -> TRUE }T
T{ S" пар+0̸+тыр+лар"		S" партырлар"		parse-test -> TRUE }T
T{ S" пар+0̸+чады+м"		S" парчадым"		parse-test -> TRUE }T
T{ S" пар+0̸+чадыр"		S" парчадыр"		parse-test -> TRUE }T
T{ S" пар+0̸+чан+да"		S" парчанда"		parse-test -> TRUE }T
T{ S" пар+0̸+чар+м"		S" парчарм"		parse-test -> FALSE }T
T{ S" пар+0̸+чары"		S" парчары"		parse-test -> TRUE }T
T{ S" пар+0̸+чары"		S" парчары"		parse-test DROP expected-found -> 1 }T
T{ S" пар+0̸+чары+м"		S" парчарым"		parse-test -> TRUE }T
T{ S" пар+0̸+чат+пас"            S" парчатпас"		parse-test -> TRUE }T
T{ S" пар+ыбыс+тах"		S" парыбыстах"		parse-test -> TRUE }T  \ #144
T{ S" пар+ыбыс+ча"		S" парыбысча"		parse-test -> TRUE }T
T{ S" пар+ып+ох+чат+тыр"	S" парыбохчаттыр"	parse-test -> TRUE }T
T{ S" пас+па+за+ңар"		S" паспазар"		parse-test -> TRUE }T
T{ S" пас+са+ңар"		S" пассар"		parse-test -> TRUE }T
T{ S" пас+са+ңар"		S" пассаар"		parse-test -> TRUE }T
T{ S" пас+ты+ңар"		S" пастар"		parse-test -> TRUE }T
T{ S" пас+ты+ңар"		S" пастаар"		parse-test -> TRUE }T
T{ S" пасты+ңар"		S" пастар"		parse-test -> FALSE }T
T{ S" пасха+лар+ы+ни+наң"	S" пасхаларынинаң"	parse-test -> TRUE }T
T{ S" піл+бин+чат+хан"		S" пілбинчатхан"	parse-test -> TRUE }T
T{ S" піл+ӌең+ің"		S" пілӌеең"		parse-test -> TRUE }T
T{ S" піс+тің+нер"		S" пістіңнер"		parse-test -> FALSE }T  \ #186
T{ S" пир+бе+дек"		S" пирбедек"		parse-test -> TRUE }T
T{ S" пир+бин+ібіс+ер+ӌік"	S" пирбинібізерӌік"	parse-test -> TRUE }T
T{ S" пир+дек"			S" пирдек"		parse-test -> TRUE }T
T{ S" поғ+ып"			S" пооп"		parse-test -> TRUE }T
\ T{ S" пол+аачых+тан+ча+зар"     S" полаачыхтанчазар"	parse-test -> TRUE }T
T{ S" пол+а+зың"		S" полазың"		parse-test -> TRUE }T
T{ S" пол+ар+ӌығ"		S" поларӌығ"		parse-test -> FALSE }T
T{ S" пол+ар+ӌых"		S" поларӌых"		parse-test -> TRUE }T
T{ S" пол+ах+ча"		S" полахча"		parse-test -> TRUE }T
T{ S" пол+баан+дыр"		S" полбаандыр"		parse-test -> TRUE }T
T{ S" пол+ба+ған+дыр"		S" полбаандыр"		parse-test -> FALSE }T  \ #188
T{ S" пол+бас+пыс"		S" полбаспыс"		parse-test -> TRUE }T
T{ S" пол+бин+ыбыс+хан"		S" полбиныбысхан"	parse-test -> TRUE }T
T{ S" пол+0̸+даа+чат"		S" полдаачат"		parse-test -> TRUE }T
T{ S" пол+0̸+даа+чат+са"		S" полдаачатса"		parse-test -> TRUE }T
T{ S" пол+ды+ни"		S" полдыни"		parse-test -> FALSE }T  \ #193
T{ S" пол+ды+ни+м"		S" полдыним"		parse-test -> TRUE }T
T{ S" пол+0̸+тыр+ох+чых"		S" полтырохчых"		parse-test -> TRUE }T
T{ S" пӧрік+ім"			S" пӧриим"		parse-test -> TRUE }T
T{ S" пӧрік+ім"			S" пӧрігім"		parse-test -> TRUE }T
T{ S" пуғ+ы"			S" пуу"			parse-test -> TRUE }T
T{ S" пу+ох"			S" пуох"		parse-test -> TRUE }T
T{ S" пурун+ы"			S" пурны"		parse-test -> TRUE }T
T{ S" пызо+ға"			S" пызаа"		parse-test -> FALSE }T
T{ S" пызо+ға"			S" пызоға"		parse-test -> TRUE }T
T{ S" сағ+им"			S" сағим"		parse-test -> TRUE }T
T{ S" сағ+ы"			S" саа"			parse-test -> TRUE }T
T{ S" сағын+адыр+даа+бын"	S" сағынадырдаабын"	parse-test -> TRUE }T
T{ S" сағыссырас+та+зар"	S" сағыссырастазар"	parse-test DROP expected-found -> 1 }T  \ # 187
T{ S" сап+ыбыс"			S" саабыс"		parse-test -> TRUE }T
T{ S" сап+ыбыс+хан"		S" саабысхан"		parse-test -> TRUE }T
T{ S" сағ+ып"			S" саап"		parse-test -> TRUE }T
T{ S" салаа+ға"			S" салааға"		parse-test -> TRUE }T
T{ S" самолёт+ы"		S" самолёды"		parse-test -> TRUE }T
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
T{ S" си+ң+е"			S" сии"			parse-test -> FALSE }T  \ #206
T{ S" сиг+ер"			S" сиир"		parse-test -> TRUE }T
T{ S" сиг+і"			S" сии"			parse-test -> TRUE }T
T{ S" сиг+іп"			S" сиип"		parse-test -> TRUE }T
T{ S" сиихта+ар"		S" сиихтир"		parse-test -> TRUE }T  \ #175
T{ S" сине+еңер"		S" синееңер"		parse-test -> TRUE }T
T{ S" сине+елер"		S" синеелер"		parse-test -> TRUE }T
T{ S" сине+ибіс"		S" синибіс"		parse-test -> TRUE }T
T{ S" сине+им"			S" синим"		parse-test -> TRUE }T
T{ S" сіліг+і"			S" сілии"		parse-test -> TRUE }T
T{ S" сірер+ди"			S" сірерди"		parse-test -> TRUE }T
T{ S" скандинав+тар"		S" скандинавтар"	parse-test -> TRUE }T
T{ S" соғ+ар"			S" соор"		parse-test -> TRUE }T
T{ S" соғ+ы"			S" соғы"		parse-test -> TRUE }T
T{ S" сӧле+ебес"		S" сӧлибес"		parse-test -> TRUE }T
T{ S" стол+зар+тын"		S" столзартын"		parse-test -> TRUE }T
T{ S" стол+ы+нзар+тын"		S" столынзартын"	parse-test -> TRUE }T
T{ S" суғ+ға"			S" суға"		parse-test -> TRUE }T
T{ S" суғ+ға+ох"		S" суғаох"		parse-test -> TRUE }T
T{ S" суғ+ы"			S" суу"			parse-test -> TRUE }T
T{ S" сӱрдег+і"			S" сӱрдее"		parse-test -> TRUE }T
T{ S" сурығ+ы"			S" сурии"		parse-test -> TRUE }T
T{ S" сустал+ып+чат+ып"		S" сусталыпчадып"	parse-test -> TRUE }T
T{ S" сыда+ӌаң+ы"		S" сыдаӌаа"		parse-test -> TRUE }T
T{ S" сын+маан+дыр"		S" сынмаандыр"		parse-test -> TRUE }T  \ #188
T{ S" сын+ма+ған+дыр"		S" сынмаандыр"		parse-test -> FALSE }T  \ #188
T{ S" сых+ар+ға"	        S" сығарға"		parse-test -> TRUE }T
T{ S" сых+ып+ох+ыс+чых"		S" сығыбоғысчых"	parse-test -> TRUE }T
T{ S" сых+лағла+п"		S" сыхлағлап"		parse-test -> TRUE }T
T{ S" сых+паан+чат+хан"		S" сыхпаанчатхан"	parse-test -> TRUE }T
T{ S" сых+таа+лар"		S" сыхтаалар"		parse-test DROP expected-found -> 1 }T  \ #197
T{ S" тағ+ы"			S" таа"			parse-test -> TRUE }T
T{ S" тайға+за"			S" тайғаза"		parse-test -> TRUE }T
T{ S" тайға+зары"		S" тайғазары"		parse-test -> TRUE }T
T{ S" тал+аачых"		S" талаачых"		parse-test -> TRUE }T
T{ S" таны+ған"			S" танаан"		parse-test -> TRUE }T
T{ S" тап+ып"			S" таап"		parse-test -> TRUE }T
T{ S" тариф+ы"			S" тарифы"		parse-test -> TRUE }T
T{ S" тарт+лағла+п+ча"		S" тартлағлапча"	parse-test -> TRUE }T
T{ S" тарт+ып+ла+ча+лар"        S" тартыплачалар"       parse-test -> TRUE }T
T{ S" таста+ар+ға"		S" тастирға"		parse-test -> TRUE }T
T{ S" тег+іп"			S" тееп"		parse-test -> TRUE }T
T{ S" теп+іп"			S" тееп"		parse-test -> TRUE }T
T{ S" тетрадь+таң"		S" тетрадьтаң"		parse-test -> TRUE }T
T{ S" теп+ібіс"			S" теебіс"		parse-test -> TRUE }T
T{ S" ті+ген"			S" теен"		parse-test -> TRUE }T
T{ S" ті+ер"			S" тир"			parse-test -> TRUE }T
T{ S" тік+ер"			S" тігер"		parse-test -> TRUE }T
T{ S" ті+ле"			S" тіле"		parse-test -> FALSE }T  \ #196
T{ S" тіле+п+чат"		S" тілепчат"		parse-test -> TRUE }T
T{ S" ті+п+чен"			S" тіпчен"		parse-test -> TRUE }T
T{ S" тоғыс+ы+наңар"		S" тоғызынаңар"		parse-test -> TRUE }T
T{ S" тӧзе+е"			S" тӧзи"		parse-test -> TRUE }T
T{ S" тӧзе+едір"		S" тӧзидір"		parse-test -> TRUE }T
T{ S" тоң+ар"			S" тоор"		parse-test -> TRUE }T
T{ S" тоң+ған"			S" тоңан"		parse-test -> TRUE }T
T{ S" тоң+ып"			S" тооп"		parse-test -> TRUE }T
T{ S" тоос+ып+таа+быс+ты+лар"	S" тоозыптаабыстылар"	parse-test -> TRUE }T
T{ S" тохта+ғла+п+ох+ыс+хан+нар" S" тохтағлабоғысханнар" parse-test -> TRUE }T
T{ S" тох+ы+нға+быс"		S" тоғынғабыс"		parse-test -> FALSE }T
T{ S" тӱк+і"			S" тӱгі"		parse-test -> TRUE }T
T{ S" тӱлгӱ+ге"			S" тӱлгее"		parse-test -> TRUE }T
T{ S" тура+ға"			S" тураа"		parse-test -> TRUE }T
T{ S" тура+да+хы+лар+ы"		S" турадағылары"	parse-test -> TRUE }T
T{ S" тура+ох"			S" турох"		parse-test -> TRUE }T
T{ S" туза+лығ+лар"		S" тузалығлар"		parse-test DROP expected-found -> 2 }T
T{ S" тус+ы"			S" тузы"		parse-test -> TRUE }T
T{ S" тут+ар+ға"		S" тударға"		parse-test -> TRUE }T
T{ S" тыы+п+ла+ыс+хан"		S" тыыплаысхан"		parse-test -> TRUE }T
T{ S" ӱг+ер"			S" ӱӱр"			parse-test -> TRUE }T
T{ S" ӱг+іп"			S" ӱӱп"			parse-test -> TRUE }T
T{ S" узу+аң"			S" узааң"		parse-test -> TRUE }T
T{ S" узу+ар"			S" узир"		parse-test -> TRUE }T
T{ S" узу+ах+ча"		S" узихча"		parse-test -> TRUE }T
T{ S" узу+ғай"			S" узаай"		parse-test -> TRUE }T
T{ S" уйғу+ға"			S" уйғаа"		parse-test -> TRUE }T
T{ S" улуғ+ы"			S" улии"		parse-test -> TRUE }T
T{ S" улуғла+аачых"		S" улуғлаачых"		parse-test -> TRUE }T
T{ S" ӱс+і+ліг"			S" ӱзіліг"		parse-test -> TRUE }T  \ #178
T{ S" ӱстӱ+нде+де"		S" ӱстӱндеде"		parse-test -> TRUE }T  \ #187
T{ S" фамилия+зы"		S" фамилиязы"		parse-test -> TRUE }T
T{ S" фамилия+ы"		S" фамилияы"		parse-test -> FALSE }T
T{ S" финн+нең"			S" финнең"		parse-test -> TRUE }T
T{ S" финн+нең"			S" финннең"		parse-test -> FALSE }T
\ T{ S" хан-пиг+і"		S" хан-пигі"		parse-test -> TRUE }T
T{ S" хаптыр+алыс"		S" хаптыралыс"		parse-test -> TRUE }T
T{ S" хара+ған+зар"		S" хараанзар"		parse-test -> FALSE }T  \ #198
T{ S" харах+ым"			S" хараам"		parse-test -> TRUE }T
T{ S" харах+ым"			S" харағым"		parse-test -> TRUE }T
T{ S" харах+тың+ы+н"		S" харахтиин"		parse-test -> FALSE }T  \ #150
T{ S" хар+бас+ыбыс+са+лар"	S" харбазыбыссалар"	parse-test -> FALSE }T  \ #211
T{ S" харын+ы"			S" харны"		parse-test -> TRUE }T
T{ S" хариб+де"			S" харибде"		parse-test -> TRUE }T  \ #127
T{ S" хас+та+ди"		S" хастади"		parse-test -> FALSE }T  \ #152
T{ S" хатхыр+0̸+чадыр+бын"	S" хатхырчадырбын"	parse-test -> TRUE }T
T{ S" хат+ым"			S" хатым"		parse-test -> TRUE }T
T{ S" хах+ы"			S" хағы"		parse-test -> TRUE }T
T{ S" хол+да+хы+лар"		S" холдағылар"		parse-test -> TRUE }T
T{ S" хол+ы+нда+хы+лар"		S" холындағылар"	parse-test -> TRUE }T
T{ S" хомай+зыбыс"		S" хомайзыбыс"		parse-test -> TRUE }T
T{ S" худай+бынаң"		S" худайбынаң"		parse-test -> TRUE }T
T{ S" худай+ым+ай"		S" худайымай"		parse-test -> TRUE }T
T{ S" худала+н+0̸+чат+чаң"	S" худаланчатчаң"	parse-test -> TRUE }T
T{ S" хузух+ым"			S" хузиим"		parse-test -> TRUE }T
T{ S" хузух+ым"			S" хузуғым"		parse-test -> TRUE }T
T{ S" хулун+ы"			S" хулуны"		parse-test -> TRUE }T
T{ S" хус+тар"			S" хустар"		parse-test -> TRUE }T
T{ S" хусхаӌах+таңары"		S" хусхаӌахтаңары"	parse-test -> TRUE }T
T{ S" хуйу"			S" хую"		        parse-test -> TRUE }T
T{ S" хыйа"			S" хыя"		        parse-test -> TRUE }T
T{ S" хын+маан+ыс+хан+да"	S" хынмаанысханда"	parse-test -> TRUE }T
T{ S" хыс+ы+ох"			S" хызох"		parse-test -> FALSE }T
T{ S" хыс+ох"			S" хызох"		parse-test -> TRUE }T
T{ S" хыс+тар+ы+н"		S" хыстарын"		parse-test -> TRUE }T
T{ S" хыс+тар+ың+ни"		S" хыстарыңни"		parse-test -> TRUE }T
T{ S" хыс+ы+м"			S" хызым"		parse-test -> FALSE }T
T{ S" хыс+ым"			S" хызым"		parse-test -> TRUE }T
T{ S" чазы+ға"			S" чазаа"		parse-test -> TRUE }T
T{ S" чайлағ+ы"			S" чайлаа"		parse-test -> TRUE }T
T{ S" чарын+ы"			S" чарны"		parse-test -> TRUE }T
T{ S" часка+лығ+ға"		S" часкалыға"		parse-test -> TRUE }T
T{ S" чі+еді+ңер"		S" чидер"		parse-test -> FALSE }T  \ #212
T{ S" чі+еді+п"			S" чидіп"		parse-test -> FALSE }T  \ #209
T{ S" чит+іп"			S" чидіп"		parse-test -> TRUE }T  \ #209
T{ S" чі+гелек"			S" чеелек"		parse-test -> TRUE }T
T{ S" чилін+і"			S" чилні"		parse-test -> TRUE }T
T{ S" чі+е+м+дір"		S" чимдір"		parse-test -> TRUE }T
T{ S" чиң+ер"			S" чиңер"		parse-test -> TRUE }T
T{ S" чиң+іп"			S" чиип"		parse-test -> TRUE }T
T{ S" чир-суғ+ы+н"		S" чир-суун"		parse-test -> TRUE }T
T{ S" чілің+і"			S" чілии"		parse-test -> TRUE }T
T{ S" чі+п+ле+іс+еді"		S" чіплеізеді"		parse-test -> TRUE }T
T{ S" чоохта+ды+ңар"            S" чоохтадар"           parse-test -> TRUE }T
T{ S" чоохта+ған+ох+пын"	S" чоохтаанохпын"       parse-test -> TRUE }T
T{ S" чох+ы+ла+ох"		S" чоғылох"		parse-test -> FALSE }T
T{ S" чӧгіг+і"			S" чӧгии"		parse-test -> TRUE }T
T{ S" чӧлег+і"			S" чӧлее"		parse-test -> TRUE }T
T{ S" чӧлег+ім"			S" чӧлеем"		parse-test -> TRUE }T
T{ S" чӧр+беен+іс+тір"		S" чӧрбееністір"	parse-test -> TRUE }T
T{ S" чӧр+ӌең+і"		S" чӧрӌее"		parse-test -> TRUE }T
T{ S" чурта+п+ох+ча+быс"	S" чуртабохчабыс"	parse-test DROP expected-found -> 2 }T
T{ S" чурта+п+чадыр"		S" чуртапчадыр"		parse-test -> TRUE }T
T{ S" чӱг+і"			S" чӱгі"		parse-test -> TRUE }T
T{ S" чӱгӱрт+ібіс+пе+гей+і+н"	S" чӱгӱртібіспеейін"	parse-test -> TRUE }T  \ #145
T{ S" чӱзӱг+і"			S" чӱзии"		parse-test -> TRUE }T
T{ S" чӱк+і"			S" чӱгі"		parse-test -> TRUE }T
T{ S" чӱрек+і+біс+ке"		S" чӱреебіске"		parse-test -> FALSE }T
T{ S" чӱрек+ібіс+ке"		S" чӱреебіске"		parse-test -> TRUE }T
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
T{ S" ээзі+м"			S" ээзім"		parse-test -> TRUE }T
