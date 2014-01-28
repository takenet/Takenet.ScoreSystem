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
        private Dictionary<string, double> _checkPatterns;

        public Dictionary<string, double> CheckPatterns
        {
            get
            {
                if (_checkPatterns == null)
                {
                    _checkPatterns = FillPatterns();
                }
                return _checkPatterns;
            }
            set
            {
                _checkPatterns = value;
            }
        }



        public ScoreSystemAzure(string azureConnectionString, string colletion)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureConnectionString);
            var client = cloudStorageAccount.CreateCloudTableClient();
            _table = client.GetTableReference(colletion);
            _table.CreateIfNotExists();
        }


        public async Task<Pattern> IncludeOrChangePattern(string pattern, double value)
        {
            var resultPattern = new Pattern(pattern, value);
            var insertPattern = TableOperation.InsertOrReplace(new Store.Pattern(resultPattern));
            var tableResult = await _table.ExecuteAsync(insertPattern);
            _checkPatterns = null;
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
            _checkPatterns = null;
        }

        public async Task<Transaction> Feedback(string clientId, string transactionId, TransactionStatus transactionStatus)
        {
            var transaction = GetTransactionById(clientId, transactionId);
            if (transaction != null)
            {
                transaction.TransactionStatus = transactionStatus;
                var operation = TableOperation.InsertOrReplace(transaction);
                await _table.ExecuteAsync(operation);
                return transaction;
            }
            return null;
        }


        public async Task<double> CheckScore(string clientId, string transactionId, string signature, DateTime transactionDate)
        {
            var pattern = new StringBuilder();
            Store.Transaction transaction = GetTransactionById(clientId, transactionId);
            if (transaction == null)
            {
                transaction = new Transaction(clientId, transactionId, signature,transactionDate);
            }
            else
            {
                transaction.Signature = signature;
            }
            var insertOperation = TableOperation.InsertOrReplace(transaction);
            var result = await _table.ExecuteAsync(insertOperation);
            var clientTransactions = GetClientTransactions(clientId,transactionDate);
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


        private IEnumerable<Store.Transaction> GetClientTransactions(string clientId, DateTime maxvalue)
        {
            
            var query =
                new TableQuery<Store.Transaction>()
                .Where(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal, clientId),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForDate("TransactionDate",QueryComparisons.LessThanOrEqual, maxvalue))
                ).Take(MAX_TRANSACTIONS);
            return _table.ExecuteQuery(query).Select(t =>  t);
        }

        private Dictionary<string, double> FillPatterns()
        {
            var query = new TableQuery<Store.Pattern>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, "pattern")).AsTableQuery();
            return _table.ExecuteQuery(query).ToDictionary(t => t.Signature,t => t.Value);
        }

        private Store.Transaction GetTransactionById(string clientId, string transactionId)
        {
            TableQuery<Store.Transaction> query =
                new TableQuery<Store.Transaction>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, clientId),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("TransactionId", QueryComparisons.Equal, transactionId)));
            return _table.ExecuteQuery(query).FirstOrDefault();
        }



    }
}
