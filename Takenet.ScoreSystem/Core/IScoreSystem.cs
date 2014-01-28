using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.ScoreSystem.Core
{
    public interface IScoreSystem
    {
        Task<Pattern> IncludeOrChangePattern(string pattern, decimal value);
        Task RemovePattern(string pattern);
        Task<Transaction> Feedback(string clientId, string transactionId, TransactionStatus transactionStatus);
        Task<decimal> CheckScore(string clientId, string transactionId, string signature);
    }
}
