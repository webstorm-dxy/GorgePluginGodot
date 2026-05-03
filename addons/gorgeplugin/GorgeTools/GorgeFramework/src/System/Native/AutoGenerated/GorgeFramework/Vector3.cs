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
public partial class Vector3 : GorgeObject
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("Vector3", "GorgeFramework", new GorgeType[]{});
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
name: "x",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 1,
name: "y",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 1
),
new FieldInformation(
id: 2,
name: "z",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 2
),
},
methods:
new MethodInformation[]
{
new MethodInformation(
id: 0,
name: "ToVector2",
returnType: global::Gorge.Native.GorgeFramework.Vector2.Implementation.Type(),
parameters: new ParameterInformation[]
{
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
},
annotations: new Annotation[]{}
),
new ConstructorInformation(
id: 1,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "x",
type: GorgeType.Float,
index: 0
),
new ParameterInformation(
id: 1,
name: "y",
type: GorgeType.Float,
index: 1
),
new ParameterInformation(
id: 2,
name: "z",
type: GorgeType.Float,
index: 2
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
name: "x",
type: GorgeType.Float,
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.Vector3.InjectorFieldMetadata_x()
),
new InjectorFieldInformation(
id: 1,
name: "y",
type: GorgeType.Float,
index: 1,
defaultValueIndex: 1,
metadata: global::Gorge.Native.GorgeFramework.Vector3.InjectorFieldMetadata_y()
),
new InjectorFieldInformation(
id: 2,
name: "z",
type: GorgeType.Float,
index: 2,
defaultValueIndex: 2,
metadata: global::Gorge.Native.GorgeFramework.Vector3.InjectorFieldMetadata_z()
),
},
annotations: global::Gorge.Native.GorgeFramework.Vector3.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 3,
boolCount: 0,
stringCount: 0,
objectCount: 0
)
,
methodCount: 1,
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
constructorCount: 2,
injectorConstructorCount: 0,
injectorConstructorImplementationId:
new int[]
{
}
,
injectorFieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 3,
boolCount: 0,
stringCount: 0,
objectCount: 0
)
,
injectorFieldDefaultValueIndexCount:
new TypeCount(
intCount: 0,
floatCount: 3,
boolCount: 0,
stringCount: 0,
objectCount: 0
)
,
injectorFieldCount: 3
);
// TODO Injector的调用
public override void InvokeMethod(GorgeObject gorgeObject, int methodId)
{
global::Gorge.Native.GorgeFramework.Vector3 Vector3;
if(gorgeObject is global::Gorge.Native.GorgeFramework.Vector3)
{
Vector3 = (global::Gorge.Native.GorgeFramework.Vector3) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
Vector3 = (global::Gorge.Native.GorgeFramework.Vector3) u.NativeObject;
}
else
{
throw new Exception($"尝试在{gorgeObject}对象上调用{Declaration.Name}类的方法");
}
switch (methodId)
{
case 0:
InvokeParameterPool.ObjectReturn = Vector3.ToVector2();
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
return global::Gorge.Native.GorgeFramework.Vector3.ConstructInstance(injector);
}
break;
case 1:
if (targetObject != null)
{
if (targetObject is CompiledGorgeObject u) // 外部继承本Native类
{
var instance = ConstructInstance(injector, InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2]);
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
return global::Gorge.Native.GorgeFramework.Vector3.ConstructInstance(injector, InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2]);
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
0 => InjectorFieldDefaultValue_x(),
1 => InjectorFieldDefaultValue_y(),
2 => InjectorFieldDefaultValue_z(),
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
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.Vector3.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
0 => ConstructInstance(this),
1 => ConstructInstance(this, (float)args[0], (float)args[1], (float)args[2]),
_ => throw new Exception($"Image类没有编号为{constructorIndex}的构造方法")
};
}
private Tuple<float, bool> _x = new(default, true);
public float x
{
get => _x.Item1;
set => _x = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _y = new(default, true);
public float y
{
get => _y.Item1;
set => _y = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _z = new(default, true);
public float z
{
get => _z.Item1;
set => _z = new Tuple<float, bool>(value,false);
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
_x = new Tuple<float, bool>(value, false);
return;
case 1:
_y = new Tuple<float, bool>(value, false);
return;
case 2:
_z = new Tuple<float, bool>(value, false);
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
_x = new Tuple<float, bool>(default, true);
return;
case 1:
_y = new Tuple<float, bool>(default, true);
return;
case 2:
_z = new Tuple<float, bool>(default, true);
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
0 => _x.Item1,
1 => _y.Item1,
2 => _z.Item1,
_ => base.GetInjectorFloat(index)
};
}
public override bool GetInjectorFloatDefault(int index)
{
return index switch
{
0 => _x.Item2,
1 => _y.Item2,
2 => _z.Item2,
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
var injector = new global::Gorge.Native.GorgeFramework.Vector3.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.Vector3.SpecificInjector toInjector)
{
base.CloneTo(toInjector);
toInjector._x = _x;
toInjector._y = _y;
toInjector._z = _z;
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
public new static global::Gorge.Native.GorgeFramework.Vector3 FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.Vector3) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.Vector3) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 0号字段
/// 索引为float:0
/// </summary>
public float x;
/// <summary>
/// 1号字段
/// 索引为float:1
/// </summary>
public float y;
/// <summary>
/// 2号字段
/// 索引为float:2
/// </summary>
public float z;
/// <summary>
/// 0号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.Vector3 ConstructInstance(Injector injector)
{
return new global::Gorge.Native.GorgeFramework.Vector3(injector);
}
/// <summary>
/// 1号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.Vector3 ConstructInstance(Injector injector, float x, float y, float z)
{
return new global::Gorge.Native.GorgeFramework.Vector3(injector, x, y, z);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
float injector_x;
if (injector.GetInjectorFloatDefault(0))
{
injector_x = InjectorFieldDefaultValue_x();
}
else
{
injector_x = injector.GetInjectorFloat(0);
}
float injector_y;
if (injector.GetInjectorFloatDefault(1))
{
injector_y = InjectorFieldDefaultValue_y();
}
else
{
injector_y = injector.GetInjectorFloat(1);
}
float injector_z;
if (injector.GetInjectorFloatDefault(2))
{
injector_z = InjectorFieldDefaultValue_z();
}
else
{
injector_z = injector.GetInjectorFloat(2);
}
this.x = InitializeField_x(injector_x);this.y = InitializeField_y(injector_y);this.z = InitializeField_z(injector_z);}
private static partial float InitializeField_x(float x);private static partial float InitializeField_y(float y);private static partial float InitializeField_z(float z);private static partial Dictionary<string, Metadata> InjectorFieldMetadata_x();/// <summary>
/// Injector的x字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_x();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_y();/// <summary>
/// Injector的y字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_y();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_z();/// <summary>
/// Injector的z字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_z();public override int GetIntField(int fieldIndex)
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
0 => x,
1 => y,
2 => z,
_ => base.GetFloatField(fieldIndex)
};
}
public override void SetFloatField(int fieldIndex, float value)
{
switch (fieldIndex)
{
case 0:
this.x = value;
return;
case 1:
this.y = value;
return;
case 2:
this.z = value;
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
/// 0号方法
/// </summary>
/// <returns></returns>
public virtual partial global::Gorge.Native.GorgeFramework.Vector2 ToVector2();
private static partial Annotation[] ClassAnnotations();
}
}
