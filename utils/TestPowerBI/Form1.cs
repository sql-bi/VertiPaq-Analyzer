using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Microsoft.Identity.Client;

#pragma warning disable IDE1006 // Naming Styles
namespace TestPowerBI
{
    public partial class mainForm : Form
    {
        public class PbiWrapper<T>
        {
            public PbiWrapper(T item)
            {
                Item = item;
            }

            public T Item { get; set; }

            public override string ToString()
            {
                return (Item as Group)?.Name
                    ?? (Item as Dataset)?.Name
                    ?? (Item as Report)?.Name
                    ?? (Item as Dataflow)?.Name
                    ?? Item.ToString();
            }
        }

        static mainForm()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                .WithAuthority($"{Instance}{Tenant}")
                .WithDefaultRedirectUri()
                .Build();
            TokenCacheHelper.EnableSerialization(PublicClientApp.UserTokenCache);
        }

        // Below are the clientId (Application Id) of your app registration and the tenant information. 
        // You have to replace:
        // - the content of ClientID with the Application Id for your app registration
        // - The content of Tenant by the information about the accounts allowed to sign-in in your application:
        //   - For Work or School account in your org, use your tenant ID, or domain
        //   - for any Work or School accounts, use organizations
        //   - for any Work or School accounts, or Microsoft personal account, use common
        //   - for Microsoft Personal account, use consumers
        // Sample MS - private static string ClientId = "4a1aa1d5-c567-49d0-ad0b-cd957a47f842";
        private static readonly string ClientId = "5bd6853c-3bb4-42eb-bc43-b97ac191224e";  // VertiPaq Analyzer

        private static readonly string Tenant = "common";
        private static readonly string Instance = "https://login.microsoftonline.com/";
        readonly string ApiUrl = "https://api.powerbi.com";

        public static IPublicClientApplication PublicClientApp { get; private set; }

        // Request permission in Scope - check permission names in App Registrations / API Permission on Azure Portal
        const string pbiApi = "https://analysis.windows.net/powerbi/api";

        //Set the scope for API call (Group = Workspace)
        readonly string[] scopes = new string[] { pbiApi + "/Dataset.Read.All", pbiApi + "/Group.Read.All" }; 

        public mainForm()
        {
            InitializeComponent();
        }

        private async Task<AuthenticationResult> LoginAAD()
        {
            AuthenticationResult authResult;
            var app = PublicClientApp;
            ResultText.Text = string.Empty;

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
                consoleWriter.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(accounts.FirstOrDefault())
                        .WithParentActivityOrWindow(this.Handle) // optional, used to center the browser on the window
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    consoleWriter.WriteLine($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                consoleWriter.WriteLine($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
                throw;
            }

            if (authResult != null)
            {
                consoleWriter.WriteLine($"Login:{authResult.Account.Username}");
            }
            return authResult;
        }
        private async Task LogoutAAD()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    consoleWriter.WriteLine("User has signed-out");
                }
                catch (MsalException ex)
                {
                    consoleWriter.WriteLine($"Error signing-out user: {ex.Message}");
                    throw;
                }
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            await LoginAAD();
            this.SignOutButton.Visible = true;
        }

        private async void SignOutButton_Click(object sender, EventArgs e)
        {
            await LogoutAAD();
            this.SignOutButton.Visible = false;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            this.listWorkspaces.Items.Clear();

            var token = (await LoginAAD()).AccessToken;
            var tokenCredentials = new TokenCredentials(token, "Bearer");
            PowerBIClient client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials);
            foreach ( var g in client.Groups.GetGroups().Value)
            {
                this.listWorkspaces.Items.Add(new PbiWrapper<Group>(g));
            }
        }

        private async void listWorkspaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listDatasets.Items.Clear();

            var token = (await LoginAAD()).AccessToken;
            var tokenCredentials = new TokenCredentials(token, "Bearer");
            PowerBIClient client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials);

            Group group = (listWorkspaces.SelectedItem as PbiWrapper<Group>)?.Item; 
            if (group != null)
            {
                foreach (var d in client.Datasets.GetDatasetsInGroup(group.Id).Value)
                {
                    this.listDatasets.Items.Add(new PbiWrapper<Dataset>(d));
                }
            }
        }

        TextBoxWriter consoleWriter = null;

        private void mainForm_Load(object sender, EventArgs e)
        {
            consoleWriter = new TextBoxWriter(ResultText);
            // Console.SetOut(writer);
        }

        private void btnVPAX_Click(object sender, EventArgs e)
        {
            Dataset dataset = (listDatasets.SelectedItem as PbiWrapper<Dataset>)?.Item;
            consoleWriter.WriteLine($"Dataset:{dataset.Name}");
            TestPbiShared(dataset.Name, dataset.Id);
        }

        private void TestPbiShared(string name, string id)
        {
            const string dataSource = "pbiazure://api.powerbi.com";
            const string identityProvider = "https://login.microsoftonline.com/common, https://analysis.windows.net/powerbi/api, 929d0ec0-7a41-4b1e-bc7c-b754a28bddcc;";
            string initialCatalog = id;
            string databaseName = "sobe_wowvirtualserver-" + initialCatalog;
            //const string integratedSecurity = "ClaimsToken";
            //const string other = "MDX Compatibility= 1; MDX Missing Member Mode= Error; Safety Options= 2; Update Isolation Level= 2; Locale Identifier= 1033;";

            const string serverName = dataSource;

            var connStr = String.Format(
                "Provider=MSOLAP;Identity Provider={0};Data Source={1};Initial Catalog={2};", // ;MDX Compatibility= 1; MDX Missing Member Mode= Error; Safety Options= 2; Update Isolation Level= 2;";
                identityProvider,
                dataSource,
                initialCatalog
                );

            var conn = new System.Data.OleDb.OleDbConnection(connStr);
            //conn.Open();
            //Console.WriteLine("Connection open");

            Dax.Metadata.Model m = new Dax.Metadata.Model();
            Dax.Model.Extractor.DmvExtractor.PopulateFromDmv(m, conn, serverName, databaseName, "Test", "0.1");

            Dax.Vpax.Tools.VpaxTools.ExportVpax(@"c:\temp\" + name + ".vpax",m);
        }
    }
}


