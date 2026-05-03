using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Priority
    {
        private Priority(Injector injector ,GorgeDelegate getPriority)
        {
            FieldInitialize(injector);
            this.getPriority = getPriority;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial GorgeDelegate InitializeField_getPriority() => null;
    }
}