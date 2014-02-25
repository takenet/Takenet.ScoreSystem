using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using Takenet.ScoreSystem.Core;
using Takenet.ScoreSystem.Base.Repository;
using Takenet.ScoreSystem.Core.Exceptions;

namespace Takenet.ScoreSystem.Base
{
    
    public class ScoreSystemBase : IScoreSystem
    {
        private int _maxTransactionHistory;
        private const string TRANSACTION_CLEARING = "$¬Clearing¬$";
        private readonly IScoreSystemRepository _scoreSystemRepository;
        private Dictionary<byte, Dictionary<string,double>> _checkPatterns;

        public Dictionary<byte, Dictionary<string, double>> CheckPatterns
        {
            get
            {
                if (_checkPatterns == null)
                {
                    _checkPatterns = _scoreSystemRepository.FillPatterns(out _maxTransactionHistory);
                }
                return _checkPatterns;
            }
        }
        public ScoreSystemBase(IScoreSystemRepository scoreSystemRepository)
        {
            _scoreSystemRepository = scoreSystemRepository;
        }
        [Obsolete("You should use min history size and max history size now")]
        public async Task<Pattern> IncludeOrChangePattern(string pattern, double value, byte historySize)
        {
            var resultPattern = new Pattern(pattern, value, historySize,historySize);
            await _scoreSystemRepository.IncludeOrChangePattern(resultPattern);
            _checkPatterns = null;
            return resultPattern;
        }

        public async Task<Pattern> IncludeOrChangePattern(string pattern, double value, byte minHistorySize, byte maxHistorySize)
        {
            var resultPattern = new Pattern(pattern, value, minHistorySize,maxHistorySize);
            await _scoreSystemRepository.IncludeOrChangePattern(resultPattern);
            _checkPatterns = null;
            return resultPattern;
        }

        public async Task RemovePattern(string pattern)
        {
            await _scoreSystemRepository.RemovePattern(pattern);
            _checkPatterns = null;
        }

        public async Task<Transaction> Feedback(string clientId, string transactionId, TransactionStatus transactionStatus)
        {
            var transaction = _scoreSystemRepository.GetTransactionById(clientId, transactionId);
            if (transaction != null)
            {
                transaction.TransactionStatus = transactionStatus;
                await _scoreSystemRepository.IncludeOrChangeTransaction(transaction);
                return transaction;
            }
            return null;
        }

        public async Task<ScoreResult> CheckScore(string clientId, string transactionId, string signature, DateTime transactionDate)
        {
            var currentPattern = new StringBuilder();
            var patternsDictionary = CheckPatterns;
            var transaction = new Transaction(clientId, transactionId, signature,transactionDate);
            await _scoreSystemRepository.IncludeOrChangeTransaction(transaction);

            var clientTransactions = _scoreSystemRepository.GetClientTransactions(clientId, transactionDate, _maxTransactionHistory).ToList();

            var result = new ScoreResult();
            for (byte index = 0; index < clientTransactions.Count ; index++)
            {
                var clientTransaction = clientTransactions[index];
                if (clientTransaction.Signature == TRANSACTION_CLEARING)
                {
                    return result;
                }
                currentPattern.Insert(0, clientTransaction.Signature);
                Dictionary<string, double> patterns;
                if (patternsDictionary.TryGetValue((byte)(index + 1), out patterns))
                {
                    foreach (var pattern in patterns.Where(comparePattern => Regex.IsMatch(currentPattern.ToString(), comparePattern.Key)))
                    {
                        result.TotalScore += pattern.Value;
                        result.PatternMatchs.Add(new PatternMatch()
                        {
                            Pattern = pattern.Key,
                            Score = pattern.Value,
                            Signature = currentPattern.ToString()
                        });
                    }
                }
            }
            return result;
        }

        public Task IncludeClearingTransaction(string clientId,string transactionId, DateTime transactionDate)
        {
            var transaction = new Transaction(clientId, transactionId, TRANSACTION_CLEARING,transactionDate);
            return _scoreSystemRepository.IncludeOrChangeTransaction(transaction);
        }
    }
}
