# Gorge Remote Player UDP API

本文档描述 `res://demo/gorge_remote_gameplay/gorge_remote_player.tscn` 的 UDP 远程控制协议。

该协议用于通过 UDP JSON 报文控制 `GamePlayer`：

- 播放谱面
- 停止谱面
- 暂停谱面
- 按秒跳转谱面进度
- 查询当前谱面时间和总时长
- 远程设置 runtime 包和谱面包路径

> 注意：这是 `gorge_remote_gameplay` demo 场景的 UDP API，不是 `GamePlayer` 插件核心节点的通用网络 API。

## 1. 启动条件

必须先运行场景：

```text
res://demo/gorge_remote_gameplay/gorge_remote_player.tscn
```

场景启动成功后，Godot 输出应包含：

```text
UDP chart control listening on 127.0.0.1:9000
```

如果没有看到该输出，UDP 监听没有启动。此时发送命令不会有响应。

## 2. 传输层

### 2.1 协议

- 传输协议：UDP
- 数据格式：UTF-8 JSON
- 单个 UDP packet 对应一个 JSON 命令
- 不需要换行符
- 不支持 TCP
- 不支持 WebSocket
- 不保证可靠送达，丢包、乱序、重复包都按 UDP 协议特性处理

### 2.2 默认监听地址

默认监听：

```text
127.0.0.1:9000
```

对应脚本导出变量：

```gdscript
@export var udp_enabled: bool = true
@export var udp_port: int = 9000
@export var udp_bind_address: String = "127.0.0.1"
```

含义：

| 配置 | 默认值 | 说明 |
|---|---:|---|
| `udp_enabled` | `true` | 是否启用 UDP 控制 |
| `udp_port` | `9000` | UDP 监听端口 |
| `udp_bind_address` | `"127.0.0.1"` | 绑定地址 |

绑定地址说明：

| 地址 | 含义 |
|---|---|
| `127.0.0.1` | 只允许本机发送命令 |
| `0.0.0.0` | 允许局域网设备发送命令 |
| 指定网卡 IP | 只监听该网卡地址 |

如果要从手机、另一台电脑或其他局域网设备控制，需要把 `udp_bind_address` 改为 `0.0.0.0`，并确保系统防火墙允许 UDP 端口。

## 3. JSON 通用规则

每个请求必须是 JSON object：

```json
{"cmd":"status"}
```

通用字段：

| 字段 | 类型 | 必填 | 说明 |
|---|---|---:|---|
| `cmd` | string | 是 | 命令名，大小写不敏感 |

当前实现中：

- 非 JSON 报文会被忽略，并在 Godot 输出 warning。
- JSON 不是 object 时会被忽略。
- 未知 `cmd` 会被忽略，并在 Godot 输出 warning。
- 只有 `status` 和 `set_packages` 有 UDP 回包。
- `play`、`stop`、`pause`、`seek` 默认没有 UDP 回包。

## 4. 命令总览

| 命令 | 用途 | 是否回包 |
|---|---|---:|
| `play` | 开始或恢复播放 | 否 |
| `stop` | 停止播放 | 否 |
| `pause` | 暂停播放 | 否 |
| `seek` | 按秒跳转谱面进度，跳转后保持暂停 | 否 |
| `status` | 查询当前谱面时间和总时长 | 是 |
| `set_packages` | 设置 runtime 包和谱面包路径 | 是 |

## 5. `play`

开始播放谱面。

### 请求

```json
{"cmd":"play"}
```

### 行为

- 如果当前未播放，调用 `player.play_chart()`。
- 如果当前是暂停状态，按 `GamePlayer` 当前实现恢复播放。
- 如果 runtime 尚未准备，`GamePlayer` 会尝试准备 runtime。
- 该命令没有 UDP 回包。

### Python 示例

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py play
```

## 6. `stop`

停止谱面播放。

### 请求

```json
{"cmd":"stop"}
```

### 行为

- 调用 `player.stop_chart()`。
- 当前实现没有显式传入停止原因，因此使用 `GamePlayer` 默认 reason。
- 如果当前没有播放，`GamePlayer` 会按自身逻辑处理。
- 该命令没有 UDP 回包。

### Python 示例

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py stop
```

## 7. `pause`

暂停谱面播放。

### 请求

```json
{"cmd":"pause"}
```

### 行为

- 调用 `player.pause_chart()`。
- 暂停会停止每帧推进和当前音频。
- 暂停不会销毁 runtime。
- 暂停后可以继续 `status` 查询当前位置。
- 暂停后发送 `play` 可恢复播放。
- 该命令没有 UDP 回包。

