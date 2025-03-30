#!/usr/bin/env bash

set -eux;

docker build -t mono-builder:local -f build.Dockerfile .

docker run -it -v ${PWD}:/source mono-builder:local bash -c "cd source/ && make ${1};"