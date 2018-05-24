using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Policy
{
    public class Policies
    {
        public Policies()
        {
            Losses = new Losses();
            Transactions = new Transactions();
        }

        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public string PolicyNumber { get; set; }
        [BsonElement]
        public string Effective { get; set; }
        [BsonElement]
        public string Term { get; set; }
        [BsonElement]
        public string Expiration { get; set; }
        [BsonElement]
        public string ExpirationDate { get; set; }
        [BsonElement]
        public string Agent { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public string DateOfBirth { get; set; }
        [BsonElement]
        public string Status { get; set; }
        [BsonElement]
        public string CancellationType { get; set; }
        [BsonElement]
        public string Premium { get; set; }
        [BsonElement]
        public string Fee { get; set; }
        [BsonElement]
        public string Charge { get; set; }
        [BsonElement]
        public string Total { get; set; }
        [BsonElement]
        public string PaidPremium { get; set; }
        [BsonElement]
        public string PaidFee { get; set; }
        [BsonElement]
        public string PaidCharge { get; set; }
        [BsonElement]
        public string PaidTotal { get; set; }
        [BsonElement]
        public string NoOfPayments { get; set; }
        [BsonElement]
        public string PaymentsMade { get; set; }
        [BsonElement]
        public Losses Losses { get; set; }
        [BsonElement]
        public Transactions Transactions { get; set; }
    }
}
