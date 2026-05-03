using System;
using Godot;
using Gorge.Native.GorgeFramework;
using GorgePlugin.addons.gorgeplugin;

namespace Gorge.GorgeFramework.Adaptor;

public class GodotAudioPlayer : IAudioPlayer
{
    private AudioStream _audioStream;
    private Audio _currentAudio;
    private readonly AudioStreamPlayer _player;

    public GodotAudioPlayer()
    {
        _player = new AudioStreamPlayer();

        // 添加到场景树以便播放
        var sceneTree = Engine.GetMainLoop() as SceneTree;
        sceneTree?.CurrentScene?.CallDeferred("add_child", _player);
    }

    public void SetAudio(Audio audio)
    {
        _currentAudio = audio;
        var stream = audio.ToAudioClip();
        _player.Stream = stream;
    }

    public void Play()
    {
        GD.Print("Audio is palied");
        _player.Play();
    }

    public void Stop()
    {
        GD.Print("Audio is palied");
        _player.Stop();
    }

    public float AudioLength()
    {
        return (float)(_player.Stream?.GetLength() ?? 0.0);
    }

    public bool IsPlaying()
    {
        return _player.Playing;
    }

    public void SetTime(float time)
    {
        if (_player.Stream != null) _player.Seek(time);
    }

    public void Destruct()
    {
        _player.QueueFree();
    }
}

public class GodotAudio : Audio
{
    private readonly AudioStream _audio;

    public GodotAudio(AudioStream audioStream)
    {
        _audio = audioStream;
    }

    public AudioStream GetAudioClip()
    {
        return _audio;
    }
}

public static class GodotAudioExtension
{
    public static Audio ToAudio(this AudioStream audioStream)
    {
        return new GodotAudio(audioStream);
    }

    public static AudioStream ToAudioClip(this Audio audio)
    {
        if (audio is not GodotAudio godotAudio) throw new Exception("Godot 底座无法识别和使用非GodotAudio类型的音频资源");

        return godotAudio.GetAudioClip();
    }
}

public class GodotAudioEffectPlayer : IAudioEffectPlayer
{
    private readonly AudioStreamPlayer _effectPlayer;

    public GodotAudioEffectPlayer(Audio audioEffect)
    {
        _effectPlayer = new AudioStreamPlayer();
        // 通过反射获取AudioStream
        // var streamProp = audioEffect.GetType().GetProperty("Stream");
        // var stream = (AudioStream)streamProp?.GetValue(audioEffect);
        var stream = audioEffect.ToAudioClip();
        _effectPlayer.Stream = stream;

        // 添加到场景树以便播放
        var sceneTree = GamePlayer.Instance;
        sceneTree?.CallDeferred("add_child", _effectPlayer);
    }

    public void Play()
    {
        _effectPlayer.Play();
    }

    public void Destruct()
    {
        _effectPlayer.QueueFree();
    }
}