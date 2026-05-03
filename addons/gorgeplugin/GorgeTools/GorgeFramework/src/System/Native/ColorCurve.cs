using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class ColorCurve
    {
        public ColorCurve(Injector injector)
        {
            FieldInitialize(injector);
        }

        public virtual partial ColorArgb Evaluate(float x)
        {
            throw new Exception("本方法实际是abstract的");
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
    }
}