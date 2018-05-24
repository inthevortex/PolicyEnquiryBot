using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Policy
{
    public class Losses
    {
        public Losses()
        {
            Loss = new List<Loss>();
        }

        [BsonElement]
        public List<Loss> Loss { get; set; }
    }

    public class Loss
    {
        public Loss()
        {
            Claim = new Claim();
        }

        [BsonElement]
        public string LossNumber { get; set; }
        [BsonElement]
        public string DateOfLossDate { get; set; }
        [BsonElement]
        public string DateOfLossTime { get; set; }
        [BsonElement]
        public string DateReported { get; set; }
        [BsonElement]
        public string ReportedBy { get; set; }
        [BsonElement]
        public bool SalvagePending { get; set; }
        [BsonElement]
        public bool SubrogationPending { get; set; }
        [BsonElement]
        public string LossLocation { get; set; }
        [BsonElement]
        public string AuthorityContacted { get; set; }
        [BsonElement]
        public string LossDesc { get; set; }
        [BsonElement]
        public string LossDescInsured { get; set; }
        [BsonElement]
        public string LossDescClaimant { get; set; }
        [BsonElement]
        public bool UnlistedDriver { get; set; }
        [BsonElement]
        public bool ExcludedDriver { get; set; }
        [BsonElement]
        public string DriverNumber { get; set; }
        [BsonElement]
        public string DriverName { get; set; }
        [BsonElement]
        public string DriverAddress { get; set; }
        [BsonElement]
        public string DriverPhoneNumber { get; set; }
        [BsonElement]
        public string DriverRelationToInsured { get; set; }
        [BsonElement]
        public string DriverDateOfBirth { get; set; }
        [BsonElement]
        public string DriverLicenseNumber { get; set; }
        [BsonElement]
        public string DriverPurposeOfUse { get; set; }
        [BsonElement]
        public bool DriverPermissionToUse { get; set; }
        [BsonElement]
        public string Age { get; set; }
        [BsonElement]
        public string Sex { get; set; }
        [BsonElement]
        public string MaritalStatus { get; set; }
        [BsonElement]
        public string VIN { get; set; }
        [BsonElement]
        public string Year { get; set; }
        [BsonElement]
        public string Make { get; set; }
        [BsonElement]
        public string Model { get; set; }
        [BsonElement]
        public string LicensePlates { get; set; }
        [BsonElement]
        public Claim Claim { get; set; }
    }
}
