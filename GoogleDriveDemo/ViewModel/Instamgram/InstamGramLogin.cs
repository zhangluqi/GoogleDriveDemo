using GoogleDriveDemo.Model.Instagram;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.View.Control.Instamgram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoogleDriveDemo.ViewModel.Instamgram
{
    public class InstamGramLogin : ILogin
    {
        private AuthInstagram authInstagram;
        public void ClearAuthRecord()
        {
            throw new NotImplementedException();
        }

        public  async void Login()
        {
      
                authInstagram = new AuthInstagram();
                InstamgramClient instamgramClient = await authInstagram.GetInstagramClient();

                if (instamgramClient != null)
                {
                    ServiceManager.Instence().InstamgramClient = instamgramClient;

                    //跳转到文件扫描窗口
                    UserControl detailControl = new InstamgramDetailControl();
                    MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
                }
                else
                {
                    MessageBox.Show("Auth facebook api failed!!!");
                }
        
        }
    }
}
