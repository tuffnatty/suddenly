#!/bin/bash
grep -E -o '[^ +]+( *‘[^’]+’)? *( *\+ *[^ >]+( *‘[^’]+’)?)+ *> *[^ ]+' khakas/doc.txt \
    | sed -r 's/( *‘[^’]+’|[ -()])//g;y/aceiӊopxyöÿ/асеіңорхуӧӱ/' \
    | sed -r 's/^(.+)>(.+)$/T{ S" \1" S" \2" parse-test -> TRUE }T/' \
    | sort | uniq > khakas/gentest.fs
