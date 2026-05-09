#!/bin/bash
# Deploy Android .so files to the standard bin/android/ directory structure
# Run this script after `cargo build --target aarch64-linux-android`
# Usage: ./deploy_android.sh [release|debug]

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
MODE="${1:-release}"

echo "Deploying Android .so ($MODE) to bin/android/..."

# Create target directories
mkdir -p "$SCRIPT_DIR/bin/android/$MODE/arm64-v8a"
mkdir -p "$SCRIPT_DIR/bin/android/${MODE}/x86_64"

# Copy from cargo target directory to bin/
TARGET_DIR="$SCRIPT_DIR/target/aarch64-linux-android/$MODE"

if [ -f "$TARGET_DIR/libnine_slice_sprite_2d.so" ]; then
    cp "$TARGET_DIR/libnine_slice_sprite_2d.so" \
       "$SCRIPT_DIR/bin/android/$MODE/arm64-v8a/libnine_slice_sprite_2d.so"
    echo "  Copied arm64-v8a/libnine_slice_sprite_2d.so"
else
    echo "  WARNING: $TARGET_DIR/libnine_slice_sprite_2d.so not found!"
    echo "  Run 'cargo build --target aarch64-linux-android' first."
fi

# Also check for x86_64 if available
TARGET_X86_DIR="$SCRIPT_DIR/target/x86_64-linux-android/$MODE"
if [ -f "$TARGET_X86_DIR/libnine_slice_sprite_2d.so" ]; then
    cp "$TARGET_X86_DIR/libnine_slice_sprite_2d.so" \
       "$SCRIPT_DIR/bin/android/$MODE/x86_64/libnine_slice_sprite_2d.so"
    echo "  Copied x86_64/libnine_slice_sprite_2d.so"
fi

echo "Done! .so files are in $SCRIPT_DIR/bin/android/$MODE/"
echo ""
echo "Next steps:"
echo "  1. Re-open the Godot editor (to regenerate extension_list.cfg)"
echo "  2. Export Android APK"
echo "  3. Verify with: unzip -l <apk> | grep libnine"
