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
        Dictionary<string, double> FillPatterns();

        Task IncudeOrChangePattern(Pattern resultPattern);

        Task IncudeOrChangeTransaction(Transaction transaction);

        IEnumerable<Transaction> GetClientTransactions(string clientId, DateTime transactionDate, int maxTransactions);

        Transaction GetTransactionById(string clientId, string transactionId);
    }
}
