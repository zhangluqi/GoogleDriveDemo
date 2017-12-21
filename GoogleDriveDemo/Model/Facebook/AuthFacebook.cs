using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Facebook
{
    public class AuthFacebook
    {
        
        private const string clientId = "884627481700055";
        private const string secret = "6644be2df2cc941a7ea9683640ec544e";
        private const string RedirectUri = "http://localhost:58241/";


        /*
    private const string clientId = "330943407380193";
    private const string secret = "18972df7a07fb6a0f45820959548c900";
    private const string RedirectUri =  "http://localhost:58240/";
    */
        private const string accessCode = "user_about_me ,user_photos,user_videos"; //这个地方之后在追加其他的权限，facebook这些权限需要在facebook开发中心配置
        public async Task<FacebookClient> GetFacebookClient()
        {
            var fb = new FacebookClient();
            var authorizationRequest = fb.GetLoginUrl(new
            {
                client_id = clientId,
                redirect_uri = RedirectUri,
                response_type = "code",
                scope = accessCode  // Add other permissions as needed

            });
            var http = new HttpListener();
            http.Prefixes.Add(RedirectUri);
            http.Start();

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest.ToString());

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app back to the foreground.
            // this.Activate();

            string code = context.Request.QueryString.Get("code");
            //获取code成功
            if (!string.IsNullOrEmpty(code))
            {
                var fb1 = new FacebookClient();
                // throws OAuthException 
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = clientId,
                    client_secret = secret,
                    redirect_uri = RedirectUri,
                    code = code

                });
                var fb2 = new FacebookClient(result.access_token);
                return fb2;
            }
            else
            {
                return null;
            }
        }

        public bool RevokePermission(FacebookClient fb)
        {
            var res = fb.Delete("me/permissions");

            if (res is bool && (bool)res == true)
            {
                return (bool)res;
            }
            return false;
        }
    }
}
