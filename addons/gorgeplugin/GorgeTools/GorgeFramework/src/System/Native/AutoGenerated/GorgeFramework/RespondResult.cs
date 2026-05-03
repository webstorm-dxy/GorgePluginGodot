using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class RespondResult : GorgeEnum
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeEnum]
public static readonly global::Gorge.Native.GorgeFramework.RespondResult Enum = new();
public override GorgeType Type { get; } = GorgeType.Enum("GorgeFramework.RespondResult");
public override bool IsNative => true;
public const int Miss = 0;
public const int Good = 1;
public const int Perfect = 2;
public const int BestPerfect = 3;
public override string[] Values { get; } = 
{
"Miss",
"Good",
"Perfect",
"BestPerfect",
};
public override string[] DisplayNames { get; } = 
{
"Miss",
"Good",
"Perfect",
"BestPerfect",
};
}
}
