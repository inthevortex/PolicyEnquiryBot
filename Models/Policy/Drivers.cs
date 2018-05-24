using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Policy
{
    public class Drivers
    {
        public Drivers()
        {
            Driver = new List<Driver>();
        }

        public List<Driver> Driver { get; set; }
    }

    public class Driver
    {
        [BsonElement]
        public string FirstName { get; set; }
        [BsonElement]
        public string LastName { get; set; }
        [BsonElement]
        public string AgeUSDriving { get; set; }
        [BsonElement]
        public string DateOfBirth { get; set; }
        [BsonElement]
        public string Sex { get; set; }
        [BsonElement]
        public string MaritalStatus { get; set; }
        [BsonElement]
        public string Relation { get; set; }
        [BsonElement]
        public string Occupation { get; set; }
        [BsonElement]
        public string Employer { get; set; }
        [BsonElement]
        public string DistanceToWork { get; set; }
        [BsonElement]
        public string TotalDriving { get; set; }
        [BsonElement]
        public List<string> Discount { get; set; }
        [BsonElement]
        public List<string> ViolationPoints { get; set; }
    }
}
