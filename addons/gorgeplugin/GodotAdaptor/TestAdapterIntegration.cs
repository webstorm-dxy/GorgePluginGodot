using Godot;
using Gorge.GorgeFramework.Adaptor;
using Gorge.Native.GorgeFramework;
using Vector3 = Gorge.Native.GorgeFramework.Vector3;

public partial class TestAdapterIntegration : Godot.Node
{
    public override void _Ready()
    {
        // 测试基础适配器
        TestBaseAdapter();
        
        // 测试精灵适配器
        TestSpriteAdapter();
        
        // 测试音频适配器
        TestAudioAdapter();
    }

    private void TestBaseAdapter()
    {
        GD.Print("=== Testing Base Adapter ===");
        GD.Print($"Persistent path: {Base.Instance.PersistentPath()}");
    }

    private void TestSpriteAdapter()
    {
        GD.Print("=== Testing Sprite Adapter ===");
        
        // 创建精灵
        var sprite = Base.Instance.CreateSprite() as GodotSprite;
        sprite.SetPosition(new Vector3(100, 100, 0));
        GetTree().CurrentScene.AddChild(sprite);
        GD.Print("Sprite created and positioned at (100, 100)");
    }

    private void TestAudioAdapter()
    {
        GD.Print("=== Testing Audio Adapter ===");
        
        // 创建音频播放器
        var player = Base.Instance.CreateAudioPlayer();
        GD.Print("Audio player created");
        
        // 测试音频加载和播放(需要实际音频文件)
        // var audio = Base.Instance.CreateAudio("res://test.mp3", File.ReadAllBytes("res://test.mp3"));
        // player.SetAudio(audio);
        // player.Play();
    }
}