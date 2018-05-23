using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using PolicyEnquiryBot.Dialogs;
using static PolicyEnquiryBot.Helper.MongoDbClient;
using static PolicyEnquiryBot.Helper.Helper;

namespace PolicyEnquiryBot
{
    public static class PolicyEnquiryBot
    {
        [FunctionName("PolicyEnquiry")]
        public static async Task<object> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("PolicyEnquiry was triggered!");

            var mongoClient = GetMongoClient(GetSetting("dbname"));

            // Initialize the azure bot
            using (BotService.Initialize())
            {
                // Deserialize the incoming activity
                var activity = JsonConvert.DeserializeObject<Activity>(await req.Content.ReadAsStringAsync());

                // authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
                // if request is authenticated
                if (!await BotService.Authenticator.TryAuthenticateAsync(req, new[] { activity }, CancellationToken.None))
                    return BotAuthenticator.GenerateUnauthorizedResponse(req);

                if (activity != null)
                {
                    // one of these will have an interface and process it
                    switch (activity.GetActivityType())
                    {
                        case ActivityTypes.Message:
                            await Conversation.SendAsync(activity, () => new RootDialog());
                            break;

                        case ActivityTypes.ConversationUpdate:
                            var client = new ConnectorClient(new Uri(activity.ServiceUrl));
                            IConversationUpdateActivity update = activity;

                            if (update.MembersAdded.Any())
                            {
                                var reply = activity.CreateReply();
                                reply.Text = "Hey! I am Polen. I can answer quick questions for your policy.";
                                await client.Conversations.ReplyToActivityAsync(reply);
                            }
                            break;

                        default:
                            log.Error($"Unknown activity type ignored: {activity.GetActivityType()}");
                            break;
                    }
                }

                return req.CreateResponse(HttpStatusCode.Accepted);
            }
        }
    }
}
