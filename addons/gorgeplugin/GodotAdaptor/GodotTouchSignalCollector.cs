using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeFramework.Utilities.Json;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.GorgeFramework;
using GorgePlugin.addons.gorgeplugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GodotVector2 = Godot.Vector2;
using GorgeMath = Gorge.Native.GorgeFramework.Math;
using GorgeVector2 = Gorge.Native.GorgeFramework.Vector2;
using GorgeVector3 = Gorge.Native.GorgeFramework.Vector3;

namespace Gorge.GorgeFramework.Adaptor;

public partial class GodotTouchSignalCollector : Godot.Node, ISignalCollector
{
    private const string TouchChannelName = "Touch";

    private readonly Dictionary<int, RawTouchState> _rawTouches = new();
    private readonly Dictionary<int, int> _touchIndexSignalIdMap = new();
    private readonly Dictionary<GorgeObject, int> _autoPlaySignalIdMap = new();

    private bool _enabled;
    private bool _autoPlaySubscribed;
    private bool _rawMousePressed;
    private int _mouseTouchId;
    private GodotVector2 _rawMousePosition;

    private readonly Dictionary<int, MoveSampleState> _touchMoveSample = new();
    private MoveSampleState _mouseMoveSample;

    private Gorge.GorgeFramework.Simulators.ISimulationDriver _driver;

    public bool EnableTouchInput { get; set; } = true;
    public bool EnableMouseInput { get; set; }
    public bool EnableAutoPlay { get; set; }
    public bool EnablePreloadSignal { get; set; }
    public string PreloadSignalPath { get; set; } = string.Empty;
    public Action<string> ErrorReporter { get; set; }

    /// <summary>
    /// Minimum distance (in gameplay coordinates, 16x10 grid) between consecutive move edges.
    /// Set to 0 to disable spatial sampling.
    /// </summary>
    public float MoveSpatialThreshold { get; set; } = 0.25f;

    /// <summary>
    /// Maximum simulate-time between forced move edges when spatial threshold is not crossed.
    /// Prevents the signal from appearing frozen during very slow movements.
    /// Set to 0 to disable temporal sampling.
    /// </summary>
    public float MoveTemporalThreshold { get; set; } = 0.05f;

    public GorgeSimulationRuntime Runtime => _driver?.Runtime;

    public override void _Ready()
    {
        SetProcessInput(true);
    }

    public void Initialize(Gorge.GorgeFramework.Simulators.ISimulationDriver driver)
    {
        DisableAutoPlaySubscription();

        _enabled = false;
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        EnsureTouchChannel();

        if (EnableAutoPlay)
        {
            _driver.Runtime.Simulation.BeforeSimulate += RecordAutoPlay;
            _autoPlaySubscribed = true;
        }

        if (EnablePreloadSignal)
        {
            LoadPreloadSignal();
        }
    }

    public void Enable()
    {
        if (_driver?.Runtime?.Automaton?.InputSignals == null)
        {
            ReportError("Cannot enable touch input before the simulation automaton is initialized.");
            return;
        }

        _enabled = true;
        RecordInitialSignal();
    }

    public void Disable()
    {
        if (_driver?.Runtime?.Automaton?.InputSignals != null)
        {
            RecordFinalSignal();
        }

        _enabled = false;
        _touchIndexSignalIdMap.Clear();
        _autoPlaySignalIdMap.Clear();
        DisableAutoPlaySubscription();
    }

    public override void _Input(InputEvent inputEvent)
    {
        switch (inputEvent)
        {
            case InputEventScreenTouch touch:
                RecordRawTouch(touch);
                if (_enabled && EnableTouchInput)
                {
                    RecordScreenTouch(touch);
                }

                break;
            case InputEventScreenDrag drag:
                RecordRawDrag(drag);
                if (_enabled && EnableTouchInput)
                {
                    RecordScreenDrag(drag);
                }

                break;
            case InputEventMouseButton mouseButton:
                RecordRawMouseButton(mouseButton);
                if (_enabled && EnableMouseInput)
                {
                    RecordMouseButton(mouseButton);
                }

                break;
            case InputEventMouseMotion mouseMotion:
                RecordRawMouseMotion(mouseMotion);
                if (_enabled && EnableMouseInput)
                {
                    RecordMouseMotion(mouseMotion);
                }

                break;
        }
    }

