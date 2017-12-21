using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using Box.V2.Config;
using Box.V2.Auth;
using Box.V2;

namespace GoogleDriveDemo.Model.Box
{
    public enum OAuthResponseType
    {
        Token,
        Code
    }

    public class AuthBox
    {
        private const string clientId = "5bf330s03sjoduoza66ww1ydz9pq6cvh";
        private const string secret = "ovj41q5ZFkMSE8M1K5NlGXZuwZPmhsgY";
        private const string RedirectUri = "http://localhost:58240/";

        public async Task<BoxClient> GetBoxClient()
        {
            BoxClient boxClient = null;
            Uri uri = string.IsNullOrEmpty(RedirectUri) ? null : new Uri(RedirectUri);
            var BoxAuthorizeRequest = GetAuthorizeUri(OAuthResponseType.Code, clientId, uri).ToString();

            var http = new HttpListener();
            http.Prefixes.Add(RedirectUri);
            http.Start();

            System.Diagnostics.Process.Start(BoxAuthorizeRequest);
            var context = await http.GetContextAsync();
            string code = context.Request.QueryString.Get("code");
            if (!string.IsNullOrEmpty(code))
            {
                string token = await BoxAuthAsync(code);
                if (!string.IsNullOrEmpty(token))
                {
                    var config = new BoxConfig(clientId, secret, new Uri(RedirectUri));
                    var session = new OAuthSession(token, "NOT_NEEDED", 3600, "bearer");
                    boxClient = new BoxClient(config, session);
                }
            }
            return boxClient;
        }

        /// <summary>
        /// 获取code
        /// </summary>
        /// <param name="oauthResponseType"></param>
        /// <param name="clientId"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state"></param>
        /// <param name="forceReapprove"></param>
        /// <param name="disableSignup"></param>
        /// <param name="requireRole"></param>
        /// <returns></returns>
        public Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, Uri redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false, string requireRole = null)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId");
            }
            if ((redirectUri == null) && (oauthResponseType != OAuthResponseType.Code))
            {
                throw new ArgumentNullException("redirectUri");
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("response_type=");
            switch (oauthResponseType)
            {
                case OAuthResponseType.Token:
                    builder.Append("token");
                    break;

                case OAuthResponseType.Code:
                    builder.Append("code");
                    break;

                default:
                    throw new ArgumentOutOfRangeException("oauthResponseType");
            }
            builder.Append("&client_id=").Append(Uri.EscapeDataString(clientId));
            if (redirectUri != null)
            {
                builder.Append("&redirect_uri=").Append(Uri.EscapeDataString(redirectUri.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                builder.Append("&state=").Append(Uri.EscapeDataString(state));
            }
            if (forceReapprove)
            {
                builder.Append("&force_reapprove=true");
            }
            if (disableSignup)
            {
                builder.Append("&disable_signup=true");
            }
            if (!string.IsNullOrWhiteSpace(requireRole))
            {
                builder.Append("&require_role=").Append(requireRole);
            }
            UriBuilder builder2 = new UriBuilder("https://account.box.com/api/oauth2/authorize")
            {
                Query = builder.ToString()
            };
            return builder2.Uri;

        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> BoxAuthAsync(string code)
        {
            try
            {
                var response = await BoxProcessCodeFlowAsync(
                   code,
                   clientId,
                   secret,
                   RedirectUri);
                return response.AccessToken;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get Box token error:" + ex.ToString());
                return "";
            }
        }

        public async Task<OAuth2Response> BoxProcessCodeFlowAsync(string code, string appKey, string appSecret, string redirectUri = null)
        {
            OAuth2Response response = null;
            string tokenRequestURI = "https://api.box.com/oauth2/token";
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code");
            }
            if (string.IsNullOrEmpty(appKey))
            {
                throw new ArgumentNullException("appKey");
            }
            if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentNullException("appSecret");
            }

            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&client_secret={3}&scope=&grant_type=authorization_code",
                code,
                RedirectUri,
                clientId,
                secret
                );

            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();
            try
            {
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    string access_token = "";
                    string expires_in = "";
                    string restricted_to = "";
                    string refresh_token = "";
                    string token_type = "";
                    JObject json = JObject.Parse(responseText);
                    if (json.Property("access_token") != null)
                    {
                        access_token = (string)json["access_token"];//tokenEndpointDecoded["access_token"];
                    }
                    if (json.Property("expires_in") != null)
                    {
                        expires_in = (string)json["expires_in"];
                    }
                    if (json.Property("restricted_to") != null)
                    {
                        //返回[] 暂时没有用
                        //restricted_to = (string)json["restricted_to"];
                    }
                    if (json.Property("refresh_token") != null)
                    {
                        refresh_token = (string)json["refresh_token"];
                    }
                    if (json.Property("token_type") != null)
                    {
                        token_type = (string)json["token_type"];
                    }
                    response = new OAuth2Response(access_token, expires_in, restricted_to, refresh_token, token_type);
                }
            }
            catch (WebException ex)
            {
                Debug.WriteLine("Get token error:" + ex.ToString());
            }

            return response;
        }
    }
}
