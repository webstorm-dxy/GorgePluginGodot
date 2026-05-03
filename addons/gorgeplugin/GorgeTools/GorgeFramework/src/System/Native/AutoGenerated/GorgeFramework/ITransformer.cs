// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class ITransformer : GorgeInterface
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeInterface]
public static readonly global::Gorge.Native.GorgeFramework.ITransformer Interface = new();
public override GorgeType Type { get; } = GorgeType.Interface("ITransformer", "GorgeFramework");
public override bool IsNative { get; } = true;
public ITransformer() : base(
new MethodInformation[]
{
new MethodInformation(
id: 0,
name: "Transform",
returnType: global::Gorge.Native.Gorge.ObjectArray.Implementation.Type(),
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "now",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
}
)
{
}
}
}
