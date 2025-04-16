\ 4-cell value support

\ Some 2-cell value stuff
: 2AND  ( ud1 ud2 -- ud' )  ]] >R ROT AND SWAP R> AND [[ ; IMMEDIATE
: 2INVERT  ( ud -- ud'  )   ]] INVERT SWAP INVERT SWAP [[ ; IMMEDIATE
: 2OR  ( ud1 ud2 -- ud' )   ]] >R ROT OR SWAP R> OR [[ ; IMMEDIATE

4 CELL = [IF]  \ We don't need the stuff on 64-bit
: 4!  ( ud1 ud2 ptr -- )               ]] DUP CELL+ >R -ROT >R 2! 2! [[ ; IMMEDIATE
: 4@  ( ptr -- ud1 ud2 )               ]] DUP CELL+ SWAP 2@ ROT 2@ [[ ; IMMEDIATE
: 4AND  ( q1 q2 -- q' )                ]] 2>R 2ROT 2AND 2SWAP 2R> 2AND [[ ; IMMEDIATE
: 4CONSTANT  ( ud1 ud2 "name" -- )     >R : R> POSTPONE 2LITERAL POSTPONE 2LITERAL POSTPONE ; ;
: 4DROP  ( ud1 ud2 -- )                ]] 2DROP 2DROP [[ ; IMMEDIATE
: 4DUP  ( ud1 ud2 -- ud1 ud2 ud1 ud2 ) ]] 2OVER 2OVER [[ ; IMMEDIATE
: 4INVERT  ( q -- q' )                 ]] 2INVERT 2SWAP 2INVERT 2SWAP [[ ; IMMEDIATE
: 4LITERAL  ( q -- )                   ]] 2SWAP POSTPONE 2LITERAL POSTPONE 2LITERAL [[ ; IMMEDIATE
: 4OR  ( q1 q2 -- q' )                 ]] 2>R 2ROT 2OR 2SWAP 2R> 2OR [[ ; IMMEDIATE
: 4OVER  ( ud1 ud2 ud3 ud4 -- ud1 ud2 ud3 ud4 ud1 ud2 )  ]] 7 PICK 7 PICK 7 PICK 7 PICK [[ ; IMMEDIATE
: 4SWAP  ( ud1 ud2 ud3 ud4 -- ud3 ud4 ud1 ud2 )          ]] 7 ROLL 7 ROLL 7 ROLL 7 ROLL [[ ; IMMEDIATE
: 4VARIABLE  ( "name" -- )             CREATE 4 CELLS ALLOT ;
: Q0=  ( ud1 ud2 -- f )                ]] D0= -ROT D0= AND [[ ; IMMEDIATE
: Q0<> ( ud1 ud2 -- f )                ]] D0<> -ROT D0<> OR [[ ; IMMEDIATE
: Q2*  ( q -- 2*q )                    ]] D2* 2>R  2DUP D0< IF 2R> 1. D+ 2>R THEN  D2* 2R> [[ ; IMMEDIATE
[THEN]
