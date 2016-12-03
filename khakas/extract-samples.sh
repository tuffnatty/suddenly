#!/bin/bash
WORD="[^ +]+"
QUOTES="( *['‘][^'’]+[’'])?"
RE_FROM="$WORD$QUOTES *< *$WORD$QUOTES( *\+ *$WORD$QUOTES)*"
RE_TO="$WORD$QUOTES( *\+ *$WORD$QUOTES)* *> *$WORD$QUOTES"
grep -E -o "($RE_FROM|$RE_TO)" khakas/doc.txt \
    | grep -E -v '^(.Г.|.) *>'
grep -E -o "($RE_FROM|$RE_TO)" khakas/doc.txt \
    | grep -E -v '^(.Г.|.) *>' \
    | sed -r "s/( *[‘'][^’']+['’]|[ (.,;)])//g;s/-/+/g;s/\\++/+/g;s/\\+й\\+/+/g;y/aceiӊopxyöÿ/асеіңорхуӧӱ/" \
    | sed -r 's/^(.+)>(.+)$/T{ S" \1" S" \2" parse-test -> TRUE }T/' \
    | sed -r 's/^(.+)<(.+)$/T{ S" \2" S" \1" parse-test -> TRUE }T/' \
    | sort | uniq > khakas/gentest.fs.new
