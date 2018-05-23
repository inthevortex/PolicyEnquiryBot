using System;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;

namespace PolicyEnquiryBot.Dialogs
{
    [Serializable]
    public class PolicyEnquiryDialog : LuisDialog<object>
    {
        public PolicyEnquiryDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        
    }
}
