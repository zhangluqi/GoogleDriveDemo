using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpUtilLibrary
{
    public class NetClient : WebClient
    {
        /// <summary>
        /// The actual cookie container to store all of the cookies we've set.
        /// </summary>
        public CookieContainer cookieContainer { get; private set; }

        private DateTime requestdate = DateTime.MinValue;
        private int rangeFrom = -1;
        private int rangeTo = -1;

        /// <summary>
        /// Get 请求参数列表
        /// </summary>
        private Dictionary<string, string> _getParamDictionary = new Dictionary<string, string>();

        public Dictionary<string, string> GetParamDictionary
        {
            get { return _getParamDictionary; }
            set { _getParamDictionary = value; }
        }





        /// <summary>
        /// Our default constructor.
        /// </summary>
        public NetClient()
        {
            cookieContainer = new CookieContainer();
            this.Encoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// Our constructor that quickly sets headers.
        /// </summary>
        /// <param name="origin">The page which we originated from.</param>
        /// <param name="referer">The page which brought us to this destination.</param>
        /// <param name="contentType">The content-type to be handled.</param>
        public NetClient(string origin, string referer, string contentType = "text/plain")
        {
            cookieContainer = new CookieContainer();
            this.Headers.Add("Origin", origin);
            this.Headers.Add("Referer", referer);
            this.Headers.Add("Content-Type", contentType);
            this.Encoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// Our constructor that allows for existing cookies.
        /// </summary>
        public NetClient(CookieContainer container)
        {
            cookieContainer = container;
            this.Encoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// Our constructor that allows for existing cookies and quickly sets headers.
        /// </summary>
        /// <param name="cookieContainer">The container holding all the cookies</param>
        /// <param name="origin">The page which we originated from.</param>
        /// <param name="referer">The page which brought us to this destination.</param>
        /// <param name="contentType">The content-type to be handled.</param>
        public NetClient(CookieContainer container, string origin, string referer, string contentType = "text/plain")
        {
            cookieContainer = container;
            this.Headers.Add("Origin", origin);
            this.Headers.Add("Referer", referer);
            this.Headers.Add("Content-Type", contentType);
            this.Encoding = System.Text.Encoding.UTF8;
        }

        public NetClient(CookieContainer container, string origin, string referer, string accept, string contentType, string userAgent)
        {
            cookieContainer = container;
            this.Headers.Add("Origin", origin);
            this.Headers.Add("Referer", referer);
            this.Headers.Add("Accept", accept);
            this.Headers.Add("Content-Type", contentType);
            this.Headers.Add("User-Agent", userAgent);
            this.Encoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// 给webClient的Header赋值
        /// </summary>
        /// <param name="headerList"></param>
        public NetClient(List<HeaderEntity> headerList)
        {
            cookieContainer = new CookieContainer();
            this.Encoding = System.Text.Encoding.UTF8;
            if (headerList != null && headerList.Count > 0)
            {
                foreach (HeaderEntity itemHeader in headerList)
                {
                    if (itemHeader.Header == "Date")
                    {
                        requestdate = Convert.ToDateTime(itemHeader.Value);
                        continue;
                    }
                    if (itemHeader.Header == "Range")
                    {
                        string pattern = "([0-9]+)";
                        MatchCollection mc = Regex.Matches(itemHeader.Value, pattern);
                        if (mc.Count >= 2)
                        {
                            rangeFrom = Convert.ToInt32(mc[0].Value);
                            rangeTo = Convert.ToInt32(mc[1].Value);
                        }
                        continue;
                    }
                    this.Headers.Add(itemHeader.Header, itemHeader.Value);
                }
            }
        }

        public void AddHeader(string header, string value)
        {
            if (header == "Date")
            {
                requestdate = Convert.ToDateTime(value);
            }
            else if (header == "Range")
            {
                string pattern = "([0-9]+)";
                MatchCollection mc = Regex.Matches(value, pattern);
                if (mc.Count >= 2)
                {
                    rangeFrom = Convert.ToInt32(mc[0].Value);
                    rangeTo = Convert.ToInt32(mc[1].Value);
                }
            }
            else
            {
                this.Headers.Add(header, value);
            }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            // Get our web request as we would normally.
            WebRequest request = base.GetWebRequest(address);

            // But give it our cookie container, so it updates our cookies.
            if (request.GetType() == typeof(HttpWebRequest))
            { ((HttpWebRequest)request).CookieContainer = cookieContainer; }
            if (rangeFrom != -1 && rangeTo != -1)
            { ((HttpWebRequest)request).AddRange(rangeFrom, rangeTo); }
            if (requestdate != DateTime.MinValue)
            { ((HttpWebRequest)request).Date = requestdate; }

            // And return it as the normal function does.
            return request;
        }


        public string GETWithParams(string host, string path, bool ssl)
        {
            string parameters = "";
            if (GetParamDictionary != null && GetParamDictionary.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in GetParamDictionary.Keys)
                {
                    sb.Append(key);
                    sb.Append("=");
                    sb.Append(GetParamDictionary[key]);
                    sb.Append("&");
                }
                parameters = sb.ToString();
                parameters = parameters.Substring(0, parameters.Length - 1);
            }

            return GET(host, path, ssl, parameters);
        }

        public byte[] GET_BYTE_WithParams(string host, string path, bool ssl)
        {
            string parameters = "";
            if (GetParamDictionary != null && GetParamDictionary.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in GetParamDictionary.Keys)
                {
                    sb.Append(key);
                    sb.Append("=");
                    sb.Append(GetParamDictionary[key]);
                    sb.Append("&");
                }
                parameters = sb.ToString();
                parameters = parameters.Substring(0, parameters.Length - 1);
            }

            string protocol = ssl ? "https://" : "http://";
            string url = null;
            if (string.IsNullOrEmpty(parameters))
            {
                url = string.Format("{0}{1}{2}", protocol, host, path);
            }
            else
            {
                url = string.Format("{0}{1}{2}?{3}", protocol, host, path, parameters);
            }
            this.Headers.Add("Host", host);
            return this.DownloadData(url);
        }


        public string GET(string host, string path, bool ssl, string parameters)
        {
            string protocol = ssl ? "https://" : "http://";
            string url = null;
            if (string.IsNullOrEmpty(parameters))
            {
                url = string.Format("{0}{1}{2}", protocol, host, path);
            }
            else
            {
                url = string.Format("{0}{1}{2}?{3}", protocol, host, path, parameters);
            }
           // this.Headers.Add("Host", host);
            return this.DownloadString(url);
        }



        public string GET(string host, string path, bool ssl)
        {
            string protocol = ssl ? "https://" : "http://";
            string url = string.Format("{0}{1}{2}", protocol, host, path);
            this.Headers.Add("Host", host);
            return this.DownloadString(url);
        }

        public Byte[] GET_BYTE(string host, string path, bool ssl)
        {
            string protocol = ssl ? "https://" : "http://";
            string url = string.Format("{0}{1}{2}", protocol, host, path);
            //   this.Headers.Add("Host", host);
            return this.DownloadData(url);
        }

        public string BuildPostData(Dictionary<string, string> postParamDic)
        {
            string parameters = "";
            if (postParamDic != null && postParamDic.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in postParamDic.Keys)
                {
                    sb.Append(key);
                    sb.Append("=");
                    sb.Append(postParamDic[key]);
                    sb.Append("&");
                }
                parameters = sb.ToString();
                parameters = parameters.Substring(0, parameters.Length - 1);
            }
            return parameters;
        }


        public string POST(string data, string host, string path, bool formurlencoded = true, bool ssl = true)
        {
            string protocol = ssl ? "https://" : "http://";
            string url = string.Format("{0}{1}{2}", protocol, host, path);
            this.Headers.Add("Host", host);
            if (formurlencoded)
                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            return this.UploadString(url, data);
        }




        public Byte[] POST_BYTE(byte[] data, string host, string path, bool formurlencoded = true, bool ssl = true)
        {
            string protocol = ssl ? "https://" : "http://";
            string url = string.Format("{0}{1}{2}", protocol, host, path);
            this.Headers.Add("Host", host);
            if (formurlencoded)
                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            return this.UploadData(url, "POST", data);
        }

        public string POST(string data, string url, bool formurlencoded = true)
        {
            if (formurlencoded)
                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            return this.UploadString(url, data);
        }

        public Byte[] POST_BYTE(byte[] data, string url, bool formurlencoded = true)
        {
            if (formurlencoded)
                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            return this.UploadData(url, "POST", data);
        }

    }
}
