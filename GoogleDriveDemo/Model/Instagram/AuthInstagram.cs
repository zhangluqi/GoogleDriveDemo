
//using HttpUtilLibrary;
using Oauth2Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace GoogleDriveDemo.Model.Instagram
{
    public class AuthInstagram
    {
#region Fields
        private const string _clientId = "2893c22f7a5c4adf85bfe08c54cb260d";
        private const string _secret = "2bcda9aeeeb44053b6d8c9a84b1d55ff";
        private const string _redirectUri = "http://127.0.0.1:58240/";
        private const string _codeEndPoint = "https://api.instagram.com/oauth/authorize/";
        private const string _tokenRequestURI = "https://api.instagram.com/oauth/access_token/";
        private const string _successUrl = "www.google.com";
        private const string _msg = "Please return to app!!!";
#endregion
        // GET: /Home/Connect
        public async Task<InstamgramClient> GetInstagramClient()
        {
            Oauth2Helper oauth2Helper = new Oauth2Helper() {
                ClientId = _clientId,
                Secret = _secret,
                RedirectUrl = _redirectUri,
                CodeEndPoint = _codeEndPoint,
                TokenEndPoint = _tokenRequestURI,
                SuccessUrl = _successUrl,
                SucessMsg = _msg
            };
            var userCredential = await oauth2Helper.GetUserCredential();
            InstamgramClient instamgramClient = new InstamgramClient(userCredential);
            return instamgramClient;
        }   
    }
}
