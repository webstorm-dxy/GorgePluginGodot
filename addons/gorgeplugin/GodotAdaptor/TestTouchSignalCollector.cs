using System;
using System.Collections.Generic;
using Godot;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Simulators;
using Gorge.Native.GorgeFramework;
using GorgePlugin.addons.gorgeplugin;
using GorgeVector2 = Gorge.Native.GorgeFramework.Vector2;

public partial class TestTouchSignalCollector : Godot.Node
{
    public override void _Ready()
    {
        TestTouchEvents();
        TestMouseEvents();
        TestPreloadSignal();
        GD.Print("=== Touch Signal Collector Tests Passed ===");

        RunRuntimeAutoPlayTest();
    }

    private async void RunRuntimeAutoPlayTest()
    {
        var errors = new List<string>();
        var player = new GamePlayer
        {
            Name = "RuntimeAutoPlayGamePlayer",
            AutoStartOnReady = true,
            EnableMouseInput = true,
            GorgeAutoPlay = true
        };
        player.RuntimePackagePaths.Add("res://Dremu.zip");
        player.ChartPackagePaths.Add("res://DremuTest.zip");
        player.PlayerError += message => errors.Add(message);
        var started = false;
        player.ChartStarted += () => started = true;
        AddChild(player);

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Assert(started, "runtime autoplay did not auto-start on ready");

        player._Process(20.0);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        DispatchMouseButton(true, new Godot.Vector2(960, 600));

        await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
        DispatchMouseMotion(new Godot.Vector2(1040, 620), new Godot.Vector2(80, 20));

        await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
        DispatchMouseButton(false, new Godot.Vector2(1040, 620));

        await ToSignal(GetTree().CreateTimer(0.25), SceneTreeTimer.SignalName.Timeout);
        player.StopChart("runtime_autoplay_test");

        Assert(errors.Count == 0, $"runtime autoplay errors: {string.Join("; ", errors)}");
        GD.Print("=== Touch Runtime AutoPlay Test Passed ===");
    }

    private static void DispatchMouseButton(bool pressed, Godot.Vector2 position)
    {
        Input.ParseInputEvent(new InputEventMouseButton
        {
            ButtonIndex = MouseButton.Left,
            Position = position,
            Pressed = pressed
        });
    }

    private static void DispatchMouseMotion(Godot.Vector2 position, Godot.Vector2 relative)
    {
        Input.ParseInputEvent(new InputEventMouseMotion
        {
            Position = position,
            Relative = relative
        });
    }

    private static void TestTouchEvents()
    {
        var driver = new FakeSimulationDriver();
        var collector = CreateCollector(driver);

        driver.SimulateTime = 1f;
        collector.Enable();
        collector._Input(new InputEventScreenTouch
        {
            Index = 2,
            Position = new Godot.Vector2(960, 600),
            Pressed = true
        });

        driver.SimulateTime = 1.5f;
        collector._Input(new InputEventScreenDrag
        {
            Index = 2,
            Position = new Godot.Vector2(1200, 600)
        });

        driver.SimulateTime = 2f;
        collector._Input(new InputEventScreenTouch
        {
            Index = 2,
            Position = new Godot.Vector2(1200, 600),
            Pressed = false
        });

        var fragment = driver.Runtime.Automaton.InputSignals["Touch"][1];
        Assert(Nearly(fragment.StartTime, 1f), "touch start time");
        Assert(Nearly(fragment.EndTime, 2f), "touch end time");
        Assert(fragment.Edges.Count == 3, "touch edge count");
        AssertTouch(fragment.StartValue, false, 8f, -5f, "touch start value");
        AssertTouch(fragment.Edges[0].Value, true, 8f, -5f, "touch begin edge");
        AssertTouch(fragment.Edges[1].Value, true, 10f, -5f, "touch drag edge");
        AssertTouch(fragment.Edges[2].Value, false, 10f, -5f, "touch end edge");
    }

