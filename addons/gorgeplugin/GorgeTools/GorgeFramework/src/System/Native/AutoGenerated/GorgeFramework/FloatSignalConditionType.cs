using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class FloatSignalConditionType : GorgeEnum
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeEnum]
public static readonly global::Gorge.Native.GorgeFramework.FloatSignalConditionType Enum = new();
public override GorgeType Type { get; } = GorgeType.Enum("GorgeFramework.FloatSignalConditionType");
public override bool IsNative => true;
public const int Keep = 0;
public const int In = 1;
public const int Out = 2;
public override string[] Values { get; } = 
{
"Keep",
"In",
"Out",
};
public override string[] DisplayNames { get; } = 
{
"Keep",
"In",
"Out",
};
}
}
