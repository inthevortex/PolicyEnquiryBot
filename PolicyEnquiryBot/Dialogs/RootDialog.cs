using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using static PolicyEnquiryBot.Helper.Helper;

namespace PolicyEnquiryBot.Dialog
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
            var message = await result;
            message.Text = await CallBingSpellCheckAsync(message.Text);


            //await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }
    }
}