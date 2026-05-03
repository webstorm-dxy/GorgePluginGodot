using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class TouchType : GorgeEnum
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeEnum]
public static readonly global::Gorge.Native.GorgeFramework.TouchType Enum = new();
public override GorgeType Type { get; } = GorgeType.Enum("GorgeFramework.TouchType");
public override bool IsNative => true;
public const int Begin = 0;
public const int Keep = 1;
public const int End = 2;
public override string[] Values { get; } = 
{
"Begin",
"Keep",
"End",
};
public override string[] DisplayNames { get; } = 
{
"Begin",
"Keep",
"End",
};
}
}
