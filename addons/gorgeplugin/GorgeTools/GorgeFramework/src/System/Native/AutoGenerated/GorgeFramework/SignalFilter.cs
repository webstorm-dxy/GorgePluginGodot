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
public partial class SignalFilter : GorgeObject
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("SignalFilter", "GorgeFramework", new GorgeType[]{});
public override GorgeClass SuperClass { get; } = null;
public override GorgeClass LatestNativeClass => this;
public override Injector EmptyInjector() => new SpecificInjector();
public override ClassDeclaration Declaration { get; } = new ClassDeclaration(
type: Type(),
isNative: true,
superClass: null,
superInterfaces: new GorgeInterface[]
{
},
fields: new FieldInformation[]
{
new FieldInformation(
id: 0,
name: "priority",
type: GorgeType.Delegate(global::Gorge.Native.Gorge.ObjectArray.Implementation.Type()),
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 1,
name: "conditionTypes",
type: global::Gorge.Native.Gorge.IntArray.Implementation.Type(),
annotations: new Annotation[]{},
index: 1
),
new FieldInformation(
id: 2,
name: "endTime",
type: GorgeType.Delegate(GorgeType.Float),
annotations: new Annotation[]{},
index: 2
),
new FieldInformation(
id: 3,
name: "timeMode",
type: global::Gorge.Native.GorgeFramework.TimeMode.Enum.Type,
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 4,
name: "acceptConsume",
type: GorgeType.Bool,
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 5,
name: "denyConsume",
type: GorgeType.Bool,
annotations: new Annotation[]{},
index: 1
),
},
methods:
new MethodInformation[]
{
new MethodInformation(
id: 0,
name: "CanDetect",
returnType: GorgeType.Bool,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "channelName",
type: GorgeType.String,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 1,
name: "Detect",
returnType: GorgeType.Bool,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "channelName",
type: GorgeType.String,
index: 0
),
new ParameterInformation(
id: 1,
name: "signalId",
type: GorgeType.Int,
index: 0
),
new ParameterInformation(
id: 2,
name: "conditionType",
type: GorgeType.Int,
index: 1
),
new ParameterInformation(
id: 3,
name: "signalValue",
type: global::Gorge.Native.GorgeFramework.ISignal.Interface.Type,
index: 0
),
new ParameterInformation(
id: 4,
name: "lastSignalValue",
type: global::Gorge.Native.GorgeFramework.ISignal.Interface.Type,
index: 1
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
id: 0,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "priority",
type: GorgeType.Delegate(global::Gorge.Native.Gorge.ObjectArray.Implementation.Type()),
index: 0
),
new ParameterInformation(
id: 1,
name: "conditionTypes",
type: global::Gorge.Native.Gorge.IntArray.Implementation.Type(),
index: 1
),
new ParameterInformation(
id: 2,
name: "endTime",
type: GorgeType.Delegate(GorgeType.Float),
index: 2
),
new ParameterInformation(
id: 3,
name: "timeMode",
type: global::Gorge.Native.GorgeFramework.TimeMode.Enum.Type,
index: 0
),
new ParameterInformation(
id: 4,
name: "acceptConsume",
type: GorgeType.Bool,
index: 0
),
new ParameterInformation(
id: 5,
name: "denyConsume",
type: GorgeType.Bool,
index: 1
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
},
annotations: global::Gorge.Native.GorgeFramework.SignalFilter.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 1,
floatCount: 0,
boolCount: 2,
stringCount: 0,
objectCount: 3
)
,
methodCount: 2,
methodOverrideId:
new Dictionary<int, int>()
{
}
,
interfaceMethodImplementationId:
new Dictionary<string, int[]>()
{
}
,
staticMethodCount: 0,
constructorCount: 1,
injectorConstructorCount: 0,
injectorConstructorImplementationId:
new int[]
{
}
,
injectorFieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 0,
boolCount: 0,
stringCount: 0,
objectCount: 0
)
,
injectorFieldDefaultValueIndexCount:
new TypeCount(
intCount: 0,
floatCount: 0,
boolCount: 0,
stringCount: 0,
objectCount: 0
)
,
injectorFieldCount: 0
);
// TODO Injector的调用
public override void InvokeMethod(GorgeObject gorgeObject, int methodId)
{
global::Gorge.Native.GorgeFramework.SignalFilter SignalFilter;
if(gorgeObject is global::Gorge.Native.GorgeFramework.SignalFilter)
{
SignalFilter = (global::Gorge.Native.GorgeFramework.SignalFilter) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
SignalFilter = (global::Gorge.Native.GorgeFramework.SignalFilter) u.NativeObject;
}
else
{
throw new Exception($"尝试在{gorgeObject}对象上调用{Declaration.Name}类的方法");
}
switch (methodId)
{
case 0:
InvokeParameterPool.BoolReturn = SignalFilter.CanDetect(InvokeParameterPool.String[0]);
break;
case 1:
InvokeParameterPool.BoolReturn = SignalFilter.Detect(InvokeParameterPool.String[0], InvokeParameterPool.Int[0], InvokeParameterPool.Int[1], InvokeParameterPool.Object[0], InvokeParameterPool.Object[1]);
break;
default:
throw new Exception($"类{Declaration.Name}不存在编号为{methodId}的方法");
}
}
public override void InvokeStaticMethod(int methodId)
{
switch (methodId)
{
default:
throw new Exception($"类{Declaration.Name}不存在编号为{methodId}的静态方法");
}
}
protected override GorgeObject DoConstruct(GorgeObject targetObject, int constructorId)
{
var injector = InvokeParameterPool.Injector;
switch (constructorId)
{
case 0:
if (targetObject != null)
{
if (targetObject is CompiledGorgeObject u) // 外部继承本Native类
{
var instance = ConstructInstance(injector, (GorgeDelegate)InvokeParameterPool.Object[0], global::Gorge.Native.Gorge.IntArray.FromGorgeObject(InvokeParameterPool.Object[1]), (GorgeDelegate)InvokeParameterPool.Object[2], InvokeParameterPool.Int[0], InvokeParameterPool.Bool[0], InvokeParameterPool.Bool[1]);
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
return global::Gorge.Native.GorgeFramework.SignalFilter.ConstructInstance(injector, (GorgeDelegate)InvokeParameterPool.Object[0], global::Gorge.Native.Gorge.IntArray.FromGorgeObject(InvokeParameterPool.Object[1]), (GorgeDelegate)InvokeParameterPool.Object[2], InvokeParameterPool.Int[0], InvokeParameterPool.Bool[0], InvokeParameterPool.Bool[1]);
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
public new class SpecificInjector : Injector
{
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.SignalFilter.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
0 => ConstructInstance(this, (GorgeDelegate)args[0], global::Gorge.Native.Gorge.IntArray.FromGorgeObject((GorgeObject)args[1]), (GorgeDelegate)args[2], (int)args[3], (bool)args[4], (bool)args[5]),
_ => throw new Exception($"Image类没有编号为{constructorIndex}的构造方法")
};
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
default:
base.SetInjectorFloat(index, value);
break;
}
}
public override void SetInjectorFloatDefault(int index)
{
switch (index)
{
default:
base.SetInjectorFloatDefault(index);
break;
}
}
public override float GetInjectorFloat(int index)
{
return index switch
{
_ => base.GetInjectorFloat(index)
};
}
public override bool GetInjectorFloatDefault(int index)
{
return index switch
{
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
var injector = new global::Gorge.Native.GorgeFramework.SignalFilter.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.SignalFilter.SpecificInjector toInjector)
{
base.CloneTo(toInjector);
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
public new static global::Gorge.Native.GorgeFramework.SignalFilter FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.SignalFilter) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.SignalFilter) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 0号字段
/// 索引为object:0
/// </summary>
public GorgeDelegate priority;
/// <summary>
/// 1号字段
/// 索引为object:1
/// </summary>
public global::Gorge.Native.Gorge.IntArray conditionTypes;
/// <summary>
/// 2号字段
/// 索引为object:2
/// </summary>
public GorgeDelegate endTime;
/// <summary>
/// 3号字段
/// 索引为int:0
/// </summary>
public int timeMode;
/// <summary>
/// 4号字段
/// 索引为bool:0
/// </summary>
public bool acceptConsume;
/// <summary>
/// 5号字段
/// 索引为bool:1
/// </summary>
public bool denyConsume;
/// <summary>
/// 0号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.SignalFilter ConstructInstance(Injector injector, GorgeDelegate priority, global::Gorge.Native.Gorge.IntArray conditionTypes, GorgeDelegate endTime, int timeMode, bool acceptConsume, bool denyConsume)
{
return new global::Gorge.Native.GorgeFramework.SignalFilter(injector, priority, conditionTypes, endTime, timeMode, acceptConsume, denyConsume);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
this.priority = InitializeField_priority();this.conditionTypes = InitializeField_conditionTypes();this.endTime = InitializeField_endTime();this.timeMode = InitializeField_timeMode();this.acceptConsume = InitializeField_acceptConsume();this.denyConsume = InitializeField_denyConsume();}
private static partial GorgeDelegate InitializeField_priority();private static partial global::Gorge.Native.Gorge.IntArray InitializeField_conditionTypes();private static partial GorgeDelegate InitializeField_endTime();private static partial int InitializeField_timeMode();private static partial bool InitializeField_acceptConsume();private static partial bool InitializeField_denyConsume();public override int GetIntField(int fieldIndex)
{
return fieldIndex switch
{
0 => timeMode,
_ => base.GetIntField(fieldIndex)
};
}
public override void SetIntField(int fieldIndex, int value)
{
switch (fieldIndex)
{
case 0:
this.timeMode = value;
return;
default:
base.SetIntField(fieldIndex,value);
break;
}
}
public override float GetFloatField(int fieldIndex)
{
return fieldIndex switch
{
_ => base.GetFloatField(fieldIndex)
};
}
public override void SetFloatField(int fieldIndex, float value)
{
switch (fieldIndex)
{
default:
base.SetFloatField(fieldIndex,value);
break;
}
}
public override bool GetBoolField(int fieldIndex)
{
return fieldIndex switch
{
0 => acceptConsume,
1 => denyConsume,
_ => base.GetBoolField(fieldIndex)
};
}
public override void SetBoolField(int fieldIndex, bool value)
{
switch (fieldIndex)
{
case 0:
this.acceptConsume = value;
return;
case 1:
this.denyConsume = value;
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
0 => priority,
1 => conditionTypes,
2 => endTime,
_ => base.GetObjectField(fieldIndex)
};
}
public override void SetObjectField(int fieldIndex, GorgeObject value)
{
switch (fieldIndex)
{
case 0:
this.priority = (GorgeDelegate)value;
return;
case 1:
this.conditionTypes = (global::Gorge.Native.Gorge.IntArray)value;
return;
case 2:
this.endTime = (GorgeDelegate)value;
return;
default:
base.SetObjectField(fieldIndex,value);
break;
}
}
/// <summary>
/// 0号方法
/// </summary>
/// <returns></returns>
public virtual partial bool CanDetect(string channelName);
/// <summary>
/// 1号方法
/// </summary>
/// <returns></returns>
public virtual partial bool Detect(string channelName, int signalId, int conditionType, GorgeObject signalValue, GorgeObject lastSignalValue);
private static partial Annotation[] ClassAnnotations();
}
}
