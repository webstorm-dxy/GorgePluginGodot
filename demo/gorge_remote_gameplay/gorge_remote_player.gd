extends Control

@export var runtime_package_path: String = "res://Dremu.zip"
@export var chart_package_path: String = "res://DremuTest.zip"

@export var udp_enabled: bool = true
@export var udp_port: int = 9000
@export var udp_bind_address: String = "127.0.0.1"

const NATIVE_PACKAGE_PATH := "res://addons/gorgeplugin/Native.zip"

var _udp := PacketPeerUDP.new()
var _udp_listening := false

@onready var player = $GamePlayer


func _ready() -> void:
	player.connect("PlayerError", _on_player_error)
	if not player.add_runtime_package_path(runtime_package_path):
		push_warning("Failed to add runtime package path: %s" % runtime_package_path)
	if not player.add_chart_package_path(chart_package_path):
		push_warning("Failed to add chart package path: %s" % chart_package_path)
	if not player.prepare_runtime():
		push_warning("Failed to prepare chart runtime for remote status queries")

	if udp_enabled:
		_start_udp_listener()


func _start_udp_listener() -> void:
	var error := _udp.bind(udp_port, udp_bind_address)
	if error != OK:
		_udp_listening = false
		push_error("Failed to bind UDP listener on %s:%d, error=%d" % [udp_bind_address, udp_port, error])
		return

	_udp_listening = true
	print("UDP chart control listening on %s:%d" % [udp_bind_address, udp_port])


func _process(_delta: float) -> void:
	if not _udp_listening:
		return

	while _udp.get_available_packet_count() > 0:
		_handle_udp_packet()


func _handle_udp_packet() -> void:
	var bytes := _udp.get_packet()
	var packet_ip := _udp.get_packet_ip()
	var packet_port := _udp.get_packet_port()
	var text := bytes.get_string_from_utf8().strip_edges()

	if text.is_empty():
		push_warning("Empty UDP packet from %s:%d" % [packet_ip, packet_port])
		return

	var parsed = JSON.parse_string(text)
	if not parsed is Dictionary:
		push_warning("Invalid UDP JSON from %s:%d: %s" % [packet_ip, packet_port, text])
		return

	var cmd := str(parsed.get("cmd", "")).to_lower()
	match cmd:
		"play":
			if not player.play_chart():
				push_warning("UDP play command failed from %s:%d" % [packet_ip, packet_port])
		"stop":
			if not player.stop_chart():
				push_warning("UDP stop command failed from %s:%d" % [packet_ip, packet_port])
		"pause":
			if not player.pause_chart():
				push_warning("UDP pause command failed from %s:%d" % [packet_ip, packet_port])
		"seek":
			_handle_seek_command(parsed, packet_ip, packet_port)
		"status":
			_send_status_response(packet_ip, packet_port)
		"set_packages":
			_handle_set_packages_command(parsed, packet_ip, packet_port)
		_:
			push_warning("Unknown UDP command from %s:%d: %s" % [packet_ip, packet_port, cmd])


func _handle_seek_command(payload: Dictionary, source_ip: String, source_port: int) -> void:
	var seconds = payload.get("seconds")
	if not (seconds is float or seconds is int):
		push_warning("UDP seek command missing numeric seconds from %s:%d" % [source_ip, source_port])
		return

	if not player.seek_chart_time(float(seconds), true):
		push_warning("UDP seek to %.3fs failed from %s:%d" % [seconds, source_ip, source_port])
		return

	print("UDP seek to %.3fs from %s:%d" % [seconds, source_ip, source_port])