    public void RecordInitialSignal()
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            ReportError("Cannot record initial touch signals while simulation is paused.");
            return;
        }

        if (EnableTouchInput)
        {
            foreach (var (touchIndex, state) in _rawTouches)
            {
                if (!state.Pressed)
                {
                    continue;
                }

                var touchId = GetTouchId(touchIndex);
                SetInitialSignal(touchId, simulateTime, true, state.Position);
            }
        }

        if (EnableMouseInput && _rawMousePressed)
        {
            _mouseTouchId = _driver.Runtime.Automaton.GetDisposableSignalId();
            SetInitialSignal(_mouseTouchId, simulateTime, true, _rawMousePosition);
        }
    }

    public void RecordFinalSignal()
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            ReportError("Cannot record final touch signals while simulation is paused.");
            return;
        }

        if (!_driver.Runtime.Automaton.InputSignals.TryGetValue(TouchChannelName, out var channel))
        {
            return;
        }

        foreach (var fragment in channel.Values)
        {
            fragment.EndTime = simulateTime;
        }
    }

    private void RecordRawTouch(InputEventScreenTouch touch)
    {
        if (touch.Pressed && !touch.Canceled)
        {
            _rawTouches[touch.Index] = new RawTouchState(true, touch.Position);
        }
        else
        {
            _rawTouches[touch.Index] = new RawTouchState(false, touch.Position);
        }
    }

    private void RecordRawDrag(InputEventScreenDrag drag)
    {
        _rawTouches[drag.Index] = new RawTouchState(true, drag.Position);
    }

    private void RecordRawMouseButton(InputEventMouseButton mouseButton)
    {
        if (mouseButton.ButtonIndex != MouseButton.Left)
        {
            return;
        }

        _rawMousePressed = mouseButton.Pressed && !mouseButton.Canceled;
        _rawMousePosition = mouseButton.Position;
    }

    private void RecordRawMouseMotion(InputEventMouseMotion mouseMotion)
    {
        _rawMousePosition = mouseMotion.Position;
    }

    private void RecordScreenTouch(InputEventScreenTouch touch)
    {
        if (touch.Pressed && !touch.Canceled)
        {
            RecordTouchDown(touch.Index, touch.Position);
        }
        else
        {
            RecordTouchUp(touch.Index, touch.Position);
        }
    }

    private void RecordScreenDrag(InputEventScreenDrag drag)
    {
        RecordTouchMove(drag.Index, drag.Position);
    }

    private void RecordMouseButton(InputEventMouseButton mouseButton)
    {
        if (mouseButton.ButtonIndex != MouseButton.Left)
        {
            return;
        }

        if (mouseButton.Pressed && !mouseButton.Canceled)
        {
            RecordMouseDown(mouseButton.Position);
        }
        else
        {
            RecordMouseUp(mouseButton.Position);
        }
    }

    private void RecordMouseMotion(InputEventMouseMotion mouseMotion)
    {
        if (_rawMousePressed)
        {
            RecordMousePressing(mouseMotion.Position);
        }
    }

    private void RecordTouchDown(int touchIndex, GodotVector2 viewportPosition)
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            return;
        }

        _touchMoveSample.Remove(touchIndex);
        var signalId = GetTouchId(touchIndex);
        var position = ViewportPointToGameplayPosition(viewportPosition);
        var fragment = GetOrCreateTouchFragment(signalId, simulateTime, false, position);

        fragment.EndTime = float.PositiveInfinity;
        fragment.Edges.Add(new Edge<GorgeObject>
        {
            Time = simulateTime,
            Value = new TouchSignal(true, position)
        });
    }

    private void RecordTouchUp(int touchIndex, GodotVector2 viewportPosition)
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            return;
        }

        var signalId = GetTouchId(touchIndex);
        var position = ViewportPointToGameplayPosition(viewportPosition);

        if (!TouchChannel().TryGetValue(signalId, out var fragment))
        {
            ReportError("Ignoring a screen touch release that did not have a recorded press.");
            ReleaseTouchIndex(touchIndex);
            return;
        }

        if (_touchMoveSample.TryGetValue(touchIndex, out var sample) && sample.Initialized)
        {
            if (GorgeVector2.Distance(sample.LastPosition, position) > 0.0001f)
            {
                fragment.Edges.Add(new Edge<GorgeObject>
                {
                    Time = simulateTime,
                    Value = new TouchSignal(true, position)
                });
            }
            _touchMoveSample.Remove(touchIndex);
        }

        fragment.EndTime = simulateTime;
        fragment.Edges.Add(new Edge<GorgeObject>
        {
            Time = simulateTime,
            Value = new TouchSignal(false, position)
        });

        ReleaseTouchIndex(touchIndex);
    }

    private void RecordTouchMove(int touchIndex, GodotVector2 viewportPosition)
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            return;
        }

        var signalId = GetTouchId(touchIndex);
        var position = ViewportPointToGameplayPosition(viewportPosition);

        if (!TouchChannel().TryGetValue(signalId, out var fragment))
        {
            ReportError("Ignoring a screen touch drag that did not have a recorded press.");
            return;
        }

        if (!_touchMoveSample.TryGetValue(touchIndex, out var sample) || !sample.Initialized)
        {
            fragment.Edges.Add(new Edge<GorgeObject>
            {
                Time = simulateTime,
                Value = new TouchSignal(true, position)
            });
            _touchMoveSample[touchIndex] = new MoveSampleState
            {
                LastPosition = position,
                LastSimulateTime = simulateTime,
                Initialized = true
            };
            return;
        }

        var spatialDelta = GorgeVector2.Distance(sample.LastPosition, position);
        var temporalDelta = simulateTime - sample.LastSimulateTime;

        if (spatialDelta < MoveSpatialThreshold && temporalDelta < MoveTemporalThreshold)
        {
            return;
        }

        fragment.Edges.Add(new Edge<GorgeObject>
        {
            Time = simulateTime,
            Value = new TouchSignal(true, position)
        });
        sample.LastPosition = position;
        sample.LastSimulateTime = simulateTime;
        _touchMoveSample[touchIndex] = sample;
    }

    private void RecordMouseDown(GodotVector2 viewportPosition)
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            return;
        }

        _mouseMoveSample = default;
        _mouseTouchId = _driver.Runtime.Automaton.GetDisposableSignalId();
        var position = ViewportPointToGameplayPosition(viewportPosition);
        var fragment = GetOrCreateTouchFragment(_mouseTouchId, simulateTime, false, position);

        fragment.EndTime = float.PositiveInfinity;
        fragment.Edges.Add(new Edge<GorgeObject>
        {
            Time = simulateTime,
            Value = new TouchSignal(true, position)
        });
    }

    private void RecordMouseUp(GodotVector2 viewportPosition)
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            return;
        }

        if (!TouchChannel().TryGetValue(_mouseTouchId, out var fragment))
        {
            return;
        }

        var position = ViewportPointToGameplayPosition(viewportPosition);

        if (_mouseMoveSample.Initialized)
        {
            if (GorgeVector2.Distance(_mouseMoveSample.LastPosition, position) > 0.0001f)
            {
                fragment.Edges.Add(new Edge<GorgeObject>
                {
                    Time = simulateTime,
                    Value = new TouchSignal(true, position)
                });
            }
            _mouseMoveSample = default;
        }

        fragment.EndTime = simulateTime;
        fragment.Edges.Add(new Edge<GorgeObject>
        {
            Time = simulateTime,
            Value = new TouchSignal(false, position)
        });
    }

    private void RecordMousePressing(GodotVector2 viewportPosition)
    {
        var simulateTime = GetRealSimulateTimeOrNegative();
        if (simulateTime < 0)
        {
            return;
        }

        if (!TouchChannel().TryGetValue(_mouseTouchId, out var fragment))
        {
            return;
        }

        var position = ViewportPointToGameplayPosition(viewportPosition);

        if (!_mouseMoveSample.Initialized)
        {
            fragment.Edges.Add(new Edge<GorgeObject>
            {
                Time = simulateTime,
                Value = new TouchSignal(true, position)
            });
            _mouseMoveSample = new MoveSampleState
            {
                LastPosition = position,
                LastSimulateTime = simulateTime,
                Initialized = true
            };
            return;
        }

        var spatialDelta = GorgeVector2.Distance(_mouseMoveSample.LastPosition, position);
        var temporalDelta = simulateTime - _mouseMoveSample.LastSimulateTime;

        if (spatialDelta < MoveSpatialThreshold && temporalDelta < MoveTemporalThreshold)
        {
            return;
        }

        fragment.Edges.Add(new Edge<GorgeObject>
        {
            Time = simulateTime,
            Value = new TouchSignal(true, position)
        });
        _mouseMoveSample.LastPosition = position;
        _mouseMoveSample.LastSimulateTime = simulateTime;
    }

    private void RecordAutoPlay(float simulateTimeFrom, float simulateTimeTo, float chartTimeFrom, float chartTimeTo)
    {
        foreach (var simulator in _driver.Runtime.Chart.AliveNotes)
        {
            var edges = GodotSignalHelper.SimulateInput(simulator, chartTimeFrom, chartTimeTo, simulateTimeFrom,
                simulateTimeTo, out var end);

            if (edges == null || edges.Count == 0)
            {
                continue;
            }

            var signalId = GetAutoPlayTouchId(simulator);
            if (end)
            {
                ReleaseAutoPlayNote(simulator);
            }

            if (!TouchChannel().TryGetValue(signalId, out var fragment))
            {
                var signalHead = edges[0];
                edges = edges.Skip(1).ToList();
                fragment = new Fragment<GorgeObject>
                {
                    SignalId = signalId,
                    StartTime = signalHead.Time,
                    StartValue = signalHead.Value,
                    EndTime = edges.Count == 0 ? signalHead.Time : edges[^1].Time,
                    Edges = edges
                };
                TouchChannel()[signalId] = fragment;
            }
            else
            {
                fragment.AppendEdges(edges);
            }
        }
    }

    private int GetTouchId(int touchIndex)
    {
        if (!_touchIndexSignalIdMap.TryGetValue(touchIndex, out var touchId))
        {
            touchId = _driver.Runtime.Automaton.GetDisposableSignalId();
            _touchIndexSignalIdMap[touchIndex] = touchId;
        }

        return touchId;
    }

    private void ReleaseTouchIndex(int touchIndex)
    {
        _touchIndexSignalIdMap.Remove(touchIndex);
    }

    private int GetAutoPlayTouchId(GorgeObject gorgeObject)
    {
        if (!_autoPlaySignalIdMap.TryGetValue(gorgeObject, out var touchId))
        {
            touchId = _driver.Runtime.Automaton.GetDisposableSignalId();
            _autoPlaySignalIdMap[gorgeObject] = touchId;
        }

        return touchId;
    }

    private void ReleaseAutoPlayNote(GorgeObject gorgeObject)
    {
        _autoPlaySignalIdMap.Remove(gorgeObject);
    }

    private void SetInitialSignal(int signalId, float simulateTime, bool isTouching, GodotVector2 viewportPosition)
    {
        var position = ViewportPointToGameplayPosition(viewportPosition);
        TouchChannel()[signalId] = new Fragment<GorgeObject>
        {
            SignalId = signalId,
            StartTime = simulateTime,
            EndTime = simulateTime,
            StartValue = new TouchSignal(isTouching, position),
            Edges = new List<Edge<GorgeObject>>()
        };
    }

    private Fragment<GorgeObject> GetOrCreateTouchFragment(int signalId, float simulateTime, bool isTouching,
        GorgeVector2 position)
    {
        var channel = TouchChannel();
        if (channel.TryGetValue(signalId, out var fragment))
        {
            return fragment;
        }

        fragment = new Fragment<GorgeObject>
        {
            SignalId = signalId,
            StartTime = simulateTime,
            StartValue = new TouchSignal(isTouching, position),
            Edges = new List<Edge<GorgeObject>>()
        };
        channel[signalId] = fragment;
        return fragment;
    }

    private GorgeVector2 ViewportPointToGameplayPosition(GodotVector2 viewportPosition)
    {
        var localPosition = viewportPosition;
        if (GamePlayer.Instance is CanvasItem canvasItem)
        {
            localPosition = canvasItem.MakeCanvasPositionLocal(viewportPosition);
        }

        var viewportSize = GamePlayer.viewportSize;
        if (viewportSize == GodotVector2.Zero && GetViewport() != null)
        {
            viewportSize = GetViewport().GetVisibleRect().Size;
        }

        var scaleX = viewportSize.X / 16f;
        var scaleY = viewportSize.Y / 10f;
        if (scaleX == 0 || scaleY == 0)
        {
            return new GorgeVector2(localPosition.X, -localPosition.Y);
        }

        return new GorgeVector2(localPosition.X / scaleX, -localPosition.Y / scaleY);
    }

    private void LoadPreloadSignal()
    {
        if (string.IsNullOrWhiteSpace(PreloadSignalPath))
        {
            ReportError("Preload touch signal is enabled, but PreloadSignalPath is empty.");
            return;
        }

        using var file = FileAccess.Open(PreloadSignalPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            ReportError($"Cannot open preload touch signal file: {PreloadSignalPath}");
            return;
        }

        try
        {
            var text = file.GetAsText();
            var preloadSignal = DeserializePreloadSignal(text);
            var channel = TouchChannel();
            foreach (var (key, value) in preloadSignal)
            {
                channel[key] = value;
            }
        }
        catch (Exception exception)
        {
            ReportError($"Cannot read preload touch signal file '{PreloadSignalPath}': {exception.Message}");
        }
    }

    private static Dictionary<int, Fragment<GorgeObject>> DeserializePreloadSignal(string text)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            Converters = new List<JsonConverter>
            {
                new GorgeVector2Converter(),
                new TouchSignalGorgeObjectConverter()
            }
        };

        return JsonConvert.DeserializeObject<Dictionary<int, Fragment<GorgeObject>>>(text, settings) ?? new();
    }

    private float GetRealSimulateTimeOrNegative()
    {
        return _driver?.GetRealSimulateTime() ?? -1f;
    }

    private void EnsureTouchChannel()
    {
        if (!_driver.Runtime.Automaton.InputSignals.ContainsKey(TouchChannelName))
        {
            _driver.Runtime.Automaton.InputSignals[TouchChannelName] = new ChannelSplit();
        }
    }

    private ChannelSplit TouchChannel()
    {
        EnsureTouchChannel();
        return _driver.Runtime.Automaton.InputSignals[TouchChannelName];
    }

    private void DisableAutoPlaySubscription()
    {
        if (!_autoPlaySubscribed || _driver?.Runtime?.Simulation == null)
        {
            _autoPlaySubscribed = false;
            return;
        }

        _driver.Runtime.Simulation.BeforeSimulate -= RecordAutoPlay;
        _autoPlaySubscribed = false;
    }

    private void ReportError(string message)
    {
        if (ErrorReporter != null)
        {
            ErrorReporter.Invoke(message);
            return;
        }

        GD.PushError(message);
    }

    private readonly record struct RawTouchState(bool Pressed, GodotVector2 Position);

    private struct MoveSampleState
    {
        public GorgeVector2 LastPosition;
        public float LastSimulateTime;
        public bool Initialized;
    }

    private sealed class TouchSignalGorgeObjectConverter : JsonConverter<GorgeObject>
    {
        public override GorgeObject ReadJson(JsonReader reader, Type objectType, GorgeObject existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            if (jObject.TryGetValue("isTouching", StringComparison.OrdinalIgnoreCase, out var isTouchingToken))
            {
                var isTouching = isTouchingToken.ToObject<bool>();
                var position = jObject["position"]?.ToObject<GorgeVector2>(serializer) ?? new GorgeVector2(0, 0);
                return new TouchSignal(isTouching, position);
            }

            throw new JsonSerializationException("Only TouchSignal values are supported in preload touch signals.");
        }

        public override void WriteJson(JsonWriter writer, GorgeObject value, JsonSerializer serializer)
        {
            if (value is not TouchSignal touchSignal)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, touchSignal);
        }
    }
}

