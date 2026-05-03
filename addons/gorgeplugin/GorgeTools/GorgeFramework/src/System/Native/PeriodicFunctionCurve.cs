using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class PeriodicFunctionCurve
    {
        public PeriodicFunctionCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial FunctionCurve InitializeField_functionCurve(Injector functionCurve)
        {
            if (functionCurve == null)
            {
                return null;
            }

            var gorgeClass =
                RuntimeStatic.Runtime.LanguageRuntime.GetClass(functionCurve.InjectedClassDeclaration.Name);
            var realIndex = gorgeClass.Declaration.InjectorConstructorImplementationId[0];
            return FunctionCurve.FromGorgeObject(functionCurve.Instantiate(realIndex));
        }

        private static partial float InitializeField_startX(float startX)
        {
            return startX;
        }

        private static partial float InitializeField_endX(float endX)
        {
            return endX;
        }

        private static partial bool InitializeField_leftClosed(bool leftClosed)
        {
            return leftClosed;
        }

        private static partial Injector InjectorFieldDefaultValue_functionCurve()
        {
            return null;
        }

        private static partial float InjectorFieldDefaultValue_startX()
        {
            return 0;
        }

        private static partial float InjectorFieldDefaultValue_endX()
        {
            return 1;
        }

        private static partial bool InjectorFieldDefaultValue_leftClosed()
        {
            return true;
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_functionCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "原函数")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startX() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "周期左边界")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endX() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "周期右边界")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_leftClosed() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "左包含")
        };

        public override partial float Evaluate(float x)
        {
            if (functionCurve == null)
            {
                return 0;
            }

            var range = endX - startX;

            var realX = (x - startX) % range + startX;
            if (leftClosed)
            {
                if (realX < startX)
                {
                    realX += range;
                }
            }
            else
            {
                if (realX <= startX)
                {
                    realX += range;
                }
            }

            return functionCurve.Evaluate(realX);
        }

        private static partial Annotation[] ClassAnnotations()
        {
            return new[]
            {
                new Annotation("Editable", null, new Dictionary<string, Metadata>()
                {
                    ["displayName"] = new Metadata(GorgeType.String, "displayName", "周期函数"),
                })
            };
        }
    }
}