### Python 示例

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py pause
```

## 8. `seek`

按秒设置谱面播放进度。

### 请求

```json
{"cmd":"seek","seconds":10.0}
```

字段：

| 字段 | 类型 | 必填 | 说明 |
|---|---|---:|---|
| `seconds` | int 或 float | 是 | 目标谱面时间，单位秒 |

### 行为

- 调用 `player.seek_chart_time(seconds, true)`。
- 语义是：先暂停，再跳转到目标秒数。
- 跳转完成后保持暂停，不自动恢复播放。
- 要继续播放，需要再发送 `play`。
- 该命令没有 UDP 回包。

### 时间含义

`seconds` 表示 Gorge 的 `ChartTime`，不是系统真实时间，也不是单独的音频播放器时间。

如果目标时间超过谱面范围，`GamePlayer` 当前实现会 clamp 到 `[beginSeconds, endSeconds]`。

### Python 示例

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py seek --seconds 10
```

或：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py seek -s 12.5
```

## 9. `status`

查询当前谱面时间和谱面总时长。

### 请求

```json
{"cmd":"status"}
```

### 成功响应

```json
{
  "type": "status",
  "ok": true,
  "currentSeconds": 10.0,
  "durationSeconds": 136.0,
  "beginSeconds": -1.0,
  "endSeconds": 135.0
}
```

字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| `type` | string | 固定为 `"status"` |
| `ok` | bool | 是否成功 |
| `currentSeconds` | float | 当前谱面时间，单位秒 |
| `durationSeconds` | float | 谱面总时长，等于 `endSeconds - beginSeconds` |
| `beginSeconds` | float | 谱面开始时间 |
| `endSeconds` | float | 谱面结束时间 |

### 失败响应

```json
{
  "type": "status",
  "ok": false,
  "error": "chart_not_ready"
}
```

错误码：

| 错误码 | 说明 |
|---|---|
| `chart_not_ready` | runtime 或谱面尚未准备好 |

### 注意事项

当前 demo 在 `_ready()` 中会调用 `player.prepare_runtime()`，因此场景启动后即使尚未播放，也通常可以查询到总时长。

示例输出中 `beginSeconds` 可能是负数，例如：

```text
Begin: -1.000s
End:   135.000s
```

这是谱面数据的时间范围，不表示错误。总时长仍按：

```text
durationSeconds = endSeconds - beginSeconds
```

计算。

### Python 示例

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py status
```

示例输出：

```text
Sent '{"cmd": "status"}' to 127.0.0.1:9000
ChartTime:   10.000s
Duration:    136.000s
Begin:       -1.000s
End:         135.000s
```

## 10. `set_packages`

设置 runtime 包路径和谱面包路径。

### 10.1 单路径请求

```json
{
  "cmd": "set_packages",
  "runtimePackagePath": "/absolute/path/to/runtime.zip",
  "chartPackagePath": "/absolute/path/to/chart.zip"
}
```

### 10.2 多路径请求

```json
{
  "cmd": "set_packages",
  "runtimePackagePaths": [
	"/absolute/path/to/runtime1.zip",
    "res://Dremu.zip"
  ],
  "chartPackagePaths": [
    "/absolute/path/to/chart.zip"
  ]
}
```

### 10.3 .gpkg 谱面包

`.gpkg` 是自包含谱面包（zip 格式），内含谱面代码和模态代码。可以将其作为 `chartPackagePath` 传入，游戏运行时自动展开其中的模态作为 runtime package：

```json
{
  "cmd": "set_packages",
  "runtimePackagePath": "/absolute/path/to/runtime.zip",
  "chartPackagePath": "/Users/daxingyi/Music/test.gpkg"
}
```

也可以用多路径写法，混用 `.zip` 和 `.gpkg`：

```json
{
  "cmd": "set_packages",
  "runtimePackagePaths": [
    "/absolute/path/to/runtime.zip"
  ],
  "chartPackagePaths": [
	"/Users/daxingyi/Music/test.gpkg",
    "/absolute/path/to/another_chart.zip"
  ]
}
```

注意：
- `.gpkg` 可以只作为 chart path 传入（推荐），它会自动提取模态文件作为 runtime。
- 你也可以把 `.gpkg` 放在 `runtimePackagePaths` 里，效果相同。
- runtime 参数仍然可以传现有的 `.zip` runtime 包；`.gpkg` 自带的模态会被追加进去。
- `set_packages` 成功后仍只 prepare，不自动播放。

### 10.4 字段

| 字段 | 类型 | 必填 | 说明 |
|---|---|---:|---|
| `runtimePackagePath` | string | 条件必填 | 单个 runtime 包路径 |
| `runtimePackagePaths` | string[] | 条件必填 | 多个 runtime 包路径 |
| `chartPackagePath` | string | 条件必填 | 单个谱面包路径 |
| `chartPackagePaths` | string[] | 条件必填 | 多个谱面包路径 |

