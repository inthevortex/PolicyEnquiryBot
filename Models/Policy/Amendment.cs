using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Policy
{
    public class Amendment
    {
        public Amendment()
        {
            Vehicles = new Vehicles();
            Drivers = new Drivers();
        }

        [BsonElement]
        public bool Processed { get; set; }
        [BsonElement]
        public string Addess1 { get; set; }
        [BsonElement]
        public string Address2 { get; set; }
        [BsonElement]
        public string City { get; set; }
        [BsonElement]
        public string State { get; set; }
        [BsonElement]
        public string Zip { get; set; }
        [BsonElement]
        public string Agent { get; set; }
        [BsonElement]
        public string Premium { get; set; }
        [BsonElement]
        public string PremiumWithDiscount { get; set; }
        [BsonElement]
        public string ProRatedPremium { get; set; }
        [BsonElement]
        public string Fee { get; set; }
        [BsonElement]
        public string Charges { get; set; }
        [BsonElement]
        public string Total { get; set; }
        [BsonElement]
        public List<string> Discount { get; set; }
        [BsonElement]
        public Drivers Drivers { get; set; }
        [BsonElement]
        public Vehicles Vehicles { get; set; }
    }
}
