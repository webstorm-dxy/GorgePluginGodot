# Godot MCP Addon

The `godot_mcp` addon connects the Godot editor to an AI assistant via WebSocket, enabling AI-driven scene editing, project management, debugging, and asset generation.

## Architecture

```
┌──────────────┐     WebSocket      ┌──────────────┐     WebSocket      ┌──────────────┐
│  AI Agent    │ ◄────────────────► │  MCP Server  │ ◄────────────────► │  Godot Editor │
│  (Claude)    │     JSON-RPC       │  (TypeScript)│     JSON-RPC       │  (plugin.gd)  │
└──────────────┘                    └──────────────┘                    └──────┬───────┘
                                                                              │
                                                              ┌───────────────┴───────────────┐
                                                              │                               │
                                                       ┌──────┴──────┐              ┌────────┴────────┐
                                                       │ MCPClient   │              │  ToolExecutor    │
                                                       │ (WebSocket) │              │  (tool routing)  │
                                                       └─────────────┘              └────────┬────────┘
                                                                                              │
                                              ┌─────────────────────────────────────────────────┼──────────────────────────────────────────────┐
                                              │                  │                  │           │           │                  │               │
                                        ┌─────┴─────┐   ┌───────┴──────┐   ┌──────┴──────┐ ┌──┴───┐  ┌────┴────┐   ┌─────────┴────────┐ ┌────┴─────┐
                                        │ FileTools │   │  SceneTools  │   │ ScriptTools │ │Proj..│  │Asset... │   │ VisualizerTools  │ │MCPRuntime│
                                        │ (I/O)     │   │ (scene CRUD) │   │ (edit/val.) │ │Tools │  │(SVG→PNG)│   │ (map_project)    │ │(in-game) │
                                        └───────────┘   └──────────────┘   └─────────────┘ └──────┘  └─────────┘   └──────────────────┘ └──────────┘
```

When enabled, the plugin:
1. Registers the `MCPRuntime` autoload in `project.godot` (done once during `_enable_plugin()`)
2. Opens a WebSocket connection to `ws://127.0.0.1:6505`
3. Listens for `tool_invoke` messages and dispatches them to the appropriate tool handler
4. Shows a status indicator in the editor toolbar

## Tool Categories

### File Operations (`file_tools.gd`)

| Tool | Description |
|------|-------------|
| `list_dir` | List files and folders under a `res://` path |
| `read_file` | Read file contents, optionally with line range |
| `search_project` | Full-text search across all project files |
| `create_script` | Create a new `.gd` script file with content |

### Scene Operations (`scene_tools.gd`)

| Tool | Description |
|------|-------------|
| `create_scene` | Create a new `.tscn` scene with a root node and children |
| `read_scene` | Parse a scene and return its node tree structure |
| `add_node` | Add a node to an existing scene, with optional children tree |
| `remove_node` | Remove a node from a scene |
| `rename_node` | Rename a node in a scene |
| `move_node` | Move a node to a different parent |
| `modify_node_property` | Change a single property on a node |
| `set_node_properties` | Set multiple properties in one call |
| `attach_script` | Attach a script to a node (use this instead of `modify_node_property` with `script` property) |
| `detach_script` | Remove a script from a node |
| `instance_scene` | Add an instance of another scene as a child node |
| `set_collision_shape` | Create and assign a collision shape to a CollisionShape2D/3D |
| `set_sprite_texture` | Assign a texture to a Sprite2D/Sprite3D/TextureRect |
| `set_mesh` | Create and assign a mesh to a MeshInstance3D |
| `set_material` | Create and assign a material to a 3D node |
| `set_resource_property` | Modify a property on a Resource attached to a node |
| `save_resource_to_file` | Save a node's Resource to a standalone `.tres` file |
| `get_resource_info` | Inspect any resource on disk or attached to a node |
| `get_node_spatial_info` | Query local/global transforms and AABB for Node3D |
| `measure_node_distance` | Measure world-space distance between two Node3D nodes |
| `snap_node_to_grid` | Snap a Node3D position to a grid |
| `get_scene_hierarchy` | Get full scene tree for the visualizer |
| `get_scene_node_properties` | Get all editable properties of a specific node |
| `set_scene_node_property` | Set a property on a node (supports complex types) |
| `set_node_groups` | Set/add/remove group memberships on a node |
| `get_node_groups` | Read a node's group memberships |
| `find_nodes_in_group` | Find all nodes in a scene that belong to a group |
| `list_signal_connections` | List persisted signal connections from a `.tscn` |
| `connect_signal` | Add a persistent signal connection between two nodes |
| `disconnect_signal` | Remove a signal connection |

### Script Operations (`script_tools.gd`)

| Tool | Description |
|------|-------------|
| `edit_script` | Apply a surgical code edit (snippet replacement) to a `.gd` file |
| `validate_script` | Parse a script and report syntax errors |
| `list_scripts` | List all `.gd` files in the project |
| `create_folder` | Create a directory |
| `delete_file` | Delete a file (with confirmation and backup) |
| `rename_file` | Rename or move a file |

