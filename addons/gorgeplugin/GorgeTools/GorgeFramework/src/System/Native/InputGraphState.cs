using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class InputGraphState
    {
        public InputGraphState(Injector injector ,SignalFilter filter, InputGraphEdge acceptedEdge, InputGraphEdge deniedEdge)
        {
            FieldInitialize(injector);

            this.filter = filter;
            this.acceptedEdge = acceptedEdge;
            this.deniedEdge = deniedEdge;
        }

        public InputGraphState()
        {
            
        }
        
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
        
        private static partial SignalFilter InitializeField_filter() => default;

        private static partial InputGraphEdge InitializeField_acceptedEdge() => default;

        private static partial InputGraphEdge InitializeField_deniedEdge() => default;
    }
}