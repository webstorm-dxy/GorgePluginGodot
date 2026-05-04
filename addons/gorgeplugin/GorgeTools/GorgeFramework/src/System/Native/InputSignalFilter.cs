using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class InputSignalFilter
    {
        private InputSignalFilter(Injector injector, GorgeDelegate priority, IntArray touchType,
            GorgeDelegate touchArea, GorgeDelegate endTime, int timeMode, bool acceptConsume, bool denyConsume) : base(
            injector, priority, touchType, endTime, timeMode, acceptConsume, denyConsume)
        {
            FieldInitialize(injector);
            this.touchArea = touchArea;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
        private static partial GorgeDelegate InitializeField_touchArea() => default;

        public override partial bool CanDetect(string channelName)
        {
            return channelName is "Touch";
        }

        public override partial bool Detect(string channelName, int signalId, int conditionType,
            GorgeObject signalValue, GorgeObject lastSignalValue)
        {
            if (signalValue == null)
            {
                return false;
            }

            if (signalValue is not TouchSignal touchValue || lastSignalValue is not TouchSignal and not null)
            {
                throw new Exception();
            }

            var lastTouchValue = (TouchSignal) lastSignalValue;

            switch (conditionType)
            {
                case TouchType.Begin:
                    /*
                     * 按下检测条件：
                     *   前后不为null，这由信号采集保证，必然会录制出边沿
                     *   前值是非触摸状态，现值是触摸状态
                     *   现值需在空间范围内
                     */
                    return lastTouchValue is {isTouching: false} && touchValue is {isTouching: true} &&
                           (bool) touchArea.Invoke(signalValue);
                case TouchType.Keep:

                    /*
                     * 保持检测条件：
                     *   现值是触摸状态且现值需在空间范围内
                     */
                    return touchValue is {isTouching: true} && (bool) touchArea.Invoke(signalValue);

                case TouchType.End:
                    /*
                     * 弹起检测条件：
                     *   前后不为null，这由信号采集保证，必然会录制出边沿
                     *   前值是触摸状态，现值是非触摸状态
                     *   前值需在空间范围内
                     */
                    return lastTouchValue is {isTouching: true} && touchValue is {isTouching: false} &&
                           (bool) touchArea.Invoke(lastSignalValue);

                default:
                    // 非已知情况不接收
                    return false;
            }
        }
    }
}
