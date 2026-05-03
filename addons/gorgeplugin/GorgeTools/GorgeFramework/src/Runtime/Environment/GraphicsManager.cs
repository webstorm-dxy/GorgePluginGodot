using System.Collections.Generic;
using Gorge.Native;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    /// 管理运行时图形数据
    /// </summary>
    public class GraphicsManager
    {
        /// <summary>
        /// 图形节点
        /// </summary>
        public List<Node> Nodes;

        public void StartSimulation()
        {
            Nodes = new List<Node>();
        }

        public void StopSimulation()
        {
            Nodes = null;
        }
    }
}