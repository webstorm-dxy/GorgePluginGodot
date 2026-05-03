using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Gorge.GorgeFramework;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Chart;
using Gorge.GorgeFramework.Runtime;

namespace GorgePlugin.addons.gorgeplugin;

public partial class GamePlayer : Node
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

    [Export]
    public bool GorgeAutoPlay { get; set; } = true;

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
    }

    private void ApplyStaticConfig()
    {
        StaticConfig.TerminateTime = TerminateTime;
        StaticConfig.IsAutoPlay = GorgeAutoPlay;
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

        IsPlaying = value;
        EmitSignal(SignalName.PlaybackStateChanged, IsPlaying);
    }

    private void OnRuntimeTerminated()
    {
        SetPlaying(false);
        EmitSignal(SignalName.ChartStopped, "terminated");
    }

    private void ReportError(string message)
    {
        GD.PushError(message);
        EmitSignal(SignalName.PlayerError, message);
    }
}
