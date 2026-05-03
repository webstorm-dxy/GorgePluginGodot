using Gorge.GorgeFramework.Stage;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    /// 管理运行时场景数据和资源
    /// </summary>
    public class SceneManager
    {
        public void RuntimeInitialize()
        {
            // TODO 可能有相机和分数的初始化
            Scoring = new ScoringV1(501);
        }

        public void RuntimeDestruct()
        {
            // TODO 可能有相机和分数的初始化
        }


        public IScoring Scoring { get; private set; }
    }
}