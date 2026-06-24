FROM mono:latest

RUN apt update; \
    apt install -y --no-install-recommends \
        make \
        g++ \
        git \
        libx11-dev \
        libxfixes-dev \
        mesa-common-dev;
