using MongoDB.Bson.Serialization.Attributes;

namespace Models.Policy
{
    public class Claim
    {
        [BsonElement]
        public string Claimant { get; set; }
        [BsonElement]
        public string Coverage { get; set; }
        [BsonElement]
        public string DateReportedDate { get; set; }
        [BsonElement]
        public string DateReportedTime { get; set; }
        [BsonElement]
        public string ContactDate { get; set; }
        [BsonElement]
        public string Phone { get; set; }
        [BsonElement]
        public string ClaimantAge { get; set; }
        [BsonElement]
        public string Status { get; set; }
        [BsonElement]
        public string DateClosed { get; set; }
        [BsonElement]
        public string Attorney { get; set; }
        [BsonElement]
        public string ClaimantAddress { get; set; }
        [BsonElement]
        public string ClaimantCity { get; set; }
        [BsonElement]
        public string ClaimantState { get; set; }
        [BsonElement]
        public string ClaimantZip { get; set; }
        [BsonElement]
        public string PropertyDescription { get; set; }
        [BsonElement]
        public string DamageInjuryDescription { get; set; }
        [BsonElement]
        public string EstimateAmount { get; set; }
        [BsonElement]
        public string PropertyLocation { get; set; }
        [BsonElement]
        public bool OpenSuit { get; set; }
    }
}
