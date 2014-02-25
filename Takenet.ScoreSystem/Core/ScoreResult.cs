using System.Collections.Generic;

namespace Takenet.ScoreSystem.Core
{
    public class ScoreResult
    {
        public ScoreResult()
        {
            TotalScore = 0;
            PatternMatchs = new List<PatternMatch>();
        }
        public double TotalScore { get; set; }
        public List<PatternMatch> PatternMatchs { get; set; }
    }
}
