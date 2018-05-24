using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Policy
{
    public class Transactions
    {
        public Transactions()
        {
            Transaction = new List<Transaction>();
        }

        [BsonElement]
        public List<Transaction> Transaction { get; set; }
    }

    public class Transaction
    {
        public Transaction()
        {
            Amendment = new Amendment();
        }

        [BsonElement]
        public string TransactionNumber { get; set; }
        [BsonElement]
        public string TransactionDate { get; set; }
        [BsonElement]
        public string TransactionTime { get; set; }
        [BsonElement]
        public string Type { get; set; }
        [BsonElement]
        public Amendment Amendment { get; set; }
        [BsonElement]
        public string PaymentBy { get; set; }
        [BsonElement]
        public string PaymentDate { get; set; }
        [BsonElement]
        public string PaymentNumber { get; set; }
        [BsonElement]
        public string PaidPremium { get; set; }
        [BsonElement]
        public string PaidFee { get; set; }
        [BsonElement]
        public string PaidCharges { get; set; }
        [BsonElement]
        public string PaidTotal { get; set; }
        [BsonElement]
        public string PaidAgentCharges { get; set; }
        [BsonElement]
        public string NextDueDate { get; set; }
        [BsonElement]
        public string NextDueAmount { get; set; }
    }
}
