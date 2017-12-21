using Dropbox.Api;
using GoogleDriveDemo.View.Control.Dropbox;
using Oauth2Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Dropbox
{
    public class AuthDropbox
    {

        private const string clientId = "yj0cqk5fy0zczqf";
        private const string secret = "f41p9c2n2wzycpr";
        private const string RedirectUri = "http://localhost:58240/";

        // GET: /Home/Connect
        public async Task<DropboxClient> GetDropboxClient()
        {
            DropboxClient dropboxClient = null;
            var authorizationRequest = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Code,
                clientId,
                RedirectUri).ToString();

            Oauth2Helper oauth2Helper = new Oauth2Helper();
            string code = await oauth2Helper.GetCode(authorizationRequest,RedirectUri,"www.google.com","return to app!!!");
            //获取code成功
            if (!string.IsNullOrEmpty(code))
            {
                string token = await AuthAsync(code);
                if (!string.IsNullOrEmpty(token))
                {
                    dropboxClient = new DropboxClient(token, new DropboxClientConfig("iMobieDriveDemo"));
                }
            }
            return dropboxClient;
        }

        // GET: /Home/Auth
        public async Task<string> AuthAsync(string code)
        {
            try
            {
                var response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
                    code,
                    clientId,
                    secret,
                    RedirectUri);
                return response.AccessToken;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get token error:" + ex.ToString());
                return "";
            }
        }
    }
}
