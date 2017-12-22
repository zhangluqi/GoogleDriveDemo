using Google.Apis.Drive.v3;
using GoogleDriveDemo.Model.Google;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.View.Control;
using GoogleDriveDemo.View.Control.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Clouder;

namespace GoogleDriveDemo.ViewModel.Google
{
    public class LoginViewModel: ILogin
    {
        public void Login(Cloudbase cloudbase)
        {
           
                AuthGoogle authGoogle = new AuthGoogle();
                DriveService driveService = authGoogle.GetService();
                if (driveService != null)
                {
                    ServiceManager.Instence().DriveService = driveService;

                    //跳转到文件扫描窗口
                    UserControl detailControl = new GoogleDetailControl();
                    MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
                }
                else
                {
                    MessageBox.Show("Auth google api failed!!!");
                }
            
           
        }

        public void ClearAuthRecord()
        {
            AuthGoogle authGoogle = new AuthGoogle();
            authGoogle.ClearAuthRecord();
        }

    }
}
