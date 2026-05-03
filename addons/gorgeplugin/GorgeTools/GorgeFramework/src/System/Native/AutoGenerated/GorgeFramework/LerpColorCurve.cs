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
public partial class LerpColorCurve : global::Gorge.Native.GorgeFramework.ColorCurve
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("LerpColorCurve", "GorgeFramework", new GorgeType[]{});
public override GorgeClass SuperClass { get; } = global::Gorge.Native.GorgeFramework.ColorCurve.Class;
public override GorgeClass LatestNativeClass => this;
public override Injector EmptyInjector() => new SpecificInjector();
public override ClassDeclaration Declaration { get; } = new ClassDeclaration(
type: Type(),
isNative: true,
superClass: global::Gorge.Native.GorgeFramework.ColorCurve.Class.Declaration,
superInterfaces: new GorgeInterface[]
{
},
fields: new FieldInformation[]
{
new FieldInformation(
id: 0,
name: "colorPoints",
type: global::Gorge.Native.Gorge.ObjectArray.Implementation.Type(),
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 1,
name: "progressCurve",
type: global::Gorge.Native.GorgeFramework.FunctionCurve.Implementation.Type(),
annotations: new Annotation[]{},
index: 1
),
},
methods:
new MethodInformation[]
{
new MethodInformation(
id: 1,
name: "Evaluate",
returnType: global::Gorge.Native.GorgeFramework.ColorArgb.Implementation.Type(),
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
name: "colorPoints",
type: GorgeType.ObjectList(GorgeType.Injector(global::Gorge.Native.GorgeFramework.ColorArgb.Implementation.Type())),
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.LerpColorCurve.InjectorFieldMetadata_colorPoints()
),
new InjectorFieldInformation(
id: 1,
name: "progressCurve",
type: GorgeType.Injector(global::Gorge.Native.GorgeFramework.FunctionCurve.Implementation.Type()),
index: 1,
defaultValueIndex: 1,
metadata: global::Gorge.Native.GorgeFramework.LerpColorCurve.InjectorFieldMetadata_progressCurve()
),
},
annotations: global::Gorge.Native.GorgeFramework.LerpColorCurve.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 0,
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
floatCount: 0,
boolCount: 0,
stringCount: 0,
objectCount: 2
)
,
injectorFieldDefaultValueIndexCount:
new TypeCount(
intCount: 0,
floatCount: 0,
boolCount: 0,
stringCount: 0,
objectCount: 2
)
,
injectorFieldCount: 2
);
// TODO Injector的调用
public override void InvokeMethod(GorgeObject gorgeObject, int methodId)
{
global::Gorge.Native.GorgeFramework.LerpColorCurve LerpColorCurve;
if(gorgeObject is global::Gorge.Native.GorgeFramework.LerpColorCurve)
{
LerpColorCurve = (global::Gorge.Native.GorgeFramework.LerpColorCurve) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
LerpColorCurve = (global::Gorge.Native.GorgeFramework.LerpColorCurve) u.NativeObject;
}
else
{
throw new Exception($"尝试在{gorgeObject}对象上调用{Declaration.Name}类的方法");
}
switch (methodId)
{
case 0:
case 1:
InvokeParameterPool.ObjectReturn = LerpColorCurve.Evaluate(InvokeParameterPool.Float[0]);
break;
default:
global::Gorge.Native.GorgeFramework.ColorCurve.Class.InvokeMethod(gorgeObject, methodId);
break;
}
}
public override void InvokeStaticMethod(int methodId)
{
switch (methodId)
{
default:
global::Gorge.Native.GorgeFramework.ColorCurve.Class.InvokeStaticMethod(methodId);
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
return global::Gorge.Native.GorgeFramework.LerpColorCurve.ConstructInstance(injector);
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
0 => InjectorFieldDefaultValue_colorPoints(),
1 => InjectorFieldDefaultValue_progressCurve(),
_ => base.GetInjectorObjectDefaultValue(defaultValueIndex)
};
}
}
public new class SpecificInjector : global::Gorge.Native.GorgeFramework.ColorCurve.SpecificInjector
{
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.LerpColorCurve.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
1 => ConstructInstance(this),
_ => throw new Exception($"Image类没有编号为{constructorIndex}的构造方法")
};
}
private Tuple<global::Gorge.Native.Gorge.ObjectList, bool> _colorPoints = new(default, true);
public global::Gorge.Native.Gorge.ObjectList colorPoints
{
get => _colorPoints.Item1;
set => _colorPoints = new Tuple<global::Gorge.Native.Gorge.ObjectList, bool>(value,false);
}
private Tuple<global::Gorge.Native.Gorge.Injector, bool> _progressCurve = new(default, true);
public global::Gorge.Native.Gorge.Injector progressCurve
{
get => _progressCurve.Item1;
set => _progressCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(value,false);
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
case 0:
_colorPoints = new Tuple<global::Gorge.Native.Gorge.ObjectList, bool>((global::Gorge.Native.Gorge.ObjectList)value, false);
return;
case 1:
_progressCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>((global::Gorge.Native.Gorge.Injector)value, false);
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
_colorPoints = new Tuple<global::Gorge.Native.Gorge.ObjectList, bool>(default, true);
return;
case 1:
_progressCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(default, true);
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
0 => _colorPoints.Item1,
1 => _progressCurve.Item1,
_ => base.GetInjectorObject(index)
};
}
public override bool GetInjectorObjectDefault(int index)
{
return index switch
{
0 => _colorPoints.Item2,
1 => _progressCurve.Item2,
_ => base.GetInjectorObjectDefault(index)
};
}
public override bool EditableEquals(Injector target)
{
throw new NotImplementedException("暂未实现比较器");
}
public override GorgeObject Clone()
{
var injector = new global::Gorge.Native.GorgeFramework.LerpColorCurve.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.LerpColorCurve.SpecificInjector toInjector)
{
base.CloneTo(toInjector);
toInjector._colorPoints = new Tuple<global::Gorge.Native.Gorge.ObjectList, bool>(global::Gorge.Native.Gorge.ObjectList.FromGorgeObject(_colorPoints.Item1?.Clone()),_colorPoints.Item2);
toInjector._progressCurve = new Tuple<global::Gorge.Native.Gorge.Injector, bool>(global::Gorge.Native.Gorge.Injector.FromGorgeObject(_progressCurve.Item1?.Clone()),_progressCurve.Item2);
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
public new static global::Gorge.Native.GorgeFramework.LerpColorCurve FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.LerpColorCurve) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.LerpColorCurve) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 0号字段
/// 索引为object:0
/// </summary>
public global::Gorge.Native.Gorge.ObjectArray colorPoints;
/// <summary>
/// 1号字段
/// 索引为object:1
/// </summary>
public global::Gorge.Native.GorgeFramework.FunctionCurve progressCurve;
/// <summary>
/// 1号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.LerpColorCurve ConstructInstance(Injector injector)
{
return new global::Gorge.Native.GorgeFramework.LerpColorCurve(injector);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
global::Gorge.Native.Gorge.ObjectList injector_colorPoints;
if (injector.GetInjectorObjectDefault(0))
{
injector_colorPoints = InjectorFieldDefaultValue_colorPoints();
}
else
{
injector_colorPoints = (global::Gorge.Native.Gorge.ObjectList)injector.GetInjectorObject(0);
}
global::Gorge.Native.Gorge.Injector injector_progressCurve;
if (injector.GetInjectorObjectDefault(1))
{
injector_progressCurve = InjectorFieldDefaultValue_progressCurve();
}
else
{
injector_progressCurve = (global::Gorge.Native.Gorge.Injector)injector.GetInjectorObject(1);
}
this.colorPoints = InitializeField_colorPoints(injector_colorPoints);this.progressCurve = InitializeField_progressCurve(injector_progressCurve);}
private static partial global::Gorge.Native.Gorge.ObjectArray InitializeField_colorPoints(global::Gorge.Native.Gorge.ObjectList colorPoints);private static partial global::Gorge.Native.GorgeFramework.FunctionCurve InitializeField_progressCurve(global::Gorge.Native.Gorge.Injector progressCurve);private static partial Dictionary<string, Metadata> InjectorFieldMetadata_colorPoints();/// <summary>
/// Injector的colorPoints字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial global::Gorge.Native.Gorge.ObjectList InjectorFieldDefaultValue_colorPoints();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_progressCurve();/// <summary>
/// Injector的progressCurve字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial global::Gorge.Native.Gorge.Injector InjectorFieldDefaultValue_progressCurve();public override int GetIntField(int fieldIndex)
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
0 => colorPoints,
1 => progressCurve,
_ => base.GetObjectField(fieldIndex)
};
}
public override void SetObjectField(int fieldIndex, GorgeObject value)
{
switch (fieldIndex)
{
case 0:
this.colorPoints = (global::Gorge.Native.Gorge.ObjectArray)value;
return;
case 1:
this.progressCurve = (global::Gorge.Native.GorgeFramework.FunctionCurve)value;
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
public override partial global::Gorge.Native.GorgeFramework.ColorArgb Evaluate(float x);
private static partial Annotation[] ClassAnnotations();
}
}
