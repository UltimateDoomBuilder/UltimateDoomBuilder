#!/usr/bin/bash

HAS_FLATPAK=1
HAS_FLATPAK_BUILDER=1

# Check if Git is installed and get the commit count if it's a Git repository
if command -v git > /dev/null 2>&1 && [ -d .git ]; then
    UDB_VERSION=$(git rev-list --count HEAD 2>/dev/null)
else
    UDB_VERSION="0"
fi

# Check if flatpak and flatpak-builder are installed
if ! command -v flatpak > /dev/null 2>&1; then
    HAS_FLATPAK=0
fi

if ! command -v flatpak-builder > /dev/null 2>&1; then
    HAS_FLATPAK_BUILDER=0
fi

if [ "$HAS_FLATPAK" -eq 0 ] || [ "$HAS_FLATPAK_BUILDER" -eq 0 ]; then
    echo
    echo "Missing the following flatpak tools:"
    if [ "$HAS_FLATPAK" -eq 0 ]; then
        echo "  - flatpak"
    fi
    if [ "$HAS_FLATPAK_BUILDER" -eq 0 ]; then
        echo "  - flatpak-builder"
    fi
    echo
    echo "Please install the missing tools and try again."
    echo
    exit 1
fi

# Check if the flathub remote is added
if ! flatpak remote-list --columns=name | grep -q '^flathub$'; then
    echo
    echo "Flathub remote is not added. Adding it now..."
    echo
    flatpak remote-add --user --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo
fi

# Build the flatpak
if ! flatpak-builder --user --force-clean --install-deps-from=flathub --repo=udb-flatpak-repo flatpak-build flatpak/io.github.ultimatedoombuilder.ultimatedoombuilder.yml; then
    echo
    echo "Flatpak build failed."
    echo
    exit 1
fi

# Create the flatpak bundle
echo
echo "Creating flatpak bundle (this may take a while)..."
if ! flatpak build-bundle udb-flatpak-repo ultimatedoombuilder-$UDB_VERSION.flatpak io.github.ultimatedoombuilder.ultimatedoombuilder --runtime-repo=https://flathub.org/repo/flathub.flatpakrepo; then
    echo
    echo "Failed to create the flatpak bundle."
    echo
    exit 1
fi

echo
echo "Flatpak bundle 'ultimatedoombuilder-$UDB_VERSION.flatpak' created successfully."
echo
echo "You can install and run it using the following commands:"
echo
echo "    flatpak install --user ./ultimatedoombuilder-$UDB_VERSION.flatpak"
echo "    flatpak run io.github.ultimatedoombuilder.ultimatedoombuilder"
echo
