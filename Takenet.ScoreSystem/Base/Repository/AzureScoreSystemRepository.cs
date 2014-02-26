using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.ScoreSystem.Core;

namespace Takenet.ScoreSystem.Base.Repository
{
    public class AzureScoreSystemRepository : IScoreSystemRepository
    {
        private readonly CloudTable _table;

        public AzureScoreSystemRepository(string azureConnectionString, string colletion)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(azureConnectionString);
            var client = cloudStorageAccount.CreateCloudTableClient();
            _table = client.GetTableReference(colletion);
            _table.CreateIfNotExists();
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

        public Dictionary<byte, Dictionary<string, double>> FillPatterns(out int size)
        {
            var query = new TableQuery<Store.Pattern>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, "pattern"));
            //Getting max value of the patterns
            var listPatterns = _table.ExecuteQuery(query).ToList();
            size = listPatterns.Select(x => x.MaxHistorySize).Max();

            var result = new Dictionary<byte, Dictionary<string, double>>();
            for (byte i = 1; i <= size; i++)
            {
                result[i] =
                    listPatterns.Where(x => x.MaxHistorySize >= i && x.MinHistorySize <= i)
                        .ToDictionary(t => t.Signature, t => t.Value);
            }
            return result;
        }

        public async Task<Pattern> GetCurrentPattern(string pattern)
        {
            var retrievePattern = TableOperation.Retrieve<Store.Pattern>("pattern", pattern);
            var tableResult = await _table.ExecuteAsync(retrievePattern);
            return tableResult.Result as Pattern;
        }

        public async Task IncludeOrChangePattern(Pattern resultPattern)
        {
            var insertPattern = TableOperation.InsertOrReplace(new Store.Pattern(resultPattern));
            var tableResult = await _table.ExecuteAsync(insertPattern);
        }

        public async Task IncludeOrChangeTransaction(Transaction transaction)
        {
            var transactionAzure = GetTransaction(transaction.ClientId, transaction.TransactionId);

            if (transactionAzure == null)
            {
                transactionAzure = (Store.Transaction)transaction;
            }
            else
            {
                transactionAzure.Signature = transaction.Signature;
            }

            var operation = TableOperation.InsertOrReplace(transactionAzure);
            var tableresult = await _table.ExecuteAsync(operation);
        }

        public IEnumerable<Transaction> GetClientTransactions(string clientId, DateTime maxvalue, int maxTransactions)
        {
            var query = new TableQuery<Store.Transaction>()
                            .Where(
                            TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, clientId),
                            TableOperators.And,
                            TableQuery.GenerateFilterConditionForDate("TransactionDate", QueryComparisons.LessThanOrEqual, maxvalue))
                            ).Take(maxTransactions);
            return _table.ExecuteQuery(query).Select(t => (Transaction)t);
        }

        public Transaction GetTransactionById(string clientId, string transactionId)
        {
            return GetTransaction(clientId, transactionId);
        }

        private Store.Transaction GetTransaction(string clientId, string transactionId)
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
