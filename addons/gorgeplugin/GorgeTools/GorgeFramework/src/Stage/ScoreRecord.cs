using System.Collections.Generic;

namespace Gorge.GorgeFramework.Stage
{
    /// <summary>
    ///     计分记录，随成绩上传
    /// </summary>
    public class ScoreRecord
    {
        public List<RespondRecord> Responds;

        /// <summary>
        ///     对该计分记录进行重新计分
        /// </summary>
        /// <param name="scoring">计分器</param>
        public void Score(IScoring scoring)
        {
            foreach (var respond in Responds) scoring.Respond(respond.Result);
        }
    }

    public class RespondRecord
    {
        public int Result;
    }
}