public static class GodotSignalHelper
{
    public static List<Edge<GorgeObject>> SimulateInput(GorgeObject noteObject, float chartTimeFrom,
        float chartTimeTo, float simulateTimeFrom, float simulateTimeTo, out bool end)
    {
        end = false;
        if (chartTimeFrom > chartTimeTo)
        {
            return null;
        }

        var note = noteObject.RealObject;
        if (!TryBuildAutoPlayNote(note, chartTimeTo, out var autoPlayNote))
        {
            return null;
        }

        var position = autoPlayNote.Position;
        var respondMoment = autoPlayNote.RespondMoment;
        var autoplayHoldTime = autoPlayNote.HoldTime;
        var isCompetitiveNote = autoPlayNote.IsCompetitive;

        if (chartTimeFrom < respondMoment)
        {
            if (chartTimeTo >= respondMoment)
            {
                var respondMomentSimulateTime = IntervalScaling(chartTimeFrom, chartTimeTo, simulateTimeFrom,
                    simulateTimeTo, respondMoment);

                if (respondMomentSimulateTime == simulateTimeFrom && simulateTimeFrom != simulateTimeTo)
                {
                    respondMomentSimulateTime = FloatIncrement(respondMomentSimulateTime);
                }

                if (chartTimeTo < respondMoment + autoplayHoldTime)
                {
                    if (isCompetitiveNote)
                    {
                        return new List<Edge<GorgeObject>>
                        {
                            TouchEdge(respondMomentSimulateTime, false, position),
                            TouchEdge(respondMomentSimulateTime, true, position)
                        };
                    }

                    return new List<Edge<GorgeObject>>
                    {
                        TouchEdge(respondMomentSimulateTime, true, position)
                    };
                }

                var finishMomentSimulateTime = IntervalScaling(chartTimeFrom, chartTimeTo, simulateTimeFrom,
                    simulateTimeTo, respondMoment + autoplayHoldTime);
                if (finishMomentSimulateTime == simulateTimeFrom)
                {
                    finishMomentSimulateTime = FloatIncrement(finishMomentSimulateTime);
                }

                end = true;
                if (isCompetitiveNote)
                {
                    return new List<Edge<GorgeObject>>
                    {
                        TouchEdge(respondMomentSimulateTime, false, position),
                        TouchEdge(respondMomentSimulateTime, true, position),
                        TouchEdge(finishMomentSimulateTime, false, position)
                    };
                }

                return new List<Edge<GorgeObject>>
                {
                    TouchEdge(respondMomentSimulateTime, true, position),
                    TouchEdge(finishMomentSimulateTime, false, position)
                };
            }

            return null;
        }

        if (chartTimeFrom < respondMoment + autoplayHoldTime)
        {
            if (chartTimeTo < respondMoment + autoplayHoldTime)
            {
                if (simulateTimeFrom == simulateTimeTo)
                {
                    return null;
                }

                return new List<Edge<GorgeObject>>
                {
                    TouchEdge(simulateTimeTo, true, position)
                };
            }

            var finishMomentSimulateTime = IntervalScaling(chartTimeFrom, chartTimeTo, simulateTimeFrom,
                simulateTimeTo, respondMoment + autoplayHoldTime);
            if (finishMomentSimulateTime == simulateTimeFrom)
            {
                finishMomentSimulateTime = FloatIncrement(finishMomentSimulateTime);
            }

            end = true;
            return new List<Edge<GorgeObject>>
            {
                TouchEdge(finishMomentSimulateTime, false, position)
            };
        }

        return null;
    }

