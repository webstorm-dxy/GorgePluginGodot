using System.Collections.Generic;

namespace Gorge.GorgeFramework.Signal
{
    /// <summary>
    /// 多通道信号切片
    /// 包含一段时间内的全部信号
    /// </summary>
    public class MultichannelSplit : Dictionary<string, ChannelSplit>
    {
    }
}