using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.ScoreSystem.Core
{
    public interface IScoreSystem
    {
        Task<Pattern> IncludeOrChangePattern(string pattern, double value);
        Task RemovePattern(string pattern);
        Task<Transaction> Feedback(string clientId, string transactionId, TransactionStatus transactionStatus);
        Task<double> CheckScore(string clientId, string transactionId, string signature, DateTime transactionDate);
    }
}