    private static bool TryBuildAutoPlayNote(GorgeObject note, float chartTime, out AutoPlayNote autoPlayNote)
    {
        return TryBuildDremuAutoPlayNote(note, chartTime, out autoPlayNote) ||
               TryBuildStandardAutoPlayNote(note, out autoPlayNote);
    }

    private static bool TryBuildStandardAutoPlayNote(GorgeObject note, out AutoPlayNote autoPlayNote)
    {
        autoPlayNote = default;
        var noteClassName = note.GorgeClass.Declaration.Name;
        var noteShortName = ShortClassName(noteClassName);
        var lineNote = noteShortName is "Tap" or "Catch" or "Hold";
        var skyNote = noteShortName is "SkyTap" or "Slider";
        if (!lineNote && !skyNote)
        {
            return false;
        }

        if (!TryGetObjectField(note, "positionY", out var positionYObject) ||
            !TryGetFloatField(note, "respondMoment", out var respondMoment))
        {
            return false;
        }

        var positionY = VariableFloat.FromGorgeObject(positionYObject).baseValue;
        var localRespondPosition = lineNote
            ? new GorgeVector2(0, positionY)
            : new GorgeVector2(VariableFloat.FromGorgeObject(note.GetObjectField("positionX")).baseValue, positionY);
        var lane = FindLane(note, skyNote ? "SkyArea" : "Line");
        if (lane == null || !TryGetObjectField(lane.RealObject, "positionNode", out var positionNodeObject))
        {
            return false;
        }

        var globalPosition = Gorge.Native.GorgeFramework.Node.FromGorgeObject(positionNodeObject)
            .LocalPositionToGlobalPosition(
            new GorgeVector3(localRespondPosition.x, localRespondPosition.y, 0));
        var holdTime = TryInvokeAutoPlayHoldTime(note, out var methodHoldTime) ? methodHoldTime : 0f;
        autoPlayNote = new AutoPlayNote(
            respondMoment,
            holdTime,
            noteShortName is "Tap" or "SkyTap",
            new GorgeVector2(globalPosition.x, globalPosition.y));
        return true;
    }

