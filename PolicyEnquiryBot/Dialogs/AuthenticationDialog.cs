using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BotAuth;
using BotAuth.Dialogs;
using BotAuth.Models;
using BotAuth.Providers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PolicyEnquiryBot.Dialogs
{
    [Serializable]
    public class AuthenticationDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;

            // Initialize AuthenticationOptions and forward to AuthDialog for token
            var options = new AuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["aad:Authority"],
                ClientId = ConfigurationManager.AppSettings["aad:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["aad:ClientSecret"],
                Scopes = new[] { "User.Read" },
                RedirectUrl = ConfigurationManager.AppSettings["aad:Callback"],
                MagicNumberView = string.Empty // "/magic.html#{0}"
            };

            await context.Forward(new AuthDialog(new MSALAuthProvider(), options), async (authContext, authResult) =>
            {
                var res = await authResult;

                // Use token to call into service
                var json = await new HttpClient().GetWithAuthAsync(res.AccessToken, "https://graph.microsoft.com/v1.0/me");
                await authContext.PostAsync($"Hi {json.Value<string>("displayName")}, what can I do for you today?");
            }, message, CancellationToken.None);
        }
    }
}