    private static void TestMouseEvents()
    {
        var driver = new FakeSimulationDriver();
        var collector = CreateCollector(driver);
        collector.EnableMouseInput = true;

        driver.SimulateTime = 3f;
        collector.Enable();
        collector._Input(new InputEventMouseButton
        {
            ButtonIndex = MouseButton.Left,
            Position = new Godot.Vector2(480, 300),
            Pressed = true
        });

        driver.SimulateTime = 3.5f;
        collector._Input(new InputEventMouseMotion
        {
            Position = new Godot.Vector2(960, 300)
        });

        driver.SimulateTime = 4f;
        collector._Input(new InputEventMouseButton
        {
            ButtonIndex = MouseButton.Left,
            Position = new Godot.Vector2(960, 300),
            Pressed = false
        });

        var fragment = driver.Runtime.Automaton.InputSignals["Touch"][1];
        Assert(Nearly(fragment.StartTime, 3f), "mouse start time");
        Assert(Nearly(fragment.EndTime, 4f), "mouse end time");
        Assert(fragment.Edges.Count == 3, "mouse edge count");
        AssertTouch(fragment.StartValue, false, 4f, -2.5f, "mouse start value");
        AssertTouch(fragment.Edges[1].Value, true, 8f, -2.5f, "mouse motion edge");
        AssertTouch(fragment.Edges[2].Value, false, 8f, -2.5f, "mouse end edge");
    }

    private static void TestPreloadSignal()
    {
        const string preloadPath = "user://touch_preload_test.json";
        using (var file = FileAccess.Open(preloadPath, FileAccess.ModeFlags.Write))
        {
            file.StoreString(
                "{\"7\":{\"SignalId\":7,\"StartTime\":0,\"EndTime\":0.5,\"StartValue\":{\"isTouching\":false,\"position\":{\"x\":1,\"y\":2}},\"Edges\":[{\"Time\":0.5,\"Value\":{\"isTouching\":true,\"position\":{\"x\":3,\"y\":4}}}]}}");
        }

        var driver = new FakeSimulationDriver();
        var collector = CreateCollector(driver);
        collector.EnablePreloadSignal = true;
        collector.PreloadSignalPath = preloadPath;
        collector.Initialize(driver);

        var fragment = driver.Runtime.Automaton.InputSignals["Touch"][7];
        Assert(Nearly(fragment.StartTime, 0f), "preload start time");
        Assert(Nearly(fragment.EndTime, 0.5f), "preload end time");
        AssertTouch(fragment.StartValue, false, 1f, 2f, "preload start value");
        AssertTouch(fragment.Edges[0].Value, true, 3f, 4f, "preload edge value");

        var errors = 0;
        var badDriver = new FakeSimulationDriver();
        var badCollector = CreateCollector(badDriver);
        badCollector.EnablePreloadSignal = true;
        badCollector.PreloadSignalPath = "res://missing_touch_preload.json";
        badCollector.ErrorReporter = _ => errors++;
        badCollector.Initialize(badDriver);
        Assert(errors == 1, "bad preload path reports an error");
    }

    private static GodotTouchSignalCollector CreateCollector(FakeSimulationDriver driver)
    {
        GamePlayer.viewportSize = new Godot.Vector2(1920, 1200);
        GamePlayer.Instance = null;

        var collector = new GodotTouchSignalCollector
        {
            EnableTouchInput = true,
            EnableMouseInput = false,
            EnableAutoPlay = false,
            EnablePreloadSignal = false
        };
        collector.Initialize(driver);
        return collector;
    }

    private static void AssertTouch(object value, bool isTouching, float x, float y, string context)
    {
        var touch = TouchSignal.FromGorgeObject((Gorge.GorgeLanguage.Objective.GorgeObject)value);
        Assert(touch.isTouching == isTouching, $"{context} isTouching");
        AssertVector(touch.position, x, y, context);
    }

    private static void AssertVector(GorgeVector2 vector, float x, float y, string context)
    {
        Assert(Nearly(vector.x, x), $"{context} x");
        Assert(Nearly(vector.y, y), $"{context} y");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException($"Touch signal collector test failed: {message}");
        }
    }

    private static bool Nearly(float left, float right)
    {
        return System.Math.Abs(left - right) < 0.0001f;
    }

    private sealed class FakeSimulationDriver : ISimulationDriver
    {
        public FakeSimulationDriver()
        {
            Runtime = new GorgeSimulationRuntime();
            Runtime.Automaton.RuntimeInitialize();
        }

        public GorgeSimulationRuntime Runtime { get; }
        public float SimulateTime { get; set; }

        public float GetRealSimulateTime()
        {
            return SimulateTime;
        }
    }
}
