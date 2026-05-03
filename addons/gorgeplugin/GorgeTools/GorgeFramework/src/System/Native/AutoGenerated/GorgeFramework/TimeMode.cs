using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class TimeMode : GorgeEnum
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeEnum]
public static readonly global::Gorge.Native.GorgeFramework.TimeMode Enum = new();
public override GorgeType Type { get; } = GorgeType.Enum("GorgeFramework.TimeMode");
public override bool IsNative => true;
public const int CatchBefore = 0;
public const int KeepUntil = 1;
public override string[] Values { get; } = 
{
"CatchBefore",
"KeepUntil",
};
public override string[] DisplayNames { get; } = 
{
"CatchBefore",
"KeepUntil",
};
}
}