`runtimePackagePath` / `runtimePackagePaths` 至少提供一个。

`chartPackagePath` / `chartPackagePaths` 至少提供一个。

如果同时提供单路径和数组路径，当前实现优先使用单路径字段：

- `runtimePackagePath` 优先于 `runtimePackagePaths`
- `chartPackagePath` 优先于 `chartPackagePaths`

### 10.5 路径规则

路径会原样传给 Godot `FileAccess.Open()`。

支持形式：

| 路径形式 | 示例 | 说明 |
|---|---|---|
| `res://` | `res://Dremu.zip` | Godot 项目资源路径 |
| `user://` | `user://charts/song.zip` | Godot 用户数据路径 |
| 绝对路径 | `/Users/name/charts/song.zip` | 操作系统绝对路径 |

注意：

- 协议允许传绝对路径。
- 绝对路径能否读取，取决于 Godot 运行平台、导出权限和系统文件权限。
- 路径不会被复制到项目目录。
- 路径不会被规范化或自动转换。

### 10.6 内置 Native 包

`set_packages` 会始终保留：

```text
res://addons/gorgeplugin/Native.zip
```

实际 runtime 包列表为：

```text
Native.zip + 请求中的 runtime 包
```

因此请求里不需要再传 `Native.zip`。

### 10.7 行为

收到 `set_packages` 后，当前实现按顺序执行：

1. `player.clear_packages()`
2. 添加 `res://addons/gorgeplugin/Native.zip`
3. 添加请求中的 runtime 包路径
4. 添加请求中的 chart 包路径
5. 调用 `player.prepare_runtime()`
6. 通过 UDP 返回结果

如果设置前正在播放，`clear_packages()` 会触发 `GamePlayer` 的包变更停止逻辑。

设置成功后：

- 新 runtime/chart 已准备好。
- 不会自动播放。
- 可以立即发送 `status` 查询新谱面总时长。
- 可以再发送 `play` 播放新谱面。

### 10.8 成功响应

```json
{
  "type": "set_packages",
  "ok": true,
  "runtimePackagePaths": [
	"res://addons/gorgeplugin/Native.zip",
    "/absolute/path/to/runtime.zip"
  ],
  "chartPackagePaths": [
    "/absolute/path/to/chart.zip"
  ],
  "durationSeconds": 136.0,
  "beginSeconds": -1.0,
  "endSeconds": 135.0
}
```

字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| `type` | string | 固定为 `"set_packages"` |
| `ok` | bool | 是否成功 |
| `runtimePackagePaths` | string[] | 实际设置的 runtime 包路径 |
| `chartPackagePaths` | string[] | 实际设置的谱面包路径 |
| `durationSeconds` | float | 新谱面总时长 |
| `beginSeconds` | float | 新谱面开始时间 |
| `endSeconds` | float | 新谱面结束时间 |

### 10.9 失败响应

```json
{
  "type": "set_packages",
  "ok": false,
  "error": "prepare_failed"
}
```

错误码：

| 错误码 | 说明 |
|---|---|
| `missing_runtime_package_path` | 未提供 runtime 包路径 |
| `missing_chart_package_path` | 未提供谱面包路径 |
| `invalid_runtime_package_path` | runtime 包路径为空或添加失败 |
| `invalid_chart_package_path` | 谱面包路径为空或添加失败 |
| `prepare_failed` | 路径已写入，但 `prepare_runtime()` 失败 |

注意：

- 当前实现会先清空旧包配置，再写入新配置。
- 如果后续 `prepare_runtime()` 失败，不保证自动恢复旧配置。
- 对不存在的文件路径，通常会在 `prepare_runtime()` 阶段失败，并返回 `prepare_failed`。

### 10.10 Python 示例

设置单个 runtime 和单个 chart：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py set-packages \
  --runtime /absolute/path/to/Dremu.zip \
  --chart /absolute/path/to/DremuTest.zip
```

重复 `--runtime` 或 `--chart` 可传多个路径：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py set-packages \
  -r /absolute/path/to/runtime1.zip \
  -r /absolute/path/to/runtime2.zip \
  -c /absolute/path/to/chart.zip
```

如果 prepare 时间较长，可以提高 timeout：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py set-packages \
  -r /absolute/path/to/runtime.zip \
  -c /absolute/path/to/chart.zip \
  --timeout 10
