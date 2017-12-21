using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Oauth2Library
{
    public class Oauth2Helper
    {
        public enum OAuthResponseType
        {
            Token,
            Code
        }
        #region Fields
        private string _clientId;

        public string ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }
        private string _secret;

        public string Secret
        {
            get { return _secret; }
            set { _secret = value; }
        }

        private string _codeEndPoint;

        public string CodeEndPoint
        {
            get { return _codeEndPoint; }
            set { _codeEndPoint = value; }
        }

        private string _tokenEndPoint;

        public string TokenEndPoint
        {
            get { return _tokenEndPoint; }
            set { _tokenEndPoint = value; }
        }

        private string _successUrl;

        public string SuccessUrl
        {
            get { return _successUrl; }
            set { _successUrl = value; }
        }

        private string _successMsg;

        public string SucessMsg
        {
            get { return _successMsg; }
            set { _successMsg = value; }
        }

        private string _html;

        public string Html
        {
            get { return _html; }
            set { _html = value; }
        }

        private string _redirectUrl;
        public string RedirectUrl
        {
            get { return _redirectUrl; }
            set { _redirectUrl = value; }
        }


        #endregion


        /// <summary>
        /// 得到请求code的url
        /// </summary>
        /// <param name="oauthResponseType"></param>
        /// <param name="endPoint">请求终结点</param>
        /// <param name="clientId">clientId</param>
        /// <param name="redirectUri">重定向url</param>
        /// <param name="state"></param>
        /// <param name="forceReapprove"></param>
        /// <param name="disableSignup"></param>
        /// <param name="requireRole"></param>
        /// <returns></returns>
        public string GetAuthorizeUri(OAuthResponseType oauthResponseType, string endPoint, string clientId, string redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false, string requireRole = null)
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
            if (!string.IsNullOrEmpty(redirectUri))
            {
                builder.Append("&redirect_uri=").Append(Uri.EscapeDataString(redirectUri));
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
            UriBuilder builder2 = new UriBuilder(endPoint)
            {
                Query = builder.ToString()
            };
            return builder2.Uri.ToString();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationRequest">请求url</param>
        /// <param name="redictUrl">重定向url</param>
        /// <param name="succeessUrl">同意之后，提示用户返回软件</param>
        /// <param name="noticeUserMsg">提示用户信息</param>
        /// <param name="html">主要是为了定制返回结果页面</param>
        /// <returns></returns>
        public async Task<string> GetCode(string authorizationRequest, string redictUrl, string succeessUrl, string noticeUserMsg, string html = null)
        {
            var http = new HttpListener();
            http.Prefixes.Add(redictUrl);
            http.Start();

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app back to the foreground.
            // this.Activate();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString;
            if (string.IsNullOrEmpty(html))
            {
                responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url={0}'></head><body>{1}</body></html>", succeessUrl, noticeUserMsg);
            }
            else
            {
                responseString = html;
            }
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                Debug.WriteLine(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));

            }
            if (context.Request.QueryString.Get("code") == null
                /*|| context.Request.QueryString.Get("state") == null*/)
            {
                Debug.WriteLine("Malformed authorization response. " + context.Request.QueryString);

            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            //获取code成功
            return code;

        }


        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="tokenRequestURI"></param>
        /// <param name="code"></param>
        /// <param name="clientId"></param>
        /// <param name="secret"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<UserCredential> GetToken(string tokenRequestURI, string code, string clientId, string secret, string redirectUri = null)
        {
            UserCredential userCredential = null;
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&client_secret={3}&scope=&grant_type=authorization_code",
                code,
                redirectUri,
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
                    //string restricted_to = "";
                    string refresh_token = "";
                    string token_type = "";
                    JObject json = JObject.Parse(responseText);
                    if (json != null)
                    {
                        userCredential = new UserCredential();
                        userCredential.Code = code;
                        if (json.Property("access_token") != null)
                        {
                            access_token = (string)json["access_token"];
                            userCredential.AccessToken = access_token;
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
                            userCredential.ReshToken = refresh_token;
                        }
                        if (json.Property("token_type") != null)
                        {
                            token_type = (string)json["token_type"];
                            userCredential.TokenType = token_type;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.WriteLine("Get token error:" + ex.ToString());
            }

            return userCredential;
        }

        /// <summary>
        /// 检查请求参数
        /// </summary>
        private void CheckRequestParameters()
        {
            if (string.IsNullOrEmpty(CodeEndPoint))
            {
                throw new Exception("codeEndPoint is null");
            }
            if (string.IsNullOrEmpty(ClientId))
            {
                throw new Exception("ClientId is null");
            }

            if (string.IsNullOrEmpty(Secret))
            {
                throw new Exception("Secrect is null");
            }

            if (string.IsNullOrEmpty(Html))
            {
                if (string.IsNullOrEmpty(SuccessUrl))
                {
                    throw new Exception("SuccessUrl is null");
                }
                if (string.IsNullOrEmpty(SucessMsg))
                {
                    throw new Exception("SucessMsg is null");
                }
            }

            if (string.IsNullOrEmpty(TokenEndPoint))
            {
                throw new Exception("TokenEndPoint is null");
            }


            if (string.IsNullOrEmpty(RedirectUrl))
            {
                throw new Exception("RedirectUrl is null");
            }
        }


        public async Task<UserCredential> GetUserCredential()
        {
            CheckRequestParameters();
            Oauth2Helper oauth2Helper = new Oauth2Helper();
            string authorizationRequest = oauth2Helper.GetAuthorizeUri(OAuthResponseType.Code, CodeEndPoint, ClientId, RedirectUrl);
            var code = await oauth2Helper.GetCode(authorizationRequest, RedirectUrl, SuccessUrl, SucessMsg);

            if (string.IsNullOrEmpty(code))
            {
                return null;
            }
            else
            {
                var userCredential = await oauth2Helper.GetToken(TokenEndPoint, code, ClientId, Secret, RedirectUrl);
                return userCredential;
            }
        }
    }
}
