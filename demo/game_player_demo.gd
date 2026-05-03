extends Control

@export var runtime_package_path: String = "res://Dremu.zip"
@export var chart_package_path: String = "res://DremuTest.zip"
@export var auto_run_test: bool = true

@onready var player = $GamePlayer
@onready var play_button: Button = $Panel/Buttons/PlayButton
@onready var stop_button: Button = $Panel/Buttons/StopButton
@onready var status_label: Label = $Panel/StatusLabel

var _started := false
var _stopped := false
var _errors: Array[String] = []

func _ready() -> void:
    play_button.pressed.connect(player.request_play)
    stop_button.pressed.connect(player.request_stop)
    player.connect("ChartStarted", Callable(self, "_on_chart_started"))
    player.connect("ChartStopped", Callable(self, "_on_chart_stopped"))
    player.connect("PlayerError", Callable(self, "_on_player_error"))

    var runtime_added: bool = player.add_runtime_package_path(runtime_package_path)
    var chart_added: bool = player.add_chart_package_path(chart_package_path)
    status_label.text = "Packages runtime=%s chart=%s" % [runtime_added, chart_added]

    if auto_run_test:
        call_deferred("_run_demo_test")

func _run_demo_test() -> void:
    play_button.emit_signal("pressed")
    await get_tree().process_frame
    await get_tree().create_timer(0.25).timeout

    stop_button.emit_signal("pressed")
    await get_tree().process_frame
    await get_tree().create_timer(0.1).timeout

    var passed := _started and _stopped and _errors.is_empty()
    status_label.text = "PASS" if passed else "FAIL"
    print("GorgeGamePlayerDemo started=%s stopped=%s errors=%s" % [_started, _stopped, _errors.size()])
    if not passed:
        push_error("GorgeGamePlayerDemo failed: started=%s stopped=%s errors=%s" % [_started, _stopped, _errors])

func _on_chart_started() -> void:
    _started = true
    status_label.text = "Chart started"

func _on_chart_stopped(reason: String) -> void:
    _stopped = true
    status_label.text = "Chart stopped: %s" % reason

func _on_player_error(message: String) -> void:
    _errors.append(message)
    status_label.text = "Error: %s" % message
