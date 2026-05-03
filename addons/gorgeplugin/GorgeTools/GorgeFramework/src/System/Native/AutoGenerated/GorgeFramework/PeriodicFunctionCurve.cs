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
public partial class PeriodicFunctionCurve : global::Gorge.Native.GorgeFramework.FunctionCurve
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("PeriodicFunctionCurve", "GorgeFramework", new GorgeType[]{});
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
name: "functionCurve",
type: global::Gorge.Native.GorgeFramework.FunctionCurve.Implementation.Type(),
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 1,
name: "startX",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 2,
name: "endX",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 1
),
new FieldInformation(
id: 3,
name: "leftClosed",
type: GorgeType.Bool,
annotations: new Annotation[]{},
index: 0
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
name: "functionCurve",
type: GorgeType.Injector(global::Gorge.Native.GorgeFramework.FunctionCurve.Implementation.Type()),
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.InjectorFieldMetadata_functionCurve()
),
new InjectorFieldInformation(
id: 1,
name: "startX",
type: GorgeType.Float,
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.InjectorFieldMetadata_startX()
),
new InjectorFieldInformation(
id: 2,
name: "endX",
type: GorgeType.Float,
index: 1,
defaultValueIndex: 1,
metadata: global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.InjectorFieldMetadata_endX()
),
new InjectorFieldInformation(
id: 3,
name: "leftClosed",
type: GorgeType.Bool,
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.InjectorFieldMetadata_leftClosed()
),
},
annotations: global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 2,
boolCount: 1,
stringCount: 0,
objectCount: 1
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
floatCount: 2,
boolCount: 1,
stringCount: 0,
objectCount: 1
)
,
injectorFieldDefaultValueIndexCount:
new TypeCount(
intCount: 0,
floatCount: 2,
boolCount: 1,
stringCount: 0,
objectCount: 1
)
,
injectorFieldCount: 4
);
// TODO Injector的调用
public override void InvokeMethod(GorgeObject gorgeObject, int methodId)
{
global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve PeriodicFunctionCurve;
if(gorgeObject is global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve)
{
PeriodicFunctionCurve = (global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
PeriodicFunctionCurve = (global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve) u.NativeObject;
}
else
{
throw new Exception($"尝试在{gorgeObject}对象上调用{Declaration.Name}类的方法");
}
switch (methodId)
{
case 0:
case 1:
InvokeParameterPool.FloatReturn = PeriodicFunctionCurve.Evaluate(InvokeParameterPool.Float[0]);
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
return global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.ConstructInstance(injector);
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
0 => InjectorFieldDefaultValue_startX(),
1 => InjectorFieldDefaultValue_endX(),
_ => base.GetInjectorFloatDefaultValue(defaultValueIndex)
};
}
public override bool GetInjectorBoolDefaultValue(int defaultValueIndex)
{
return defaultValueIndex switch
{
0 => InjectorFieldDefaultValue_leftClosed(),
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
0 => InjectorFieldDefaultValue_functionCurve(),
_ => base.GetInjectorObjectDefaultValue(defaultValueIndex)
};
}
}
public new class SpecificInjector : global::Gorge.Native.GorgeFramework.FunctionCurve.SpecificInjector
{
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
1 => ConstructInstance(this),
_ => throw new Exception($"Image类没有编号为{constructorIndex}的构造方法")
};
}
private Tuple<global::Gorge.Native.Gorge.Injector, bool> _functionCurve = new(default, true);
public global::Gorge.Native.Gorge.Injector functionCurve
{
get => _functionCurve.Item1;
set => _functionCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(value,false);
}
private Tuple<float, bool> _startX = new(default, true);
public float startX
{
get => _startX.Item1;
set => _startX = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _endX = new(default, true);
public float endX
{
get => _endX.Item1;
set => _endX = new Tuple<float, bool>(value,false);
}
private Tuple<bool, bool> _leftClosed = new(default, true);
public bool leftClosed
{
get => _leftClosed.Item1;
set => _leftClosed = new Tuple<bool, bool>(value,false);
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
_startX = new Tuple<float, bool>(value, false);
return;
case 1:
_endX = new Tuple<float, bool>(value, false);
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
_startX = new Tuple<float, bool>(default, true);
return;
case 1:
_endX = new Tuple<float, bool>(default, true);
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
0 => _startX.Item1,
1 => _endX.Item1,
_ => base.GetInjectorFloat(index)
};
}
public override bool GetInjectorFloatDefault(int index)
{
return index switch
{
0 => _startX.Item2,
1 => _endX.Item2,
_ => base.GetInjectorFloatDefault(index)
};
}
public override void SetInjectorBool(int index, bool value)
{
switch (index)
{
case 0:
_leftClosed = new Tuple<bool, bool>(value, false);
return;
default:
base.SetInjectorBool(index, value);
break;
}
}
public override void SetInjectorBoolDefault(int index)
{
switch (index)
{
case 0:
_leftClosed = new Tuple<bool, bool>(default, true);
return;
default:
base.SetInjectorBoolDefault(index);
break;
}
}
public override bool GetInjectorBool(int index)
{
return index switch
{
0 => _leftClosed.Item1,
_ => base.GetInjectorBool(index)
};
}
public override bool GetInjectorBoolDefault(int index)
{
return index switch
{
0 => _leftClosed.Item2,
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
_functionCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>((global::Gorge.Native.Gorge.Injector)value, false);
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
_functionCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(default, true);
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
0 => _functionCurve.Item1,
_ => base.GetInjectorObject(index)
};
}
public override bool GetInjectorObjectDefault(int index)
{
return index switch
{
0 => _functionCurve.Item2,
_ => base.GetInjectorObjectDefault(index)
};
}
public override bool EditableEquals(Injector target)
{
throw new NotImplementedException("暂未实现比较器");
}
public override GorgeObject Clone()
{
var injector = new global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve.SpecificInjector toInjector)
{
base.CloneTo(toInjector);
toInjector._functionCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(global::Gorge.Native.Gorge.Injector.FromGorgeObject(_functionCurve.Item1?.Clone()),_functionCurve.Item2);
toInjector._startX = _startX;
toInjector._endX = _endX;
toInjector._leftClosed = _leftClosed;
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
public new static global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 0号字段
/// 索引为object:0
/// </summary>
public global::Gorge.Native.GorgeFramework.FunctionCurve functionCurve;
/// <summary>
/// 1号字段
/// 索引为float:0
/// </summary>
public float startX;
/// <summary>
/// 2号字段
/// 索引为float:1
/// </summary>
public float endX;
/// <summary>
/// 3号字段
/// 索引为bool:0
/// </summary>
public bool leftClosed;
/// <summary>
/// 1号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve ConstructInstance(Injector injector)
{
return new global::Gorge.Native.GorgeFramework.PeriodicFunctionCurve(injector);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
global::Gorge.Native.Gorge.Injector injector_functionCurve;
if (injector.GetInjectorObjectDefault(0))
{
injector_functionCurve = InjectorFieldDefaultValue_functionCurve();
}
else
{
injector_functionCurve = (global::Gorge.Native.Gorge.Injector)injector.GetInjectorObject(0);
}
float injector_startX;
if (injector.GetInjectorFloatDefault(0))
{
injector_startX = InjectorFieldDefaultValue_startX();
}
else
{
injector_startX = injector.GetInjectorFloat(0);
}
float injector_endX;
if (injector.GetInjectorFloatDefault(1))
{
injector_endX = InjectorFieldDefaultValue_endX();
}
else
{
injector_endX = injector.GetInjectorFloat(1);
}
bool injector_leftClosed;
if (injector.GetInjectorBoolDefault(0))
{
injector_leftClosed = InjectorFieldDefaultValue_leftClosed();
}
else
{
injector_leftClosed = injector.GetInjectorBool(0);
}
this.functionCurve = InitializeField_functionCurve(injector_functionCurve);this.startX = InitializeField_startX(injector_startX);this.endX = InitializeField_endX(injector_endX);this.leftClosed = InitializeField_leftClosed(injector_leftClosed);}
private static partial global::Gorge.Native.GorgeFramework.FunctionCurve InitializeField_functionCurve(global::Gorge.Native.Gorge.Injector functionCurve);private static partial float InitializeField_startX(float startX);private static partial float InitializeField_endX(float endX);private static partial bool InitializeField_leftClosed(bool leftClosed);private static partial Dictionary<string, Metadata> InjectorFieldMetadata_functionCurve();/// <summary>
/// Injector的functionCurve字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial global::Gorge.Native.Gorge.Injector InjectorFieldDefaultValue_functionCurve();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startX();/// <summary>
/// Injector的startX字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_startX();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endX();/// <summary>
/// Injector的endX字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_endX();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_leftClosed();/// <summary>
/// Injector的leftClosed字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial bool InjectorFieldDefaultValue_leftClosed();public override int GetIntField(int fieldIndex)
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
0 => startX,
1 => endX,
_ => base.GetFloatField(fieldIndex)
};
}
public override void SetFloatField(int fieldIndex, float value)
{
switch (fieldIndex)
{
case 0:
this.startX = value;
return;
case 1:
this.endX = value;
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
0 => leftClosed,
_ => base.GetBoolField(fieldIndex)
};
}
public override void SetBoolField(int fieldIndex, bool value)
{
switch (fieldIndex)
{
case 0:
this.leftClosed = value;
return;
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
0 => functionCurve,
_ => base.GetObjectField(fieldIndex)
};
}
public override void SetObjectField(int fieldIndex, GorgeObject value)
{
switch (fieldIndex)
{
case 0:
this.functionCurve = (global::Gorge.Native.GorgeFramework.FunctionCurve)value;
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
