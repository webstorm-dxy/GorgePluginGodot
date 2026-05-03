using System;
using System.Text;
using Godot;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.Native.GorgeFramework;
using GorgePlugin.addons.gorgeplugin;
using Node = Godot.Node;
using Vector3 = Gorge.Native.GorgeFramework.Vector3;


namespace Gorge.GorgeFramework.Adaptor;

public static class Vector3Extensions
{
    public static Vector3 Zero => new(0, 0, 0);

    public static Godot.Vector3 ToGodotVector3(this Vector3 vec)
    {
        return new Godot.Vector3(vec.x, vec.y, vec.z);
    }

    public static Vector3 ToGorgeVector3(this Godot.Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Z);
    }
}

public partial class GodotBase : Node, IGorgeFrameworkBase
{
    public void Log(string msg)
    {
        GD.Print(msg);
    }

    public void Warning(string msg)
    {
        GD.PushWarning(msg);
    }

    public string PersistentPath()
    {
        return OS.GetUserDataDir();
    }

    public ISprite CreateSprite()
    {
        var sprite = new GodotSprite();
        GamePlayer.Instance.AddChild(sprite);
        return sprite;
    }

    public INineSliceSprite CreateNineSliceSprite()
    {
        var sprite = new GodotNineSliceSprite();
        GamePlayer.Instance.AddChild(sprite);
        return sprite;
    }


    public ICurveSprite CreateCurveSprite()
    {
        var sprite = new GodotCurveSprite();

        GamePlayer.Instance.AddChild(sprite);
        return sprite;
    }


    public Graph CreateGraph(string assetFilePath, byte[] data)
    {
        var image = new Image();
        image.LoadPngFromBuffer(data);
        Texture2D texture = ImageTexture.CreateFromImage(image);
        return new GodotGraph(texture);
    }

    public Audio CreateAudio(string assetFilePath, byte[] data)
    {
        // 检查音频有效性
        if (data.Length < 44 || !Encoding.ASCII.GetString(data, 0, 4).Equals("RIFF"))
        {
            GD.PrintErr("Invalid WAV header");
            return null;
        }

        var audioStream = AudioStreamWav.LoadFromBuffer(data);
        return new GodotAudio(audioStream);
    }

    public IAudioEffectPlayer CreateAudioEffectPlayer(Audio audioEffect)
    {
        var player = new GodotAudioEffectPlayer(audioEffect);
        return player;
    }

    public IAudioPlayer CreateAudioPlayer()
    {
        return new GodotAudioPlayer();
    }

    public GorgeSimulationRuntime CreateSimulationRuntime(Action onTerminate = null)
    {
        return new GorgeSimulationRuntime(onTerminate);
    }

    public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
    {
        return screenPoint;
    }

    public override void _Ready()
    {
        Base.Instance = this;
    }
}