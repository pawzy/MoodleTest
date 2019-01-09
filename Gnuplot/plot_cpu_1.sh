#!/bin/sh -u
#   $0 [atop logfile to plot]

log=${1-'/var/log/atop.log'}

tmp=/tmp/atop$$
rm -f $tmp
trap "rm -f $tmp" 0 1 2

atop -PCPL -r "$log" >$tmp

gnuplot -persist <<EOF

set xdata time 
set timefmt '%Y/%m/%d %H:%M:%S' 
set format x "%Y\n%m/%d\n%H:%M" 
set grid
set key right top
set title "CPU load for 40 users"
set terminal pdf
set output "plot_cpu_1.pdf"
plot \
   "$tmp" using 4:8 t ''  , \
   "$tmp" using 4:8 smooth csplines t 'CPU load avg' ,  ;

EOF
