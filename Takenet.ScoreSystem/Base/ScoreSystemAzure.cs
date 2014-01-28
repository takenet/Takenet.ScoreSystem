using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using Takenet.ScoreSystem.Core;
using Takenet.ScoreSystem.Base.Repository;

namespace Takenet.ScoreSystem.Base
{
    
    public class ScoreSystemBase : IScoreSystem
    {
        private const int MAX_TRANSACTIONS = 15;
        private IScoreSystemRepository _scoreSystemRepository;
        private Dictionary<string, double> _checkPatterns;

        public Dictionary<string, double> CheckPatterns
        {
            get
            {
                if (_checkPatterns == null)
                {
                    _checkPatterns = _scoreSystemRepository.FillPatterns();
                }
                return _checkPatterns;
            }
        }
        public ScoreSystemBase(IScoreSystemRepository scoreSystemRepository)
        {
            _scoreSystemRepository = scoreSystemRepository;
        }
        
        public async Task<Pattern> IncludeOrChangePattern(string pattern, double value)
        {
            var resultPattern = new Pattern(pattern, value);
            await _scoreSystemRepository.IncudeOrChangePattern(resultPattern);
            _checkPatterns = null;
            return resultPattern;
        }

        

        public async Task RemovePattern(string pattern)
        {
            _scoreSystemRepository.RemovePattern(pattern);
            _checkPatterns = null;
        }

        public async Task<Transaction> Feedback(string clientId, string transactionId, TransactionStatus transactionStatus)
        {
            var transaction = _scoreSystemRepository.GetTransactionById(clientId, transactionId);
            if (transaction != null)
            {
                transaction.TransactionStatus = transactionStatus;
                await _scoreSystemRepository.IncudeOrChangeTransaction(transaction);
                return transaction;
            }
            return null;
        }

        


        public async Task<double> CheckScore(string clientId, string transactionId, string signature, DateTime transactionDate)
        {
            var pattern = new StringBuilder();
            var transaction = new Transaction(clientId, transactionId, signature,transactionDate);
            _scoreSystemRepository.IncudeOrChangeTransaction(transaction);
            var clientTransactions = _scoreSystemRepository.GetClientTransactions(clientId,transactionDate,MAX_TRANSACTIONS);
            double resul = 0;            
            foreach (Transaction clientTransaction in clientTransactions)
            {
                pattern.Insert(0,clientTransaction.Signature);
                double value;
                CheckPatterns.TryGetValue(pattern.ToString(), out value);
                resul += value;
            }
            return resul;
        }


    }
}
