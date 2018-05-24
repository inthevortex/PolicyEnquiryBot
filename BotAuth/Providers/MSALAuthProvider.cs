using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Identity.Client;
using Models.BotAuth;

namespace BotAuth.Providers
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class MSALAuthProvider : IAuthProvider
    {
        public string Name => "MSALAuthProvider";

        public async Task<AuthResult> GetAccessToken(AuthenticationOptions authOptions, IDialogContext context)
        {
            if (context.UserData.TryGetValue($"{Name}{ContextConstants.AuthResultKey}", out AuthResult authResult) &&
                (!authOptions.UseMagicNumber ||
                (context.UserData.TryGetValue($"{Name}{ContextConstants.MagicNumberValidated}", out string validated) &&
                validated == "true")))
            {

                try
                {
                    var tokenCache = new InMemoryTokenCacheMSAL(authResult.TokenCache).GetMsalCacheInstance();
                    var client = new ConfidentialClientApplication(authOptions.ClientId,
                        authOptions.RedirectUrl, new ClientCredential(authOptions.ClientSecret), tokenCache, null);
                    authResult = (await client.AcquireTokenSilentAsync(authOptions.Scopes, client.GetUser(authResult.UserUniqueId)))
                        .FromMSALAuthenticationResult(tokenCache);
                    context.StoreAuthResult(authResult, this);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Failed to renew token: " + ex.Message);
                    await context.PostAsync("Your credentials expired and could not be renewed automatically!");
                    await Logout(authOptions, context);
                    return null;
                }
                return authResult;
            }

            return null;
        }

        public async Task<string> GetAuthUrlAsync(AuthenticationOptions authOptions, string state)
        {
            var redirectUri = new Uri(authOptions.RedirectUrl);
            var tokenCache = new InMemoryTokenCacheMSAL().GetMsalCacheInstance();
            var client = new ConfidentialClientApplication(authOptions.ClientId, redirectUri.ToString(),
                new ClientCredential(authOptions.ClientSecret),
                tokenCache, null);
            var uri = await client.GetAuthorizationRequestUrlAsync(authOptions.Scopes, null, $"state={state}");
            return uri.ToString();
        }

        public async Task<AuthResult> GetTokenByAuthCodeAsync(AuthenticationOptions authOptions, string authorizationCode)
        {
            var tokenCache = new InMemoryTokenCacheMSAL().GetMsalCacheInstance();
            var authResult = (await new ConfidentialClientApplication(authOptions.ClientId, authOptions.RedirectUrl,
                new ClientCredential(authOptions.ClientSecret), tokenCache, null)
                .AcquireTokenByAuthorizationCodeAsync(authorizationCode, authOptions.Scopes))
                .FromMSALAuthenticationResult(tokenCache);

            return authResult;
        }

        public async Task Logout(AuthenticationOptions authOptions, IDialogContext context)
        {
            context.UserData.RemoveValue($"{Name}{ContextConstants.AuthResultKey}");
            context.UserData.RemoveValue($"{Name}{ContextConstants.MagicNumberKey}");
            context.UserData.RemoveValue($"{Name}{ContextConstants.MagicNumberValidated}");
            var signoutURl = "https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=" + WebUtility.UrlEncode(authOptions.RedirectUrl);
            await context.PostAsync($"In order to finish the sign out, please click at this [link]({signoutURl}).");
        }

        public async Task<AuthResult> GetAccessTokenSilent(AuthenticationOptions options, IDialogContext context)
        {
            if (context.UserData.TryGetValue($"{Name}{ContextConstants.AuthResultKey}", out AuthResult result) &&
                context.UserData.TryGetValue($"{Name}{ContextConstants.MagicNumberValidated}", out string validated) &&
                validated == "true")
            {
                try
                {
                    var tokenCache = new InMemoryTokenCacheMSAL(result.TokenCache).GetMsalCacheInstance();
                    var client = new ConfidentialClientApplication(options.ClientId,
                        options.RedirectUrl, new ClientCredential(options.ClientSecret), tokenCache, null);
                    var r = await client.AcquireTokenSilentAsync(options.Scopes, client.GetUser(result.UserUniqueId));
                    result = r.FromMSALAuthenticationResult(tokenCache);
                    context.StoreAuthResult(result, this);
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
