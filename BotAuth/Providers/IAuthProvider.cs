using System.Threading.Tasks;
using BotAuth.Models;
using Microsoft.Bot.Builder.Dialogs;

namespace BotAuth.Providers
{
    public interface IAuthProvider
    {
        Task<string> GetAuthUrlAsync(AuthenticationOptions authOptions, string state);
        Task<AuthResult> GetTokenByAuthCodeAsync(AuthenticationOptions authOptions, string authorizationCode);
        Task<AuthResult> GetAccessToken(AuthenticationOptions authOptions, IDialogContext context);
        Task<AuthResult> GetAccessTokenSilent(AuthenticationOptions options, IDialogContext context);
        Task Logout(AuthenticationOptions authOptions, IDialogContext context);

        string Name
        {
            get;
        }
    }
}
