using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class InputGraphEdge
    {
        public InputGraphEdge(Injector injector, bool deny, int jump, GorgeDelegate stackAction, bool accept,
            bool stackRespond, bool edgeRespond, string exportState)
        {
            FieldInitialize(injector);

            this.deny = deny;
            this.jump = jump;
            this.stackAction = stackAction;
            this.accept = accept;
            this.stackRespond = stackRespond;
            this.edgeRespond = edgeRespond;
            this.exportState = exportState;
        }

        public InputGraphEdge()
        {
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
        
        private static partial bool InitializeField_accept() => default;

        private static partial bool InitializeField_edgeRespond() => default;

        private static partial string InitializeField_exportState() => default;

        private static partial bool InitializeField_deny() => default;

        private static partial int InitializeField_jump() => default;

        private static partial GorgeDelegate InitializeField_stackAction() => default;

        private static partial bool InitializeField_stackRespond() => default;

        public override string ToString()
        {
            return
                $"{nameof(deny)}: {deny}, {nameof(jump)}: {jump}, {nameof(stackAction)}: {stackAction}, {nameof(accept)}: {accept}, {nameof(stackRespond)}: {stackRespond}, {nameof(edgeRespond)}: {edgeRespond}, {nameof(exportState)}: {exportState}";
        }
    }
}