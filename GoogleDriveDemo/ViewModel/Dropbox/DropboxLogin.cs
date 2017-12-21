using Dropbox.Api;
using GoogleDriveDemo.Model.Dropbox;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.View.Control.Dropbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoogleDriveDemo.ViewModel.Dropbox
{
    public class DropboxLogin : ILogin
    {
        public void ClearAuthRecord()
        {
            throw new NotImplementedException();
        }

        public async void Login()
        {
            AuthDropbox authDropbox = new AuthDropbox();
            DropboxClient dropboxClient =await authDropbox.GetDropboxClient();
            if (dropboxClient != null)
            {
                ServiceManager.Instence().DropboxClient = dropboxClient;

                //跳转到文件扫描窗口
                UserControl detailControl = new DropboxDetailControl();
                MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
            }
            else
            {
                MessageBox.Show("Auth dropbox api failed!!!");
            }
        }
    }
}
