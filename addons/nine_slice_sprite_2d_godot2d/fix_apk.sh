#!/bin/bash
# Post-export script: Inject GDExtension .so into Android APK and re-sign
#
# Works around Godot engine bug: GDExtension .so files are not included
# in the APK's lib/arm64-v8a/ during Android export (one-click deploy).
# See: https://github.com/godotengine/godot-cpp/issues/905
#
# Usage:
#   ./fix_apk.sh <input.apk> [release|debug]

set -e

find_android_tool() {
    local tool="$1"
    local path

    if path="$(command -v "$tool" 2>/dev/null)"; then
        echo "$path"
        return 0
    fi

    for sdk in "${ANDROID_HOME:-}" "${ANDROID_SDK_ROOT:-}" "$HOME/Library/Android/sdk" "$HOME/Android/Sdk"; do
        if [ -n "$sdk" ] && [ -d "$sdk/build-tools" ]; then
            path="$(/usr/bin/find "$sdk/build-tools" -name "$tool" -type f 2>/dev/null | /usr/bin/sort | /usr/bin/tail -n 1)"
            if [ -n "$path" ]; then
                echo "$path"
                return 0
            fi
        fi
    done

    return 1
}

APK_PATH="$1"
MODE="${2:-release}"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
SO_DIR="$SCRIPT_DIR/bin/android/$MODE/arm64-v8a"
SO_FILE="$SO_DIR/libnine_slice_sprite_2d.so"

if [ ! -f "$APK_PATH" ]; then
    echo "ERROR: APK not found: $APK_PATH"
    echo "Usage: $0 <input.apk> [release|debug]"
    exit 1
fi

APK_PATH="$(cd "$(dirname "$APK_PATH")" && pwd)/$(basename "$APK_PATH")"

if [ ! -f "$SO_FILE" ]; then
    echo "ERROR: .so not found: $SO_FILE"
    echo "Run './deploy_android.sh $MODE' first"
    exit 1
fi

APK_DIR="$(dirname "$APK_PATH")"
APK_NAME="$(basename "$APK_PATH" .apk)"
FIXED_APK="$APK_DIR/${APK_NAME}_fixed.apk"
SIGNED_APK="$APK_DIR/${APK_NAME}_signed.apk"
TEMP_ALIGNED="$APK_DIR/${APK_NAME}_aligned.apk"
WORK_DIR="$(mktemp -d)"
DEBUG_KEYSTORE="$HOME/.android/debug.keystore"
ZIPALIGN="$(find_android_tool zipalign || true)"
APKSIGNER="$(find_android_tool apksigner || true)"

echo "=== Fixing APK: $APK_PATH ==="

# Step 1: Unzip
echo "[1/5] Extracting APK..."
cd "$WORK_DIR"
unzip -q "$APK_PATH"

# Step 2: Inject .so
echo "[2/5] Injecting libnine_slice_sprite_2d.so into lib/arm64-v8a/..."
mkdir -p lib/arm64-v8a
cp "$SO_FILE" "lib/arm64-v8a/libnine_slice_sprite_2d.so"
SO_SIZE=$(ls -lh lib/arm64-v8a/libnine_slice_sprite_2d.so | awk '{print $5}')
echo "  Added: lib/arm64-v8a/libnine_slice_sprite_2d.so ($SO_SIZE)"

# Step 3: Re-zip
echo "[3/5] Repackaging APK..."
rm -f "$FIXED_APK" "$SIGNED_APK" "$TEMP_ALIGNED"
zip -qr "$FIXED_APK" .

# Step 4: Align
echo "[4/5] Aligning APK..."
if [ -n "$ZIPALIGN" ]; then
    "$ZIPALIGN" -p -f 4 "$FIXED_APK" "$TEMP_ALIGNED" 2>/dev/null && \
    mv "$TEMP_ALIGNED" "$FIXED_APK"
    echo "  Aligned"
else
    echo "  zipalign not found, skipping"
fi

# Step 5: Sign with debug keystore (auto if available)
echo "[5/5] Signing APK..."
if [ -f "$DEBUG_KEYSTORE" ] && [ -n "$APKSIGNER" ]; then
    echo "  Using debug keystore ($DEBUG_KEYSTORE)..."
    "$APKSIGNER" sign \
        --ks "$DEBUG_KEYSTORE" \
        --ks-pass pass:android \
        --ks-key-alias androiddebugkey \
        --key-pass pass:android \
        --out "$SIGNED_APK" \
        "$FIXED_APK" 2>&1
    if [ $? -eq 0 ]; then
        rm "$FIXED_APK"
        FINAL_APK="$SIGNED_APK"
        echo "  Signed with debug keystore"
    else
        echo "  Signing failed, keeping unsigned APK"
        FINAL_APK="$FIXED_APK"
    fi
elif [ -n "$APKSIGNER" ]; then
    echo "  Debug keystore not found at $DEBUG_KEYSTORE"
    echo "  Create it with: keytool -genkey -v -keystore $DEBUG_KEYSTORE -alias androiddebugkey -keyalg RSA -keysize 2048 -validity 10000 -storepass android -keypass android"
    echo "  --- Or sign manually: ---"
    echo "  apksigner sign --ks <keystore> '$FIXED_APK'"
    FINAL_APK="$FIXED_APK"
else
    echo "  apksigner not found. Install Android SDK build-tools."
    FINAL_APK="$FIXED_APK"
fi

# Cleanup
rm -rf "$WORK_DIR"

echo ""
echo "=== Done! ==="
echo "APK with .so: $FINAL_APK"
echo ""
echo "Install on device:"
echo "  adb install -r \"$FINAL_APK\""
echo ""
echo "Or to install and run directly:"
echo "  adb install -r \"$FINAL_APK\" && adb shell am start -n com.example.\$genname/com.godot.game.GodotApp"
