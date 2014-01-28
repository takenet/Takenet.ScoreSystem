using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Takenet.ScoreSystem.Core;

namespace Takenet.ScoreSystem.Store
{
    public class Transaction : TableEntity
    {
        public Transaction() : base()
        {
            
        }

        public Transaction(Core.Transaction transaction) : base()
        {
            Signature = transaction.Signature;
            TransactionStatus = transaction.TransactionStatus;
            TransactionId = transaction.TransactionId;
            TransactionDate = transaction.TransactionDate;
            this.RowKey = string.Format("{0:D19}-{1}", DateTime.MaxValue.Ticks - transaction.TransactionDate.Ticks,transaction.TransactionId);
            this.PartitionKey = transaction.ClientId;
        }

        public string ClientId { get { return this.PartitionKey; }  }
        public string TransactionId { get; set; }
        public string Signature { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        public static implicit operator Core.Transaction(Transaction transaction)
        {
            if (transaction == null)
            {
                return null;
            }
            return new Core.Transaction(transaction.ClientId,transaction.TransactionId,transaction.Signature, transaction.TransactionDate)
            {
                TransactionStatus = transaction.TransactionStatus
            };
        }
        public static implicit operator Transaction(Core.Transaction transaction)
        {
            if (transaction == null)
            {
                return null;
            }
            return new Transaction(transaction);
        }
    }
}
