using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.ScoreSystem.Core
{
    public class Transaction
    {
        public Transaction(string clientId, string transactionId, string signature, DateTime transactionDate)
        {
            ClientId = clientId;
            TransactionId = transactionId;
            Signature = signature;
            TransactionDate = transactionDate;
            TransactionStatus = TransactionStatus.Inconclusive;
        }
        public string ClientId { get; set; }
        public string TransactionId { get; set; }
        public string Signature { get; set; }
        public DateTime TransactionDate { get; set; }

        public TransactionStatus TransactionStatus
        {
            get
            {
                return (TransactionStatus)TransactionStatusId;
            }
            set
            {
                TransactionStatusId = (int)value;
            }
        }

        public int TransactionStatusId { get; set; }
    }
}
