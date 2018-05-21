using BotAuth.Models;
using BotAuth.Providers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BotAuth
{
    public static class Extensions
    {
        public static void StoreAuthResult(this IBotContext context, AuthResult authResult, IAuthProvider authProvider) =>
            context.UserData.SetValue($"{authProvider.Name}{ContextConstants.AuthResultKey}", authResult);

        public static AuthResult FromMSALAuthenticationResult(this AuthenticationResult authResult, TokenCache tokenCache)
        {
            var result = new AuthResult
            {
                AccessToken = authResult.AccessToken,
                UserName = $"{authResult.User.Name}",
                UserUniqueId = authResult.User.Identifier,
                ExpiresOnUtcTicks = authResult.ExpiresOn.UtcTicks,
                TokenCache = tokenCache.Serialize()
            };

            return result;
        }

        public static async Task<JObject> GetWithAuthAsync(this HttpClient client, string accessToken, string endpoint)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (var response = await client.GetAsync(endpoint))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(json);
                }
                else
                    return null;
            }
        }
    }
}
