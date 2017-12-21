using HttpUtilLibrary;
using Oauth2Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Instagram
{
     public class InstamgramClient
    {
       
        public InstamgramClient(UserCredential userCredential)
        {
            this._userCredential = userCredential;
        }
        private UserCredential _userCredential;
        public UserCredential UserCredential
        {
            get { return _userCredential; }
            set { _userCredential = value; }
        }

        /// <summary>
        /// 获取最近发的照片
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public string SearchCurrent(string host, string path)
        {
            NetClient netClient = new NetClient();
            netClient.GetParamDictionary.Add("access_token", _userCredential.AccessToken);
            string photoList = netClient.GETWithParams(host, path, true);
            return photoList;
        }

    }
}
