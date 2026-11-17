# Ultimate Linux Doom Builder

> [!IMPORTANT]
> **This is an unofficial fork of [Ultimate Doom Builder](https://github.com/UltimateDoomBuilder/UltimateDoomBuilder), and the port is AI-coded.**
> The Linux port was written largely by an AI coding assistant (Claude, by Anthropic) under the direction of the maintainer ([@theoriginalzads](https://github.com/theoriginalzads)). Every AI-authored commit carries a `Co-Authored-By: Claude` line. Please review it with that in mind.
>
> ULDB is not affiliated with or endorsed by the Ultimate Doom Builder team. **Please report ULDB problems [here](https://github.com/theoriginalzads/UltimateLinuxDoomBuilder/issues), not to the UDB team.**

> [!WARNING]
> **Beta (v0.6.0-beta): keep backups of your maps.** The editor, 2D/3D views, editing modes, saving, the dialogs, the script editor and the bundled plugins are all in place, but it has had little real-world use yet. See [Status](#status).

Ultimate Linux Doom Builder (ULDB) is a **native Linux** map editor for **GZDoom and UZDoom**, based on Ultimate Doom Builder.
<img width="1655" height="1180" alt="image" src="https://github.com/user-attachments/assets/aaf30540-6980-46c8-96d4-7b61d39cba94" />
<img width="1090" height="798" alt="image" src="https://github.com/user-attachments/assets/3389c7e9-7585-4ead-89ed-a08765ae338e" />


Upstream UDB is a Windows application. Its Linux build runs on Mono's WinForms, which is slow and doesn't fit into the Linux desktop. ULDB replaces that:

- **.NET 10** instead of Mono, and **[Avalonia](https://avaloniaui.net/)** instead of WinForms.
- **Desktop integration:** follows your desktop's dark/light mode and accent colour, with HiDPI scaling and the native file dialogs. It's aimed at KDE Plasma and also works on GNOME and others.
- **X11 and Wayland:** runs natively on X11 and through XWayland on Wayland sessions.
- **Familiar UI:** the same windows, menus and shortcuts as UDB, so existing users feel at home. It's cleaned up and more responsive, but not reinvented. See [`docs/ux-changes.md`](docs/ux-changes.md) for every difference.
- **Settings import:** imports your existing UDB settings (from the Mono build, the Flatpak, Wine or Windows).
- **Build tools from source:** node builders and script compilers (ZDBSP, ACC, ZT-BCC, BCC) are built from source for Linux.

## Scope: GZDoom and UZDoom only

ULDB is deliberately **not** a vanilla, Boom/MBF21 or other-port editor. Only the GZDoom and UZDoom game configurations are offered (UDMF, plus the older Doom and Hexen formats for existing GZDoom projects). Vanilla-only node builders are left out.

The code for other targets stays in the source tree, so updates from upstream UDB keep merging cleanly, but it's hidden in the UI. [`docs/scope-hidden.md`](docs/scope-hidden.md) lists what's hidden.

**If you map for vanilla, Boom or other ports, use [upstream Ultimate Doom Builder](https://github.com/UltimateDoomBuilder/UltimateDoomBuilder).**

## Status

| Phase | What | Status |
|---|---|---|
| 0 | Fork, plan, toolchain, test maps | ✅ Done |
| 1 | Editor core on .NET 10 without WinForms; load/save tests | ✅ Done |
| 2 | 3D renderer on a host-provided OpenGL context; ACC/ZDBSP/ZT-BCC/BCC built from source | ✅ Done |
| 3 | Avalonia main window, menus, 2D/3D view | ✅ Done, in testing: main window, map view, input, save, edit dialogs, texture browser, preferences, script editor, Breeze-style icons; main window modernisation to come |
| 4 | Editing modes | ✅ Work through upstream's code; BuilderModes' windows and dockers ported |
| 5 | Dialogs (about 160) | ✅ Ported (a few unused helpers left as placeholders); being tested |
| 6 | Plugins: 3D floors and slopes, stairs, effects, tag range, sound propagation, colour picker | ✅ All nine in-scope plugins and their windows ported |
| 7 | Desktop integration, settings import, Flatpak, AUR package | 🚧 Settings import, CI, `.desktop`/AppStream, AUR `PKGBUILD` and a Flatpak built and released by CI; the packages still need testing on more desktops. See [`packaging/`](packaging/README.md) |

Details: [`docs/ULDB-Plan.md`](docs/ULDB-Plan.md) is the plan, and [`docs/progress.md`](docs/progress.md) tracks progress.

## Building (developers)

Build, run the tests, then start the editor with `Source/ULDB/Builder/bin/Debug/net10.0/uldb`:

```sh
# Arch/CachyOS: sudo pacman -S dotnet-sdk python cmake ninja base-devel libx11 libxfixes mesa zlib freedoom
# Ubuntu:       apt install dotnet-sdk-10.0 python3 cmake ninja-build build-essential libx11-dev libxfixes-dev libgl-dev zlib1g-dev freedoom
git clone --recurse-submodules https://github.com/theoriginalzads/UltimateLinuxDoomBuilder.git
cd UltimateLinuxDoomBuilder
dotnet build Builder.Linux.sln
dotnet test Builder.Linux.sln
```

[`docs/architecture.md`](docs/architecture.md) explains how the fork is put together.

## Credits and licence

ULDB is built on the work of the Ultimate Doom Builder, GZDoom Builder and Doom Builder 2 authors and contributors. All credit for the editor itself goes to them. Upstream's original README is kept in [`docs/upstream-README.md`](docs/upstream-README.md).

Licensed under the **GNU General Public License v3.0**, the same as upstream. See [`LICENSE.txt`](LICENSE.txt).
