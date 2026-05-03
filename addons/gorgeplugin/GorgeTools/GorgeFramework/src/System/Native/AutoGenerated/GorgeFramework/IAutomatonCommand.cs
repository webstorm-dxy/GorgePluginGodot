// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class IAutomatonCommand : GorgeInterface
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeInterface]
public static readonly global::Gorge.Native.GorgeFramework.IAutomatonCommand Interface = new();
public override GorgeType Type { get; } = GorgeType.Interface("IAutomatonCommand", "GorgeFramework");
public override bool IsNative { get; } = true;
public IAutomatonCommand() : base(
new MethodInformation[]
{
}
)
{
}
}
}
