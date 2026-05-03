#nullable enable
using System;
using Godot;
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;
using GorgePlugin.addons.gorgeplugin;
using Vector2 = Gorge.Native.GorgeFramework.Vector2;
using Vector3 = Gorge.Native.GorgeFramework.Vector3;

namespace Gorge.GorgeFramework.Adaptor;

public class GodotGraph : Graph
{
    private readonly Texture2D _graph;

    public GodotGraph(Texture2D graph)
    {
        _graph = graph;

        width = _graph.GetWidth();
        height = _graph.GetHeight();
    }

    public Texture2D GetTexture()
    {
        return _graph;
    }
}

public static class GodotGraphExtension
{
    public static Graph ToGraph(this Texture2D texture)
    {
        return new GodotGraph(texture);
    }

    public static Texture2D ToTexture(this Graph graph)
    {
        if (graph is not GodotGraph godotGraph) throw new Exception("Godot底座无法识别和使用非GodotGraph类型的图像资源");

        return godotGraph.GetTexture();
    }
}

public static class GodotVectorExtenstion
{
    public static Godot.Vector2 CoverGorgeCoodinateToGodot(this Vector2 vector2)
    {
        var scaleX = GamePlayer.viewportSize.X / 16f;
        var scaleY = GamePlayer.viewportSize.Y / 10f;
        var x = vector2.x * scaleX;
        var y = -vector2.y * scaleY;
        return new Godot.Vector2(x, y);
    }

    public static Godot.Vector2 CoverGorgeCoodinateToGodot(this Vector3 vec)
    {
        var scaleX = GamePlayer.viewportSize.X / 16f;
        var scaleY = GamePlayer.viewportSize.Y / 10f;
        var x = vec.x * scaleX;
        var y = -vec.y * scaleY;
        return new Godot.Vector2(x, y);
    }

    public static Godot.Vector2 TransformCoordinateSystemToGodot(this Godot.Vector2 vec)
    {
        var scaleX = GamePlayer.viewportSize.X / 16f;
        var scaleY = GamePlayer.viewportSize.Y / 10f;
        var x = vec.X * scaleX;
        var y = vec.Y * scaleY;
        return new Godot.Vector2(x, y);
    }

    public static Godot.Vector2 CoverGorgeSizeToGodot(this Vector2 vector2)
    {
        var scaleX = GamePlayer.viewportSize.X / 16f;
        var scaleY = GamePlayer.viewportSize.Y / 10f;
        return new Godot.Vector2(vector2.x * scaleX, vector2.y * scaleY);
    }

    public static Godot.Vector2 CoverGorgeSizeToGodot(this Vector3 vec)
    {
        var scaleX = GamePlayer.viewportSize.X / 16f;
        var scaleY = GamePlayer.viewportSize.Y / 10f;
        return new Godot.Vector2(vec.x * scaleX, vec.y * scaleY);
    }

    public static Godot.Vector2 ToGodotVector2(this Vector2 vec)
    {
        return new Godot.Vector2(vec.x, vec.y);
    }

    public static Godot.Vector2 ToGodotVector2(this Vector3 vec)
    {
        return new Godot.Vector2(vec.x, vec.y);
    }
}

public partial class GodotSprite : Sprite2D, ISprite
{
    public void SetPosition(Vector3 position)
    {
        // Position = ConvertGorgeToGodot(new Godot.Vector2(position.x*10, position.y*10));
        Position = position.CoverGorgeCoodinateToGodot();
        ZIndex = (int)position.z;
    }

    public void SetRotation(Vector3 rotation)
    {
        RotationDegrees = -rotation.z;
    }

    public void SetScale(Vector3 scale)
    {
        Scale = ConvertGorgeToGodot(new Godot.Vector2(scale.x, scale.y)); //鬼知道为什么可以跑，但就这么写没有Bug
        // Scale = scale.ToGodotVector2()/80; //这么写反而有Bug
    }

    public void Destroy()
    {
        QueueFree();
    }

    public void SetGraph(Graph? graph)
    {
        if (graph == null) return;

        Texture = Graph.FromGorgeObject(graph).ToTexture();
    }

    public void SetColor(ColorArgb color)
    {
        Modulate = new Color(
            color.r,
            color.g,
            color.b,
            color.a
        );
    }

