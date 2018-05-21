using Newtonsoft.Json;
using PolicyEnquiryBot.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PolicyEnquiryBot.Helper
{
    public static class Helper
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public static string GetSetting(string name) => ConfigurationManager.AppSettings[name];

        public static int GenerateRandomNumber()
        {
            int number = 0;
            byte[] randomNumber = new byte[1];

            do
            {
                rngCsp.GetBytes(randomNumber);
                var digit = randomNumber[0] % 10;
                number = number * 10 + digit;
            } while (number.ToString().Length < 6);

            return number;
        }

        public static void GetQueryParams(HttpRequestMessage req, out string code, out string state)
        {
            var parameters = req.GetQueryNameValuePairs().ToList();
            code = string.Empty;
            state = string.Empty;

            foreach (var param in parameters)
            {
                if (param.Key == "code")
                    code = param.Value;

                if (param.Key == "state")
                    state = param.Value;
            }
        }

        public static async Task<string> CallBingSpellCheckAsync(string query)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            var uri = GetSetting("BingSpellCheckUrl") + GetSetting("BingSpellCheckParams");

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", GetSetting("BingSpellCheckAPIKey"));

            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("text", query)
            };

            using (FormUrlEncodedContent content = new FormUrlEncodedContent(values))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                response = await client.PostAsync(uri, content);
            }

            var spellCheckResponse = JsonConvert.DeserializeObject<SpellCheckResponse>(await response.Content.ReadAsStringAsync());

            if (spellCheckResponse.FlaggedTokens.Count > 0 && spellCheckResponse.CorrectionType == "High")
                foreach (var flaggedToken in spellCheckResponse.FlaggedTokens)
                    query = query.Replace(flaggedToken.Token, flaggedToken.Suggestions.First().Suggestion);

            return query;
        }
    }
}
