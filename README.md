# Ultimate Linux Doom Builder

> [!IMPORTANT]
> **This is an unofficial fork of [Ultimate Doom Builder](https://github.com/UltimateDoomBuilder/UltimateDoomBuilder), and the port is AI-coded.**
> The Linux port was written largely by an AI coding assistant (Claude, by Anthropic) under the direction of the maintainer ([@theoriginalzads](https://github.com/theoriginalzads)). Every AI-authored commit carries a `Co-Authored-By: Claude` line. Please review it with that in mind.
>
> ULDB is not affiliated with or endorsed by the Ultimate Doom Builder team. **Please report ULDB problems [here](https://github.com/theoriginalzads/UltimateLinuxDoomBuilder/issues), not to the UDB team.**

> [!WARNING]
> **Beta (v0.6.1-beta): keep backups of your maps.** The editor, 2D/3D views, editing modes, saving, the dialogs, the script editor and the bundled plugins are all in place, but it has had little real-world use yet. See [Status](#status).

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

## FAQ and stuff

**But this is AI slop, why?**
I'm not a dotnet developer. Even if I was, this is way too much of a project for me to take on using hand-crafted artisan code. I understand why people dislike AI code and that's fine. The underlying code that does all the heavy non-UI lifting is still Ultimate Doom Builder and everything underneath. The changes are mostly UI related with some bugfixes and changes to allow this to work on .net 10 instead of Mono. I am fully upfront about it being AI coded and you are more than welcome to make a choice whether to use it or not based on that alone. 

**Couldn't a human do this port? Isn't there a Linux port already?**
A human could do this port and there is already an official UDB port for Linux. Whilst Ultimate Doom Builder is an excellent (probably the best) map editor for Doom on Windows, the Linux port is not great. They admit it is a community maintained unofficial build with a few bugs, some pretty nasty. It also looks like crap since it is using Mono to convert the Winforms UI. I don't have any grudge about them not prioritising the Linux build. It is their project and they need to prioritise what they think is right. But it is still open-source software and means that I can come along and prioritise what I want. I wanted a better code editor (that doesn't crash), I wanted native dark mode support, I wanted Linux native and I wanted some gripes of mine fixed.

So I used the tools I had to make it happen. I wholeheartedly hope that someone more skilled than me picks this up or redoes this work without AI or with limited AI tooling. Until that happens then this is what I could do. If you are interested in contributing feel free to reach out.

**Is this going to be radically different to the original?**
No. There are differences. The code editor is overhauled for example and the icons are all different (styled in Breeze). There are a lot of small documented changes that are all around improving or cleaning up the UI and fixing any bugs coming from the upstream project. If you are a heavy Ultimate Doom Builder user and move to this, you will find nothing that wrecks your muscle memory. This isn't a "I think the original developers were wrong in everyway so I changed everything" project. It is a better functioning UI with some polish for Linux users.

**Why no support for anything except GZ and UZDoom?**
I did drop support for this. It was mainly as the tools in the background were not all Linux native and because I never map in anything except UZDoom. If you do want to see that support then I'm happy to add it. Since I never map in these modes I'd be relying on others to report any bugs and issues. So if you want to do that, raise an issue asking for it and explaining which parts you'd be fine providing feedback on. 

**Will you lodge any bugs the AI agent finds in the upstream code?**
I will talk with the UDB team before I start doing that. If I do then I'd have to verify the bugs myself first and verify the changes actually fix issues. I'm not sure what UDB's stance on AI is. If they outright disallow it then I'm not going to violate that.

**Should I use this fork?**
I built it for myself because of problems I had. As with any software you should decide if this is right for you. I believe for Linux users this runs much better than the official Linux builds. But I'm 1 person testing this running it on 1 machine. It is code I don't entirely understand in a language I certainly don't. If you do use it and find a bug, or if you (or another AI) runs through it and finds any critical issues or vulnerabilities please do raise an issue.

**Why's there reference to a CachoOS version?**
I use CachyOS and had considered putting out a separate build that targets Zen 4. It is beyond unlikely it will make any substantial performance difference but I have a capable machine and there's nothing stopping me from doing it. 
