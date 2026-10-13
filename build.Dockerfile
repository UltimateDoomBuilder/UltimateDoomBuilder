FROM debian:bookworm-20261005

SHELL ["bash", "-euc"]

RUN apt update; \
    apt install -y gnupg ca-certificates; \
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF; \
    echo "deb https://download.mono-project.com/repo/debian stable-buster main" > /etc/apt/sources.list.d/mono-official-stable.list; \
    apt update; \
    apt install -y --no-install-recommends \
        dirmngr \
        gnupg \
        g++ \
        git \
        libx11-dev \
        libxfixes-dev \
        make \
        mesa-common-dev \
        mono-complete;