    private Godot.Vector2 ConvertGorgeToGodot(Godot.Vector2 gorgeScale)
    {
        var x = 256f;
        var y = 160f;
        var scaleX = gorgeScale.X * (GamePlayer.viewportSize.X / x);
        var scaleY = gorgeScale.Y * (GamePlayer.viewportSize.Y / y);

        return new Godot.Vector2(scaleX, scaleY); //TODO:变换的代码，这里还没有写完，有时间再写，但已经找到哪里有问题了

        // return new Godot.Vector2(mappedX, mappedY);
    }
}

public partial class GodotNineSliceSprite : Godot.Node, INineSliceSprite
{
    private Godot.Node _rustNineSlice;
    private bool _graphSet;

    public override void _Ready()
    {
        if (_rustNineSlice == null)
        {
            try
            {
                _rustNineSlice = ClassDB.Instantiate("NineSliceSprite2D").As<Godot.Node>();
                if (_rustNineSlice != null)
                {
                    _rustNineSlice.Set("centered", true);
                    _rustNineSlice.Set("draw_center", true);
                    AddChild(_rustNineSlice);
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"NineSliceSprite2D GDExtension not loaded: {e.Message}");
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (_rustNineSlice is Node2D node2d)
        {
            node2d.Position = position.CoverGorgeCoodinateToGodot();
            node2d.ZIndex = (int)position.z;
        }
    }

    public void SetRotation(Vector3 rotation)
    {
        if (_rustNineSlice is Node2D node2d)
        {
            node2d.RotationDegrees = -rotation.z;
        }
    }

    public void SetScale(Vector3 scale)
    {
        if (_rustNineSlice == null || !_graphSet) return;

        _rustNineSlice.Set("size", scale.CoverGorgeSizeToGodot());
    }

    public void Destroy()
    {
        QueueFree();
    }

    public void SetGraph(Graph graph, Vector2 baseSize,
        Vector2 sliceLeftTop, Vector2 sliceRightBottom)
    {
        var texture = Graph.FromGorgeObject(graph).ToTexture();
        if (texture == null) return;

        // Ensure Rust node exists (may be called before _Ready)
        EnsureRustNode();

        if (_rustNineSlice != null)
        {
            _rustNineSlice.Set("texture", texture);
            _rustNineSlice.Set("base_size", baseSize.CoverGorgeSizeToGodot());
            _rustNineSlice.Set("patch_margin_left", (int)sliceLeftTop.x);
            _rustNineSlice.Set("patch_margin_top", (int)sliceLeftTop.y);
            _rustNineSlice.Set("patch_margin_right", (int)sliceRightBottom.x);
            _rustNineSlice.Set("patch_margin_bottom", (int)sliceRightBottom.y);
            _graphSet = true;
        }
    }

    public void SetColor(ColorArgb color)
    {
        if (_rustNineSlice != null)
        {
            _rustNineSlice.Set("modulate", new Color(color.r, color.g, color.b, color.a));
        }
    }

    public void SetHsl(Vector3 hsl)
    {
        // HSL adjustment not directly supported
    }

    private void EnsureRustNode()
    {
        if (_rustNineSlice != null) return;

        try
        {
            _rustNineSlice = ClassDB.Instantiate("NineSliceSprite2D").As<Godot.Node>();
            if (_rustNineSlice != null)
            {
                _rustNineSlice.Set("centered", true);
                _rustNineSlice.Set("draw_center", true);
                AddChild(_rustNineSlice);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"NineSliceSprite2D GDExtension not loaded: {e.Message}");
        }
    }
}

public partial class GodotCurveSprite : Line2D, ICurveSprite
{
    // 注意：由于ObjectArray类型不可用，此处使用object作为替代
    // 实际使用时需要确保传入对象包含可枚举的Vector3数组
    public void SetPosition(Vector3 position)
    {
        Position = position.CoverGorgeCoodinateToGodot();
        ZIndex = (int)position.z;
    }

    public void SetRotation(Vector3 rotation)
    {
        RotationDegrees = -rotation.z;
    }

    public void SetScale(Vector3 scale)
    {
        Scale = new Godot.Vector2(scale.x, scale.y);
    }

    public void Destroy()
    {
        Free();
    }

    public void SetLine(ObjectArray? pointArray)
    {
        try
        {
            ClearPoints();

            if (pointArray == null || pointArray.length == 0) return;

            for (var i = 0; i < pointArray.length; i++)
            {
                var point = Vector2.FromGorgeObject(pointArray.Get(i));

                AddPoint(point.CoverGorgeCoodinateToGodot());
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to set curve points: {ex.Message}");
        }
    }

    public void SetColor(ColorArgb color)
    {
        DefaultColor = new Color(
            color.r,
            color.g,
            color.b,
            color.a
        );
    }
}
