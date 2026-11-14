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

## 3. Practical next steps
1. Make the big edit dialogs fit small screens (scroll body, pinned buttons): the one real UI bug carried over from upstream.
2. Check on the user's machine: Slope Arch mouse release, Escape in 3D-view dialogs, the Find dialog's Enter/Escape, a Wayland scaling check at 150%.
3. Take the six "Do" items; open each as a GitHub issue on the fork first so the user can reorder them.
