using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class FunctionCurve
    {
        protected FunctionCurve(Injector injector)
        {
            FieldInitialize(injector);
        }

        public FunctionCurve()
        {
        }
        
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        public virtual partial float Evaluate(float x)
        {
            throw new Exception("本方法事实上是abstract的，不应直接调用");
        }
    }
}