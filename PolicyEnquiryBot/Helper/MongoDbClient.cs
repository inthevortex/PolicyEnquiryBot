using System;
using System.Security.Authentication;
using MongoDB.Driver;
using static PolicyEnquiryBot.Helper.Helper;

namespace PolicyEnquiryBot.Helper
{
    public static class MongoDbClient
    {
        public static MongoClient GetMongoClient(string dbname) => new MongoClient(new MongoClientSettings
        {
            Server = new MongoServerAddress(GetSetting("host"), Convert.ToInt32(GetSetting("port"))),
            UseSsl = true,
            SslSettings = new SslSettings
            {
                EnabledSslProtocols = SslProtocols.Tls12
            },
            Credential = new MongoCredential(
                "SCRAM-SHA-1",
                new MongoInternalIdentity(dbname, GetSetting("uname")),
                new PasswordEvidence(GetSetting("password")))
        });
    }
}
