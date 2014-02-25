using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.ScoreSystem.Base;

namespace Takenet.ScoreSystem.Core
{
    public interface IScoreSystem
    {
        /// <summary>
        /// Includes a new pattern or change values to a existent one
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">The value.</param>
        /// <param name="historySize">Size of the history.</param>
        /// <returns></returns>
        [Obsolete("You should use min history size and max history size now")]
        Task<Pattern> IncludeOrChangePattern(string pattern, double value, byte historySize);

        /// <summary>
        /// Includes a new pattern or change values to a existent one
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">The value.</param>
        /// <param name="minHistorySize">Min value for the history size</param>
        /// <param name="maxHistorySize">Max value for the history size</param>
        /// <returns></returns>
        Task<Pattern> IncludeOrChangePattern(string pattern, double value, byte minHistorySize, byte maxHistorySize);

        /// <summary>
        /// Removes the pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        Task RemovePattern(string pattern);
        /// <summary>
        /// Feedbacks including the real transaction status.
        /// </summary>
        /// <param name="clientId">The client unique identifier.</param>
        /// <param name="transactionId">The transaction unique identifier.</param>
        /// <param name="transactionStatus">The transaction status.</param>
        /// <returns></returns>
        Task<Transaction> Feedback(string clientId, string transactionId, TransactionStatus transactionStatus);
        /// <summary>
        /// Checks the score os a signature for a client.
        /// This process will include the transaction being checked in the client transaction history
        /// </summary>
        /// <param name="clientId">The client unique identifier.</param>
        /// <param name="transactionId">The transaction unique identifier.</param>
        /// <param name="signature">The transaction signature.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <returns></returns>
        Task<ScoreResult> CheckScore(string clientId, string transactionId, string signature, DateTime transactionDate);

        /// <summary>
        /// Includes the clearing transaction.
        /// </summary>
        /// <param name="clientId">The client unique identifier.</param>
        /// <param name="transactionId"></param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <returns></returns>
        Task IncludeClearingTransaction(string clientId, string transactionId, DateTime transactionDate);
    }

}
