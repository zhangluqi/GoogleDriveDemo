using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace OneDrive.OneDriveOperation
{
    class AuthOneDrive
    {
        // The Client ID is used by the application to uniquely identify itself to the v2.0 authentication endpoint.
        private const string ClientId = "4fb54c9c-89c6-4e97-a05e-deeb06c03a15"; //这个是自己申请的

        private static string[] Scopes = { "Files.ReadWrite" };

        private GraphServiceClient _graphClient;
        private DateTimeOffset _expiration;

        private PublicClientApplication IdentityClientApp = new PublicClientApplication(ClientId);

        private string _tokenForUser;

        // Get an access token for the given context and resourceId. An attempt is first made to 
        // acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
        public GraphServiceClient GetAuthenticatedClient()
        {
            if (_graphClient == null)
            {
                // Create Microsoft Graph client.
                try
                {
                    _graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0",
                                                          new DelegateAuthenticationProvider(async requestMessage =>
                                                          {
                                                              var token = await GetTokenForUserAsync();
                                                              requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                                                              // This header has been added to identify our sample in the Microsoft Graph service.  If extracting this code for your project please remove.
                                                              requestMessage.Headers.Add("SampleID", "uwp-csharp-connect-sample");
                                                          }));

                    return _graphClient;
                }

                catch (Exception ex)
                {
                    Debug.WriteLine("Could not create a graph client: " + ex.Message);
                }
            }

            return _graphClient;
        }

        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        private async Task<string> GetTokenForUserAsync()
        {
            AuthenticationResult authResult;
            try
            {
                authResult = await IdentityClientApp.AcquireTokenSilentAsync(Scopes, IdentityClientApp.Users.First());
                _tokenForUser = authResult.AccessToken;
            }

            catch (Exception)
            {
                if (_tokenForUser == null || _expiration <= DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    authResult = await IdentityClientApp.AcquireTokenAsync(Scopes);

                    _tokenForUser = authResult.AccessToken;
                    _expiration = authResult.ExpiresOn;
                }
            }

            return _tokenForUser;
        }

        /// <summary>
        /// Signs the user out of the service.
        /// </summary>
        public void SignOut()
        {
            foreach (var user in IdentityClientApp.Users)
            {
                IdentityClientApp.Remove(user);
            }
            _graphClient = null;
            _tokenForUser = null;
        }
    }
}
