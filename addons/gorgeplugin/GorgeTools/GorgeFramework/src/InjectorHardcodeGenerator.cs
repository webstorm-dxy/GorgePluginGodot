using System;
using System.Collections.Generic;
using System.Text;
using Gorge.GorgeCompiler.Visitors;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.GorgeFramework
{
    /// <summary>
    /// Injector硬编码代码生成器
    /// </summary>
    public static class InjectorHardcodeGenerator
    {
        /// <summary>
        /// 生成Injector的硬编码代码
        /// </summary>
        /// <param name="injector"></param>
        /// <param name="indentation">缩进值</param>
        /// <returns></returns>
        public static string Generate(Injector injector, int indentation = 0)
        {
            var injectedClass = injector.InjectedClassDeclaration;

            var stringBuilder = new StringBuilder();

            // 记录是否填入了任意字段，用于处理空Injector情况
            var anyField = false;

            stringBuilder.AppendLine($"{injectedClass.Type.HardcodeType()} : {{");

            for (var i = 0; i < injectedClass.InjectorFieldCount; i++)
            {
                if (!injectedClass.TryGetInjectorFieldById(i, out var field))
                {
                    throw new Exception($"{injectedClass.Name}类没有编号为{i}的注入器字段");
                }

                var fieldIndex = field.Index;
                string fieldValueString;

                switch (field.Type.BasicType)
                {
                    case BasicType.Int:
                        if (injector.GetInjectorIntDefault(fieldIndex))
                        {
                            continue;
                        }

                        fieldValueString = injector.GetInjectorInt(field.Index).ToString();
                        break;
                    case BasicType.Float:
                        if (injector.GetInjectorFloatDefault(fieldIndex))
                        {
                            continue;
                        }

                        var floatValue = injector.GetInjectorFloat(field.Index);
                        if (floatValue == float.PositiveInfinity)
                        {
                            fieldValueString = "(1.0/0.0)";
                        }
                        else if (floatValue == float.NegativeInfinity)
                        {
                            fieldValueString = "(-1.0/0.0)";
                        }
                        else
                        {
                            fieldValueString = injector.GetInjectorFloat(field.Index).ToString("0.0######");
                        }

                        break;
                    case BasicType.Bool:
                        if (injector.GetInjectorBoolDefault(fieldIndex))
                        {
                            continue;
                        }

                        fieldValueString = injector.GetInjectorBool(field.Index) ? "true" : "false";
                        break;
                    case BasicType.Enum:
                        if (injector.GetInjectorIntDefault(fieldIndex))
                        {
                            continue;
                        }

                        var enumValue = RuntimeStatic.Runtime.LanguageRuntime.GetEnum(field.Type.FullName)
                            .Values[injector.GetInjectorInt(fieldIndex)];
                        fieldValueString = $"{field.Type.HardcodeType()}.{enumValue}";
                        break;
                    case BasicType.String:
                        if (injector.GetInjectorStringDefault(fieldIndex))
                        {
                            continue;
                        }

                        fieldValueString = LiteralHelper.StringToStringLiteral(injector.GetInjectorString(fieldIndex));

                        break;
                    case BasicType.Object:
                    case BasicType.Interface:
                    case BasicType.Delegate:
                        if (injector.GetInjectorObjectDefault(fieldIndex))
                        {
                            continue;
                        }

                        var value = injector.GetInjectorObject(fieldIndex);
                        if (value == null)
                        {
                            fieldValueString = "null";
                            break;
                        }

                        if (field.Type.BasicType is BasicType.Object)
                        {
                            switch (field.Type.FullName)
                            {
                                case "Gorge.Injector":
                                    fieldValueString = Generate((Injector) value, indentation + 1);
                                    break;
                                // case "Gorge.IntList":
                                //     fieldValueString = Generate((IntList) value, indentation + 1);
                                //     break;
                                // case "Gorge.StringList":
                                //     fieldValueString = Generate((StringList) value, indentation + 1);
                                //     break;
                                case "Gorge.ObjectList":
                                    fieldValueString = Generate((ObjectList) value, true, indentation + 1);
                                    break;
                                default:
                                    throw new Exception($"{field.Type}类型不能对非null值生成硬编码代码");
                            }
                        }
                        else
                        {
                            throw new Exception($"{field.Type}类型不能对非null值生成硬编码代码");
                        }

                        break;

                    default:
                        throw new Exception($"{field.Type}类型不能生成硬编码代码");
                }

                stringBuilder.AppendLine($"{field.Name} : {fieldValueString},", indentation + 1);
                anyField = true;
            }

            stringBuilder.Append("}", indentation);

            if (!anyField)
            {
                return $"{injectedClass.Type.HardcodeType()} : {{ : }}";
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 生成Injector的硬编码代码
        /// </summary>
        /// <param name="arrayInjector"></param>
        /// <param name="isValue">是否为值，即包含类型头，否则不包含(用于new)</param>
        /// <param name="indentation">缩进值</param>
        /// <returns></returns>
        public static string Generate(ObjectList arrayInjector, bool isValue = true, int indentation = 0)
        {
            var itemObjectType = arrayInjector.ItemClassType;

            if (arrayInjector.length == 0)
            {
                if (isValue)
                {
                    return $"{itemObjectType.HardcodeType()} : {{ , }}";
                }
                else
                {
                    return $"{{ , }}";
                }
            }

            var stringBuilder = new StringBuilder();

            if (isValue)
            {
                stringBuilder.AppendLine($"{itemObjectType.HardcodeType()} : {{");
            }
            else
            {
                stringBuilder.AppendLine($"{{");
            }

            for (var i = 0; i < arrayInjector.length; i++)
            {
                var item = arrayInjector.Get(i);
                string fieldValueString;

                switch (itemObjectType.BasicType)
                {
                    case BasicType.Object:
                    case BasicType.Interface:
                    case BasicType.Delegate:
                        if (item == null)
                        {
                            fieldValueString = "null";
                            break;
                        }

                        if (itemObjectType.BasicType is BasicType.Object)
                        {
                            switch (itemObjectType.FullName)
                            {
                                case "Gorge.Injector":
                                    fieldValueString = Generate((Injector) item, indentation + 1);
                                    break;
                                // case "Gorge.IntList":
                                //     fieldValueString = Generate((IntList) value, indentation + 1);
                                //     break;
                                // case "Gorge.StringList":
                                //     fieldValueString = Generate((StringList) value, indentation + 1);
                                //     break;
                                case "Gorge.ObjectList":
                                    fieldValueString = Generate((ObjectList) item, true, indentation + 1);
                                    break;
                                default:
                                    throw new Exception($"{itemObjectType}类型不能对非null值生成硬编码代码");
                            }
                        }
                        else
                        {
                            throw new Exception($"{itemObjectType}类型不能对非null值生成硬编码代码");
                        }

                        break;
                    case BasicType.Int:
                    case BasicType.Float:
                    case BasicType.Bool:
                    case BasicType.Enum:
                    case BasicType.String:
                    default:
                        throw new Exception($"{itemObjectType}类型序列注入器不能按对象序列注入器生成硬编码代码");
                }

                stringBuilder.AppendLine($"{fieldValueString},", indentation + 1);
            }

            stringBuilder.Append("}", indentation);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 生成Injector的硬编码代码
        /// </summary>
        /// <param name="itemObjectType"></param>
        /// <param name="arrayInjector"></param>
        /// <param name="isValue">是否为值，即包含类型头，否则不包含(用于new)</param>
        /// <param name="indentation">缩进值</param>
        /// <returns></returns>
        public static string Generate(GorgeType itemObjectType, List<Injector> arrayInjector, bool isValue = true,
            int indentation = 0)
        {
            if (arrayInjector.Count == 0)
            {
                if (isValue)
                {
                    return $"{itemObjectType.HardcodeType()} : {{ , }}";
                }
                else
                {
                    return $"{{ , }}";
                }
            }

            var stringBuilder = new StringBuilder();

            if (isValue)
            {
                stringBuilder.AppendLine($"{itemObjectType.HardcodeType()} : {{");
            }
            else
            {
                stringBuilder.AppendLine($"{{");
            }

            for (var i = 0; i < arrayInjector.Count; i++)
            {
                var item = arrayInjector[i];
                var fieldValueString = Generate(item, indentation + 1);

                stringBuilder.AppendLine($"{fieldValueString},", indentation + 1);
            }

            stringBuilder.Append("}", indentation);

            return stringBuilder.ToString();
        }
    }
}