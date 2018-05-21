using BotAuth.Models;
using BotAuth.Providers;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BotAuth.Dialogs
{
    [Serializable]
    public class AuthDialog : IDialog<AuthResult>
    {
        protected IAuthProvider _authProvider;
        protected AuthenticationOptions _authOptions;
        protected string _prompt { get; }

        public AuthDialog(IAuthProvider AuthProvider, AuthenticationOptions AuthOptions, string Prompt = "Please click to sign in: ")
        {
            _prompt = Prompt;
            _authProvider = AuthProvider;
            _authOptions = AuthOptions;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;

            string validated = "";
            int magicNumber = 0;
            if (context.UserData.TryGetValue($"{_authProvider.Name}{ContextConstants.AuthResultKey}", out AuthResult authResult))
            {
                try
                {
                    //IMPORTANT: DO NOT REMOVE THE MAGIC NUMBER CHECK THAT WE DO HERE. THIS IS AN ABSOLUTE SECURITY REQUIREMENT
                    //REMOVING THIS WILL REMOVE YOUR BOT AND YOUR USERS TO SECURITY VULNERABILITIES. 
                    //MAKE SURE YOU UNDERSTAND THE ATTACK VECTORS AND WHY THIS IS IN PLACE.
                    context.UserData.TryGetValue($"{_authProvider.Name}{ContextConstants.MagicNumberValidated}", out validated);
                    if (validated == "true" || !_authOptions.UseMagicNumber)
                    {
                        // Try to get token to ensure it is still good
                        var token = await _authProvider.GetAccessToken(_authOptions, context);
                        if (token != null)
                            context.Done(token);
                        else
                        {
                            // Save authenticationOptions in UserData
                            context.UserData.SetValue($"{_authProvider.Name}{ContextConstants.AuthOptions}", _authOptions);

                            // Get ConversationReference and combine with AuthProvider type for the callback
                            var conversationRef = context.Activity.ToConversationReference();
                            var state = GetStateParam(conversationRef);
                            string authenticationUrl = await _authProvider.GetAuthUrlAsync(_authOptions, state);
                            await PromptToLogin(context, msg, authenticationUrl);
                            context.Wait(MessageReceivedAsync);
                        }
                    }
                    else if (context.UserData.TryGetValue($"{_authProvider.Name}{ContextConstants.MagicNumberKey}", out magicNumber))
                    {
                        if (msg.Text == null)
                        {
                            await context.PostAsync($"Please paste back the number you received in your authentication screen.");

                            context.Wait(MessageReceivedAsync);
                        }
                        else
                        {
                            // handle at mentions in Teams
                            var text = msg.Text;
                            if (text.Contains("</at>"))
                                text = text.Substring(text.IndexOf("</at>") + 5).Trim();

                            if (text.Length >= 6 && magicNumber.ToString() == text.Substring(0, 6))
                            {
                                context.UserData.SetValue<string>($"{_authProvider.Name}{ContextConstants.MagicNumberValidated}", "true");
                                await context.PostAsync($"Thanks {authResult.UserName}. You are now logged in. ");
                                context.Done(authResult);
                            }
                            else
                            {
                                context.UserData.RemoveValue($"{_authProvider.Name}{ContextConstants.AuthResultKey}");
                                context.UserData.SetValue<string>($"{_authProvider.Name}{ContextConstants.MagicNumberValidated}", "false");
                                context.UserData.RemoveValue($"{_authProvider.Name}{ContextConstants.MagicNumberKey}");
                                await context.PostAsync($"I'm sorry but I couldn't validate your number. Please try authenticating once again. ");
                                context.Wait(MessageReceivedAsync);
                            }
                        }
                    }
                }
                catch
                {
                    context.UserData.RemoveValue($"{_authProvider.Name}{ContextConstants.AuthResultKey}");
                    context.UserData.SetValue($"{_authProvider.Name}{ContextConstants.MagicNumberValidated}", "false");
                    context.UserData.RemoveValue($"{_authProvider.Name}{ContextConstants.MagicNumberKey}");
                    await context.PostAsync($"I'm sorry but something went wrong while authenticating.");
                    context.Done<AuthResult>(null);
                }
            }
            else
            {
                // Try to get token
                var token = await _authProvider.GetAccessToken(_authOptions, context);
                if (token != null)
                    context.Done(token);
                else
                {
                    if (msg.Text != null &&
                        CancellationWords.GetCancellationWords().Contains(msg.Text.ToUpper()))
                    {
                        context.Done<AuthResult>(null);
                    }
                    else
                    {
                        // Save authenticationOptions in UserData
                        context.UserData.SetValue($"{_authProvider.Name}{ContextConstants.AuthOptions}", _authOptions);

                        // Get ConversationReference and combine with AuthProvider type for the callback
                        var conversationRef = context.Activity.ToConversationReference();
                        var state = GetStateParam(conversationRef);
                        string authenticationUrl = await _authProvider.GetAuthUrlAsync(_authOptions, state);
                        await PromptToLogin(context, msg, authenticationUrl);
                        context.Wait(MessageReceivedAsync);
                    }
                }
            }
        }

        private string GetStateParam(ConversationReference conversationRef)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["conversationRef"] = UrlToken.Encode(conversationRef);
            queryString["providerassembly"] = _authProvider.GetType().Assembly.FullName;
            queryString["providertype"] = _authProvider.GetType().FullName;
            queryString["providername"] = _authProvider.Name;
            return HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(queryString.ToString()));
        }

        /// <summary>
        /// Prompts the user to login. This can be overridden inorder to allow custom prompt messages or cards per channel.
        /// </summary>
        /// <param name="context">Chat context</param>
        /// <param name="msg">Chat message</param>
        /// <param name="authenticationUrl">OAuth URL for authenticating user</param>
        /// <returns>Task from Posting or prompt to the context.</returns>
        protected virtual Task PromptToLogin(IDialogContext context, IMessageActivity msg, string authenticationUrl)
        {
            Attachment plAttachment = null;
            SigninCard plCard;
            if (msg.ChannelId == "msteams")
                plCard = new SigninCard(_prompt, GetCardActions(authenticationUrl, "openUrl"));
            else
                plCard = new SigninCard(_prompt, GetCardActions(authenticationUrl, "signin"));
            plAttachment = plCard.ToAttachment();

            IMessageActivity response = context.MakeMessage();
            response.Recipient = msg.From;
            response.Type = "message";

            response.Attachments = new List<Attachment>
            {
                plAttachment
            };

            return context.PostAsync(response);
        }

        private List<CardAction> GetCardActions(string authenticationUrl, string actionType)
        {
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction()
            {
                Value = authenticationUrl,
                Type = actionType,
                Title = "Authentication Required"
            };
            cardButtons.Add(plButton);
            return cardButtons;
        }
    }
}
