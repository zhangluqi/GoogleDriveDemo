using Google.Apis.Gmail.v1;
using GoogleDriveDemo.Model.Gmail;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.View.Control.Gmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoogleDriveDemo.ViewModel.Gmail
{
    public class GmailLogin : ILogin
    {
        public void ClearAuthRecord()
        {
            throw new NotImplementedException();
        }

        public void Login()
        {
            AuthGmail authGmail = new AuthGmail();
            GmailService dropboxClient = authGmail.GetService();
            if (dropboxClient != null)
            {
                ServiceManager.Instence().GmailServive = dropboxClient;

                //跳转到文件扫描窗口
                UserControl detailControl = new GmailDetailControl();
                MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
            }
            else
            {
                MessageBox.Show("Auth gmail api failed!!!");
            }
        }
    }
}
