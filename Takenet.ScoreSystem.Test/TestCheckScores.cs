using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Takenet.ScoreSystem.Base;
using Takenet.ScoreSystem.Base.Repository;

namespace Takenet.ScoreSystem.Test
{
    [TestClass]
    public class TestCheckScores
    {
        [TestMethod]
        public void TesteBaseFirstPatternMatch()
        {
            var scoreSystem = GenerateScoreSystem();
            Assert.AreEqual(10, scoreSystem.CheckScore("1", "1", "AAA", DateTime.Parse("2014-01-01")).Result.TotalScore);
        }


        [TestMethod]
        public void TesteBaseFirstPatternNoMatch()
        {
            var scoreSystem = GenerateScoreSystem();
            Assert.AreEqual(0, scoreSystem.CheckScore("2", "1", "AAB", DateTime.Parse("2014-01-01")).Result.TotalScore);
        }

        [TestMethod]
        public void TesteBaseFirstPatternNoMatchSecondPattern2Matches()
        {
            var scoreSystem = GenerateScoreSystem();
            Assert.AreEqual(15, scoreSystem.CheckScore("2", "2", "AAA", DateTime.Parse("2014-01-02")).Result.TotalScore);
        }

        [TestMethod]
        public void TesteBaseFirstPatternMatchSecondPattern2Matches()
        {
            var scoreSystem = GenerateScoreSystem();
            Assert.AreEqual(20, scoreSystem.CheckScore("1", "2", "AAA", DateTime.Parse("2014-01-02")).Result.TotalScore);
        }

        [TestMethod]
        public void TesteClearingPatternTest()
        {
            var scoreSystem = GenerateScoreSystem();
            scoreSystem.IncludeClearingTransaction("1","3",DateTime.Parse("2014-01-03")).Wait();
            Assert.AreEqual(10, scoreSystem.CheckScore("1", "4", "AAA", DateTime.Parse("2014-01-04")).Result.TotalScore);
        }

        private static ScoreSystemBase GenerateScoreSystem()
        {
            var scoreSystem = new ScoreSystemBase(new AzureScoreSystemRepository("DefaultEndpointsProtocol=https;AccountName=testscoresystem;AccountKey=pxafdfvMnB69SIGpXTGTWkrH/DRmx1IuB7FM2QufLZOGcs7OljtGWsyIDBZ2N42Joi+ZZB71MemzR6bdRc3UNw==", "test"));
            SetupPatterns(scoreSystem);
            return scoreSystem;
        }

        private static void SetupPatterns(ScoreSystemBase scoreSystem)
        {
            scoreSystem.IncludeOrChangePattern("AAA", 10, 1,1).Wait();
            scoreSystem.IncludeOrChangePattern("AABAAA", 5, 2,2).Wait();
            scoreSystem.IncludeOrChangePattern("AAAAAA", 10, 2,2).Wait();
        }



    }
}