    private static bool TryBuildDremuAutoPlayNote(GorgeObject note, float chartTime, out AutoPlayNote autoPlayNote)
    {
        autoPlayNote = default;
        if (!note.GorgeClass.Declaration.Is("Dremu.DremuNote") ||
            !TryGetFloatField(note, "hitTime", out var hitTime) ||
            !TryGetObjectField(note, "lane", out var lane) ||
            lane == null)
        {
            return false;
        }

        var noteShortName = ShortClassName(note.GorgeClass.Declaration.Name);
        var holdTime = TryGetFloatField(note, "holdTime", out var dremuHoldTime) ? dremuHoldTime : 0f;
        var positionTime = holdTime > 0
            ? Clamp(chartTime - hitTime, 0f, holdTime)
            : 0f;
        var localPosition = DremuAimPosition(note, lane.RealObject, hitTime, positionTime, chartTime);
        if (!TryGetObjectField(lane.RealObject, "noteReferenceNode", out var noteReferenceNodeObject))
        {
            return false;
        }

        var globalPosition = Gorge.Native.GorgeFramework.Node.FromGorgeObject(noteReferenceNodeObject)
            .LocalPositionToGlobalPosition(
            new GorgeVector3(localPosition.x, localPosition.y, 0));
        autoPlayNote = new AutoPlayNote(
            hitTime,
            holdTime,
            noteShortName is "DremuTap" or "DremuTaplik",
            new GorgeVector2(globalPosition.x, globalPosition.y));
        return true;
    }

