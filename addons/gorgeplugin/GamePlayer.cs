using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Gorge.GorgeFramework;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Chart;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Simulators;

namespace GorgePlugin.addons.gorgeplugin;

public partial class GamePlayer : Node, ISimulationDriver
{
    private const string DefaultNativePackagePath = "res://addons/gorgeplugin/Native.zip";

    [Signal]
    public delegate void PackagesChangedEventHandler();

    [Signal]
    public delegate void ChartPreparedEventHandler();

    [Signal]
    public delegate void ChartStartedEventHandler();

    [Signal]
    public delegate void ChartStoppedEventHandler(string reason);

    [Signal]
    public delegate void PlaybackStateChangedEventHandler(bool isPlaying);

    [Signal]
    public delegate void PlayerErrorEventHandler(string message);

    [Signal]
    public delegate void BytecodeCompiledEventHandler(string cachePath);

    [Signal]
    public delegate void BytecodeCompileFailedEventHandler(string message);

    [Export]
    public Godot.Collections.Array<string> RuntimePackagePaths { get; set; } = new()
    {
        DefaultNativePackagePath
    };

    [Export]
    public Godot.Collections.Array<string> ChartPackagePaths { get; set; } = new();

    [Export]
    public bool AutoStartOnReady { get; set; } = false;

    [Export]
    public float TerminateTime { get; set; } = 135.0f;

    private bool _gorgeAutoPlay = true;

    [Export]
    public bool GorgeAutoPlay
    {
        get => _gorgeAutoPlay;
        set
        {
            _gorgeAutoPlay = value;
            StaticConfig.IsAutoPlay = value;
            if (_touchSignalCollector != null)
                _touchSignalCollector.EnableAutoPlay = value;
        }
    }

    [Export]
    public bool EnableTouchInput { get; set; } = true;

    [Export]
    public bool EnableMouseInput { get; set; } = false;

    [Export]
    public bool EnablePreloadSignal { get; set; } = false;

    [Export]
    public string PreloadSignalPath { get; set; } = "";

    public readonly List<Package> Packages = new();

    public string ChartName;

    public static bool IsPlaying = false;

    public static Node Instance;

    public static Vector2 viewportSize;

    public static float SizeScale = 80.0f;

    private readonly List<Package> _manualPackages = new();
    private bool _runtimeDirty = true;
    private bool _runtimePrepared;
    private Node2D _playerRoot;
    private Camera2D _camera;
    private GodotTouchSignalCollector _touchSignalCollector;
    private double _syncRealTime;
    private float _syncSimulateTime;

    public GorgeSimulationRuntime Runtime => RuntimeStatic.Runtime?.SimulationRuntime;

    public override void _Ready()
    {
        InitializeRenderRoot();
        ApplyStaticConfig();

        if (AutoStartOnReady)
        {
            PlayChart();
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            StopChart("window_close");
        }

        base._Notification(what);
    }

    public override void _Process(double delta)
    {
        if (IsPlaying && RuntimeStatic.Runtime?.SimulationRuntime?.Simulation?.SimulationMachine != null)
        {
            RuntimeStatic.Runtime.SimulationRuntime.Simulation.SimulationMachine.Drive((float)delta);
        }

        base._Process(delta);
    }

    public bool AddPackage(Package package)
    {
        if (package == null)
        {
            ReportError("Cannot add a null package.");
            return false;
        }

        StopForPackageMutation();
        _manualPackages.Add(package);
        Packages.Add(package);
        MarkPackagesChanged();
        return true;
    }

    public bool AddPackagePath(string path, bool isChart)
    {
        return isChart ? AddChartPackagePath(path) : AddRuntimePackagePath(path);
    }

    public bool AddRuntimePackagePath(string path)
    {
        if (!TryValidatePackagePath(path))
        {
            return false;
        }

        StopForPackageMutation();
        AddPathIfMissing(RuntimePackagePaths, path);
        MarkPackagesChanged();
        return true;
    }

    public bool AddChartPackagePath(string path)
    {
        if (!TryValidatePackagePath(path))
        {
            return false;
        }

        StopForPackageMutation();
        AddPathIfMissing(ChartPackagePaths, path);
        MarkPackagesChanged();
        return true;
    }

    public void ClearPackages()
    {
        StopForPackageMutation();
        RuntimePackagePaths.Clear();
        ChartPackagePaths.Clear();
        _manualPackages.Clear();
        Packages.Clear();
        MarkPackagesChanged();
    }

