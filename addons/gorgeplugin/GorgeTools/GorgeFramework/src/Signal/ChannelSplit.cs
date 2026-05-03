using System.Collections.Generic;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.GorgeFramework.Signal
{
    /// <summary>
    /// 单通道信号切片
    /// 包含一段时间内的单个信号通道全部信息
    /// </summary>
    public class ChannelSplit : Dictionary<int, Fragment<GorgeObject>>
    {
        
    }
}