# Ultimate Linux Doom Builder v0.6.0-beta

An **unofficial** native Linux fork of [Ultimate Doom Builder](https://github.com/UltimateDoomBuilder/UltimateDoomBuilder), built on .NET 10 and Avalonia instead of Mono and WinForms. It targets **GZDoom / UZDoom** maps only. The port is **AI-coded** (Claude), and that is disclosed publicly. It isn't endorsed by the UDB team.

This is the first **beta**: every window the editor opens is ported, and the work has moved from porting to fixing and polishing. It has still had little real-world use, so **back up your maps and save often.**

## New since v0.5.0-preview

**Script editor**
- A find bar inside the editor: **Ctrl+F** to find, **Ctrl+H** to replace, F3 / Shift+F3 for next and previous, with match case, whole word and regular expressions. The Find and Replace window (all open scripts) is still in the toolbar and on Ctrl+Shift+F. The toolbar's old quick-search box is gone.
- Rest the pointer on a function name to see its signature; the completion list shows it too.
- Compile errors are underlined in the text, and one click (or Enter) on an error in the list shows its line.
- Tabs: middle-click closes a tab, dragging reorders them.
- Zoom (Ctrl+Plus / Minus / 0) now stays, is the same in every open script, and is remembered between sessions.
- Folded blocks are saved with the map again, stay folded when you type above them, and open when you jump to an error or a search result inside one. Error markers and highlighted words follow the text when lines are added or removed.
- Faster on long scripts: only the changed part is coloured again, and moving the caret no longer redraws the whole editor.
- A new option for comments in italics (Preferences → Interface → ULDB).

**Dialogs and Preferences**
- Preferences opens large enough to show its pages without scrolling, and shrinks only on a small screen.
- Preferences → Controls: the action list scrolls by itself, the Action control panel stays in place, and actions are listed by category and name.
- Dialogs that can be resized can be maximised again.

**Testing maps**
- A Flatpak GZDoom or UZDoom can now read IWADs in the system's folders (for example the distribution's Freedoom package under `/usr/share`).
- The interface scale option also works in portable mode.

**Other**
- The window title no longer shows a revision number; the version is in Help → About.

## What works

- Opening, editing and saving GZDoom/UZDoom maps: the editing modes, the 2D and 3D views, undo/redo and all the map element dialogs.
- The built-in compilers (ACC, ZDBSP, ZT-BCC, BCC), and Test Map with GZDoom or UZDoom, installed natively or as a Flatpak, with launch options you tick instead of type.
- The bundled plugins: Builder Modes, 3D Floor Mode, Builder Effects, Stair Sector Builder, Tag Range, Sound Propagation, Color Picker, Tag Explorer and the Comments Panel.
- Texture, thing, action and effect browsers, Map Options, Game Configurations, Preferences, the command palette and the script editor.
- Importing settings from an existing UDB installation.
- Follows the system's light or dark theme, with Breeze-style icons and a layout UDB users will recognise. Dialogs fit small screens, and there is an interface scale option for high-resolution displays.

## Known limits

- Much of this release was checked in a headless test environment and not yet by eye on a real desktop. In particular: the script editor changes above, Test Map with a Flatpak engine, dialogs on small or scaled screens, and the interface scale option. Reports on these are especially welcome.
- Only GZDoom and UZDoom game configurations are shown, on purpose.
- The update window is not included: update through Flatpak or your package manager.
- The splash picture is a placeholder, and the Help menu has no manual yet.

## Install (Flatpak)

Download `ultimate-linux-doom-builder-v0.6.0-beta.flatpak` below (it appears once the build finishes), then:

```sh
flatpak install --user ./ultimate-linux-doom-builder-v0.6.0-beta.flatpak
flatpak run io.github.theoriginalzads.UltimateLinuxDoomBuilder
```

The runtime (`org.freedesktop.Platform//26.08`) comes from Flathub. Arch users can instead build from `packaging/arch/PKGBUILD`.

## Feedback

Bug reports are very welcome on the [issue tracker](https://github.com/theoriginalzads/UltimateLinuxDoomBuilder/issues). Please include what you were doing and, if possible, the text of the Errors window. Current status is tracked in `docs/progress.md`.