    public bool PrepareRuntime()
    {
        try
        {
            ApplyStaticConfig();
            DisposeRuntime();
            Packages.Clear();

            var runtimePackageCount = 0;
            var chartPackageCount = 0;

            foreach (var path in RuntimePackagePaths)
            {
                Packages.Add(LoadPackageFromGodotPath(path, false));
                runtimePackageCount++;
            }

            foreach (var path in ChartPackagePaths)
            {
                Packages.Add(LoadPackageFromGodotPath(path, true));
                chartPackageCount++;
            }

            foreach (var package in _manualPackages)
            {
                Packages.Add(package);
                if (IsChartPackage(package))
                {
                    chartPackageCount++;
                }
                else
                {
                    runtimePackageCount++;
                }
            }

            if (runtimePackageCount == 0)
            {
                throw new InvalidOperationException("At least one runtime package is required.");
            }

            if (chartPackageCount == 0)
            {
                throw new InvalidOperationException("At least one chart package is required.");
            }

            RuntimeStatic.Runtime = new RuntimeManager();
            RuntimeStatic.Runtime.CreateLanguageRuntime(Packages);
            RuntimeStatic.Runtime.ExtractSimulationResources();
            RuntimeStatic.Runtime.CreateSimulationRuntime(OnRuntimeTerminated);
            RuntimeStatic.Runtime.LoadScore();

            _runtimeDirty = false;
            _runtimePrepared = true;
            EmitSignal(SignalName.ChartPrepared);
            return true;
        }
        catch (Exception exception)
        {
            _runtimePrepared = false;
            IsPlaying = false;
            ReportError($"PrepareRuntime failed: {exception.Message}");
            return false;
        }
    }

    public bool PlayChart()
    {
        try
        {
            if (_runtimeDirty || !_runtimePrepared || RuntimeStatic.Runtime == null)
            {
                if (!PrepareRuntime())
                {
                    return false;
                }
            }

            if (RuntimeStatic.Runtime.State == RuntimeState.Simulating)
            {
                StopChart("restart");
            }

            if (RuntimeStatic.Runtime.State == RuntimeState.SimulationInitialized)
            {
                RuntimeStatic.Runtime.LoadScore();
            }

            RuntimeStatic.Runtime.StartSimulation();
            SetPlaying(true);
            ConfigureAndEnableTouchSignalCollector();
            EmitSignal(SignalName.ChartStarted);
            return true;
        }
        catch (Exception exception)
        {
            SetPlaying(false);
            ReportError($"PlayChart failed: {exception.Message}");
            return false;
        }
    }

    public bool StopChart(string reason = "stopped")
    {
        try
        {
            var wasPlaying = IsPlaying;

            if (wasPlaying)
            {
                DisableTouchSignalCollector();
            }

            if (RuntimeStatic.Runtime?.SimulationRuntime?.IsSimulating == true)
            {
                RuntimeStatic.Runtime.StopSimulation();
            }

            SetPlaying(false);

            if (wasPlaying)
            {
                EmitSignal(SignalName.ChartStopped, reason);
            }

            return true;
        }
        catch (Exception exception)
        {
            SetPlaying(false);
            ReportError($"StopChart failed: {exception.Message}");
            return false;
        }
    }

    public bool RestartChart()
    {
        StopChart("restart");
        return PlayChart();
    }

    public void RequestPlay()
    {
        PlayChart();
    }

    public void RequestStop()
    {
        StopChart();
    }

    /// <summary>
    /// Compile currently loaded .g source files to .gorge bytecode and save to cache.
    /// The bytecode can be reused on next startup without recompilation.
    /// </summary>
    public bool CompileToBytecode()
    {
        try
        {
            ApplyStaticConfig();

            // Collect packages from configured paths
            var packages = new List<Package>();
            foreach (var path in RuntimePackagePaths)
                packages.Add(LoadPackageFromGodotPath(path, false));
            foreach (var path in ChartPackagePaths)
                packages.Add(LoadPackageFromGodotPath(path, true));

            var cachePath = RuntimeManager.CompilePackagesToCache(packages);
            if (cachePath != null)
            {
                GD.Print($"Gorge bytecode compiled successfully: {cachePath}");
                EmitSignal(SignalName.BytecodeCompiled, cachePath);
                return true;
            }
            else
            {
                var msg = "Bytecode compilation failed: cache file was not written.";
                GD.PushError(msg);
                EmitSignal(SignalName.BytecodeCompileFailed, msg);
                return false;
            }
        }
        catch (Exception ex)
        {
            var msg = $"Bytecode compilation failed: {ex.Message}";
            GD.PushError(msg);
            EmitSignal(SignalName.BytecodeCompileFailed, msg);
            return false;
        }
    }

    public bool compile_to_bytecode()
    {
        return CompileToBytecode();
    }

    public bool add_package_path(string path, bool isChart)
    {
        return AddPackagePath(path, isChart);
    }

    public bool add_runtime_package_path(string path)
    {
        return AddRuntimePackagePath(path);
    }

    public bool add_chart_package_path(string path)
    {
        return AddChartPackagePath(path);
    }

    public void clear_packages()
    {
        ClearPackages();
    }

    public bool prepare_runtime()
    {
        return PrepareRuntime();
    }

    public bool play_chart()
    {
        return PlayChart();
    }

    public bool stop_chart()
    {
        return StopChart();
    }

    public bool restart_chart()
    {
        return RestartChart();
    }

    public void request_play()
    {
        RequestPlay();
    }

    public void request_stop()
    {
        RequestStop();
    }

