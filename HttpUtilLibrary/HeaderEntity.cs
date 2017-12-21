using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpUtilLibrary
{
    public class HeaderEntity
    {
        public static string ACCEPT = "Accept";
        public static string AUTHORIZATION = "Authorization";
        public static string CONTENTTYPE = "Content-Type";
        public static string USERAGENT = "User-Agent";
        public static string XAPPLEMMCSDATACLASS = "x-apple-mmcs-dataclass";
        public static string XAPPLEMMCSAUTH = "x-apple-mmcs-auth";
        public static string XAPPLEMMCSPROTOVERSION = "x-apple-mmcs-proto-version";
        public static string XAPPLEMMEDSID = "x-apple-mme-dsid";
        public static string XAPPLEREQUESTUUID = "X-Apple-Request-UUID";
        public static string XCLOUDKITAUTHTOKEN = "X-CloudKit-AuthToken";
        public static string XCLOUDKITBUNDLEID = "X-CloudKit-BundleId";
        public static string XCLOUDKITCONTAINERID = "X-CloudKit-ContainerId";
        public static string XCLOUDKITCONTAINER = "X-CloudKit-Container";
        public static string XCLOUDKITENVIRONMENT = "X-CloudKit-Environment";
        public static string XCLOUDKITPARTITION = "X-CloudKit-Partition";
        public static string XCLOUDKITPROTOCOLVERSION = "X-CloudKit-ProtocolVersion";
        public static string XCLOUDKITUSERID = "X-CloudKit-UserId";
        public static string XCLOUDKITZONES = "X-CloudKit-Zone";
        public static string XMMECLIENTINFO = "X-Mme-Client-Info";

        private string _header = null;
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

        private string _value = null;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public HeaderEntity(string header, string value)
        {
            this.Header = header;
            this.Value = value;
        }
    }
}
