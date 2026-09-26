#!/usr/bin/env bash
cd "$(dirname "$0")"
export LD_LIBRARY_PATH="$PWD:$LD_LIBRARY_PATH"
mono Builder.exe
