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
public partial class ColorArgb : GorgeObject
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("ColorArgb", "GorgeFramework", new GorgeType[]{});
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
name: "a",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 0
),
new FieldInformation(
id: 1,
name: "r",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 1
),
new FieldInformation(
id: 2,
name: "g",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 2
),
new FieldInformation(
id: 3,
name: "b",
type: GorgeType.Float,
annotations: new Annotation[]{},
index: 3
),
},
methods:
new MethodInformation[]
{
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
name: "a",
type: GorgeType.Float,
index: 0
),
new ParameterInformation(
id: 1,
name: "r",
type: GorgeType.Float,
index: 1
),
new ParameterInformation(
id: 2,
name: "g",
type: GorgeType.Float,
index: 2
),
new ParameterInformation(
id: 3,
name: "b",
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
name: "a",
type: GorgeType.Float,
index: 0,
defaultValueIndex: 0,
metadata: global::Gorge.Native.GorgeFramework.ColorArgb.InjectorFieldMetadata_a()
),
new InjectorFieldInformation(
id: 1,
name: "r",
type: GorgeType.Float,
index: 1,
defaultValueIndex: 1,
metadata: global::Gorge.Native.GorgeFramework.ColorArgb.InjectorFieldMetadata_r()
),
new InjectorFieldInformation(
id: 2,
name: "g",
type: GorgeType.Float,
index: 2,
defaultValueIndex: 2,
metadata: global::Gorge.Native.GorgeFramework.ColorArgb.InjectorFieldMetadata_g()
),
new InjectorFieldInformation(
id: 3,
name: "b",
type: GorgeType.Float,
index: 3,
defaultValueIndex: 3,
metadata: global::Gorge.Native.GorgeFramework.ColorArgb.InjectorFieldMetadata_b()
),
},
annotations: global::Gorge.Native.GorgeFramework.ColorArgb.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 4,
boolCount: 0,
stringCount: 0,
objectCount: 0
)
,
methodCount: 0,
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
global::Gorge.Native.GorgeFramework.ColorArgb ColorArgb;
if(gorgeObject is global::Gorge.Native.GorgeFramework.ColorArgb)
{
ColorArgb = (global::Gorge.Native.GorgeFramework.ColorArgb) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
ColorArgb = (global::Gorge.Native.GorgeFramework.ColorArgb) u.NativeObject;
}
else
{
throw new Exception($"尝试在{gorgeObject}对象上调用{Declaration.Name}类的方法");
}
switch (methodId)
{
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
return global::Gorge.Native.GorgeFramework.ColorArgb.ConstructInstance(injector);
}
break;
case 1:
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
return global::Gorge.Native.GorgeFramework.ColorArgb.ConstructInstance(injector, InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2], InvokeParameterPool.Float[3]);
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
0 => InjectorFieldDefaultValue_a(),
1 => InjectorFieldDefaultValue_r(),
2 => InjectorFieldDefaultValue_g(),
3 => InjectorFieldDefaultValue_b(),
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
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.ColorArgb.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
0 => ConstructInstance(this),
1 => ConstructInstance(this, (float)args[0], (float)args[1], (float)args[2], (float)args[3]),
_ => throw new Exception($"Image类没有编号为{constructorIndex}的构造方法")
};
}
private Tuple<float, bool> _a = new(default, true);
public float a
{
get => _a.Item1;
set => _a = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _r = new(default, true);
public float r
{
get => _r.Item1;
set => _r = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _g = new(default, true);
public float g
{
get => _g.Item1;
set => _g = new Tuple<float, bool>(value,false);
}
private Tuple<float, bool> _b = new(default, true);
public float b
{
get => _b.Item1;
set => _b = new Tuple<float, bool>(value,false);
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
_a = new Tuple<float, bool>(value, false);
return;
case 1:
_r = new Tuple<float, bool>(value, false);
return;
case 2:
_g = new Tuple<float, bool>(value, false);
return;
case 3:
_b = new Tuple<float, bool>(value, false);
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
_a = new Tuple<float, bool>(default, true);
return;
case 1:
_r = new Tuple<float, bool>(default, true);
return;
case 2:
_g = new Tuple<float, bool>(default, true);
return;
case 3:
_b = new Tuple<float, bool>(default, true);
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
0 => _a.Item1,
1 => _r.Item1,
2 => _g.Item1,
3 => _b.Item1,
_ => base.GetInjectorFloat(index)
};
}
public override bool GetInjectorFloatDefault(int index)
{
return index switch
{
0 => _a.Item2,
1 => _r.Item2,
2 => _g.Item2,
3 => _b.Item2,
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
var injector = new global::Gorge.Native.GorgeFramework.ColorArgb.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.ColorArgb.SpecificInjector toInjector)
{
base.CloneTo(toInjector);
toInjector._a = _a;
toInjector._r = _r;
toInjector._g = _g;
toInjector._b = _b;
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
public new static global::Gorge.Native.GorgeFramework.ColorArgb FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.ColorArgb) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.ColorArgb) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 0号字段
/// 索引为float:0
/// </summary>
public float a;
/// <summary>
/// 1号字段
/// 索引为float:1
/// </summary>
public float r;
/// <summary>
/// 2号字段
/// 索引为float:2
/// </summary>
public float g;
/// <summary>
/// 3号字段
/// 索引为float:3
/// </summary>
public float b;
/// <summary>
/// 0号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.ColorArgb ConstructInstance(Injector injector)
{
return new global::Gorge.Native.GorgeFramework.ColorArgb(injector);
}
/// <summary>
/// 1号构造方法对应的工厂方法
/// </summary>
public new static global::Gorge.Native.GorgeFramework.ColorArgb ConstructInstance(Injector injector, float a, float r, float g, float b)
{
return new global::Gorge.Native.GorgeFramework.ColorArgb(injector, a, r, g, b);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
float injector_a;
if (injector.GetInjectorFloatDefault(0))
{
injector_a = InjectorFieldDefaultValue_a();
}
else
{
injector_a = injector.GetInjectorFloat(0);
}
float injector_r;
if (injector.GetInjectorFloatDefault(1))
{
injector_r = InjectorFieldDefaultValue_r();
}
else
{
injector_r = injector.GetInjectorFloat(1);
}
float injector_g;
if (injector.GetInjectorFloatDefault(2))
{
injector_g = InjectorFieldDefaultValue_g();
}
else
{
injector_g = injector.GetInjectorFloat(2);
}
float injector_b;
if (injector.GetInjectorFloatDefault(3))
{
injector_b = InjectorFieldDefaultValue_b();
}
else
{
injector_b = injector.GetInjectorFloat(3);
}
this.a = InitializeField_a(injector_a);this.r = InitializeField_r(injector_r);this.g = InitializeField_g(injector_g);this.b = InitializeField_b(injector_b);}
private static partial float InitializeField_a(float a);private static partial float InitializeField_r(float r);private static partial float InitializeField_g(float g);private static partial float InitializeField_b(float b);private static partial Dictionary<string, Metadata> InjectorFieldMetadata_a();/// <summary>
/// Injector的a字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_a();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_r();/// <summary>
/// Injector的r字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_r();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_g();/// <summary>
/// Injector的g字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_g();private static partial Dictionary<string, Metadata> InjectorFieldMetadata_b();/// <summary>
/// Injector的b字段的默认值生成方法
/// </summary>
/// <returns>该字段的默认值</returns>
private static partial float InjectorFieldDefaultValue_b();public override int GetIntField(int fieldIndex)
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
0 => a,
1 => r,
2 => g,
3 => b,
_ => base.GetFloatField(fieldIndex)
};
}
public override void SetFloatField(int fieldIndex, float value)
{
switch (fieldIndex)
{
case 0:
this.a = value;
return;
case 1:
this.r = value;
return;
case 2:
this.g = value;
return;
case 3:
this.b = value;
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
private static partial Annotation[] ClassAnnotations();
}
}
