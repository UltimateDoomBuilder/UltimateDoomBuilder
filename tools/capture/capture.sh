#!/bin/sh
# Renders one of ULDB's windows to a PNG without a desktop: opens a test map in a headless X
# server (Xvfb, Mesa llvmpipe through EGL), optionally opens a dialog (ULDB_SHOW_FORM), and has
# ULDB render its own window (ULDB_CAPTURE). Never captures the screen.
#
# Usage: tools/capture/capture.sh OUT.png [FORM] [MAP.wad]
#   FORM: what ULDB_SHOW_FORM takes, e.g. sector, linedef:1, thing, action:buildermodes_sectorsmode
#         (omit for the main window)
#   MAP:  a UDMF map with MAP01 (default: tests/maps/small.wad)
# Environment: ULDB_TEST_IWAD (default: Freedoom 2 from the freedoom package), ULDB_CONFIGURATION
# (default UZDoom_DoomUDMF.cfg), ULDB_BUILD (default Debug).
set -eu

ROOT=$(cd "$(dirname "$0")/../.." && pwd)
OUT=$(realpath -m "$1")
FORM=${2:-}
MAP=$(realpath "${3:-$ROOT/tests/maps/small.wad}")
IWAD=${ULDB_TEST_IWAD:-/usr/share/games/doom/freedoom2.wad}
CFG=${ULDB_CONFIGURATION:-UZDoom_DoomUDMF.cfg}
APP="$ROOT/Source/ULDB/Builder/bin/${ULDB_BUILD:-Debug}/net10.0/uldb"

# A scratch copy of the map and its own settings folder, so captures never touch real ones.
WORK=$(mktemp -d)
trap 'rm -rf "$WORK"' EXIT
cp "$MAP" "$WORK/map.wad"
mkdir -p "$WORK/config"

ULDB_RENDERING=egl ULDB_CAPTURE="$OUT" ULDB_CAPTURE_EXIT=1 ULDB_SHOW_FORM="$FORM" XDG_CONFIG_HOME="$WORK/config" \
	timeout 90 xvfb-run -a -s "-screen 0 ${ULDB_SCREEN:-1600x1000}x24" \
	"$APP" -nosettings "$WORK/map.wad" -map MAP01 -cfg "$CFG" -resource wad "$IWAD" > "$WORK/log.txt" 2>&1 || true

# ULDB_KEEP_LOG=path keeps ULDB's console output (e.g. the ULDB_DUMP_TOOLBARS listing).
[ -z "${ULDB_KEEP_LOG:-}" ] || cp "$WORK/log.txt" "$ULDB_KEEP_LOG"

if [ -f "$OUT" ]; then
	echo "$OUT"
else
	echo "capture.sh: no capture written; ULDB's output:" >&2
	tail -20 "$WORK/log.txt" >&2
	exit 1
fi
