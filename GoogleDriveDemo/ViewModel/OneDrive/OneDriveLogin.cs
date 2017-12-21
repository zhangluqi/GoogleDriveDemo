using GoogleDriveDemo.Model.Amazon;
//using GoogleDriveDemo.Model.Instagram;
using GoogleDriveDemo.Model.OneDrive;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.View.Control;
using GoogleDriveDemo.View.Control.OneDrive;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoogleDriveDemo.ViewModel.OneDrive
{
    public class OneDriveLogin : ILogin
    {
        public void ClearAuthRecord()
        {
            //测试Amazon
            //AuthAmazon authAmazon = new AuthAmazon();
            //authAmazon.test();


        }

        public void Login()
        {
            
                AuthOneDrive authOneDrive = new AuthOneDrive();
                GraphServiceClient graphClient = authOneDrive.GetAuthenticatedClient();
                if (graphClient != null)
                {
                    ServiceManager.Instence().GraphClient = graphClient;

                    //跳转到文件扫描窗口
                    UserControl detailControl = new OnedriveControl();
                    MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
                }
                else
                {
                    MessageBox.Show("Auth onedrive api failed!!!");
                }
          
        }
    }
}
