using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class GraphAsset
    {
        public GraphAsset(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        public GraphAsset(String name) : base(name)
        {
            
        }
        
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        public virtual  partial Graph GetAsset()
        {
            throw new Exception("当前调用的方法实际是抽象的，暂未实现抽象方法机制");
        }
    }
}