    private static GorgeVector2 DremuAimPosition(GorgeObject note, GorgeObject lane, float hitTime,
        float positionTime, float chartTime)
    {
        var position = VariableFloat.FromGorgeObject(note.GetObjectField("position"));
        var mainX = position.EvaluateAdd(positionTime);
        var aimX = mainX;

        if (TryGetObjectField(note, "holdLine", out var holdLineObject))
        {
            var targetProjectionFix = 1f;
            if (TryGetBoolField(note, "targetProjectionFix", out var shouldFixProjection) && shouldFixProjection)
            {
                var normalAngle = InvokeLaneFloatMethod(lane, "EvaluateNormalVectorAngle", mainX, chartTime);
                targetProjectionFix = GorgeMath.SinDeg(normalAngle);
            }

            aimX += FunctionCurve.FromGorgeObject(holdLineObject).Evaluate(positionTime) * targetProjectionFix;
        }

        return InvokeLaneVector2Method(lane, "EvaluatePointPosition", aimX, 0f, hitTime + positionTime);
    }

    private static GorgeObject FindLane(GorgeObject note, string laneShortClassName)
    {
        if (!TryGetStringField(note, "laneName", out var laneName))
        {
            return null;
        }

        return RuntimeStatic.Runtime?.SimulationRuntime?.Chart?.AliveElements?.FirstOrDefault(lane =>
            ShortClassName(lane.RealObject.GorgeClass.Declaration.Name) == laneShortClassName &&
            TryGetStringField(lane.RealObject, "name", out var name) &&
            name == laneName);
    }

