using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class DeriveElementCommand
    {
        public DeriveElementCommand(Injector injector, Element element, bool changeAutomaton)
        {
            FieldInitialize(injector);
            this.element = element;
            this.changeAutomaton = changeAutomaton;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
        private static partial Element InitializeField_element() => default;

        private static partial bool InitializeField_changeAutomaton() => default;
    }
}