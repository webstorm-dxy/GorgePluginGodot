using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class ElementLinePoint
    {
        public ElementLinePoint(Injector injector, float time, float position, float width)
        {
            FieldInitialize(injector);
            this.time = time;
            this.position = position;
            this.width = width;
        }

        private static partial float InitializeField_time()
        {
            return 0;
        }

        private static partial float InitializeField_position()
        {
            return 0;
        }

        private static partial float InitializeField_width()
        {
            return 0;
        }

        private static partial Annotation[] ClassAnnotations()
        {
            return Array.Empty<Annotation>();
        }
    }
}