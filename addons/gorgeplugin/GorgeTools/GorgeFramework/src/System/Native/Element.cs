using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Element
    {
        protected Element(Injector injector)
        {
            FieldInitialize(injector);
        }
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial ObjectArray InitializeField_nodes() => null;

        private static partial ElementSimulator InitializeField_simulator() => null;

        private static partial ElementSimulator InitializeField_lateIndependentSimulator()=> null;
    }
}