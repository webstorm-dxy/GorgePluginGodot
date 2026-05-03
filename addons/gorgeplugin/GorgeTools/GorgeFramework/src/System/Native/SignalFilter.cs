using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class SignalFilter
    {
        protected SignalFilter(Injector injector, GorgeDelegate priority, IntArray conditionTypes,
            GorgeDelegate endTime, int timeMode, bool acceptConsume, bool denyConsume)
        {
            FieldInitialize(injector);
            this.priority = priority;
            this.conditionTypes = conditionTypes;
            this.endTime = endTime;
            this.timeMode = timeMode;
            this.acceptConsume = acceptConsume;
            this.denyConsume = denyConsume;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
        private static partial bool InitializeField_acceptConsume() => default;
        private static partial bool InitializeField_denyConsume() => default;
        private static partial GorgeDelegate InitializeField_endTime() => default;
        private static partial GorgeDelegate InitializeField_priority() => default;
        private static partial int InitializeField_timeMode() => default;
        private static partial IntArray InitializeField_conditionTypes() => default;

        public virtual partial bool CanDetect(string channelName)
        {
            throw new Exception("实为abstract");
        }

        public virtual partial bool Detect(string channelName, int signalId, int conditionType, GorgeObject signalValue,
            GorgeObject lastSignalValue)
        {
            throw new Exception("实为abstract");
        }
    }
}