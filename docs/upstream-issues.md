# Upstream UDB issues: do they affect ULDB, and which requests are worth taking

Read on 14 Nov 2026 from the issue tracker of UltimateDoomBuilder/UltimateDoomBuilder (about 410 open issues, 435 labelled bug, 202 labelled enhancement), plus the code of upstream's `master` (`9f6d6bf`, three commits past our base `42c8b37`: a persistent-event-lines option, release notes artifact, Flatpak runtime). Issue pages were summarised by a reader tool, so details are second-hand; the maintainer's reply (biwa) is quoted where it decides the matter. "Fixed upstream" bugs are already in our base, because it is newer than every fix listed here.

## 1. UI bugs that upstream never fixed

| Upstream issue | What it is | Does ULDB have it? |
|---|---|---|
| #1051 Edit Linedef dialog has no OK/Cancel at 1280x720 (won't fix: "0.22% of users") | The dialog is taller than the screen, so its buttons are off-screen | **Probably yes, worse.** Our Linedef (UDMF) dialog has `MinHeight` 840 (opens at 920), Sector/Thing/Preferences 600-640. On a 1366x768 or 1600x900 laptop the buttons are off-screen and the window can't be made smaller. **Fix: scrollable bodies, buttons pinned, windows sized to the screen** |
| #1319 labels overlap the boxes in Edit Sector/Linedef (font mismatch) | WinForms font issue | No: our layouts use shared label columns and Avalonia text |
| #1132 colour swatches tiny when Windows scaling isn't 100% | WinForms DPI | No (Avalonia scales). Check on a Wayland/XWayland 150% setup |
| #933 unreadable text in High Contrast theme | Windows theme | Unknown; we follow the system light/dark variant, not high contrast. Low priority |
| #1317 Map Options / Open Map windows can't be resized | Long resource paths cut off | No: both windows resize (only the small Resource Options window is fixed) |
| #1315 "Select Things in Selected Sectors" menu item calls an action dropped 10 years ago and has confusing side effects | Dead menu entry | **Yes**, it is in our Things menu (we even gave it an icon). Check the action and remove or fix |
| #1114 Custom tab doesn't show class-specific keywords until the thing is edited again | Stale tab | Probably yes (same model code, our view); minor |
| #1331 (Linux) Slope Arch mode doesn't unlock the mouse; #902 (Linux) 3D view edit dialogs don't close with Escape and the mouse stays locked | Mouse lock released too late when a dialog opens in Visual/arch modes | **Unknown, must be tried on a real machine.** The upstream lock only applies while the main window is active (`MainForm` clips the pointer only when `windowactive`), our dialogs have Escape bound; the Slope Arch pointer grab is the part to check |
| #1377 (Linux) pasting in 3D hangs when the system clipboard holds a big image | Clipboard parsed as text | Probably not: ULDB's clipboard keeps UDB's own formats in-process and only text goes to the desktop. Untested |
| #1391 X11 instance crashes when a window opens, #1392 Fedora broken scaling, #1255 black screen on Debian, #1361 random NREs on Linux | Platform/driver reports without a diagnosis | **Unknown.** Different stack (Avalonia, not Mono WinForms), so likely not the same cause; worth asking testers |
| #1328 can't indent in the script editor under Flatpak | Mono's Scintilla fallback | No: ULDB has its own editor engine. Check Tab / indent selection once |
| #1394 OBJ export fails on case-sensitive file systems when extension capitalisation differs | Upstream code | **Yes** (same code, Linux file system). Plugin-side fix: compare extensions case-insensitively |
| #972 Enter/Escape in the script editor's Find dialog | Fixed upstream in the WinForms form | **Probably not carried over**: our `ScriptFindReplaceFormView` has no default/cancel button (the fix lives in the form's key handlers, which our view may not route). Check, then add `IsDefault`/`IsCancel` |
| #1275 compile button stays in the wrong state when switching tabs | Fixed upstream | Check our script editor view's toolbar follows the tab (it's our own view) |

