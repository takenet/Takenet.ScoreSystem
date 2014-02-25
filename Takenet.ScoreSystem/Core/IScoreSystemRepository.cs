using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.ScoreSystem.Core
{
    public interface IScoreSystemRepository
    {
        Task RemovePattern(string pattern);

        Dictionary<byte, Dictionary<string,double>> FillPatterns(out int maxTransactionHistory);

        Task<Pattern> GetCurrentPattern(string pattern);

        Task IncludeOrChangePattern(Pattern resultPattern);

        Task IncludeOrChangeTransaction(Transaction transaction);

        IEnumerable<Transaction> GetClientTransactions(string clientId, DateTime transactionDate, int maxTransactions);

        Transaction GetTransactionById(string clientId, string transactionId);
    }
}