#region oldcode
/*
 * 
 *         // private AuthenticationContext authContext = null;

        //        const string aadInstance = "https://login.microsoftonline.com/{0}";
        //        const string resourceId = "https://analysis.windows.net/powerbi/api";
        //        const string tenant = "sqlbi.com";
        ////        TokenCredentials tokenCredentials = null;
        //        string Token = null;
        //        string ApiUrl = "https://api.powerbi.com";
        //        private static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);



        private async void LoginAAD()
        {

            AuthenticationResult result = null;

            try
            {
                // Continue from here:
                // https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/wiki/AuthenticationContext-the-connection-to-Azure-AD

                var authContext = new AuthenticationContext("https://login.microsoftonline.com/common");
                //                result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", resourceId, new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient"), new PlatformParameters(PromptBehavior.Auto));
                result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", "VertiPaq Analyzer", new Uri("https://www.sqlbi.com"), new PlatformParameters(PromptBehavior.Auto));
                MessageBox.Show(result.AccessToken);


                // result = await authContext.AcquireTokenAsync(resourceId, "VPAX", new Uri(@"https://www.sqlbi.com"), new PlatformParameters(PromptBehavior.SelectAccount));
*/
/*
result = await authContext.AcquireTokenAsync(resourceId, "VertiPaq Analyzer",
                      new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient"),
                      new PlatformParameters(PromptBehavior.Auto));
                      */
/*
                Token = result.AccessToken;
                tokenCredentials = new TokenCredentials(Token, "Bearer");
                Console.WriteLine("Logged");

            }
            catch (AdalException ex)
            {
                // An unexpected error occurred, or user canceled the sign in.
                if (ex.ErrorCode != "access_denied")
                    MessageBox.Show(ex.Message);

                return;
            }
        }
*/

/*
*private async void LoginClient()
    {
        //const string ClientId = @"5bd6853c-3bb4-42eb-bc43-b97ac191224e"; // VertiPaq Analyzer
        //IPublicClientApplication PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
        //        .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
        //        // .WithAuthority(AzureCloudInstance.AzurePublic, "common")
        //        .WithAuthority("https://login.microsoftonline.com/common/")
        //        .Build();

        var scopes = new string[] { "sqlbi.com" };
        //var authResult = PublicClientApp.AcquireTokenInteractive( scopes ).ExecuteAsync();


        AuthenticationResult authResult = null;
        var app = PublicClientApp;

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
            Console.WriteLine($"MsalUiRequiredException: {ex.Message}");
            try
            {
                authResult = await app.AcquireTokenInteractive(scopes)
                    .WithAccount(accounts.FirstOrDefault())
                    .WithParentActivityOrWindow(this.Handle) // optional, used to center the browser on the window
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();
            }
            catch (MsalException msalex)
            {
                ResultText.Text = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
            }
        }
        catch (Exception ex)
        {
            ResultText.Text = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
            return;
        }

        if (authResult != null)
        {
            ResultText.Text = "Login:";
            this.Refresh();
            // Console.WriteLine( await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken) );
            // DisplayBasicTokenInfo(authResult);
            //this.SignOutButton.Visibility = Visibility.Visible;
        }
    }
*/
#endregion