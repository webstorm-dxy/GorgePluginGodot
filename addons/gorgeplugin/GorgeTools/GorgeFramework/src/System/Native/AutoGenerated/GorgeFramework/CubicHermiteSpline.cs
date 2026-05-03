// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419
#pragma warning disable 0105
#pragma warning disable 0109

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedVariable
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable RedundantEmptyObjectOrCollectionInitializer
// ReSharper disable RedundantAssignment
// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable RedundantIfElseBlock
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable ReplaceAutoPropertyWithComputedProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable RedundantUsingDirective
// ReSharper disable RedundantExplicitArrayCreation
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberHidesStaticFromOuterClass
using System;
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;
using Gorge.GorgeLanguage.VirtualMachine;
using Gorge.Native.GorgeFramework;
using Gorge.Native.GorgeFramework;
using Gorge.Native.Gorge;
namespace Gorge.Native.GorgeFramework
{
public partial class CubicHermiteSpline : global::Gorge.Native.GorgeFramework.FunctionCurve
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("CubicHermiteSpline", "GorgeFramework", new GorgeType[]{});
public override GorgeClass SuperClass { get; } = global::Gorge.Native.GorgeFramework.FunctionCurve.Class;
public override GorgeClass LatestNativeClass => this;
public override Injector EmptyInjector() => new SpecificInjector();
public override ClassDeclaration Declaration { get; } = new ClassDeclaration(
type: Type(),
isNative: true,
superClass: global::Gorge.Native.GorgeFramework.FunctionCurve.Class.Declaration,
superInterfaces: new GorgeInterface[]
{
},
fields: new FieldInformation[]
{
new FieldInformation(
id: 0,
name: "startPoint",
type: global::Gorge.Native.GorgeFramework.Vector2.Implementation.Type(),
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 1,
name: "startTangent",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 2,
name: "startWeight",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 1
),
new FieldInformation(
id: 3,
name: "endPoint",
type: global::Gorge.Native.GorgeFramework.Vector2.Implementation.Type(),
annotations: new Annotation[]{},
index: 1
),
new FieldInformation(
id: 4,
name: "endTangent",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 2
),
new FieldInformation(
id: 5,
name: "endWeight",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 3
),
},
methods:
new MethodInformation[]
{
new MethodInformation(
id: 1,
name: "Evaluate",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "x",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
}
,
staticMethods:
new MethodInformation[]
{
}
,
constructors:
new ConstructorInformation[]
{
new ConstructorInformation(
id: 1,
parameters: new ParameterInformation[]
{
},
annotations: new Annotation[]{}
),
}
,
injectorConstructors:
new ConstructorInformation[]
{
}
,
injectorFields: new InjectorFieldInformation[]
{
new InjectorFieldInformation(
id: 0,
name: "startPoint",
type: GorgeType.Injector(global::Gorge.Native.GorgeFramework.Vector2.Implementation.Type()),
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.CubicHermiteSpline.InjectorFieldMetadata_startPoint()
),
new InjectorFieldInformation(
id: 1,
name: "startTangent",
type: GorgeType.Float,
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.CubicHermiteSpline.InjectorFieldMetadata_startTangent()
),
new InjectorFieldInformation(
id: 2,
name: "startWeight",
type: GorgeType.Float,
index: 1,
defaultValueIndex: 1,
metadata: global::Gorge.Native.GorgeFramework.CubicHermiteSpline.InjectorFieldMetadata_startWeight()
),
new InjectorFieldInformation(
id: 3,
name: "endPoint",
type: GorgeType.Injector(global::Gorge.Native.GorgeFramework.Vector2.Implementation.Type()),
index: 1,
defaultValueIndex: 1,
metadata: global::Gorge.Native.GorgeFramework.CubicHermiteSpline.InjectorFieldMetadata_endPoint()
),
new InjectorFieldInformation(
id: 4,
name: "endTangent",
type: GorgeType.Float,
index: 2,
defaultValueIndex: 2,
metadata: global::Gorge.Native.GorgeFramework.CubicHermiteSpline.InjectorFieldMetadata_endTangent()
),
new InjectorFieldInformation(
id: 5,
name: "endWeight",
type: GorgeType.Float,
index: 3,
defaultValueIndex: 3,
metadata: global::Gorge.Native.GorgeFramework.CubicHermiteSpline.InjectorFieldMetadata_endWeight()
),
},
annotations: global::Gorge.Native.GorgeFramework.CubicHermiteSpline.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 4,
boolCount: 0,
stringCount: 0,
objectCount: 2
)
,
methodCount: 2,
methodOverrideId:
new Dictionary<int, int>()
{
{ 0, 1 },
}
,
interfaceMethodImplementationId:
new Dictionary<string, int[]>()
{
}
,
staticMethodCount: 0,
constructorCount: 2,
injectorConstructorCount: 1,
injectorConstructorImplementationId:
new int[]
{
1,
}
,
injectorFieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 4,
boolCount: 0,
stringCount: 0,
objectCount: 2
)
,
injectorFieldDefaultValueIndexCount:
new TypeCount(
intCount: 0,
floatCount: 4,
boolCount: 0,
stringCount: 0,
objectCount: 2
)
,
injectorFieldCount: 6
);
// TODO Injector的调用
public override void InvokeMethod(GorgeObject gorgeObject, int methodId)
{
global::Gorge.Native.GorgeFramework.CubicHermiteSpline CubicHermiteSpline;
if(gorgeObject is global::Gorge.Native.GorgeFramework.CubicHermiteSpline)
{
CubicHermiteSpline = (global::Gorge.Native.GorgeFramework.CubicHermiteSpline) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
CubicHermiteSpline = (global::Gorge.Native.GorgeFramework.CubicHermiteSpline) u.NativeObject;
}
else
{
throw new Exception($"尝试在{gorgeObject}对象上调用{Declaration.Name}类的方法");
}
switch (methodId)
{
case 0:
case 1:
InvokeParameterPool.FloatReturn = CubicHermiteSpline.Evaluate(InvokeParameterPool.Float[0]);
break;
default:
global::Gorge.Native.GorgeFramework.FunctionCurve.Class.InvokeMethod(gorgeObject, methodId);
break;
}
}
public override void InvokeStaticMethod(int methodId)
{
switch (methodId)
{
default:
global::Gorge.Native.GorgeFramework.FunctionCurve.Class.InvokeStaticMethod(methodId);
break;
}
}
protected override GorgeObject DoConstruct(GorgeObject targetObject, int constructorId)
{
var injector = InvokeParameterPool.Injector;
switch (constructorId)
{
case 1:
if (targetObject != null)
{
if (targetObject is CompiledGorgeObject u) // 外部继承本Native类
{
var instance = ConstructInstance(injector);
instance.OuterCompiledObject = u;
u.NativeObject = instance;
return targetObject;
}
else
{
throw new Exception($"类{Declaration.Name}的{constructorId}号构造方法被Native类调用");
}
}
else // 直接从本Native类构造
{
return global::Gorge.Native.GorgeFramework.CubicHermiteSpline.ConstructInstance(injector);
}
break;
default:
throw new Exception($"类{Declaration.Name}无编号为{constructorId}的构造方法");
}
}
public override int GetInjectorIntDefaultValue(int defaultValueIndex)
{
return defaultValueIndex switch
{
_ => base.GetInjectorIntDefaultValue(defaultValueIndex)
};
}
public override float GetInjectorFloatDefaultValue(int defaultValueIndex)
{
return defaultValueIndex switch
{
0 => InjectorFieldDefaultValue_startTangent(),
1 => InjectorFieldDefaultValue_startWeight(),
2 => InjectorFieldDefaultValue_endTangent(),
3 => InjectorFieldDefaultValue_endWeight(),
_ => base.GetInjectorFloatDefaultValue(defaultValueIndex)
};
}
public override bool GetInjectorBoolDefaultValue(int defaultValueIndex)
{
return defaultValueIndex switch
{
_ => base.GetInjectorBoolDefaultValue(defaultValueIndex)
};
}
public override string GetInjectorStringDefaultValue(int defaultValueIndex)
{
return defaultValueIndex switch
{
_ => base.GetInjectorStringDefaultValue(defaultValueIndex)
};
}
public override GorgeObject GetInjectorObjectDefaultValue(int defaultValueIndex)
{
return defaultValueIndex switch
{
0 => InjectorFieldDefaultValue_startPoint(),
1 => InjectorFieldDefaultValue_endPoint(),
_ => base.GetInjectorObjectDefaultValue(defaultValueIndex)
};
}
}
public new class SpecificInjector : global::Gorge.Native.GorgeFramework.FunctionCurve.SpecificInjector
{
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.CubicHermiteSpline.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
1 => ConstructInstance(this),
_ => throw new Exception($"Image类没有编号为{constructorIndex}的构造方法")
};
}
private Tuple<global::Gorge.Native.Gorge.Injector, bool> _startPoint = new(default, true);
public global::Gorge.Native.Gorge.Injector startPoint
{
get => _startPoint.Item1;
set => _startPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(value,false);
}
private Tuple<float, bool> _startTangent = new(default, true);
public float startTangent
{
get => _startTangent.Item1;
set => _startTangent = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _startWeight = new(default, true);
public float startWeight
{
get => _startWeight.Item1;
set => _startWeight = new Tuple<float, bool>(value,false);
}
private Tuple<global::Gorge.Native.Gorge.Injector, bool> _endPoint = new(default, true);
public global::Gorge.Native.Gorge.Injector endPoint
{
get => _endPoint.Item1;
set => _endPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(value,false);
}
private Tuple<float, bool> _endTangent = new(default, true);
public float endTangent
{
get => _endTangent.Item1;
set => _endTangent = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _endWeight = new(default, true);
public float endWeight
{
get => _endWeight.Item1;
set => _endWeight = new Tuple<float, bool>(value,false);
}
public override void SetInjectorInt(int index, int value)
{
switch (index)
{
default:
base.SetInjectorInt(index, value);
break;
}
}
public override void SetInjectorIntDefault(int index)
{
switch (index)
{
default:
base.SetInjectorIntDefault(index);
break;
}
}
public override int GetInjectorInt(int index)
{
return index switch
{
_ => base.GetInjectorInt(index)
};
}
public override bool GetInjectorIntDefault(int index)
{
return index switch
{
_ => base.GetInjectorIntDefault(index)
};
}
public override void SetInjectorFloat(int index, float value)
{
switch (index)
{
case 0:
_startTangent = new Tuple<float, bool>(value, false);
return;
case 1:
_startWeight = new Tuple<float, bool>(value, false);
return;
case 2:
_endTangent = new Tuple<float, bool>(value, false);
return;
case 3:
_endWeight = new Tuple<float, bool>(value, false);
return;
default:
base.SetInjectorFloat(index, value);
break;
}
}
public override void SetInjectorFloatDefault(int index)
{
switch (index)
{
case 0:
_startTangent = new Tuple<float, bool>(default, true);
return;
case 1:
_startWeight = new Tuple<float, bool>(default, true);
return;
case 2:
_endTangent = new Tuple<float, bool>(default, true);
return;
case 3:
_endWeight = new Tuple<float, bool>(default, true);
return;
default:
base.SetInjectorFloatDefault(index);
break;
}
}
public override float GetInjectorFloat(int index)
{
return index switch
{
0 => _startTangent.Item1,
1 => _startWeight.Item1,
2 => _endTangent.Item1,
3 => _endWeight.Item1,
_ => base.GetInjectorFloat(index)
};
}
public override bool GetInjectorFloatDefault(int index)
{
return index switch
{
0 => _startTangent.Item2,
1 => _startWeight.Item2,
2 => _endTangent.Item2,
3 => _endWeight.Item2,
_ => base.GetInjectorFloatDefault(index)
};
}
public override void SetInjectorBool(int index, bool value)
{
switch (index)
{
default:
base.SetInjectorBool(index, value);
break;
}
}
public override void SetInjectorBoolDefault(int index)
{
switch (index)
{
default:
base.SetInjectorBoolDefault(index);
break;
}
}
public override bool GetInjectorBool(int index)
{
return index switch
{
_ => base.GetInjectorBool(index)
};
}
public override bool GetInjectorBoolDefault(int index)
{
return index switch
{
_ => base.GetInjectorBoolDefault(index)
};
}
public override void SetInjectorString(int index, string value)
{
switch (index)
{
default:
base.SetInjectorString(index, value);
break;
}
}
public override void SetInjectorStringDefault(int index)
{
switch (index)
{
default:
base.SetInjectorStringDefault(index);
break;
}
}
public override string GetInjectorString(int index)
{
return index switch
{
_ => base.GetInjectorString(index)
};
}
public override bool GetInjectorStringDefault(int index)
{
return index switch
{
_ => base.GetInjectorStringDefault(index)
};
}
public override void SetInjectorObject(int index, GorgeObject value)
{
switch (index)
{
case 0:
_startPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>((global::Gorge.Native.Gorge.Injector)value, false);
return;
case 1:
_endPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>((global::Gorge.Native.Gorge.Injector)value, false);
return;
default:
base.SetInjectorObject(index, value);
break;
}
}
public override void SetInjectorObjectDefault(int index)
{
switch (index)
{
case 0:
_startPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(default, true);
return;
case 1:
_endPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(default, true);
return;
default:
base.SetInjectorObjectDefault(index);
break;
}
}
public override GorgeObject GetInjectorObject(int index)
{
return index switch
{
0 => _startPoint.Item1,
1 => _endPoint.Item1,
_ => base.GetInjectorObject(index)
};
}
public override bool GetInjectorObjectDefault(int index)
{
return index switch
{
0 => _startPoint.Item2,
1 => _endPoint.Item2,
_ => base.GetInjectorObjectDefault(index)
};
}
public override bool EditableEquals(Injector target)
{
throw new NotImplementedException("暂未实现比较器");
}
public override GorgeObject Clone()
{
var injector = new global::Gorge.Native.GorgeFramework.CubicHermiteSpline.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.CubicHermiteSpline.SpecificInjector toInjector)
{
base.CloneTo(toInjector);
toInjector._startPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(global::Gorge.Native.Gorge.Injector.FromGorgeObject(_startPoint.Item1?.Clone()),_startPoint.Item2);
toInjector._startTangent = _startTangent;
toInjector._startWeight = _startWeight;
toInjector._endPoint = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(global::Gorge.Native.Gorge.Injector.FromGorgeObject(_endPoint.Item1?.Clone()),_endPoint.Item2);
toInjector._endTangent = _endTangent;
toInjector._endWeight = _endWeight;
}
}
/// <summary>
/// 静态单例
/// </summary>
[GorgeNativeClass]
public new static readonly Implementation Class = new();
public new static SpecificInjector EmptyInjector() => new SpecificInjector();
public override GorgeClass GorgeClass { get; } = Class;
public override GorgeObject RealObject => OuterCompiledObject ?? this;
public new GorgeObject OuterCompiledObject;
public new static global::Gorge.Native.GorgeFramework.CubicHermiteSpline FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.CubicHermiteSpline) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.CubicHermiteSpline) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 0号字段
/// 索引为object:0
/// </summary>
public global::Gorge.Native.GorgeFramework.Vector2 startPoint;
/// <summary>
/// 1号字段
/// 索引为float:0
/// </summary>
public float startTangent;
/// <summary>
/// 2号字段
/// 索引为float:1
/// </summary>
public float startWeight;
/// <summary>
/// 3号字段
/// 索引为object:1
/// </summary>
public global::Gorge.Native.GorgeFramework.Vector2 endPoint;
/// <summary>
/// 4号字段
/// 索引为float:2
/// </summary>
public float endTangent;
/// <summary>
/// 5号字段
/// 索引为float:3
/// </summary>
public float endWeight;
/// <summary>
/// 1号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.CubicHermiteSpline ConstructInstance(Injector injector)
{
return new global::Gorge.Native.GorgeFramework.CubicHermiteSpline(injector);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
global::Gorge.Native.Gorge.Injector injector_startPoint;
if (injector.GetInjectorObjectDefault(0))
{
injector_startPoint = InjectorFieldDefaultValue_startPoint();
}
else
{
injector_startPoint = (global::Gorge.Native.Gorge.Injector)injector.GetInjectorObject(0);
}
float injector_startTangent;
if (injector.GetInjectorFloatDefault(0))
{
injector_startTangent = InjectorFieldDefaultValue_startTangent();
}
else
{
injector_startTangent = injector.GetInjectorFloat(0);
}
float injector_startWeight;
if (injector.GetInjectorFloatDefault(1))
{
injector_startWeight = InjectorFieldDefaultValue_startWeight();
}
else
{
injector_startWeight = injector.GetInjectorFloat(1);
}
global::Gorge.Native.Gorge.Injector injector_endPoint;
if (injector.GetInjectorObjectDefault(1))
{
injector_endPoint = InjectorFieldDefaultValue_endPoint();
}
else
{
injector_endPoint = (global::Gorge.Native.Gorge.Injector)injector.GetInjectorObject(1);
}
float injector_endTangent;
if (injector.GetInjectorFloatDefault(2))
{
injector_endTangent = InjectorFieldDefaultValue_endTangent();
}
else
{
injector_endTangent = injector.GetInjectorFloat(2);
}
float injector_endWeight;
if (injector.GetInjectorFloatDefault(3))
{
injector_endWeight = InjectorFieldDefaultValue_endWeight();
}
else
{
injector_endWeight = injector.GetInjectorFloat(3);
}
this.startPoint = InitializeField_startPoint(injector_startPoint);this.startTangent = InitializeField_startTangent(injector_startTangent);this.startWeight = InitializeField_startWeight(injector_startWeight);this.endPoint = InitializeField_endPoint(injector_endPoint);this.endTangent = InitializeField_endTangent(injector_endTangent);this.endWeight = InitializeField_endWeight(injector_endWeight);}
private static partial global::Gorge.Native.GorgeFramework.Vector2 InitializeField_startPoint(global::Gorge.Native.Gorge.Injector startPoint);private static partial float InitializeField_startTangent(float startTangent);private static partial float InitializeField_startWeight(float startWeight);private static partial global::Gorge.Native.GorgeFramework.Vector2 InitializeField_endPoint(global::Gorge.Native.Gorge.Injector endPoint);private static partial float InitializeField_endTangent(float endTangent);private static partial float InitializeField_endWeight(float endWeight);private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startPoint();/// <summary>
/// Injector的startPoint字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial global::Gorge.Native.Gorge.Injector InjectorFieldDefaultValue_startPoint();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startTangent();/// <summary>
/// Injector的startTangent字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_startTangent();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startWeight();/// <summary>
/// Injector的startWeight字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_startWeight();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endPoint();/// <summary>
/// Injector的endPoint字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial global::Gorge.Native.Gorge.Injector InjectorFieldDefaultValue_endPoint();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endTangent();/// <summary>
/// Injector的endTangent字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_endTangent();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endWeight();/// <summary>
/// Injector的endWeight字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_endWeight();public override int GetIntField(int fieldIndex)
{
return fieldIndex switch
{
_ => base.GetIntField(fieldIndex)
};
}
public override void SetIntField(int fieldIndex, int value)
{
switch (fieldIndex)
{
default:
base.SetIntField(fieldIndex,value);
break;
}
}
public override float GetFloatField(int fieldIndex)
{
return fieldIndex switch
{
0 => startTangent,
1 => startWeight,
2 => endTangent,
3 => endWeight,
_ => base.GetFloatField(fieldIndex)
};
}
public override void SetFloatField(int fieldIndex, float value)
{
switch (fieldIndex)
{
case 0:
this.startTangent = value;
return;
case 1:
this.startWeight = value;
return;
case 2:
this.endTangent = value;
return;
case 3:
this.endWeight = value;
return;
default:
base.SetFloatField(fieldIndex,value);
break;
}
}
public override bool GetBoolField(int fieldIndex)
{
return fieldIndex switch
{
_ => base.GetBoolField(fieldIndex)
};
}
public override void SetBoolField(int fieldIndex, bool value)
{
switch (fieldIndex)
{
default:
base.SetBoolField(fieldIndex,value);
break;
}
}
public override string GetStringField(int fieldIndex)
{
return fieldIndex switch
{
_ => base.GetStringField(fieldIndex)
};
}
public override void SetStringField(int fieldIndex, string value)
{
switch (fieldIndex)
{
default:
base.SetStringField(fieldIndex,value);
break;
}
}
public override GorgeObject GetObjectField(int fieldIndex)
{
return fieldIndex switch
{
0 => startPoint,
1 => endPoint,
_ => base.GetObjectField(fieldIndex)
};
}
public override void SetObjectField(int fieldIndex, GorgeObject value)
{
switch (fieldIndex)
{
case 0:
this.startPoint = (global::Gorge.Native.GorgeFramework.Vector2)value;
return;
case 1:
this.endPoint = (global::Gorge.Native.GorgeFramework.Vector2)value;
return;
default:
base.SetObjectField(fieldIndex,value);
break;
}
}
/// <summary>
/// 1号方法
/// </summary>
/// <returns></returns>
public override partial float Evaluate(float x);
private static partial Annotation[] ClassAnnotations();
}
}