### Project Configuration (`project_tools.gd`)

| Tool | Description |
|------|-------------|
| `get_project_settings` | Quick summary: main scene, window size, physics, render |
| `list_settings` | Browse project settings by category with current values |
| `update_project_settings` | Update one or more project settings |
| `get_input_map` | Get all input actions and their key/button bindings |
| `configure_input_map` | Add, remove, or replace input actions |
| `get_collision_layers` | Get named 2D/3D physics collision layers |
| `get_node_properties` | Get available properties for a node type (via ClassDB) |
| `setup_autoload` | Register, unregister, or list autoload singletons |
| `classdb_query` | Query Godot's ClassDB for class info, methods, signals |
| `get_console_log` | Read the editor Output panel |
| `get_errors` | Get errors from Output panel and Debugger > Errors tab |
| `clear_console_log` | Clear the editor Output panel |
| `open_in_godot` | Open a file in the editor at a specific line |
| `scene_tree_dump` | Dump the current scene tree as text |
| `rescan_filesystem` | Trigger a filesystem rescan |

### Runtime Tools (`project_tools.gd` + `mcp_runtime.gd`)

| Tool | Description |
|------|-------------|
| `run_scene` | Launch a scene (blocks until started, optionally waits for runtime) |
| `stop_scene` | Stop the running scene |
| `is_playing` | Check if a scene is currently playing |
| `get_runtime_status` | Combined editor + runtime status snapshot |
| `wait` | Server-side sleep (clamped to 20s max) |

### In-Game Runtime Tools (`mcp_runtime.gd` — dispatched by the server directly to the running game)

| Tool | Description |
|------|-------------|
| `take_screenshot` | Capture the game viewport as PNG |
| `send_input` | Synthesize and dispatch InputEvent (key, mouse, action) |
| `query_runtime_node` | Inspect a live node's class, path, groups, and properties |
| `get_runtime_log` | Read the in-game ring buffer log |
| `list_signal_connections` | List runtime signal connections on a live node |

### Asset Generation (`asset_tools.gd`)

| Tool | Description |
|------|-------------|
| `generate_2d_asset` | Render an SVG string to a PNG asset on disk |

### Visualization (`visualizer_tools.gd`)

| Tool | Description |
|------|-------------|
| `map_project` | Crawl all GDScript files and build a structural project map |
| `map_scenes` | Crawl all `.tscn` files and build a scene dependency map |

## Connection Lifecycle

1. Editor starts → `_enter_tree()` creates `MCPClient` and `ToolExecutor`
2. `MCPClient` connects to `ws://127.0.0.1:6505`
3. On connect, sends `{type: "godot_ready", role: "editor", project_path: "..."}`
4. Server routes `tool_invoke` messages by tool name
5. On disconnect, auto-reconnects with exponential backoff (2s → 4s → 8s → 10s max)

## Status Indicator

A label in the editor toolbar shows connection state:

| Text | Color | Meaning |
|------|-------|---------|
| `MCP: Connecting...` | Yellow | Attempting WebSocket connection |
| `MCP: No Agent` | Orange | Connected to server, no AI agent attached |
| `MCP: Agent Active` | Green | AI agent connected and ready |
| `MCP: Agents (N)` | Green | Multiple agents connected |
| `MCP: Disconnected` | Red | Lost connection, reconnecting |

If the `MCPRuntime` in-game helper connects, ` + Runtime` is appended.

## MCPRuntime Autoload

`MCPRuntime` is a Godot autoload that lives inside the user's running game. It connects to the same WebSocket server with `role: "runtime"` so the server can route runtime tool calls to it instead of the editor plugin. It:

- Maintains a ring buffer of the last 500 runtime log lines
- Captures screenshots via `viewport.get_texture().get_image()`
- Dispatches synthetic input events via `Input.parse_input_event()`
- Queries live node properties in the running scene tree

### Sending Custom Runtime Logs

From any game script:

```gdscript
MCPRuntime.push_runtime_log("info", "Player reached checkpoint 3")
MCPRuntime.push_runtime_log("warn", "Low memory warning")
MCPRuntime.push_runtime_log("error", "Failed to load asset")
```

These entries appear in `get_runtime_log` results visible to the AI agent.

## WebSocket Protocol

All messages are JSON. Key message types:

### Client → Server

```json
{"type": "godot_ready", "role": "editor", "project_path": "/path/to/project"}
{"type": "godot_ready", "role": "runtime", "project_path": "...", "started_at": 123456789}
{"type": "tool_result", "id": "req-123", "success": true, "result": {...}}
{"type": "pong"}
```

### Server → Client

```json
{"type": "tool_invoke", "id": "req-123", "tool": "read_scene", "args": {"scene_path": "res://main.tscn"}}
{"type": "ping"}
{"type": "client_status", "count": 1}
{"type": "runtime_status", "connected": true}
```

## Buffer Sizes

- Editor WebSocket: 4MB outbound / 1MB inbound
- Runtime WebSocket: 8MB outbound (for screenshots) / 256KB inbound
- Editor packet processing: max 32 packets per frame
