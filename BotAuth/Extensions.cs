using System.Net.Http;
using System.Threading.Tasks;
using BotAuth.Providers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Identity.Client;
using Models.BotAuth;
using Newtonsoft.Json.Linq;

namespace BotAuth
{
    public static class Extensions
    {
        public static void StoreAuthResult(this IBotContext context, AuthResult authResult, IAuthProvider authProvider) =>
            context.UserData.SetValue($"{authProvider.Name}{ContextConstants.AuthResultKey}", authResult);

        // ReSharper disable once InconsistentNaming
        public static AuthResult FromMSALAuthenticationResult(this AuthenticationResult authResult, TokenCache tokenCache) => new AuthResult
        {
            AccessToken = authResult.AccessToken,
            UserName = $"{authResult.User.Name}",
            UserUniqueId = authResult.User.Identifier,
            ExpiresOnUtcTicks = authResult.ExpiresOn.UtcTicks,
            TokenCache = tokenCache.Serialize()
        };

        public static async Task<JObject> GetWithAuthAsync(this HttpClient client, string accessToken, string endpoint)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            using (var response = await client.GetAsync(endpoint))
                return !response.IsSuccessStatusCode ? null : JObject.Parse(await response.Content.ReadAsStringAsync());
        }
    }
}
