using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Note
    {
        public Note(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }
        
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial SignalTsiga InitializeField_automaton() => null;

        public virtual partial ObjectArray DoRespond(string respondMode, float respondChartTime)
        {
            throw new Exception("这事实上是个抽象方法，不应该被调用，但是没有提供抽象机制");
        }
    }
}