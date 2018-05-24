using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BotAuth;
using BotAuth.Dialogs;
using BotAuth.Providers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using static PolicyEnquiryBot.Helper.Helper;
using static BotAuth.ContextConstants;
using Models.BotAuth;

namespace PolicyEnquiryBot.Dialogs
{
    [Serializable]
    internal class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result as Activity;
            var authProvider = new MSALAuthProvider();

            if (context.UserData.TryGetValue($"{authProvider.Name}{MagicNumberValidated}", out string validated))
            {
                if (validated == "true" && message != null)
                {
                    // Check for spelling
                    message.Text = await CallBingSpellCheckAsync(message.Text);

                    await context.Forward(new PolicyEnquiryDialog(),
                        async (luisContext, obj) =>
                        {
                            var res = await obj;
                            // Do Something
                        },
                        message, CancellationToken.None);
                } 
            }

            else
            {
                await context.Forward(
                    new AuthDialog(
                        new MSALAuthProvider(),
                        new AuthenticationOptions(GetSetting("aad:Authority"), GetSetting("aad:ClientId"),
                            GetSetting("aad:ClientSecret"), new[] { "User.Read" }, GetSetting("aad:Callback"))), 
                    async (authContext, authResult) =>
                    {
                        var res = await authResult;

                        // Use token to call into service
                        var json = await new HttpClient().GetWithAuthAsync(res.AccessToken, "https://graph.microsoft.com/v1.0/me");
                        await authContext.PostAsync($"Hi {json.Value<string>("displayName")}, what can I do for you today?");
                    }, message, CancellationToken.None);
            }

            // TODO: Implement logout in luis dialog
            if (message?.Text.ToUpperInvariant() == "BYE")
            {
                context.UserData.RemoveValue($"{authProvider.Name}{AuthResultKey}");
                context.UserData.SetValue($"{authProvider.Name}{MagicNumberValidated}", "false");
                context.UserData.RemoveValue($"{authProvider.Name}{MagicNumberKey}");

                context.Wait(MessageReceivedAsync);
            }
        }
    }
}