    private static bool TryInvokeAutoPlayHoldTime(GorgeObject note, out float holdTime)
    {
        holdTime = 0f;
        if (!note.GorgeClass.Declaration.ContainsMethodWithName("AutoPlayHoldTime"))
        {
            return false;
        }

        holdTime = (float)note.GorgeClass.InvokeMethod(note, "AutoPlayHoldTime", Array.Empty<GorgeType>(),
            new Dictionary<GorgeType, GorgeType>(), Array.Empty<object>());
        return true;
    }

    private static GorgeVector2 InvokeLaneVector2Method(GorgeObject lane, string methodName, float x, float distance,
        float chartTime)
    {
        return (GorgeVector2)lane.GorgeClass.InvokeMethod(lane, methodName,
            new[] { GorgeType.Float, GorgeType.Float, GorgeType.Float },
            new Dictionary<GorgeType, GorgeType>(),
            new object[] { x, distance, chartTime });
    }

    private static float InvokeLaneFloatMethod(GorgeObject lane, string methodName, float x, float chartTime)
    {
        return (float)lane.GorgeClass.InvokeMethod(lane, methodName,
            new[] { GorgeType.Float, GorgeType.Float },
            new Dictionary<GorgeType, GorgeType>(),
            new object[] { x, chartTime });
    }

