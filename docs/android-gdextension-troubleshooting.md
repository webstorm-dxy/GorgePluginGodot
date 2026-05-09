# Android GDExtension 排查记录

本文记录 Android 端出现以下错误时的原因、排查过程和最终解决方案：

```text
ERROR: NineSliceSprite2D class not found in ClassDB (EnsureRustNode).
```

这个错误表面上像是 Rust GDExtension 没有注册 `NineSliceSprite2D` 类，但本项目在 Windows、macOS 和 Godot 编辑器中都能正常工作。最终确认：Android 端的问题主要出在 APK 导出和安装链路，而不只是 Rust 代码本身。

## 现象

- `NineSliceSprite2D` 在 Godot 编辑器、Windows、macOS 上可用。
- Android 运行时报：

```text
NineSliceSprite2D class not found in ClassDB
```

- APK 中看起来有 `libnine_slice_sprite_2d.so`，但 Android 实际加载的可能是旧的 debug 库。
- 旧 APK 中曾经包含一个 109 MB 的 debug `.so`：

```text
lib/arm64-v8a/libnine_slice_sprite_2d.so
```

- 正确重新构建后的 release `.so` 约 4 MB，路径应为：

```text
lib/arm64-v8a/libnine_slice_sprite_2d.so
```

并且项目资源中对应源文件为：

```text
addons/nine_slice_sprite_2d_godot2d/bin/android/release/arm64-v8a/libnine_slice_sprite_2d.so
```

## 根本原因

这次问题不是单一原因，而是多个问题叠加。

### 1. 手机上运行的是旧 APK

设备上实际运行的 APK 还是旧的：

```text
expo/GorgeGodotPlugin.apk
```

该 APK 导出时间是：

```text
2026-05-09 16:40
```

它内部仍然包含旧版 `.gdextension` 配置和旧的 109 MB debug 原生库。因此即使本地代码、Cargo 依赖和 `.gdextension` 已经修好，只要没有重新正常导出并安装新 APK，Android 上仍会复现旧问题。

### 2. Android 导出过滤器错误地包含了所有 `.so`

旧的 `export_presets.cfg` 中包含：

```ini
include_filter="*.zip,*.so"
```

这会把项目目录里的所有 `.so` 都作为普通资源打进 APK，包括：

```text
target/debug/libnine_slice_sprite_2d.so
target/aarch64-linux-android/debug/libnine_slice_sprite_2d.so
target/aarch64-linux-android/release/libnine_slice_sprite_2d.so
```

这样会让 APK 内同时存在多个版本的原生库，排查时很容易误判“APK 里有 so，所以 GDExtension 应该能加载”。

正确做法是不要把所有 `.so` 当普通资源打包，而是让 Godot Android exporter 根据 `.gdextension` 自动把目标库放到：

```text
lib/arm64-v8a/libnine_slice_sprite_2d.so
```

### 3. `.gdextension` 的 Android debug 配置曾指向旧 debug 库

旧配置中 Android debug/template debug 指向 debug `.so`。这个 debug 库来自旧依赖状态，Android 端可能加载到它。

为了降低变量，目前 Android debug、release、template debug、template release 都统一指向已验证的 release `.so`。

### 4. godot-rust 0.4.5 存在 Android 加载回归风险

根据 `godot-rust/gdext` 官方仓库 Issue #1423，`godot` crate 在 0.4.3 之后的 Android 加载链路中存在回归风险。该 Issue 中明确提到 pin 回：

```toml
godot = "=0.4.2"
```

可以绕过相关 Android 加载问题。

因此本项目采用保守方案，将 `godot` 依赖固定为 `=0.4.2`。

### 5. 第一次命令行导出缺少权限，导致 APK 漏掉 C# assemblies

第一次使用 Godot CLI 导出时，进程没有权限写入：

```text
~/Library/Application Support/Godot/mono/build_logs
```

导出日志中出现过类似错误：

```text
Access to the path '~/Library/Application Support/Godot/mono/build_logs/...' is denied.
```

这会导致 APK 中缺少 C# 运行所需文件：

```text
assets/.godot/mono/publish/arm64/GorgeGodotPlugin.dll
assets/.godot/mono/publish/arm64/GodotSharp.dll
```

设备日志随后会出现：

```text
ERROR: .NET: Assemblies not found
```

这是另一个问题，和 `NineSliceSprite2D` 的 GDExtension 加载问题不同，但会让 Android 运行结果继续失败。

## 最终解决方案

### 1. 固定 godot-rust 到 0.4.2

修改：

```text
addons/nine_slice_sprite_2d_godot2d/Cargo.toml
```

内容：

```toml
[dependencies]
godot = "=0.4.2"
```

然后重新构建 Android release 库：

```bash
cd addons/nine_slice_sprite_2d_godot2d
cargo build --target aarch64-linux-android --release
./deploy_android.sh
```

### 2. Android 统一使用 release `.so`

修改：

```text
addons/nine_slice_sprite_2d_godot2d/nine_slice_sprite_2d.gdextension
```

关键配置如下：