func _send_status_response(dest_ip: String, dest_port: int) -> void:
	var current: float = player.get_chart_time()
	var begin: float = player.get_chart_begin_time()
	var end: float = player.get_chart_end_time()
	var duration: float = player.get_chart_duration()

	if current == -1.0 and begin == -1.0 and end == -1.0:
		var error_response := {
			"type": "status",
			"ok": false,
			"error": "chart_not_ready"
		}
		_udp.set_dest_address(dest_ip, dest_port)
		_udp.put_packet(JSON.stringify(error_response).to_utf8_buffer())
		return

	if current == -1.0:
		current = begin

	var response := {
		"type": "status",
		"ok": true,
		"currentSeconds": current,
		"durationSeconds": duration,
		"beginSeconds": begin,
		"endSeconds": end
	}
	_udp.set_dest_address(dest_ip, dest_port)
	_udp.put_packet(JSON.stringify(response).to_utf8_buffer())


func _normalize_path_list(payload: Dictionary, single_key: String, array_key: String) -> Array:
	var single = payload.get(single_key, "")
	if single is String and not single.is_empty():
		return [single]

	var result: Array = []
	var arr = payload.get(array_key, [])
	if arr is Array:
		for path in arr:
			if path is String and not path.is_empty():
				result.append(path)
	return result


func _send_set_packages_error(dest_ip: String, dest_port: int, error_code: String) -> void:
	var response := {
		"type": "set_packages",
		"ok": false,
		"error": error_code
	}
	_udp.set_dest_address(dest_ip, dest_port)
	_udp.put_packet(JSON.stringify(response).to_utf8_buffer())


func _handle_set_packages_command(payload: Dictionary, source_ip: String, source_port: int) -> void:
	var runtime_paths: Array = _normalize_path_list(payload, "runtimePackagePath", "runtimePackagePaths")
	var chart_paths: Array = _normalize_path_list(payload, "chartPackagePath", "chartPackagePaths")

	if runtime_paths.is_empty():
		_send_set_packages_error(source_ip, source_port, "missing_runtime_package_path")
		return

	if chart_paths.is_empty():
		_send_set_packages_error(source_ip, source_port, "missing_chart_package_path")
		return

	player.clear_packages()

	# Check if any chart path is a .gpkg file
	var has_gpkg := false
	for path in chart_paths:
		if path.get_extension().to_lower() == "gpkg":
			has_gpkg = true
			break

	var effective_runtime_paths: Array = []

	if has_gpkg:
		# gpkg provides modal code; only use Native.zip as runtime
		effective_runtime_paths = [NATIVE_PACKAGE_PATH]
	else:
		# Always ensure Native.zip is included (deduplicate by filename)
		var native_filename := NATIVE_PACKAGE_PATH.get_file()
		var has_native := false
		for path in runtime_paths:
			if path.get_file() == native_filename:
				has_native = true
				break
		if not has_native:
			runtime_paths.push_front(NATIVE_PACKAGE_PATH)
		effective_runtime_paths = runtime_paths

	for path in effective_runtime_paths:
		if not player.add_runtime_package_path(path):
			_send_set_packages_error(source_ip, source_port, "invalid_runtime_package_path")
			return

	for path in chart_paths:
		if not player.add_chart_package_path(path):
			_send_set_packages_error(source_ip, source_port, "invalid_chart_package_path")
			return

	if not player.prepare_runtime():
		_send_set_packages_error(source_ip, source_port, "prepare_failed")
		return

	var begin: float = player.get_chart_begin_time()
	var end: float = player.get_chart_end_time()
	var duration: float = player.get_chart_duration()

	var response := {
		"type": "set_packages",
		"ok": true,
		"runtimePackagePaths": effective_runtime_paths.duplicate(),
		"chartPackagePaths": chart_paths.duplicate(),
		"durationSeconds": duration,
		"beginSeconds": begin,
		"endSeconds": end
	}
	_udp.set_dest_address(source_ip, source_port)
	_udp.put_packet(JSON.stringify(response).to_utf8_buffer())


func _on_player_error(message: String) -> void:
	print("PlayerError: %s" % message)


func _exit_tree() -> void:
	if _udp.is_bound():
		_udp.close()
	_udp_listening = false
