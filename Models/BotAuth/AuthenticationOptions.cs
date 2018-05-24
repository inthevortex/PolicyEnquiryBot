using System;

namespace Models.BotAuth
{
    [Serializable]
    public class AuthenticationOptions
    {
        public AuthenticationOptions() { }
        

        public AuthenticationOptions(string authority, string clientId, string clientSecret, string[] scopes, string redirectUrl)
        {
            Authority = authority;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scopes = scopes;
            RedirectUrl = redirectUrl;
        }

        public bool UseMagicNumber { get; } = true;
        public string ClientType { get; set; }
        public string Authority { get; set; }
        public string ResourceId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string[] Scopes { get; set; }
        public string RedirectUrl { get; set; }
        public string Policy { get; set; }
        public string MagicNumberView { get; set; } = string.Empty;
    }
}
