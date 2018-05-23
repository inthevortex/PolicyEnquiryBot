using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using BotAuth;
using BotAuth.Models;
using BotAuth.Providers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using static PolicyEnquiryBot.Helper.Helper;

namespace PolicyEnquiryBot
{
    public static class Callback
    {
        private const uint MaxWriteAttempts = 5;

        [FunctionName("Callback")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Callback was triggered!");

            using (BotService.Initialize())
            {
                ConfigureStateStore();
                GetQueryParams(req, out var code, out var state);

                try
                {
                    // Use the state parameter to get correct IAuthProvider and ResumptionCookie
                    var decoded = Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(state) ?? throw new InvalidOperationException());
                    var queryString = HttpUtility.ParseQueryString(decoded);
                    var assembly = Assembly.Load(queryString["providerassembly"]);
                    var type = assembly.GetType(queryString["providertype"]);
                    var providername = queryString["providername"];
                    IAuthProvider authProvider;

                    if (type.GetConstructor(new[] { typeof(string) }) != null)
                        authProvider = (IAuthProvider)Activator.CreateInstance(type, providername);

                    else
                        authProvider = (IAuthProvider)Activator.CreateInstance(type);

                    // Get the conversation reference
                    var conversationRef = UrlToken.Decode<ConversationReference>(queryString["conversationRef"]);
                    var message = conversationRef.GetPostToBotMessage();

                    var magicNumber = GenerateRandomNumber();
                    var writeSuccessful = false;
                    uint writeAttempts = 0;

                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                    {
                        // Get the UserData from the original conversation
                        var stateStore = scope.Resolve<IBotDataStore<BotData>>();
                        var key = Address.FromActivity(message);
                        var userData = await stateStore.LoadAsync(key, BotStoreType.BotUserData, CancellationToken.None);

                        // Get Access Token using authorization code
                        var authOptions = userData.GetProperty<AuthenticationOptions>($"{authProvider.Name}{ContextConstants.AuthOptions}");
                        var token = await authProvider.GetTokenByAuthCodeAsync(authOptions, code);

                        // Generate magic number and attempt to write to userdata
                        while (!writeSuccessful && writeAttempts++ < MaxWriteAttempts)
                        {
                            try
                            {
                                userData.SetProperty($"{authProvider.Name}{ContextConstants.AuthResultKey}", token);

                                if (authOptions.UseMagicNumber)
                                {
                                    userData.SetProperty($"{authProvider.Name}{ContextConstants.MagicNumberKey}", magicNumber);
                                    userData.SetProperty($"{authProvider.Name}{ContextConstants.MagicNumberValidated}", "false");
                                }

                                await stateStore.SaveAsync(key, BotStoreType.BotUserData, userData, CancellationToken.None);
                                await stateStore.FlushAsync(key, CancellationToken.None);

                                writeSuccessful = true;
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex.Message);
                                writeSuccessful = false;
                            }
                        }

                        var resp = new HttpResponseMessage(HttpStatusCode.OK);

                        if (!writeSuccessful)
                        {
                            message.Text = string.Empty; // fail the login process if we can't write UserData
                            await Conversation.ResumeAsync(conversationRef, message);
                            resp.Content = new StringContent("<html><body>Could not log you in at this time, please try again later</body></html>", Encoding.UTF8, @"text/html");

                            return resp;
                        }

                        await Conversation.ResumeAsync(conversationRef, message);

                        // check if the user has configured an alternate magic number view
                        if (!String.IsNullOrEmpty(authOptions.MagicNumberView))
                        {
                            var redirect = req.CreateResponse(HttpStatusCode.Moved);
                            redirect.Headers.Location = new Uri(String.Format(authOptions.MagicNumberView, magicNumber), UriKind.Relative);

                            return redirect;
                        }

                        resp.Content = new StringContent($"<html><body>Almost done! Please copy this number and paste it back to your chat so your authentication can complete:<br/> <h1>{magicNumber}</h1>.</body></html>", Encoding.UTF8, @"text/html");

                        return resp;
                    }
                }
                catch (Exception ex)
                {
                    // Callback is called with no pending message as a result the login flow cannot be resumed.
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                } 
            }
        }
    }
}
