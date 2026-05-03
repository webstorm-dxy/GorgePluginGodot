using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class TimeItem
    {
        private TimeItem(Injector injector,GorgeDelegate time,bool accept, string respondMode)
        {
            FieldInitialize(injector);
            this.time = time;
            this.accept = accept;
            this.respondMode = respondMode;
        }
        
        public TimeItem()
        {
            
        }
        
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
        
        private static partial bool InitializeField_accept() => default;

        private static partial string InitializeField_respondMode() => default;

        private static partial GorgeDelegate InitializeField_time() => default;
    }
}