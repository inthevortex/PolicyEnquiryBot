using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Policy
{
    public class Vehicles
    {
        public Vehicles()
        {
            Vehicle = new List<Vehicle>();
        }

        public List<Vehicle> Vehicle { get; set; }
    }

    public class Vehicle
    {
        [BsonElement]
        public string DriverAssigned { get; set; }
        [BsonElement]
        public string Year { get; set; }
        [BsonElement]
        public string Type { get; set; }
        [BsonElement]
        public string Model { get; set; }
        [BsonElement]
        public string SubModel { get; set; }
        [BsonElement]
        public string Make { get; set; }
        [BsonElement]
        public string Engine { get; set; }
        [BsonElement]
        public string Symbol { get; set; }
        [BsonElement]
        public string Price { get; set; }
        [BsonElement]
        public string Mileage { get; set; }
        [BsonElement]
        public string Rental { get; set; }
        [BsonElement]
        public List<string> Discount { get; set; }
    }
}
