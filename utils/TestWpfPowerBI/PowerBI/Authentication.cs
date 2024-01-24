using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Microsoft.Identity.Client;
using Serilog;

namespace TestWpfPowerBI.PowerBI
{
    class Authentication
    {
        public static IPublicClientApplication PublicClientApp { get; private set; }

        // Below are the clientId (Application Id) of your app registration and the tenant information. 
        // You have to replace:
        // - the content of ClientID with the Application Id for your app registration
        // - The content of Tenant by the information about the accounts allowed to sign-in in your application:
        //   - For Work or School account in your org, use your tenant ID, or domain
        //   - for any Work or School accounts, use organizations
        //   - for any Work or School accounts, or Microsoft personal account, use common
        //   - for Microsoft Personal account, use consumers

        // TODO : should we change this ClientID for DAX Studio? 
        private static readonly string ClientId = "5bd6853c-3bb4-42eb-bc43-b97ac191224e";  // VertiPaq Analyzer (registered app on sqlbi.com tenant)

        private static readonly string Tenant = "common";
        private static readonly string Instance = "https://login.microsoftonline.com/";

        static Authentication()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                .WithAuthority($"{Instance}{Tenant}")
                .WithDefaultRedirectUri()
                .Build();
            TokenCacheHelper.EnableSerialization(PublicClientApp.UserTokenCache);
        }
        // Request permission in Scope - check permission names in App Registrations / API Permission on Azure Portal
        const string pbiApi = "https://analysis.windows.net/powerbi/api";

        //Set the scope for API call (Group = Workspace)
        private static readonly string[] scopes = new string[] { pbiApi + "/Dataset.Read.All", pbiApi + "/Group.Read.All" };

        /// <summary>
        /// Azure AD sign in
        /// </summary>
        /// <returns>Authentication result</returns>
        public static async Task<AuthenticationResult> LoginAAD()
        {
            AuthenticationResult authResult;
            var app = PublicClientApp;
            // TODO: clear login state notification ?

            var accounts = await app.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await app.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent. 
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                Log.Information($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(accounts.FirstOrDefault())
                        // .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle) // optional, used to center the browser on the window
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    Log.Information($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Information($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
                throw;
            }

            if (authResult != null)
            {
                Log.Information($"Login:{authResult.Account.Username}");
            }
            return authResult;
        }

        /// <summary>
        /// Azure AD sign out
        /// </summary>
        /// <returns></returns>
        public static async Task LogoutAAD()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    Log.Information("User has signed-out");
                }
                catch (MsalException ex)
                {
                    Log.Information($"Error signing-out user: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
