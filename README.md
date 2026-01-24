# Ultimate Doom Builder

## System requirements
- 2.4 GHz CPU or faster (multi-core recommended)
- Windows 7, 8, 10, or 11
- Graphics card with OpenGL 3.2 support

### Required software on Windows
- [Microsoft .Net Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)

## Building on Linux

__Note:__ this is experimental. None of the main developers are using Linux as a desktop OS, so you're pretty much on your own if you encounter any problems with running the application.

### Manual build

These instructions are for Debian-based distros and were tested with Ubuntu 24.04 LTS and Arch.

- Install Mono
  - **Ubuntu:** The `mono-complete` package from the Debian repo doesn't include `msbuild`, so you have to install `mono-complete` by following the instructions on the Mono project's website: https://www.mono-project.com/download/stable/#download-lin
  - **Arch:** mono (and msbuild which is also required) is in the *extra/* repo, which is enabled by default. `sudo pacman -S mono mono-msbuild`
- Install additional required packages
  - **Ubuntu:** `sudo apt install make g++ git libx11-dev libxfixes-dev mesa-common-dev`
  - **Arch:** `sudo pacman -S base-devel`
    - If you're using X11 display manager you may need to install these packages: `libx11 libxfixes`
    - If you are not using the proprietary nvidia driver you may need to install `mesa`
- Go to a directory of your choice and clone the repository (it'll automatically create an `UltimateDoomBuilder` directory in the current directory): `git clone https://github.com/UltimateDoomBuilder/UltimateDoomBuilder.git`
- Compile UDB: `cd UltimateDoomBuilder && make`
- Run UDB: `cd Build && ./builder`
- Alternatively, to compile UDB in debug mode:
  - Run `make BUILDTYPE=Debug` in the root project directory
  - This includes a debug output terminal in the bottom panel

### Flatpak build

The advantage of using Flatpak to build a package is that you do not need to install Mono directly into your system, since everything in the build process will be self-contained. This also means that this works on Linux distributions that do not have `msbuild` in their repository.

- Install Flatpak and Flatpak Builder
  To use and build UDB using Flatpak you need both Flatpak and Flatpak Builder. How they are installed depends on your distribution. For example on Debian-based distributions they can be installed using `sudo apt install flatpak flatpak-builder`. Check your distro's documentation for information on how to install them.
- Go to a directory of your choice and clone the repository (it'll automatically create an UltimateDoomBuilder directory in the current directory):
  ```
  git clone https://github.com/UltimateDoomBuilder/UltimateDoomBuilder.git
  ``` 
- Go to the cloned directory:
  ```
  cd UltimateDoomBuilder
  ```

- Build the Flatpak:
  ```
  ./build_flatpak.sh
  ```

- This will create a file in the format `ultimatedoombuilder-<version>.flatpak`. You can now install and run the Flatpak:

  ```
  flatpak install --user ultimatedoombuilder-<version>.flatpak
  flatpak run io.ultimatedoombuilder.ultimatedoombuilder
  ```


# Links
- [Official thread link](https://forum.zdoom.org/viewtopic.php?f=232&t=66745)
- [Git builds at DRDTeam.org](https://devbuilds.drdteam.org/ultimatedoombuilder/) 

More detailed info can be found in the **editor documentation** (Refmanual.chm)

