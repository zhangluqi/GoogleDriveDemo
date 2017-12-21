using GoogleDriveDemo.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Facebook
{
    public class FacebookLoad
    {
        #region Events
        public delegate void ProgressHandler(long downloadedSize, long fileSize);
        public event ProgressHandler ProgressEvent;

        public delegate void FinishedHandler();
        public event FinishedHandler FinishedEvent;

        public delegate void FailedHandler();
        public event FailedHandler FailedEvent;
        #endregion

        public void DownLoadFile( string url,string savePath)
        {
            NetUtil netUtil = new NetUtil();
            netUtil.Download(url, savePath);


        }


     
    }
}
