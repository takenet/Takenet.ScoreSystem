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

namespace Takenet.ScoreSystem.Base
{
    
    public class ScoreSystemAzure : IScoreSystem
    {
        private const int MAX_TRANSACTIONS = 15;
        private CloudTable _table;
        private Dictionary<string, decimal> _checkPatterns;

        public Dictionary<string, decimal> CheckPatterns
        {
            get
            {
                if (_checkPatterns == null)
                {
                    _checkPatterns = FillPatterns();
                }
                return _checkPatterns;
            }
        }

        public ScoreSystemAzure(string azureConnectionString, string colletion)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureConnectionString);
            var client = cloudStorageAccount.CreateCloudTableClient();
            _table = client.GetTableReference(colletion);
            _table.CreateIfNotExists();
        }


        public async Task<Pattern> IncludeOrChangePattern(string pattern, decimal value)
        {
            var resultPattern = new Pattern(pattern, value);
            var insertPattern = TableOperation.InsertOrReplace(new Store.Pattern(resultPattern));
            var tableResult = await _table.ExecuteAsync(insertPattern);
            return resultPattern;
        }

        public async Task RemovePattern(string pattern)
        {
            var retrievePattern = TableOperation.Retrieve<Store.Pattern>("pattern", pattern);
            var tableResult = await _table.ExecuteAsync(retrievePattern);
            if (tableResult.Result != null)
            {
                var operation = TableOperation.Delete((Store.Pattern)tableResult.Result);
                var result = await _table.ExecuteAsync(operation);
            }
        }

        public async Task<Transaction> Feedback(string clientId, string transactionId, TransactionStatus transactionStatus)
        {
            TableQuery<Store.Transaction> query = new TableQuery<Store.Transaction>().Where(t => t.PartitionKey == clientId && t.TransactionId == transactionId).AsTableQuery();
            foreach (Store.Transaction transaction in _table.ExecuteQuery(query))
            {
                transaction.TransactionStatus = transactionStatus;
                var operation = TableOperation.InsertOrReplace(transaction);
                await _table.ExecuteAsync(operation);
                return transaction;
            }
            return null;
        }

        public async Task<decimal> CheckScore(string clientId, string transactionId, string signature)
        {
            var pattern = new StringBuilder();
            var transaction = new Transaction(clientId, transactionId, signature);

            var insertOperation = TableOperation.InsertOrReplace((Store.Transaction)transaction);
            var result = await _table.ExecuteAsync(insertOperation);
            var clientTransactions = GetClientTransactions(clientId);
            decimal resul = 0;            
            foreach (Transaction clientTransaction in clientTransactions)
            {
                pattern.Insert(0,clientTransaction.Signature);
                decimal value;
                CheckPatterns.TryGetValue(pattern.ToString(), out value);
                resul += value;
            }
            return resul;
        }


        private IEnumerable<Transaction> GetClientTransactions(string clientId)
        {
            var query = new TableQuery<Store.Transaction>().Where(c => c.PartitionKey == clientId).Take(MAX_TRANSACTIONS).AsTableQuery();
            return _table.ExecuteQuery(query).Select(t => (Transaction) t);
        }

        private Dictionary<string, decimal> FillPatterns()
        {
            var query = new TableQuery<Store.Pattern>().Where(p => p.PartitionKey == "pattern").AsTableQuery();
            return _table.ExecuteQuery(query).ToDictionary(t => t.Signature,t => t.Value);
        }


    }
}
