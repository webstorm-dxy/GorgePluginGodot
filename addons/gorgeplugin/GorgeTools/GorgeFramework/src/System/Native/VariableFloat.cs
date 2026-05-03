using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class VariableFloat
    {
        protected VariableFloat(Injector injector)
        {
            FieldInitialize(injector);
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "可变数"),
            })
        };

        private static partial float InitializeField_baseValue(float baseValue) => baseValue;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_baseValue() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "基值")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_variationCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "变化曲线")
        };

        private static ConstructorInformation FunctionCurveInjectorConstructor
        {
            get
            {
                if (_functionCurveInjectorConstructor == null)
                {
                    var argumentList = Array.Empty<GorgeType>();
                    var constructors = FunctionCurve.Class.Declaration.GetInjectorConstructorByArgumentTypes(
                        RuntimeStatic.Runtime.LanguageRuntime, argumentList);
                    if (constructors.Length == 0)
                    {
                        throw new Exception(
                            $"FunctionCurve类没有参数表为{string.Join(",", argumentList.Select(a => a.ToString()))}的注入器方法");
                    }

                    if (constructors.Length > 1)
                    {
                        throw new Exception(
                            $"FunctionCurve类有多个参数表为{string.Join(",", argumentList.Select(a => a.ToString()))}的注入器方法");
                    }

                    _functionCurveInjectorConstructor = constructors[0];
                }

                return _functionCurveInjectorConstructor;
            }
        }

        private static ConstructorInformation _functionCurveInjectorConstructor;

        private static partial FunctionCurve InitializeField_variationCurve(
            Injector variationCurve)
        {
            if (variationCurve == null)
            {
                return null;
            }

            var injector = (Injector) variationCurve.RealObject;
            var index =
                injector.InjectedClassDeclaration.InjectorConstructorImplementationId[
                    FunctionCurveInjectorConstructor.Id];
            return FunctionCurve.FromGorgeObject(injector.Instantiate(index, Array.Empty<object>()));
        }

        private static partial Injector InjectorFieldDefaultValue_variationCurve() => null;

        /// <summary>
        ///     加值曲线计算
        /// </summary>
        /// <param name="curveTime"></param>
        /// <returns></returns>
        public virtual partial float EvaluateAdd(float curveTime)
        {
            if (variationCurve == null) return baseValue;

            return baseValue + variationCurve.Evaluate(curveTime);
        }

        /// <summary>
        ///     双向插值曲线计算，若为0则返回基值，若为1则返回上边界，若为-1则返回下边界
        /// </summary>
        /// <param name="curveTime"></param>
        /// <param name="min">下边界</param>
        /// <param name="max">上边界</param>
        /// <returns></returns>
        public virtual partial float EvaluateDoubleLerp(float curveTime, float min, float max)
        {
            if (variationCurve == null) return baseValue;

            var value = variationCurve.Evaluate(curveTime);
            return value switch
            {
                > 0 => Math.Lerp(baseValue, max, value),
                < 0 => Math.Lerp(min, baseValue, value + 1),
                _ => baseValue
            };
        }
    }
}