CELL 4 = [IF]
  $0. 2CONSTANT flag-none
  $1. 2CONSTANT flag/1
  2 CONSTANT flag/sizeof
  : flag/@  ( ptr -- ud )                 ]] 2@ [[ ; IMMEDIATE
  : flag/!  ( ud ptr -- )                 ]] 2! [[ ; IMMEDIATE
  : flag/VARIABLE  ( "name" -- )          2VARIABLE ; IMMEDIATE
  : flag/0=  ( ud -- f )                  ]] D0= [[ ; IMMEDIATE
  : flag/0<>  ( ud -- f )                 ]] D0<> [[ ; IMMEDIATE
  : flag/DROP  ( ud -- )                  ]] 2DROP [[ ; IMMEDIATE
  : flag/DUP  ( ud -- ud ud )             ]] 2DUP [[ ; IMMEDIATE
  : flag/OVER  ( ud1 ud2 -- ud1 ud2 ud1)  ]] 2OVER [[ ; IMMEDIATE
  : flag/SWAP  ( ud1 ud2 -- ud2 ud1 )     ]] 2SWAP [[ ; IMMEDIATE
  : flag/2*  ( ud -- ud')                 ]] D2* [[ ; IMMEDIATE
  : flag/OR  ( ud1 ud2 -- ud' )           >R ROT OR SWAP R> OR ;
  : flag/AND  ( ud1 ud2 -- ud' )          >R ROT AND SWAP R> AND ;
  : flag/INVERT  ( ud -- ud' )            INVERT SWAP INVERT SWAP ;
  : flag/CONSTANT  ( id "name" -- )       2CONSTANT ; IMMEDIATE
  : flag/]]L POSTPONE ]]2L ; IMMEDIATE
  : flag/LITERAL POSTPONE 2LITERAL ; IMMEDIATE
[ELSE]
  0 CONSTANT flag-none
  1 CONSTANT flag/1
  1 CONSTANT flag/sizeof
  : flag/@  ( ptr -- u )                  ]] @ [[ ; IMMEDIATE
  : flag/!  ( u ptr -- )                  ]] ! [[ ; IMMEDIATE
  : flag/VARIABLE  ( "name" -- )          VARIABLE ; IMMEDIATE
  : flag/0=  ( u -- f )                   ]] 0= [[ ; IMMEDIATE
  : flag/0<>  ( u -- f )                  ]] 0<> [[ ; IMMEDIATE
  : flag/DROP  ( u -- )                   ]] DROP [[ ; IMMEDIATE
  : flag/DUP  ( u -- u u )                ]] DUP [[ ; IMMEDIATE
  : flag/OVER  ( u1 u2 -- u1 u2 u1 )      ]] OVER [[ ; IMMEDIATE
  : flag/SWAP  ( u1 u2 -- u2 u1 )         ]] SWAP [[ ; IMMEDIATE
  : flag/2*  ( u -- u')                   ]] 2* [[ ; IMMEDIATE
  : flag/OR  ( u1 u2 -- u' )              OR ;
  : flag/AND  ( u1 u2 -- u' )             AND ;
  : flag/INVERT  ( u -- u' )              INVERT ;
  : flag/CONSTANT  ( ud "name" )          CONSTANT ; IMMEDIATE
  : flag/]]L POSTPONE ]]L ; IMMEDIATE
  : flag/LITERAL POSTPONE LITERAL ; IMMEDIATE
[THEN]

CREATE (flag-ids) 64 CELLS flag/sizeof * ALLOT
(flag-ids) VALUE (flag-id-ptr)
VARIABLE flags(-sys

VOCABULARY flagtype
0 VALUE flagtype-wid
GET-CURRENT  ( -- wordlist )
ALSO flagtype DEFINITIONS
context @ TO flagtype-wid

: )  ( -- mask )
  PREVIOUS
  DEPTH  flags(-sys @  -  flag/sizeof /  1 +DO flag/OR LOOP  ]  POSTPONE flag/LITERAL
  ; IMMEDIATE

PREVIOUS
SET-CURRENT  ( wordlist -- )

: flagtype/FIND-NAME  ( addr u -- nt )
  flagtype-wid SEARCH-WORDLIST IF >NAME ELSE FALSE THEN ;

: flags.  ( mask -- )
  flag/1  ( mask bit )
  flag/sizeof CELLS 8 *  0  DO
    flag/OVER flag/OVER flag/AND flag/0<> IF
      (flag-ids) flag/sizeof I * CELLS + @ .ID
    THEN
    flag/2*  ( mask bit' )
  LOOP flag/DROP flag/DROP ;

: flags(  ( "name ..." -- )
  DEPTH flags(-sys !
  ALSO flagtype POSTPONE [ ; IMMEDIATE

: flag  ( "name" -- mask )
  PARSE-NAME flagtype/FIND-NAME  NAME>INT EXECUTE  POSTPONE flag/LITERAL ; IMMEDIATE

: flag-mask:  ( u "name" -- )
  \ debug-mode? IF
  \   CELL 4 = IF 2VALUE ELSE DROP VALUE THEN
  \ ELSE
    CELL 4 = IF 2CONSTANT ELSE CONSTANT THEN
  \ THEN
  ; IMMEDIATE

: (flag:-check)  ( mask -- )
  flag/DUP flag/2* flag/0= ABORT"  Flag storage overflow!" ;

: flag:  ( mask "name" -- mask' )
  (flag:-check)
  flag/DUP  POSTPONE flag/CONSTANT
  LATEST (flag-id-ptr) !
  flag/sizeof CELLS (flag-id-ptr) +  TO (flag-id-ptr)
  flag/2*                              ( mask')
  ; IMMEDIATE

: flagenum:  ( -- enumsys-wordlist enumsys-mask )
  GET-CURRENT ALSO flagtype DEFINITIONS
  CELL 4 = IF $1. ELSE $1 THEN ; IMMEDIATE

: flagenum;  ( enumsys-wordlist enumsys-mask -- )
  flag/DROP PREVIOUS SET-CURRENT ; IMMEDIATE