    private void InitializeRenderRoot()
    {
        Base.Instance = new GodotBase();

        if (_playerRoot == null)
        {
            _playerRoot = new Node2D
            {
                Position = Vector2.Zero,
                Visible = true
            };
            AddChild(_playerRoot);
        }

        if (_camera == null)
        {
            _camera = new Camera2D
            {
                Position = Vector2.Zero
            };
            _playerRoot.AddChild(_camera);
        }

        viewportSize = GetViewport().GetVisibleRect().Size;
        Instance = _camera;

        if (_touchSignalCollector == null)
        {
            _touchSignalCollector = new GodotTouchSignalCollector
            {
                Name = "GodotTouchSignalCollector"
            };
            AddChild(_touchSignalCollector);
        }
    }

    private void ApplyStaticConfig()
    {
        StaticConfig.TerminateTime = TerminateTime;
        StaticConfig.IsAutoPlay = GorgeAutoPlay;
        StaticConfig.CacheDirectory = System.IO.Path.Combine(OS.GetUserDataDir(), "gorge_cache");
    }

    private void ConfigureAndEnableTouchSignalCollector()
    {
        if (_touchSignalCollector == null)
        {
            InitializeRenderRoot();
        }

        _touchSignalCollector.EnableTouchInput = EnableTouchInput;
        _touchSignalCollector.EnableMouseInput = EnableMouseInput;
        _touchSignalCollector.EnableAutoPlay = GorgeAutoPlay;
        _touchSignalCollector.EnablePreloadSignal = EnablePreloadSignal;
        _touchSignalCollector.PreloadSignalPath = PreloadSignalPath;
        _touchSignalCollector.ErrorReporter = ReportError;
        _touchSignalCollector.Initialize(this);
        _touchSignalCollector.Enable();
    }

    private void DisableTouchSignalCollector()
    {
        _touchSignalCollector?.Disable();
    }

    private void StopForPackageMutation()
    {
        if (IsPlaying || RuntimeStatic.Runtime?.SimulationRuntime?.IsSimulating == true)
        {
            StopChart("packages_changed");
        }
    }

    private void MarkPackagesChanged()
    {
        _runtimeDirty = true;
        _runtimePrepared = false;
        EmitSignal(SignalName.PackagesChanged);
    }

    private void DisposeRuntime()
    {
        if (RuntimeStatic.Runtime == null)
        {
            return;
        }

        if (RuntimeStatic.Runtime.SimulationRuntime?.IsSimulating == true)
        {
            DisableTouchSignalCollector();
            RuntimeStatic.Runtime.StopSimulation();
        }

        RuntimeStatic.Runtime.DestructSimulationRuntime();
        RuntimeStatic.Runtime = null;
        SetPlaying(false);
    }

    private Package LoadPackageFromGodotPath(string path, bool isChart)
    {
        if (!TryValidatePackagePath(path))
        {
            throw new InvalidOperationException("Package path is empty.");
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new InvalidOperationException($"Cannot open package file: {path}");
        }

        return Package.LoadZipPackage(file.GetBuffer((long)file.GetLength()), isChart);
    }

    private bool TryValidatePackagePath(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            return true;
        }

        ReportError("Package path cannot be empty.");
        return false;
    }

    private static bool IsChartPackage(Package package)
    {
        return package.SourceCodeFiles.Any(source => source.IsChartSourceCode) ||
               package.AssetFiles.Any(asset => asset.IsChartAsset);
    }

    private static void AddPathIfMissing(Godot.Collections.Array<string> paths, string path)
    {
        if (!paths.Contains(path))
        {
            paths.Add(path);
        }
    }

    private void SetPlaying(bool value)
    {
        if (IsPlaying == value)
        {
            return;
        }

        if (value)
        {
            _syncRealTime = GetRealTimeSeconds();
            _syncSimulateTime = RuntimeStatic.Runtime?.SimulationRuntime?.Simulation?.SimulationMachine?.SimulateTime ?? 0f;
        }

        IsPlaying = value;
        EmitSignal(SignalName.PlaybackStateChanged, IsPlaying);
    }

    private void OnRuntimeTerminated()
    {
        if (IsPlaying)
        {
            DisableTouchSignalCollector();
        }

        SetPlaying(false);
        EmitSignal(SignalName.ChartStopped, "terminated");
    }

    public float GetRealSimulateTime()
    {
        if (!IsPlaying || RuntimeStatic.Runtime?.SimulationRuntime?.Simulation?.SimulationMachine == null)
        {
            return -1f;
        }

        var realTime = (float)(GetRealTimeSeconds() - _syncRealTime + _syncSimulateTime);
        var simTime = RuntimeStatic.Runtime.SimulationRuntime.Simulation.SimulationMachine.SimulateTime;
        return Math.Max(realTime, simTime);
    }

    private static double GetRealTimeSeconds()
    {
        return Godot.Time.GetTicksUsec() / 1_000_000.0;
    }

    private void ReportError(string message)
    {
        GD.PushError(message);
        EmitSignal(SignalName.PlayerError, message);
    }
}
