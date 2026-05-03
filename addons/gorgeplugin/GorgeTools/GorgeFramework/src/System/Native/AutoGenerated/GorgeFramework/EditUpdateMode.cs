using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class EditUpdateMode : GorgeEnum
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeEnum]
public static readonly global::Gorge.Native.GorgeFramework.EditUpdateMode Enum = new();
public override GorgeType Type { get; } = GorgeType.Enum("GorgeFramework.EditUpdateMode");
public override bool IsNative => true;
public const int Static = 0;
public const int ReInject = 1;
public const int ReGenerate = 2;
public const int RePlay = 3;
public override string[] Values { get; } = 
{
"Static",
"ReInject",
"ReGenerate",
"RePlay",
};
public override string[] DisplayNames { get; } = 
{
"Static",
"ReInject",
"ReGenerate",
"RePlay",
};
}
}
