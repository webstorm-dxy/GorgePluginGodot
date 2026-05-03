using System;
using Gorge.GorgeFramework.Simulators;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.GorgeFramework.Automaton
{
    /// <summary>
    ///     信号检测条件
    /// </summary>
    public class SignalDetectionCondition
    {
        /// <summary>
        ///     检测接收回调
        ///     输入是触发接收的信道、信号编号、电平、接收时间
        ///     输出是需要执行的动作
        /// </summary>
        public Func<string, int, GorgeObject, float, IGameplayAction[]> Accept;

        /// <summary>
        /// 信道过滤
        /// 输入是信道名
        /// 输出是是否参与检测
        /// </summary>
        public Func<string, bool> CanDetect;

        /// <summary>
        ///     检测方法。
        ///     输入值为信道、信号编号和电平。
        ///     输出值1为是否可接受
        ///     输出值2为是否遮挡信号
        /// </summary>
        public Func<string, int, GorgeObject, Tuple<bool, bool>> Detect;

        public ObjectArray Priority;
    }
}