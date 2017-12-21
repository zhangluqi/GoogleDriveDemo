using Dropbox.Api;
using Facebook;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box.V2;
//using GoogleDriveDemo.Model.Instagram;

namespace GoogleDriveDemo.Util
{
    public class ServiceManager
    {
        #region Fileds
        private DropboxClient _dropboxClient;//Dropbox
        public DropboxClient DropboxClient
        {
            get { return _dropboxClient; }
            set { _dropboxClient = value; }
        }

        private GmailService _gmailServive;   //Google Gmail
        public GmailService GmailServive
        {
            get { return _gmailServive; }
            set { _gmailServive = value; }
        }

        private FacebookClient _facebookClient; //Facebook
        public FacebookClient FacebookClient
        {
            get { return _facebookClient; }
            set { _facebookClient = value; }
        }

        private DriveService _driveService;  //Goolge Drive
        public DriveService DriveService
        {
            get { return _driveService; }
            set { _driveService = value; }
        }

        private BoxClient _boxClient;
        public BoxClient BoxClient
        {
            get { return _boxClient; }
            set { _boxClient = value; }
        }

        private GraphServiceClient _graphClient;//微软OneDrive
        public GraphServiceClient GraphClient
        {
            get { return _graphClient; }
            set { _graphClient = value; }
        }


        /*private InstamgramClient _instamgramClient;

        public InstamgramClient InstamgramClient
        {
            get { return _instamgramClient; }
            set { _instamgramClient = value; }
        }*/


        #endregion
        #region  Singleton
        private ServiceManager()
        {

        }
        private static readonly object syncLock = new object();
        private static volatile ServiceManager _instance = null;

        public static ServiceManager Instence()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServiceManager();

                    }
                }
            }
            return _instance;
        }
        #endregion
    }
}
