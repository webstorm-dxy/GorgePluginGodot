using System.Collections.Generic;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Stage
{
    /// <summary>
    ///     实现了基本计数器的计分器
    /// </summary>
    public abstract class ScoringCounter : IScoring
    {
        public Dictionary<int, int> RespondResultCount { get; } = new()
        {
            {RespondResult.Miss, 0},
            {RespondResult.Good, 0},
            {RespondResult.Perfect, 0},
            {RespondResult.BestPerfect, 0}
        };

        public int Combo { get; private set; }

        public int MaxCombo { get; private set; }

        public ScoreMilepost Milepost { get; private set; } = ScoreMilepost.MaxScore;

        public virtual void Respond(int respond)
        {
            RespondResultCount[respond] += 1;

            switch (respond)
            {
                default:
                case RespondResult.Miss:
                    Combo = 0;
                    Milepost = ScoreMilepost.Complete;
                    break;
                case RespondResult.Good:
                    Combo += 1;
                    Milepost = Milepost switch
                    {
                        ScoreMilepost.MaxScore or ScoreMilepost.AllPerfect => ScoreMilepost.FullCombo,
                        _ => Milepost
                    };
                    break;
                case RespondResult.Perfect:
                    Combo += 1;
                    Milepost = Milepost switch
                    {
                        ScoreMilepost.MaxScore => ScoreMilepost.AllPerfect,
                        _ => Milepost
                    };
                    break;
                case RespondResult.BestPerfect:
                    Combo += 1;
                    break;
            }

            MaxCombo = System.Math.Max(MaxCombo, Combo);
        }

        public abstract float Accuracy { get; protected set; }
        public abstract int Score { get; protected set; }
    }
}