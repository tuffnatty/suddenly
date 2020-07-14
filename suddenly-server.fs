#! /usr/local/bin/gforth -m10M

: language-path  ( -- addr u )
  1 ARG ;

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
  ;

parse-cmdline

8192 CONSTANT req-size
CREATE req req-size ALLOT
0 VALUE socket

: respond  ( addr u -- )
  2DUP TYPE
  socket WRITE-SOCKET
  socket CLOSE-SOCKET ;

CREATE my-emit-buf 8 ALLOT
VARIABLE my-emit-buf-len
ACTION-OF TYPE CONSTANT orig-type
ACTION-OF EMIT CONSTANT orig-emit
ACTION-OF XEMIT CONSTANT orig-xemit
: my-type
  2DUP STDERR WRITE-FILE THROW
  socket WRITE-SOCKET ;
: my-emit
  my-emit-buf DUP >R C! R> 1 my-type ;
: my-xemit  ( u -- )
  dup max-single-byte u< IF  my-emit  EXIT  THEN \ special case ASCII
  0 my-emit-buf-len !
  0 swap  $3F
  BEGIN  2dup u>  WHILE
    2/ >r  dup $3F and $80 or swap 6 rshift r>
  REPEAT  $7F xor 2* or
  BEGIN   dup $80 u>= WHILE
    my-emit-buf-len @  my-emit-buf OVER +  SWAP 1+ my-emit-buf-len ! C!
  REPEAT  drop
  my-emit-buf my-emit-buf-len @ my-type ;

CREATE query 1024 ALLOT
0 VALUE query-len
: parse-query  ( addr u -- )
  \stack-mark
  0 TO query-len
  BEGIN DUP 0> WHILE
    OVER C@  BL  <> IF
      OVER C@  query query-len +  C!
      query-len 1+ TO query-len
      1 /STRING
    ELSE DROP 0 THEN
  REPEAT
  \stack-check
  2DROP ;

REQUIRE parser.fs
: serve  ( -- )
  \stack-mark
  TRY
    BEGIN  req req-size socket READ-LINE  THROW  DROP  ?DUP WHILE ( size )
      req OVER  S" GET /?"  STRING-PREFIX? IF
        req OVER STDERR WRITE-LINE THROW
        req 6 +  SWAP 6 - parse-query
      ELSE DROP THEN
    REPEAT
    s\" HTTP/1.1 200 OK\r\nContent-Type: text/plain; charset=utf-8\r\n" socket WRITE-SOCKET

    query query-len parse-args

    ['] my-emit IS EMIT
    ['] my-type IS TYPE
    ['] my-xemit IS XEMIT
    parse-mode @ IF
      input-word parse-khak
    THEN
  ENDTRY-IFERROR
    ." ERROR " . bt
  THEN
  socket CLOSE-SOCKET  0 TO socket
  orig-emit IS EMIT
  orig-type IS TYPE
  orig-xemit IS XEMIT
  \stack-check
  ;

c-library forklib
\c #include <signal.h>
\c #include <stdio.h>
\c #include <unistd.h>
\c int n_children = 0;
\c pid_t children[8];
\c int do_fork(void) {
\c   fflush(stdout);
\c   fflush(stderr);
\c   pid_t child = fork();
\c   if (child) {
\c     children[n_children++] = child;
\c   } else {
\c     fprintf(stderr, "forked, in child from C\n"); fflush(stderr);
\c     signal(SIGINT, SIG_DFL);
\c     fprintf(stderr, "forked, in child from C after signal\n"); fflush(stderr);
\c   }
\c   return child;
\c }
\c #include <sys/types.h>
\c #include <sys/wait.h>
\c void reap(int sig) {
\c   int wstatus, n;
\c   pid_t pid;
\c   fprintf(stderr, "reap:");
\c   while ((pid = waitpid(-1, &wstatus, WNOHANG)) != 0 && pid != -1) {
\c     for (n = 0; n < 8; n++) {
\c       if (children[n] == pid) {
\c         fprintf(stderr, " %d", pid);
\c         children[n] = 0;
\c         n_children--;
\c       }
\c     }
\c   }
\c   fprintf(stderr, "; %d children left\n", n_children);
\c   fflush(stderr);
\c }
\c sig_t orig_sigint_handler = NULL;
\c void hunt(int sig) {
\c   int n;
\c   fprintf(stderr, "hunt: kill -INT ");
\c   for (n = 0; n < 8; n++) {
\c     if (children[n]) {
\c       fprintf(stderr, " %d", children[n]);
\c       kill(children[n], SIGINT);
\c     }
\c   }
\c   fprintf(stderr, "\n");
\c   fflush(stderr);
\c   orig_sigint_handler(sig);
\c }
\c int init_reaper(void) {
\c   struct sigaction sa = {
\c     .sa_handler = reap,
\c     .sa_flags = SA_RESTART,
\c   };
\c   sigfillset(&sa.sa_mask);
\c   if (sigaction(SIGCHLD, &sa, NULL)) {
\c     perror("sigaction ERROR");
\c     fflush(stderr);
\c   }
\c   orig_sigint_handler = signal(SIGINT, hunt);
\c   return 0;
\c }
c-function fork do_fork -- n
c-function init-reaper init_reaper -- n
c-function pause pause -- n
end-c-library


: main
  init-reaper DROP
  server @  100 LISTEN
  fork IF fork IF fork IF fork IF
    "4 workers preforked" STDERR WRITE-LINE THROW  STDERR FLUSH-FILE THROW
    BEGIN pause DROP AGAIN
  THEN THEN THEN THEN
  BEGIN
    \stack-mark
    TRY
      server @  ACCEPT-SOCKET TRUE
    ENDTRY-IFERROR
      errno 4 <> IF THROW THEN
      FALSE NOTHROW
    THEN
    IF
      \ fork IF CLOSE-SOCKET ELSE
        \ server @  CLOSE-SOCKET ." rrrr"
        TO socket
        serve \ BYE
      \ THEN
    THEN
    \stack-check
  AGAIN ;

main
