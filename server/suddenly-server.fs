#! /usr/bin/gforth-fast -m22M

REQUIRE servecommon.fs

REQUIRE unix/socket.fs

VARIABLE server

: parse-cmdline  ( -- )
  ARGC @ 3 < IF
    ." Syntax: " 0 ARG TYPE ."  language_path port [DEBUG]" CR BYE
  THEN
  ARGC @ 4 = IF
    ." Running in debug mode" CR
    1 TO debug-mode?
  THEN
  0 0  2 ARG  >NUMBER  2DROP D>S  ( port )
  CREATE-SERVER server !               ( )

  1 ARG  FPATH  ALSO-PATH ;

parse-cmdline

8192 CONSTANT req-size
CREATE req req-size ALLOT
0 VALUE socket

: respond  ( addr u -- )
  2DUP TYPE
  socket WRITE-SOCKET
  socket CLOSE-SOCKET ;

CREATE my-emit-buf 1 ALLOT
: my-type socket WRITE-SOCKET ;
: my-emit my-emit-buf C! my-emit-buf 1 my-type ;
: my-xemit ( u -- )
  dup max-single-byte u< IF  my-emit  EXIT  THEN \ special case ASCII
  0 swap  $3F
  BEGIN  2dup u>  WHILE
    2/ >r  dup $3F and $80 or swap 6 rshift r>
  REPEAT  $7F xor 2* or
  BEGIN   dup $80 u>= WHILE  my-emit  REPEAT  drop ;

CREATE query 1024 ALLOT
0 VALUE query-len
: parse-query  ( addr u -- )
  BEGIN DUP 0> WHILE
    OVER C@ BL = IF EXIT THEN
    OVER C@ query query-len + C!
    query-len 1+ TO query-len
    1 /STRING
  REPEAT ;

REQUIRE parser.fs
: serve
  BEGIN  req req-size socket READ-LINE  THROW  DROP  ?DUP WHILE ( size )
    req OVER S" GET /?" STRING-PREFIX? IF
      req 6 +  SWAP 6 - parse-query
    THEN
  REPEAT
  s\" HTTP/1.1 200 OK\r\nContent-Type: text/plain; charset=utf-8\r\n" socket WRITE-SOCKET

  ['] my-emit IS EMIT
  ['] my-type IS TYPE
  ['] my-xemit IS XEMIT
  query query-len parse-args

  parse-mode @ IF
    input-word parse-khak
  THEN
  socket CLOSE-SOCKET
  ;

require lib.fs
libc fork (int) fork
libc signal int int (int) signal
17 CONSTANT SIGCHLD  \ Linux!
1 CONSTANT SIG_IGN   \ Linux!

: main
  SIGCHLD SIG_IGN signal
  BEGIN
    server @  100 LISTEN
    TRY
      server @  ACCEPT-SOCKET TRUE
    ENDTRY-IFERROR
      errno 4 <> IF THROW THEN
      FALSE NOTHROW
    THEN
    IF
      fork IF
        CLOSE-SOCKET
      ELSE
        \ server @  CLOSE-SOCKET ." rrrr"
        TO socket
        serve BYE
      THEN
    THEN
  AGAIN ;

main
