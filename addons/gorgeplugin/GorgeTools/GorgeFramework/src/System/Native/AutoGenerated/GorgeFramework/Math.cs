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
public partial class Math : GorgeObject
{
public new class Implementation : GorgeClass
{
public static GorgeType Type() => GorgeType.Object("Math", "GorgeFramework", new GorgeType[]{});
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
},
methods:
new MethodInformation[]
{
}
,
staticMethods:
new MethodInformation[]
{
new MethodInformation(
id: 0,
name: "Abs",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 1,
name: "Sqrt",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 2,
name: "Max",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f1",
type: GorgeType.Float,
index: 0
),
new ParameterInformation(
id: 1,
name: "f2",
type: GorgeType.Float,
index: 1
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 3,
name: "Max",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f1",
type: GorgeType.Float,
index: 0
),
new ParameterInformation(
id: 1,
name: "f2",
type: GorgeType.Float,
index: 1
),
new ParameterInformation(
id: 2,
name: "f3",
type: GorgeType.Float,
index: 2
),
new ParameterInformation(
id: 3,
name: "f4",
type: GorgeType.Float,
index: 3
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 4,
name: "Min",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f1",
type: GorgeType.Float,
index: 0
),
new ParameterInformation(
id: 1,
name: "f2",
type: GorgeType.Float,
index: 1
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 5,
name: "Atan",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 6,
name: "Sin",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 7,
name: "Cos",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 8,
name: "CosDeg",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 9,
name: "SinDeg",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 10,
name: "Pi",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 11,
name: "FloatPositiveInfinity",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 12,
name: "FloatNegativeInfinity",
returnType: GorgeType.Float,
parameters: new ParameterInformation[]
{
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 13,
name: "Floor",
returnType: GorgeType.Int,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 14,
name: "Ceil",
returnType: GorgeType.Int,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "f",
type: GorgeType.Float,
index: 0
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 15,
name: "ClampInt",
returnType: GorgeType.Int,
parameters: new ParameterInformation[]
{
new ParameterInformation(
id: 0,
name: "a",
type: GorgeType.Int,
index: 0
),
new ParameterInformation(
id: 1,
name: "b",
type: GorgeType.Int,
index: 1
),
new ParameterInformation(
id: 2,
name: "value",
type: GorgeType.Int,
index: 2
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 16,
name: "Lerp",
returnType: GorgeType.Float,
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
name: "b",
type: GorgeType.Float,
index: 1
),
new ParameterInformation(
id: 2,
name: "t",
type: GorgeType.Float,
index: 2
),
},
annotations: new Annotation[]{}
),
new MethodInformation(
id: 17,
name: "InverseLerp",
returnType: GorgeType.Float,
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
name: "b",
type: GorgeType.Float,
index: 1
),
new ParameterInformation(
id: 2,
name: "v",
type: GorgeType.Float,
index: 2
),
},
annotations: new Annotation[]{}
),
}
,
constructors:
new ConstructorInformation[]
{
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
annotations: global::Gorge.Native.GorgeFramework.Math.ClassAnnotations(),
fieldIndexCount:
new TypeCount(
intCount: 0,
floatCount: 0,
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
staticMethodCount: 18,
constructorCount: 0,
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
global::Gorge.Native.GorgeFramework.Math Math;
if(gorgeObject is global::Gorge.Native.GorgeFramework.Math)
{
Math = (global::Gorge.Native.GorgeFramework.Math) gorgeObject;
}
else if(gorgeObject is CompiledGorgeObject u)
{
Math = (global::Gorge.Native.GorgeFramework.Math) u.NativeObject;
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
case 0:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Abs(InvokeParameterPool.Float[0]);
break;
case 1:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Sqrt(InvokeParameterPool.Float[0]);
break;
case 2:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Max(InvokeParameterPool.Float[0], InvokeParameterPool.Float[1]);
break;
case 3:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Max(InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2], InvokeParameterPool.Float[3]);
break;
case 4:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Min(InvokeParameterPool.Float[0], InvokeParameterPool.Float[1]);
break;
case 5:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Atan(InvokeParameterPool.Float[0]);
break;
case 6:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Sin(InvokeParameterPool.Float[0]);
break;
case 7:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Cos(InvokeParameterPool.Float[0]);
break;
case 8:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.CosDeg(InvokeParameterPool.Float[0]);
break;
case 9:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.SinDeg(InvokeParameterPool.Float[0]);
break;
case 10:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Pi();
break;
case 11:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.FloatPositiveInfinity();
break;
case 12:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.FloatNegativeInfinity();
break;
case 13:
InvokeParameterPool.IntReturn = GorgeFramework.Math.Floor(InvokeParameterPool.Float[0]);
break;
case 14:
InvokeParameterPool.IntReturn = GorgeFramework.Math.Ceil(InvokeParameterPool.Float[0]);
break;
case 15:
InvokeParameterPool.IntReturn = GorgeFramework.Math.ClampInt(InvokeParameterPool.Int[0], InvokeParameterPool.Int[1], InvokeParameterPool.Int[2]);
break;
case 16:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.Lerp(InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2]);
break;
case 17:
InvokeParameterPool.FloatReturn = GorgeFramework.Math.InverseLerp(InvokeParameterPool.Float[0], InvokeParameterPool.Float[1], InvokeParameterPool.Float[2]);
break;
default:
throw new Exception($"类{Declaration.Name}不存在编号为{methodId}的静态方法");
}
}
protected override GorgeObject DoConstruct(GorgeObject targetObject, int constructorId)
{
var injector = InvokeParameterPool.Injector;
switch (constructorId)
{
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
public override ClassDeclaration InjectedClassDeclaration { get; } = global::Gorge.Native.GorgeFramework.Math.Class.Declaration;
public override GorgeObject Instantiate(int constructorIndex, params object[] args)
{
return constructorIndex switch
{
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
var injector = new global::Gorge.Native.GorgeFramework.Math.SpecificInjector();
CloneTo(injector);
return injector;
}
public void CloneTo(global::Gorge.Native.GorgeFramework.Math.SpecificInjector toInjector)
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
public new static global::Gorge.Native.GorgeFramework.Math FromGorgeObject(GorgeObject gorgeObject)
{
if (gorgeObject is CompiledGorgeObject u)
{
return (global::Gorge.Native.GorgeFramework.Math) u.NativeObject;
}
return (global::Gorge.Native.GorgeFramework.Math) gorgeObject;
}
public override void InvokeMethod(int methodIndex)
{
Class.InvokeMethod(this, methodIndex);
}
/// <summary>
/// 字段初始化方法，应当被构造方法的实现首先调用
/// </summary>
private void FieldInitialize(Injector injector)
{
}
public override int GetIntField(int fieldIndex)
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
public static partial float Abs(float f);
/// <summary>
/// 1号方法
/// </summary>
/// <returns></returns>
public static partial float Sqrt(float f);
/// <summary>
/// 2号方法
/// </summary>
/// <returns></returns>
public static partial float Max(float f1, float f2);
/// <summary>
/// 3号方法
/// </summary>
/// <returns></returns>
public static partial float Max(float f1, float f2, float f3, float f4);
/// <summary>
/// 4号方法
/// </summary>
/// <returns></returns>
public static partial float Min(float f1, float f2);
/// <summary>
/// 5号方法
/// </summary>
/// <returns></returns>
public static partial float Atan(float f);
/// <summary>
/// 6号方法
/// </summary>
/// <returns></returns>
public static partial float Sin(float f);
/// <summary>
/// 7号方法
/// </summary>
/// <returns></returns>
public static partial float Cos(float f);
/// <summary>
/// 8号方法
/// </summary>
/// <returns></returns>
public static partial float CosDeg(float f);
/// <summary>
/// 9号方法
/// </summary>
/// <returns></returns>
public static partial float SinDeg(float f);
/// <summary>
/// 10号方法
/// </summary>
/// <returns></returns>
public static partial float Pi();
/// <summary>
/// 11号方法
/// </summary>
/// <returns></returns>
public static partial float FloatPositiveInfinity();
/// <summary>
/// 12号方法
/// </summary>
/// <returns></returns>
public static partial float FloatNegativeInfinity();
/// <summary>
/// 13号方法
/// </summary>
/// <returns></returns>
public static partial int Floor(float f);
/// <summary>
/// 14号方法
/// </summary>
/// <returns></returns>
public static partial int Ceil(float f);
/// <summary>
/// 15号方法
/// </summary>
/// <returns></returns>
public static partial int ClampInt(int a, int b, int value);
/// <summary>
/// 16号方法
/// </summary>
/// <returns></returns>
public static partial float Lerp(float a, float b, float t);
/// <summary>
/// 17号方法
/// </summary>
/// <returns></returns>
public static partial float InverseLerp(float a, float b, float v);
private static partial Annotation[] ClassAnnotations();
}
}
