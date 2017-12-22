using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CloudManagerment.Connect2PHP
{
    public sealed class ConnectPHP
    {
        private readonly string url = "https://fuhanwen.cn/api/auth/login";
        private readonly string url1 = "https://fuhanwen.cn/api/drive/list";
        private readonly string url2 = "https://fuhanwen.cn/api/drive";
        private static volatile ConnectPHP _instance = null;

        public static ConnectPHP Instence()
        {
            if (_instance == null)
            {
                _instance = new ConnectPHP();
            }
            return _instance;
        }

        public string GetUserInfo(Dictionary<string, string> postData)
        {
            WebClient clientNet = new WebClient();
            string responseData = "";
            var boundary = DateTime.Now.Ticks.ToString();
            Dictionary<string, string> fixedHeaders = new Dictionary<string, string>
            {
                { "accept", "application/json" },
                { "Client", "Cloud Drive/1.0.0" }
            };
            try
            {
                if (fixedHeaders != null)
                {
                    foreach (var key in fixedHeaders.Keys)
                    {
                        clientNet.Headers.Add(key, fixedHeaders[key]);
                    }
                }
                clientNet.Headers.Add("Content-Type",
                    string.Format("multipart/form-data; boundary=---------------------------{0}", boundary));
                string data = SubmitForm(fixedHeaders, postData, boundary);
                responseData = clientNet.UploadString(url, "POST", data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                clientNet.Dispose();
            }
            return responseData;
        }

        public string SubmitForm(Dictionary<string, string> fixedHeaders, Dictionary<string, string> postData, string boundary)
        {
            var sb = new StringBuilder();
            if (postData != null)
            {
                foreach (var key in postData.Keys)
                {
                    sb.Append(string.Format("-----------------------------{0}\n", boundary));
                    sb.Append("Content-Disposition:form-data; name=\"" + key + "\"\n\n");
                    sb.Append(postData[key] + "\n");
                }
            }
            sb.Append(string.Format("-----------------------------{0}--\n", boundary));
            return sb.ToString();
        }

        public string GetCloudList(string token)
        {
            WebClient clientNet = new WebClient();
            string responseData = "";
            try
            {
                clientNet.Headers.Add("accept", "application/json");
                clientNet.Headers.Add("Client", "Cloud Drive/1.0.0");
                clientNet.Headers.Add("Authorization", "Bearer " + token);
                Stream st = clientNet.OpenRead(url1);
                StreamReader sr = new StreamReader(st);
                responseData = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                clientNet.Dispose();
            }
            return responseData;
        }

        public string RefreToken(string cloudId, string userToken)
        {
            WebClient clientNet = new WebClient();
            string responseData = "";
            try
            {
                string realUrl = string.Format("{0}/{1}", url2, cloudId);
                realUrl = realUrl + "/refresh";

                clientNet.Headers.Add("accept", "application/json");
                clientNet.Headers.Add("Client", "Cloud Drive/1.0.0");
                clientNet.Headers.Add("Authorization", "Bearer " + userToken);

                Stream st = clientNet.OpenRead(realUrl);
                StreamReader sr = new StreamReader(st);
                responseData = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return responseData;
        }
    }
}
