namespace Gorge.GorgeFramework.Stage
{
    /// <summary>
    ///     与ScoreManager对接的分数显示器
    /// </summary>
    public interface IScoreDisplay
    {
        public void UpdateScore(int score);
        public void UpdateAccuracy(float accuracy);
        public void UpdateCombo(int combo);

        public void UpdateMilepost(ScoreMilepost milepost);
    }
}