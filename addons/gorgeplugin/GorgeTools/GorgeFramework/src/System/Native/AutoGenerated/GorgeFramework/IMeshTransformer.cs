// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public class IMeshTransformer : GorgeInterface
{
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeInterface]
public static readonly global::Gorge.Native.GorgeFramework.IMeshTransformer Interface = new();
public override GorgeType Type { get; } = GorgeType.Interface("IMeshTransformer", "GorgeFramework");
public override bool IsNative { get; } = true;
public IMeshTransformer() : base(
new MethodInformation[]
{
new MethodInformation(
id: 0,
name: "Transform",
returnType: global::Gorge.Native.GorgeFramework.Vector3.Implementation.Type(),
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "vertex",
type: global::Gorge.Native.GorgeFramework.Vector3.Implementation.Type(),
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