```ini
[configuration]
entry_symbol = "gdext_rust_init"
compatibility_minimum = 4.2
reloadable = true

[libraries]
android.debug.arm64 = "res://addons/nine_slice_sprite_2d_godot2d/bin/android/release/arm64-v8a/libnine_slice_sprite_2d.so"
android.release.arm64 = "res://addons/nine_slice_sprite_2d_godot2d/bin/android/release/arm64-v8a/libnine_slice_sprite_2d.so"
android.template_debug.arm64 = "res://addons/nine_slice_sprite_2d_godot2d/bin/android/release/arm64-v8a/libnine_slice_sprite_2d.so"
android.template_release.arm64 = "res://addons/nine_slice_sprite_2d_godot2d/bin/android/release/arm64-v8a/libnine_slice_sprite_2d.so"
```

注意：Android 架构 key 使用的是 `android.debug.arm64` / `android.release.arm64`，不是 `android.aarch64` 或 `android.arm64-v8a`。

### 3. 不要把所有 `.so` 作为普通资源打进 APK

修改：

```text
export_presets.cfg
```

使用：

```ini
include_filter="*.zip"
```

不要使用：

```ini
include_filter="*.zip,*.so"
```

### 4. 保持 APK 签名开启

`export_presets.cfg` 中应保持：

```ini
package/signed=true
```

未签名 APK 可能导出成功，但安装或运行测试结果会误导排查。

### 5. 使用 Godot 正常导出，不要手工改 APK

不要解压 APK、手工复制 `.so`、重新 zip、重新签名。这样很容易导致安装包损坏或签名错误。

推荐使用 Godot 正常导出流程。

命令行导出示例：

```bash
/Applications/Godot_mono.app/Contents/MacOS/Godot \
  --headless \
  --path /Users/daxingyi/GorgeGodotPlugin \
  --export-debug Android \
  /Users/daxingyi/GorgeGodotPlugin/expo/GorgeGodotPlugin_reexport.apk
```

如果命令行导出时遇到 `~/Library/Application Support/Godot/...` 权限问题，需要用有权限的环境重新导出，否则 APK 可能缺少 C# assemblies。

## 验证方法

### 1. 验证 APK 签名

```bash
~/Library/Android/sdk/build-tools/34.0.0-rc2/apksigner \
  verify --verbose expo/GorgeGodotPlugin_reexport.apk
```

期望输出包含：

```text
Verifies
```

### 2. 验证 APK 内容

```bash
unzip -l expo/GorgeGodotPlugin_reexport.apk | grep -E \
  'lib/arm64-v8a/libnine_slice_sprite_2d\.so|assets/.godot/mono/publish/arm64/GorgeGodotPlugin\.dll|assets/.godot/mono/publish/arm64/GodotSharp\.dll|assets/addons/nine_slice_sprite_2d_godot2d/nine_slice_sprite_2d\.gdextension|assets/.godot/extension_list.cfg'
```

期望至少包含：

```text
assets/.godot/mono/publish/arm64/GorgeGodotPlugin.dll
assets/.godot/mono/publish/arm64/GodotSharp.dll
lib/arm64-v8a/libnine_slice_sprite_2d.so
assets/addons/nine_slice_sprite_2d_godot2d/nine_slice_sprite_2d.gdextension
assets/.godot/extension_list.cfg
```

其中：

```text
lib/arm64-v8a/libnine_slice_sprite_2d.so
```

应约为 4 MB，而不是旧的 109 MB debug 库。

### 3. 验证 Android `.so`

```bash
file addons/nine_slice_sprite_2d_godot2d/bin/android/release/arm64-v8a/libnine_slice_sprite_2d.so
nm -gU addons/nine_slice_sprite_2d_godot2d/bin/android/release/arm64-v8a/libnine_slice_sprite_2d.so | grep gdext_rust_init
```

期望输出包含：

```text
ELF 64-bit LSB shared object, ARM aarch64
T gdext_rust_init
```

### 4. 设备端验证

```bash
adb install -r expo/GorgeGodotPlugin_reexport.apk
adb logcat -c
adb shell monkey -p com.example.gorgegodotplugin -c android.intent.category.LAUNCHER 1
adb logcat -d -v time | grep -i -E "dlopen|gdextension|godot|nine_slice|nineslice|classdb|assemblies|fatal|sigsegv|androidruntime|error"
```

修复后的构建不应再出现：

```text
NineSliceSprite2D class not found in ClassDB
GDExtension ... dlopen failed
.NET: Assemblies not found
```

## 不要做的事

- 不要手工解压、修改、重新打包 APK。
- 不要安装 `*_fixed.apk` 这类后处理产物。
- 不要继续测试旧的 `expo/GorgeGodotPlugin.apk`。
- 不要保留 `include_filter="*.zip,*.so"`，除非已经清理所有旧 `.so` 并且非常确定需要这样做。
- 不要只看“APK 里有 `.so`”就判断 GDExtension 会加载成功；必须确认它位于 `lib/arm64-v8a/`，且是最新 release 构建。

## 已验证可用的测试包

本次排查中第一个验证通过的 APK 是：

```text
/Users/daxingyi/GorgeGodotPlugin/expo/GorgeGodotPlugin_reexport.apk
```

该 APK 满足以下条件：

- 由 Godot 正常 Android exporter 生成。
- `apksigner verify` 通过。
- 包含 4.1 MB 左右的 `lib/arm64-v8a/libnine_slice_sprite_2d.so`。
- 包含 `GorgeGodotPlugin.dll` 和 `GodotSharp.dll`。
- 在连接的 Android 设备上运行时，没有再出现 `NineSliceSprite2D class not found in ClassDB`。
