#!/bin/bash

for i in  `cat args.txt` ; do
        #USERID=`echo "$i" | sed 's|,.*||'`
        #echo "$USERID"
        cat ct_args_online.txt > ct_args.txt.$i
        echo "/stepperArg:\"$i\""  >> ct_args.txt.$i
        mono ./bin/ct.exe @ct_args.txt.$i &
done
