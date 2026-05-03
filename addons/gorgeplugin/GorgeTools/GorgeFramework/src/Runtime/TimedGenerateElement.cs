using Gorge.Native;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.GorgeFramework.Runtime
{
    /// <summary>
    /// 定时构造的元素
    /// </summary>
    public class TimedGenerateElement
    {
        public Injector Injector { get; }
        public ConstructorInformation Constructor { get; }
        private readonly GorgeDelegate _timeDelegate;

        /// <summary>
        /// 预定时间，只计算一次，计算后缓存
        /// 看起来是可以提前在运行之前完成计算的，但由于逻辑是任意编写的，所以没有办法确定能提前到的位置，可能只能提前到完全就绪后
        /// 但是这样的提前仍然可以支撑排序
        /// </summary>
        public float Time
        {
            get
            {
                if (!_timeIsCalculated)
                {
                    _time = (float)_timeDelegate.Invoke(Injector);
                    _timeIsCalculated = true;
                }
                
                return _time;
            }
        }

        private bool _timeIsCalculated = false;
        private float _time;
        
        public TimedGenerateElement(Injector injector, ConstructorInformation constructor, GorgeDelegate timeDelegate)
        {
            Injector = injector;
            Constructor = constructor;
            _timeDelegate = timeDelegate;
        }

        /// <summary>
        /// 删除已计算的创生时间
        /// </summary>
        public void RemoveCalculatedTime()
        {
            _timeIsCalculated = false;
        }
    }
}