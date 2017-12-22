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
using Clouder;

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

        public void Login(Cloudbase cloudbase)
        {
            UserControl detailControl = new OnedriveControl(cloudbase);
            MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
        }
    }
}
