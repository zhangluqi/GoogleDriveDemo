using Facebook;
using GoogleDriveDemo.Model.Facebook;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.View.Control.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoogleDriveDemo.ViewModel.Facebook
{
    public class FacebookLogin : ILogin
    {
        public FacebookLogin()
        {
          
        }

        AuthFacebook authFacebook;
        public void ClearAuthRecord()
        {
            if(authFacebook != null)
            {
                authFacebook.RevokePermission(ServiceManager.Instence().FacebookClient);
            }
          
        }

        public async void Login()
        {
            authFacebook = new AuthFacebook();
            FacebookClient facebookClient = await authFacebook.GetFacebookClient();
            if (facebookClient != null)
            {
                ServiceManager.Instence().FacebookClient = facebookClient;

                //跳转到文件扫描窗口
                UserControl detailControl = new FacebookDetailControl();
                MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
            }
            else
            {
                MessageBox.Show("Auth facebook api failed!!!");
            }
        }
    }
}
