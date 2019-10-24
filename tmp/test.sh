#!/bin/sh

git symbolic-ref HEAD

ticket=$(git rev-parse --abbrev-ref HEAD | awk -F- '/^PREPIT-/ {print $2}')
if [ -n "$ticket" ]; then
	    echo "ticket #$ticket"
fi