    private static bool TryGetObjectField(GorgeObject gorgeObject, string fieldName, out GorgeObject value)
    {
        value = null;
        if (!gorgeObject.GorgeClass.Declaration.TryGetFieldByName(fieldName, out _))
        {
            return false;
        }

        value = gorgeObject.GetObjectField(fieldName);
        return true;
    }

    private static bool TryGetFloatField(GorgeObject gorgeObject, string fieldName, out float value)
    {
        value = 0f;
        if (!gorgeObject.GorgeClass.Declaration.TryGetFieldByName(fieldName, out _))
        {
            return false;
        }

        value = gorgeObject.GetFloatField(fieldName);
        return true;
    }

    private static bool TryGetBoolField(GorgeObject gorgeObject, string fieldName, out bool value)
    {
        value = false;
        if (!gorgeObject.GorgeClass.Declaration.TryGetFieldByName(fieldName, out _))
        {
            return false;
        }

        value = gorgeObject.GetBoolField(fieldName);
        return true;
    }

    private static bool TryGetStringField(GorgeObject gorgeObject, string fieldName, out string value)
    {
        value = null;
        if (!gorgeObject.GorgeClass.Declaration.TryGetFieldByName(fieldName, out _))
        {
            return false;
        }

        value = gorgeObject.GetStringField(fieldName);
        return true;
    }

    private static string ShortClassName(string className)
    {
        var dotIndex = className.LastIndexOf('.');
        return dotIndex < 0 ? className : className[(dotIndex + 1)..];
    }

    private static float Clamp(float value, float min, float max)
    {
        return System.Math.Min(System.Math.Max(value, min), max);
    }

    private static Edge<GorgeObject> TouchEdge(float time, bool isTouching, GorgeVector2 position)
    {
        return new Edge<GorgeObject>
        {
            Time = time,
            Value = new TouchSignal(isTouching, position)
        };
    }

    private static float IntervalScaling(float preimageIntervalStart, float preimageIntervalEnd,
        float imageIntervalStart, float imageIntervalEnd, float preimageIntervalValue)
    {
        if (preimageIntervalStart == preimageIntervalEnd)
        {
            return imageIntervalEnd;
        }

        if (preimageIntervalValue == preimageIntervalEnd)
        {
            return imageIntervalEnd;
        }

        if (preimageIntervalValue == preimageIntervalStart)
        {
            return imageIntervalStart;
        }

        return GorgeMath.Lerp(imageIntervalStart, imageIntervalEnd,
            GorgeMath.InverseLerp(preimageIntervalStart, preimageIntervalEnd, preimageIntervalValue));
    }

    private static float FloatIncrement(float value)
    {
        var convertedInt = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        switch (value)
        {
            case > 0:
                convertedInt++;
                break;
            case < 0:
                convertedInt--;
                break;
            case 0:
                return float.Epsilon;
        }

        return BitConverter.ToSingle(BitConverter.GetBytes(convertedInt), 0);
    }

    private readonly record struct AutoPlayNote(float RespondMoment, float HoldTime, bool IsCompetitive,
        GorgeVector2 Position);
}
