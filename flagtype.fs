require quadruple.fs

128 CONSTANT flag-capacity
flag-capacity  CELL 8 *  /  CONSTANT flag-cells

flag-cells 4 = [IF]
  $0. $0. 4CONSTANT flag-none
  $1. $0. 4CONSTANT flag/1
  4 CONSTANT flag/sizeof
  : flag/@  ( ptr -- ud )                 ]] 4@ [[ ; IMMEDIATE
  : flag/!  ( ud ptr -- )                 ]] 4! [[ ; IMMEDIATE
  : flag/VARIABLE  ( "name" -- )          4VARIABLE ; IMMEDIATE
  : flag/0=  ( ud -- f )                  ]] Q0= [[ ; IMMEDIATE
  : flag/0<>  ( ud -- f )                 ]] Q0<> [[ ; IMMEDIATE
  : flag/DROP  ( ud -- )                  ]] 4DROP [[ ; IMMEDIATE
  : flag/DUP  ( ud -- ud ud )             ]] 4DUP [[ ; IMMEDIATE
  : flag/OVER  ( ud1 ud2 -- ud1 ud2 ud1)  ]] 4OVER [[ ; IMMEDIATE
  : flag/SWAP  ( ud1 ud2 -- ud2 ud1 )     ]] 4SWAP [[ ; IMMEDIATE
  : flag/2*  ( ud -- ud')                 ]] Q2* [[ ; IMMEDIATE
  : flag/OR  ( ud1 ud2 -- ud' )           4OR ; IMMEDIATE
  : flag/AND  ( ud1 ud2 -- ud' )          ]] 4AND [[ ; IMMEDIATE
  : flag/INVERT  ( ud -- ud' )            ]] 4INVERT [[ ; IMMEDIATE
  : flag/CONSTANT  ( id "name" -- )       4CONSTANT ; IMMEDIATE
  : flag/LITERAL                          ]] 4LITERAL [[ ; IMMEDIATE
  : flag-mask:  ( u "name" -- )           4CONSTANT ; IMMEDIATE
[THEN]
flag-cells 2 = [IF]
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
  : flag/OR  ( ud1 ud2 -- ud' )           2OR ;
  : flag/AND  ( ud1 ud2 -- ud' )          ]] 2AND [[ ; IMMEDIATE
  : flag/INVERT  ( ud -- ud' )            ]] 2INVERT [[ ; IMMEDIATE
  : flag/CONSTANT  ( id "name" -- )       2CONSTANT ; IMMEDIATE
  : flag/LITERAL                          ]] 2LITERAL [[ ; IMMEDIATE
  : flag-mask:  ( u "name" -- )           2CONSTANT ; IMMEDIATE
[THEN]
flag-cells 1 = [IF]
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
  : flag/LITERAL                          ]] LITERAL [[ ; IMMEDIATE
  : flag-mask:  ( u "name" -- )           CONSTANT ; IMMEDIATE
[THEN]

CREATE (flag-ids) flag-capacity CELLS flag/sizeof * ALLOT
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
  PARSE-NAME flagtype/FIND-NAME  NAME>INTERPRET EXECUTE  POSTPONE flag/LITERAL ; IMMEDIATE

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
  flag/1 ; IMMEDIATE

: flagenum;  ( enumsys-wordlist enumsys-mask -- )
  flag/DROP PREVIOUS SET-CURRENT ; IMMEDIATE
