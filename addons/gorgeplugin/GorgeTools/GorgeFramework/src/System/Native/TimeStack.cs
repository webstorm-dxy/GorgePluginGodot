#nullable enable
using System;
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;
using JetBrains.Annotations;

namespace Gorge.Native.GorgeFramework
{
    public partial class TimeStack
    {
        private readonly Stack<TimeItem> _stack;

        private TimeStack(Injector injector, bool accept, string respondMode)
        {
            FieldInitialize(injector);
            _stack = new Stack<TimeItem>();
            Accept = accept;
            RespondMode = respondMode;
        }

        /// <summary>
        ///     时间栈
        /// </summary>
        /// <param name="accept">初始接收状态</param>
        /// <param name="respondMode">初始响应模式</param>
        public TimeStack(bool accept = false,
             string? respondMode = null)
        {
            _stack = new Stack<TimeItem>();
            Accept = accept;
            RespondMode = respondMode;
        }
        
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        /// <summary>
        ///     弹栈时间
        /// </summary>
        public float PopTime =>
            _stack.TryPeek(out var top) ? (float) top.time.Invoke(Array.Empty<object>()) : float.MaxValue;

        /// <summary>
        ///     弹栈，不尝试响应
        /// </summary>
        public bool TryPop(float chartTime, out TimeItem timeItem, HistoryStack historyStack)
        {
            if (!_stack.TryPop(out timeItem)) return false;
            var lastAccept = Accept;
            var lastRespondMode = RespondMode;
            Accept = timeItem.accept;
            RespondMode = timeItem.respondMode;
            historyStack.Push(new TimeStackPopHistory(chartTime, timeItem, lastAccept, lastRespondMode));
            return true;
        }

        /// <summary>
        /// 弹栈，不尝试响应，如果栈内无内容则返回null
        /// </summary>
        /// <param name="chartTime"></param>
        /// <param name="historyStack"></param>
        /// <returns></returns>
        public virtual partial TimeItem Pop(float chartTime, HistoryStack historyStack)
        {
            if (!_stack.TryPop(out var timeItem)) return null;
            var lastAccept = Accept;
            var lastRespondMode = RespondMode;
            Accept = timeItem.accept;
            RespondMode = timeItem.respondMode;
            historyStack.Push(new TimeStackPopHistory(chartTime, timeItem, lastAccept, lastRespondMode));
            // Debug.Log($"Pop ChartTime={chartTime} PopItem={timeItem.Time()} NowCount={_stack.Count}"+ (_stack.Count==0?"":$" Next={_stack.Peek().Time()}"));
            return timeItem;
        }

        public void RevertPop(TimeItem timeItem, bool acceptBeforePop, string respondModeBeforePop)
        {
            RespondMode = respondModeBeforePop;
            Accept = acceptBeforePop;
            _stack.Push(timeItem);
        }

        /// <summary>
        ///     将目标压栈
        /// </summary>
        /// <param name="chartTime">压栈发生时间</param>
        /// <param name="timeItem">压栈内容</param>
        /// <param name="historyStack"></param>
        public virtual partial void Push(float chartTime, TimeItem timeItem, HistoryStack historyStack)
        {
            _stack.Push(timeItem);
            historyStack.Push(new TimeStackPushHistory(chartTime));
        }

        /// <summary>
        /// 初始目标压栈，不会写对应的history
        /// </summary>
        /// <param name="timeItem"></param>
        public virtual partial void InitPush(TimeItem timeItem)
        {
            _stack.Push(timeItem);
        }

        public void RevertPush()
        {
            _stack.Pop();
        }

        #region 状态

        /// <summary>
        ///     接收模式
        /// </summary>
        public bool Accept { get; private set; }

        /// <summary>
        ///     响应模式，null代表不响应
        /// </summary>
        public string? RespondMode { get; private set; }

        #endregion
    }

    // /// <summary>
    // ///     时序项
    // /// </summary>
    // public class TimeItem
    // {
    //     /// <summary>
    //     ///     是否接收
    //     /// </summary>
    //     public bool Accept;
    //
    //     /// <summary>
    //     ///     响应结果，为null代表不响应
    //     /// </summary>
    //     public string RespondMode;
    //
    //     /// <summary>
    //     ///     出栈时间
    //     /// </summary>
    //     public Func<float> Time;
    // }

    // /// <summary>
    // ///     输入项的时间类型
    // /// </summary>
    // public enum TimeMode
    // {
    //     /// <summary>
    //     ///     在结束时间前捕获一次，捕获成功则跳转
    //     /// </summary>
    //     CatchBefore,
    //
    //     /// <summary>
    //     ///     在结束时间前始终存在，时间结束时跳转
    //     /// </summary>
    //     KeepUntil
    // }

    public class TimeStackPushHistory : IHistoryItem
    {
        public TimeStackPushHistory(float chartTime)
        {
            ChartTime = chartTime;
        }

        public float ChartTime { get; }

        void IHistoryItem.Revert(InputGraph inputGraph, TimeStack timeStack)
        {
            timeStack.RevertPush();
        }
    }

    public class TimeStackPopHistory : IHistoryItem
    {
        private readonly TimeItem _timeItem;
        private readonly bool _acceptBeforePop;
        private readonly string _respondModeBeforePop;

        public TimeStackPopHistory(float chartTime, TimeItem timeItem, bool acceptBeforePop,
            string respondModeBeforePop)
        {
            _timeItem = timeItem;
            _acceptBeforePop = acceptBeforePop;
            _respondModeBeforePop = respondModeBeforePop;
            ChartTime = chartTime;
        }

        public float ChartTime { get; }

        void IHistoryItem.Revert(InputGraph inputGraph, TimeStack timeStack)
        {
            timeStack.RevertPop(_timeItem, _acceptBeforePop, _respondModeBeforePop);
        }
    }
}