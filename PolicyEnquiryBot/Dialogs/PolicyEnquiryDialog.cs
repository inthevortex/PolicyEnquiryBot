using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using static PolicyEnquiryBot.Helper.SSMLHelper;

namespace PolicyEnquiryBot.Dialog
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