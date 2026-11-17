# Ultimate Linux Doom Builder v0.6.1-beta

An **unofficial** native Linux fork of [Ultimate Doom Builder](https://github.com/UltimateDoomBuilder/UltimateDoomBuilder), built on .NET 10 and Avalonia instead of Mono and WinForms. It targets **GZDoom / UZDoom** maps only. The port is **AI-coded** (Claude), and that is disclosed publicly. It isn't endorsed by the UDB team.

This is a **beta**: every window the editor opens is ported, and the work is fixing and polishing. It has had little real-world use, so **back up your maps and save often.** v0.6.1 is a small update on v0.6.0: a clearer Test menu, better engine detection, a tidier main window and a few bug fixes.

## Why use ULDB instead of UDB on Linux

UDB is a Windows program; on Linux it runs on Mono's WinForms. ULDB is the same editor (UDB's own map, mode and plugin code, unchanged where possible) with a new native front end. What that gets you:

- **A native Linux application.** .NET 10 and Avalonia, no Mono or Wine. Settings live in `~/.config`, it installs as a Flatpak or an Arch package, and the compilers (ACC, ZDBSP, ZT-BCC, BCC) are built from source and included.
- **It looks like it belongs on your desktop.** It follows the system's light or dark theme, has Breeze-style icons, scales cleanly on high-resolution displays, and has its own interface scale option.
- **A real script editor.** Syntax colours, folding, autocompletion, call tips, a find bar, hover help, error underlines, zoom, and line editing shortcuts (comment, move, duplicate, delete lines).
- **Testing maps without typing command lines.** ULDB finds the GZDoom and UZDoom you have installed, natively or as Flatpaks, and offers their launch options as tick boxes. It works from inside ULDB's own Flatpak too.
- **Dialogs that fit your screen.** The big edit dialogs scroll and keep OK and Cancel in reach on small screens; upstream's are cut off at 1280x720.
- **Long-standing UDB requests, done:**
  - new maps are autosaved before their first save;
  - Select Connected Linedefs;
  - a choice of background in the texture browser, including a chequerboard for transparency;
  - an adjustable thing rotation step;
  - a choice of how tags are treated when inserting prefabs;
  - a filter box in long lists.
- **Your UDB setup carries over.** ULDB can import the settings of an existing UDB installation on first start.

**What you give up:** ULDB only edits for GZDoom and UZDoom (other game configurations are hidden), it doesn't include UDBScript or the update window, and it is new, so it has had far less use than UDB. On Windows, use UDB.

## New since v0.6.0-beta

**Testing maps**
- **A clearer Test menu** (the arrow beside the Test button). Upstream listed the five skill levels twice, once with monsters and once without. Now there is one **Skill level** list with the current one ticked, a single **Spawn monsters** switch (off adds `-nomonsters`), **Test with engine** (only when more than one engine is set up), **Extra parameters...** (command-line switches for this map's configuration, such as `+god`; upstream's text box in the menu, which didn't work here) and **Test engine settings...**, which opens Game Configurations on the Testing tab.
- **GZDoom and UZDoom AppImages are found.** The Testing tab's quick setup now also looks for `*zdoom*.AppImage` files (executable) in your home folder, Downloads, `~/Applications`, `~/AppImages`, `~/Games`, `~/.local/bin`, `~/.local/share/AppImages` and `/opt`, as well as programs on the PATH and Flatpaks.

**Main window**
- Map statistics (vertices, linedefs, sidedefs, sectors, things) are a row of tiles in the info panel; with nothing highlighted, the mode name is brighter and has an accent bar.
- Clearer separators between groups of buttons in the toolbar and the edit modes strip.
- Docker tabs are rounded, and the selected tab is bold with an accent bar.
- The coordinate and zoom readouts in the status bar keep a fixed width, so the bar no longer shifts as you move the mouse.
- **Help docker:** shortcuts are drawn as key caps and a small mouse with the pressed button lit (left, right, middle, wheel), instead of bold text.

**Opening maps**
- The "Opening map..." cover shows what the editor is doing ("Loading data resources", and so on) and keeps repainting, so the window no longer looks frozen. The load itself still runs on the main thread (issue #13 stays open for a full fix).
- The Open Map dialog offers "Autosaves and backups" and "All files", so `map.wad.MAP01.autosave1` and `.backup1` files can be opened.

**Fixes**
- After pressing OK in **Game Configurations**, the plugins' buttons in the edit modes strip (Apply Directional Shading, Randomize, Pick Sector/Light Color) disappeared and their Mode menu items jumped to the top. They keep their place now (the same bug exists in UDB).
- The check boxes in the Game Configurations list are no longer clipped.

## What works

- Opening, editing and saving GZDoom/UZDoom maps: the editing modes, the 2D and 3D views, undo/redo and all the map element dialogs.
- The built-in compilers (ACC, ZDBSP, ZT-BCC, BCC), and Test Map with GZDoom or UZDoom, installed natively or as a Flatpak, with launch options you tick instead of type.
- The bundled plugins: Builder Modes, 3D Floor Mode, Builder Effects, Stair Sector Builder, Tag Range, Sound Propagation, Color Picker, Tag Explorer and the Comments Panel.
- Texture, thing, action and effect browsers, Map Options, Game Configurations, Preferences, the command palette and the script editor.
- Importing settings from an existing UDB installation.
- Follows the system's light or dark theme, with Breeze-style icons and a layout UDB users will recognise. Dialogs fit small screens, and there is an interface scale option for high-resolution displays.

## Known limits

- Much of this release was checked in a headless test environment and not yet by eye on a real desktop. In particular: the new Test menu and its extra parameters, finding an AppImage engine, Test Map with a Flatpak engine, the script editor, dialogs on small or scaled screens, and the interface scale option. Reports on these are especially welcome.
- Only GZDoom and UZDoom game configurations are shown, on purpose.
- The update window is not included: update through Flatpak or your package manager.
- The splash picture is a placeholder, and the Help menu has no manual yet.

## Install (Flatpak)

Download `ultimate-linux-doom-builder-v0.6.1-beta.flatpak` below (it appears once the build finishes), then:

```sh
flatpak install --user ./ultimate-linux-doom-builder-v0.6.1-beta.flatpak
flatpak run io.github.theoriginalzads.UltimateLinuxDoomBuilder
```

The runtime (`org.freedesktop.Platform//26.08`) comes from Flathub. Arch users can instead build from `packaging/arch/PKGBUILD`.

## Feedback

Bug reports are very welcome on the [issue tracker](https://github.com/theoriginalzads/UltimateLinuxDoomBuilder/issues). Please include what you were doing and, if possible, the text of the Errors window. Current status is tracked in `docs/progress.md`.
