using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class ElementLine
    {
        public ElementLine(Injector injector, ColorArgb color, ObjectArray points)
        {
            FieldInitialize(injector);
            this.color = color;
            this.points = points;
        }

        private static partial ColorArgb InitializeField_color()
        {
            return ColorArgb.White;
        }

        private static partial ObjectArray InitializeField_points()
        {
            return null;
        }

        private static partial Annotation[] ClassAnnotations()
        {
            return Array.Empty<Annotation>();
        }
    }
}