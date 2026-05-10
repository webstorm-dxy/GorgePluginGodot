# GorgePluginGodot 开发日志

## 1. 移植 .gorge 字节码序列化代码

> 将 `GorgeConpile` 项目中的 `.gorge` 二进制序列化/反序列化代码移植到 `GorgePluginGodot`。

### 新建文件

| 文件 | 说明 |
|---|---|
| `addons/gorgeplugin/GorgeTools/GorgeCoreCSharp/src/Serialization/GorgeBytecodeFormat.cs` | 二进制格式常量（magic bytes `GORG`、版本号、操作数/值标签） |
| `addons/gorgeplugin/GorgeTools/GorgeCoreCSharp/src/Serialization/DeserializedImplementationContext.cs` | 反序列化结果容器，实现 `IImplementationBase` |
| `addons/gorgeplugin/GorgeTools/GorgeCoreCSharp/src/Serialization/GorgeBinaryWriter.cs` | 将 `IImplementationBase`（类/接口/枚举 + 字节码）写入 `.gorge` 二进制文件 |
| `addons/gorgeplugin/GorgeTools/GorgeCoreCSharp/src/Serialization/GorgeBinaryReader.cs` | 从 `.gorge` 二进制文件反序列化回运行时对象 |

### 两项目 API 差异及适配

`GorgeConpile` 与 `GorgePluginGodot` 中的代码库存在多处 API 差异，已在移植时适配：

| 差异点 | GorgeConpile | GorgePluginGodot | 适配方案 |
|---|---|---|---|
| `GorgeType` 构造函数 | `public` | `private` | 使用静态工厂方法（`GorgeType.Object()`、`GorgeType.Int` 等）重建类型 |
| `InjectorFieldInformation` 元数据 | `Metadatas` 属性 | `_metadatas` 公共字段 | 直接访问 `_metadatas` 字段 |
| `CompiledGorgeClass` 构造函数 | 7 参数（含 `FixedFieldValuePool`） | 6 参数（无该参数，从注解构建） | 反序列化后通过反射设置 `_injectorDefaultValues` |
| `ClassDeclaration.InheritanceDepth()` | 有 | 无 | 移除排序调用 |

### GorgeBinaryReader 增强

为支持用户类继承 native 类的场景，对 `GorgeBinaryReader` 做了以下修改：

- `Read()` / `ReadFromFile()` 增加可选参数 `IImplementationBase nativeBase`
- `ReadContext` 增加 `NativeBase` 字段
- `ReadClassDeclaration()` 解析超类/超接口时：先在反序列化上下文中查找，未找到则回退到 `NativeBase` 查找

---

## 2. 添加 .gorge 字节码缓存机制

> 首次运行时自动编译所有 `.g` 文件为 `.gorge` 字节码并缓存，后续运行直接加载缓存。

### 修改文件

**`StaticConfig.cs`** — 增加 `CacheDirectory` 字段：
```csharp
public static string CacheDirectory; // .gorge 缓存目录的文件系统路径
```

**`GamePlayer.cs`** — `ApplyStaticConfig()` 中设置缓存目录：
```csharp
StaticConfig.CacheDirectory = System.IO.Path.Combine(OS.GetUserDataDir(), "gorge_cache");
```

**`RuntimeManager.cs`** — 增加以下方法和逻辑：

| 方法 | 可见性 | 说明 |
|---|---|---|
| `ComputeSourceHash(packages)` | `private static` | 对所有 `.g` 源文件（路径 + 内容）计算 SHA256 哈希 |
| `LoadFromCache(packages)` | `private static` | 按哈希查找缓存 `.gorge` 文件并加载，失败返回 null |
| `SaveToCache(packages, context)` | `private static` | 将编译结果写入 `{CacheDirectory}/{hash}.gorge` |
| `GetCacheFilePath(hash)` | `private static` | 获取缓存文件完整路径 |
| `CompilePackagesToCache(packages)` | `public static` | 强制编译并保存到缓存，返回缓存文件路径 |

**`CreateLanguageRuntime()` 逻辑变更**：
```
原流程：编译 → 创建 LanguageRuntime
新流程：检查缓存 → 命中：加载缓存创建 LanguageRuntime
                  → 未命中：编译 → 保存缓存 → 创建 LanguageRuntime
```

### 缓存策略

- 缓存键 = 所有源文件路径和内容的 SHA256 哈希
- 缓存位置 = `{OS.GetUserDataDir()}/gorge_cache/{hash}.gorge`
- 仅缓存用户代码（`ClassImplementationContext`），native 类通过反射加载
- 缓存文件损坏时自动删除并回退到源码编译
- 缓存写入失败不影响运行时（仅丢失缓存加速）

---

## 3. 添加 "Compile" 手动编译按钮

> 在 Demo 界面上增加一个按钮，点击后将当前加载的 `.g` 文件编译为 `.gorge` 字节码。

### 修改文件

**`RuntimeManager.cs`** — 新增 `CompilePackagesToCache()` 公开方法：
- 强制重新编译（绕过缓存检查）
- 将结果写入缓存文件
- 返回缓存文件路径

**`GamePlayer.cs`** — 新增：
- 信号 `BytecodeCompiled(string cachePath)` — 编译成功
- 信号 `BytecodeCompileFailed(string message)` — 编译失败
- 方法 `CompileToBytecode()` / `compile_to_bytecode()` — 读取已配置的包路径，调用 `RuntimeManager.CompilePackagesToCache()`

**`demo/game_player_demo.tscn`** — 在 `Panel/Buttons` 中添加 `CompileButton`

**`demo/game_player_demo.gd`** — 连接按钮信号和处理函数：
- `compile_button.pressed` → `player.compile_to_bytecode`
- `BytecodeCompiled` → 状态栏显示编译成功及路径
- `BytecodeCompileFailed` → 状态栏显示错误信息

### 按钮行为

点击 Compile 按钮时：
1. 读取 `RuntimePackagePaths` 和 `ChartPackagePaths` 中的所有 ZIP 包
2. 提取 `.g` 源文件，通过 ANTLR 四轮编译生成字节码
3. 将字节码序列化为 `.gorge` 文件保存到 `user://gorge_cache/`
4. 在状态栏显示结果

---

## 编译状态

`dotnet build` — **0 错误，警告均为已有代码中的 nullability 警告**
