using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.OneDrive
{
    public class AuthOneDrive
    {
        // The Client ID is used by the application to uniquely identify itself to the v2.0 authentication endpoint.
        private const string clientId = "4fb54c9c-89c6-4e97-a05e-deeb06c03a15"; //这个是自己申请的
        public static string[] Scopes = { "Files.ReadWrite" };

        public  PublicClientApplication IdentityClientApp = new PublicClientApplication(clientId);

        public  string TokenForUser = null;
        public  DateTimeOffset Expiration;

        private  GraphServiceClient graphClient = null;

        // Get an access token for the given context and resourceId. An attempt is first made to 
        // acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
        public  GraphServiceClient GetAuthenticatedClient()
        {
            if (graphClient == null)
            {
                // Create Microsoft Graph client.
                try
                {
                    graphClient = new GraphServiceClient(
                        "https://graph.microsoft.com/v1.0",
                        new DelegateAuthenticationProvider(
                            async (requestMessage) =>
                            {
                                var token = await GetTokenForUserAsync();
                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                                // This header has been added to identify our sample in the Microsoft Graph service.  If extracting this code for your project please remove.
                                requestMessage.Headers.Add("SampleID", "uwp-csharp-connect-sample");

                            }));


                  
                    return graphClient;
                }

                catch (Exception ex)
                {
                    Debug.WriteLine("Could not create a graph client: " + ex.Message);
                }
            }
            return graphClient;
        }


        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        public  async Task<string> GetTokenForUserAsync()
        {
            AuthenticationResult authResult;
            try
            {
                authResult = await IdentityClientApp.AcquireTokenSilentAsync(Scopes, IdentityClientApp.Users.First());
                TokenForUser = authResult.AccessToken;
            }

            catch (Exception)
            {
                if (TokenForUser == null || Expiration <= DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    authResult = await IdentityClientApp.AcquireTokenAsync(Scopes);

                    TokenForUser = authResult.AccessToken;
                    Expiration = authResult.ExpiresOn;
                }
            }
            return TokenForUser;
        }

        /// <summary>
        /// Signs the user out of the service.
        /// </summary>
        public  void SignOut()
        {
            foreach (var user in IdentityClientApp.Users)
            {
                IdentityClientApp.Remove(user);
            }
            graphClient = null;
            TokenForUser = null;

        }
    }
}