Also seen and already fine in ULDB: #1152 Alt-F4 in Visual Mode (fixed upstream, in our base; closing goes through our Closing handler), #1313 blockmap explorer resize (fixed, plugin not shipped), #1061 two mouse buttons crash (fixed), #1229 ellipse "Sides" box missing on Linux (fixed; ours is an Avalonia view), #1095/#979 directory resources failing on Linux (tested: a folder resource with a texture and a flat loads), #1329/#1330 Test Map in Flatpak (done in ULDB), #1156 script editor ignores preferences on Linux (ours applies them).

## 2. Complaints about how things work or look that upstream left alone

Ranked by how much they seem worth doing in ULDB (value for effort, and whether it needs our own UI):

**Do (small, clear win)**
1. **Autosave on an unsaved new map** (#1355): fails with a warning toast and sound every 5 minutes. ULDB already has a Restore folder: autosave an unsaved map there silently instead.
2. **Image browser background colour** (#1216): white or black only, dark textures vanish and black/transparent look the same. We own the tile list's drawing: add a grey option and a transparency chequerboard.
3. **Texture browser remembers its "Show:" filter** (#652, open since 2021).
4. **Dialogs sized to the screen** (#1051, above).
5. **Remove the dead "Select Things in Selected Sectors" item** (#1315).
6. **Text filter in the Thing pickers** (#1138: filter by name or DoomEdNum). Our thing browser is our own view.

**Consider (needs a decision or more work)**
7. **UI scale / font size** (#1146, "too small on a 2K Linux display"; the maintainer first answered "won't fix"). Avalonia gives per-monitor scaling, but under XWayland the scale is often 1.0; an *Interface scale* option in Preferences (a layout scale on windows) would cover it. Wanted by Linux users.
8. **Select all connected linedefs** (#1227, closed "won't add: use UDBScript"; #1301 select between two selections). ULDB doesn't ship UDBScript, so there's no answer for this: a native "select connected" is a small action.
9. **Faster thing rotation in UDMF** (#1191: 5 degrees per scroll click, 36 clicks for a half turn) and **more control over rotation** (#1180): make the step a preference.
10. **Prefab insertion: separate "renumber tags" setting** (#1335).
11. **Selection threshold** (#1219: dragging slightly while clicking restarts a box select and clears a big selection; the maintainer's answer was "hold Shift or raise the threshold"). Consider a larger default threshold.
12. **Environment variables in test engine settings** (#1365), **nodebuilder command lines preserved** (#1376), **preset multiplayer launch options** (#792, #646): the engine quick-setup card added on 22 Oct covers presets; extend it later.
13. **Test engine setting for the unsaved-map / "no countsecret warning" etc.** are content checks, not UI: skip.

**Probably not**
- Split 2D/3D views (#764): the maintainer calls it "rather unlikely" (modes are event-driven and independent); a large change to Visual mode.
- Dark theme (#1154, "not feasible with WinForms"): **ULDB already follows the system light/dark theme.** Worth mentioning in release notes.
- High contrast (#933), 3D-mode rendering requests (sector effects, shadows, 3D floors in 2D #688), model formats (IQM #818): rendering features, not UI.

## 3. Status (branch `phase-3/upstream-fixes`, 14 Nov 2026, no screen)

Done in the container (built, tested, most captured; see `docs/ux-changes.md` and `docs/upstream-edits.md`):

| Item | Upstream | State |
|---|---|---|
| Dialogs and main window fit small screens, buttons pinned | #1051 | Done; checked at 1366x768 and 1280x720 captures |
| Dead "Select Things in Selected Sectors" menu item hidden | #1315 | Done |
| OBJ export flat extension case | #1394 | Done |
| Escape closes Find/Replace | #972 | Done (Enter was already handled) |
| Autosave of a never-saved map | #1355 | Done; tested rotation and recovery file |
| Filter box in long grids (Filter Selected Things) | #1138 | Done (the Thing browser already had one) |
| Browser background (theme / black / greys / chequerboard) | #1216 | Done; captured |
| Texture browser remembers "Show:" | #652 | Already upstream behaviour; not changed. **Verify on the machine** |
| Interface scale 50-400 % | #1146 | Done (restart needed); Preferences → Interface → ULDB |
| Select Connected Linedefs action | #1227, #1301 | Done; unit test `SelectConnectedLinedefsSelectsTheWholeChain` |
| Thing rotation step | #1191 | Done (preference) |
| Prefab tag handling | #1335 | Done (preference) |
| Selection threshold default 4 | #1219 | Done |
| Dark theme | #1154 | Already provided: follows the system theme |
| Preset launch options | #792, #646 | Covered by the engine presets (22 Oct) |

Not done and not needed: #1114 (Custom tab keywords refresh; minor), #1365 environment variables for the engine and #1376 node builder command lines (extend the engine card later), #1180 finer rotation control beyond the step.

## 4. Not in scope for now (kept for future consideration)

These were the "Probably not" items. They are **not planned**, but worth revisiting if users ask:

- **Split or side-by-side 2D/3D views (#764).** The maintainer called it "rather unlikely"; modes are event-driven and independent, so it means reworking Visual mode and its input. A real feature, not a fix.
- **High-contrast theme support (#933).** We follow the system light/dark variant only. Needs a look at how Avalonia handles forced-colours on Linux (KDE has no real equivalent).
- **Rendering requests**: sector effects, shadows and 3D floors drawn in the 2D view (#688), more model formats such as IQM (#818), and similar renderer work (#128, #62). These live in the native renderer and are a separate project.
- **Environment variables / node builder line editing** (#1365, #1376): small, but not requested by ULDB users yet.

## 5. Needs doing on the user's machine

Nothing below could be checked in the container (no real screen, one scale factor, no real engines). Tick off and report anything odd as a GitHub issue.

1. **Small screens and scaling:** open Edit Linedef / Edit Sector / Edit Thing, Preferences and Game Configurations on a small or second monitor and on Wayland at 125-150 %. The buttons must stay visible and the tabs scroll. Check the main window opens fully on screen.
2. **Interface scale** (Preferences → Interface → ULDB): choose 125 % and 150 %, press Apply, restart; the whole UI scales. Choose Automatic afterwards to undo. (Wayland: the value is read from the program config before the window opens.)
3. **Slope Arch mode and Escape in 3D-view dialogs (#1331, #902):** the mouse must be released when a dialog opens and Escape must close it.
4. **Find / Replace in the script editor:** Enter finds the next match, Escape closes the window.
5. **Browser background:** open the texture, flat and sprite browsers with each background choice; dark textures must be visible, and the chequerboard must show through transparent pictures.
6. **Filter Selected Things** on a map with more than 12 thing types: the filter box appears, selection survives a change of filter.
7. **Thing rotation step:** change it to 15 and use the wheel on things in a UDMF map. **Prefab tags:** insert a prefab with tags using each option.
8. **Selection threshold** default 4: does clicking with a slight drag still feel right? (The setting is in Preferences → Classic Modes.)
9. **Select Connected Linedefs:** bind a shortcut (Preferences → Controls → Linedefs) and try it from Linedefs mode and from the command palette.
10. **Unsaved map autosave:** make a new map, wait for the autosave (or lower the interval), look in `~/.config/UltimateLinuxDoomBuilder/Autosaves/` (path may differ in the Flatpak) and open the file.
11. **Texture browser "Show:" memory** (#652): pick Flats, close, reopen.
12. **OBJ export** on the real file system with textures that have upper-case names.
13. **Test Map from the Flatpak** against real GZDoom/UZDoom, native and Flatpak (from the 22 Oct work; still unconfirmed on the machine).
14. **Flatpak build:** build and run the Flatpak to confirm all of the above work inside the sandbox.