```

示例输出：

```text
Sent '{"cmd": "set_packages", ...}' to 127.0.0.1:9000
Runtime packages: ['res://addons/gorgeplugin/Native.zip', '/absolute/path/to/runtime.zip']
Chart packages:   ['/absolute/path/to/chart.zip']
Duration:         136.000s
Begin:            -1.000s
End:              135.000s
```

使用 .gpkg 谱面包：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py set-packages \
  -r /absolute/path/to/Dremu.zip \
  -c /Users/daxingyi/Music/test.gpkg
```

## 11. Python 工具

项目提供辅助脚本：

```text
demo/gorge_remote_gameplay/send_udp_cmd.py
```

建议使用 Python 3：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py status
```

全局参数：

| 参数 | 默认值 | 说明 |
|---|---:|---|
| `--host` | `127.0.0.1` | 目标 Godot UDP 地址 |
| `--port` | `9000` | 目标 Godot UDP 端口 |

示例：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py --host 127.0.0.1 --port 9000 status
```

如果用局域网设备控制，把 `--host` 改为运行 Godot 的机器 IP：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py --host 192.168.1.100 status
```

## 12. 原始 UDP 示例

不使用项目脚本时，也可以直接发送 UDP JSON。

### Python 原始示例：查询 status

```python
import json
import socket

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.settimeout(2.0)

payload = {"cmd": "status"}
sock.sendto(json.dumps(payload).encode("utf-8"), ("127.0.0.1", 9000))

data, addr = sock.recvfrom(4096)
print(addr, json.loads(data.decode("utf-8")))
```

### Python 原始示例：播放

```python
import json
import socket

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
payload = {"cmd": "play"}
sock.sendto(json.dumps(payload).encode("utf-8"), ("127.0.0.1", 9000))
```

## 13. 推荐调用流程

### 13.1 使用默认包播放

```text
status -> play -> status -> pause -> seek -> status -> play -> stop
```

对应命令：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py status
python3 demo/gorge_remote_gameplay/send_udp_cmd.py play
python3 demo/gorge_remote_gameplay/send_udp_cmd.py status
python3 demo/gorge_remote_gameplay/send_udp_cmd.py pause
python3 demo/gorge_remote_gameplay/send_udp_cmd.py seek --seconds 10
python3 demo/gorge_remote_gameplay/send_udp_cmd.py status
python3 demo/gorge_remote_gameplay/send_udp_cmd.py play
python3 demo/gorge_remote_gameplay/send_udp_cmd.py stop
```

### 13.2 远程设置包后播放

```text
set_packages -> status -> play
```

对应命令：

```bash
python3 demo/gorge_remote_gameplay/send_udp_cmd.py set-packages \
  -r /absolute/path/to/runtime.zip \
  -c /absolute/path/to/chart.zip

python3 demo/gorge_remote_gameplay/send_udp_cmd.py status
python3 demo/gorge_remote_gameplay/send_udp_cmd.py play
```

## 14. 常见问题

### 14.1 `No response within 2.0s`

常见原因：

- 场景没有运行。
- UDP 端口不是 `9000`。
- `udp_bind_address` 不是当前发送端能访问的地址。
- 防火墙拦截 UDP。
- 发的是 `play` / `stop` / `pause` / `seek`，这些命令本来没有回包。

只有 `status` 和 `set_packages` 会返回 UDP 响应。

### 14.2 `Error: chart_not_ready`

表示 Godot 端无法获得当前谱面状态。

常见原因：

- runtime prepare 失败。
- 包路径不存在。
- chart 包不是有效 Gorge 谱面包。
- runtime 包缺失或不匹配。

### 14.3 `Begin` 是负数是不是错误？

不是。

示例：

```text
Begin: -1.000s
End:   135.000s
```

表示谱面时间范围从 `-1` 秒开始，到 `135` 秒结束，总时长是：

```text
135 - (-1) = 136s
```

### 14.4 为什么 `seek` 后没有继续播放？

这是当前 API 的设计：

```json
{"cmd":"seek","seconds":10}
```

语义是先暂停，再跳转，并保持暂停。

如果要继续播放，需要再发送：

```json
{"cmd":"play"}
```

### 14.5 为什么 `set_packages` 成功后没有自动播放？

`set_packages` 只负责设置包路径并 prepare runtime。

设置成功后需要显式发送：

```json
{"cmd":"play"}
```

这样远程端可以先读取新谱面时长，再决定是否播放。

## 15. 当前限制

- UDP 不可靠，控制端需要自行处理超时和重试。
- `play` / `stop` / `pause` / `seek` 没有 ACK 回包。
- 没有鉴权、token 或签名校验。
- 绑定到 `0.0.0.0` 后，局域网内能访问该端口的设备都可能发送命令。
- `set_packages` 失败后不自动恢复旧包配置。
- 绝对路径读取受 Godot 运行环境和系统权限影响。
