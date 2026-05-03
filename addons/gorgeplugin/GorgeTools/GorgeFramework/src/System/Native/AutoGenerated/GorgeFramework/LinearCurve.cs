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
public partial class LinearCurve : global::Gorge.Native.GorgeFramework.FunctionCurve
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("LinearCurve", "GorgeFramework", new GorgeType[]{});
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
name: "timeStart",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 1,
name: "valueStart",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 1
),
new FieldInformation(
id: 2,
name: "timeEnd",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 2
),
new FieldInformation(
id: 3,
name: "valueEnd",
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
name: "time",
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
new ConstructorInformation(
id: 2,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "timeStart",
type: GorgeType.Float,
index: 0
),
new ParameterInformation(
id: 1,
name: "valueStart",
type: GorgeType.Float,
index: 1
),
new ParameterInformation(
id: 2,
name: "timeEnd",
type: GorgeType.Float,
index: 2
),
new ParameterInformation(
id: 3,
name: "valueEnd",
type: GorgeType.Float,
index: 3
),
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
name: "timeStart",
type: GorgeType.Float,
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.LinearCurve.InjectorFieldMetadata_timeStart()
),
new InjectorFieldInformation(
id: 1,
name: "valueStart",
type: GorgeType.Float,
index: 1,
defaultValueIndex: 1,
metadata: global::Gorge.Native.GorgeFramework.LinearCurve.InjectorFieldMetadata_valueStart()
),
new InjectorFieldInformation(
id: 2,
name: "timeEnd",
type: GorgeType.Float,
index: 2,
defaultValueIndex: 2,
metadata: global::Gorge.Native.GorgeFramework.LinearCurve.InjectorFieldMetadata_timeEnd()
),
new InjectorFieldInformation(
id: 3,
name: "valueEnd",
type: GorgeType.Float,
index: 3,
defaultValueIndex: 3,
metadata: global::Gorge.Native.GorgeFramework.LinearCurve.InjectorFieldMetadata_valueEnd()
),
},
annotations: global::Gorge.Native.GorgeFramework.LinearCurve.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 4,
boolCount: 0,
stringCount: 0,
objectCount: 0
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
constructorCount: 3,
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
objectCount: 0
)
,
injectorFieldDefaultValueIndexCount:
new TypeCount(
intCount: 0,
floatCount: 4,
boolCount: 0,
stringCount: 0,
objectCount: 0
)
,
injectorFieldCount: 4
);
// TODO Injector的调用
public override void InvokeMethod(GorgeObject gorgeObject, int methodId)
{
global::Gorge.Native.GorgeFramework.LinearCurve LinearCurve;
if(gorgeObject is global::Gorge.Native.GorgeFramework.LinearCurve)
{
LinearCurve = (global::Gorge.Native.GorgeFramework.LinearCurve) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
LinearCurve = (global::Gorge.Native.GorgeFramework.LinearCurve) u.NativeObject;
}
else
{
throw new Exception($"尝试在{gorgeObject}对象上调用{Declaration.Name}类的方法");
}
switch (methodId)
{
case 0:
case 1:
InvokeParameterPool.FloatReturn = LinearCurve.Evaluate(InvokeParameterPool.Float[0]);
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
return global::Gorge.Native.GorgeFramework.LinearCurve.ConstructInstance(injector);
}
break;
case 2:
if (targetObject != null)
{
if (targetObject is CompiledGorgeObject u) // 外部继承本Native类
{
var instance = ConstructInstance(injector, InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2], InvokeParameterPool.Float[3]);
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
return global::Gorge.Native.GorgeFramework.LinearCurve.ConstructInstance(injector, InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2], InvokeParameterPool.Float[3]);
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
0 => InjectorFieldDefaultValue_timeStart(),
1 => InjectorFieldDefaultValue_valueStart(),
2 => InjectorFieldDefaultValue_timeEnd(),
3 => InjectorFieldDefaultValue_valueEnd(),
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
_ => base.GetInjectorObjectDefaultValue(defaultValueIndex)
};
}
}
public new class SpecificInjector : global::Gorge.Native.GorgeFramework.FunctionCurve.SpecificInjector
{
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.LinearCurve.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
1 => ConstructInstance(this),
2 => ConstructInstance(this, (float)args[0], (float)args[1], (float)args[2], (float)args[3]),
_ => throw new Exception($"Image类没有编号为{constructorIndex}的构造方法")
};
}
private Tuple<float, bool> _timeStart = new(default, true);
public float timeStart
{
get => _timeStart.Item1;
set => _timeStart = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _valueStart = new(default, true);
public float valueStart
{
get => _valueStart.Item1;
set => _valueStart = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _timeEnd = new(default, true);
public float timeEnd
{
get => _timeEnd.Item1;
set => _timeEnd = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _valueEnd = new(default, true);
public float valueEnd
{
get => _valueEnd.Item1;
set => _valueEnd = new Tuple<float, bool>(value,false);
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
_timeStart = new Tuple<float, bool>(value, false);
return;
case 1:
_valueStart = new Tuple<float, bool>(value, false);
return;
case 2:
_timeEnd = new Tuple<float, bool>(value, false);
return;
case 3:
_valueEnd = new Tuple<float, bool>(value, false);
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
_timeStart = new Tuple<float, bool>(default, true);
return;
case 1:
_valueStart = new Tuple<float, bool>(default, true);
return;
case 2:
_timeEnd = new Tuple<float, bool>(default, true);
return;
case 3:
_valueEnd = new Tuple<float, bool>(default, true);
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
0 => _timeStart.Item1,
1 => _valueStart.Item1,
2 => _timeEnd.Item1,
3 => _valueEnd.Item1,
_ => base.GetInjectorFloat(index)
};
}
public override bool GetInjectorFloatDefault(int index)
{
return index switch
{
0 => _timeStart.Item2,
1 => _valueStart.Item2,
2 => _timeEnd.Item2,
3 => _valueEnd.Item2,
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
default:
base.SetInjectorObject(index, value);
break;
}
}
public override void SetInjectorObjectDefault(int index)
{
switch (index)
{
default:
base.SetInjectorObjectDefault(index);
break;
}
}
public override GorgeObject GetInjectorObject(int index)
{
return index switch
{
_ => base.GetInjectorObject(index)
};
}
public override bool GetInjectorObjectDefault(int index)
{
return index switch
{
_ => base.GetInjectorObjectDefault(index)
};
}
public override bool EditableEquals(Injector target)
{
throw new NotImplementedException("暂未实现比较器");
}
public override GorgeObject Clone()
{
var injector = new global::Gorge.Native.GorgeFramework.LinearCurve.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.LinearCurve.SpecificInjector toInjector)
{
base.CloneTo(toInjector);
toInjector._timeStart = _timeStart;
toInjector._valueStart = _valueStart;
toInjector._timeEnd = _timeEnd;
toInjector._valueEnd = _valueEnd;
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
public new static global::Gorge.Native.GorgeFramework.LinearCurve FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.LinearCurve) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.LinearCurve) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 0号字段
/// 索引为float:0
/// </summary>
public float timeStart;
/// <summary>
/// 1号字段
/// 索引为float:1
/// </summary>
public float valueStart;
/// <summary>
/// 2号字段
/// 索引为float:2
/// </summary>
public float timeEnd;
/// <summary>
/// 3号字段
/// 索引为float:3
/// </summary>
public float valueEnd;
/// <summary>
/// 1号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.LinearCurve ConstructInstance(Injector injector)
{
return new global::Gorge.Native.GorgeFramework.LinearCurve(injector);
}
/// <summary>
/// 2号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.LinearCurve ConstructInstance(Injector injector, float timeStart, float valueStart, float timeEnd, float valueEnd)
{
return new global::Gorge.Native.GorgeFramework.LinearCurve(injector, timeStart, valueStart, timeEnd, valueEnd);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
float injector_timeStart;
if (injector.GetInjectorFloatDefault(0))
{
injector_timeStart = InjectorFieldDefaultValue_timeStart();
}
else
{
injector_timeStart = injector.GetInjectorFloat(0);
}
float injector_valueStart;
if (injector.GetInjectorFloatDefault(1))
{
injector_valueStart = InjectorFieldDefaultValue_valueStart();
}
else
{
injector_valueStart = injector.GetInjectorFloat(1);
}
float injector_timeEnd;
if (injector.GetInjectorFloatDefault(2))
{
injector_timeEnd = InjectorFieldDefaultValue_timeEnd();
}
else
{
injector_timeEnd = injector.GetInjectorFloat(2);
}
float injector_valueEnd;
if (injector.GetInjectorFloatDefault(3))
{
injector_valueEnd = InjectorFieldDefaultValue_valueEnd();
}
else
{
injector_valueEnd = injector.GetInjectorFloat(3);
}
this.timeStart = InitializeField_timeStart(injector_timeStart);this.valueStart = InitializeField_valueStart(injector_valueStart);this.timeEnd = InitializeField_timeEnd(injector_timeEnd);this.valueEnd = InitializeField_valueEnd(injector_valueEnd);}
private static partial float InitializeField_timeStart(float timeStart);private static partial float InitializeField_valueStart(float valueStart);private static partial float InitializeField_timeEnd(float timeEnd);private static partial float InitializeField_valueEnd(float valueEnd);private static partial Dictionary<string, Metadata> InjectorFieldMetadata_timeStart();/// <summary>
/// Injector的timeStart字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_timeStart();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_valueStart();/// <summary>
/// Injector的valueStart字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_valueStart();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_timeEnd();/// <summary>
/// Injector的timeEnd字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_timeEnd();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_valueEnd();/// <summary>
/// Injector的valueEnd字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_valueEnd();public override int GetIntField(int fieldIndex)
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
0 => timeStart,
1 => valueStart,
2 => timeEnd,
3 => valueEnd,
_ => base.GetFloatField(fieldIndex)
};
}
public override void SetFloatField(int fieldIndex, float value)
{
switch (fieldIndex)
{
case 0:
this.timeStart = value;
return;
case 1:
this.valueStart = value;
return;
case 2:
this.timeEnd = value;
return;
case 3:
this.valueEnd = value;
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
_ => base.GetObjectField(fieldIndex)
};
}
public override void SetObjectField(int fieldIndex, GorgeObject value)
{
switch (fieldIndex)
{
default:
base.SetObjectField(fieldIndex,value);
break;
}
}
/// <summary>
/// 1号方法
/// </summary>
/// <returns></returns>
public override partial float Evaluate(float time);
private static partial Annotation[] ClassAnnotations();
}
}
