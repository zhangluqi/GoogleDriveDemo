using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;
using GoogleDriveDemo.ViewModel;
using GoogleDriveDemo.View.Control.Box;
using GoogleDriveDemo.Model.Box;
using GoogleDriveDemo.Util;
using Box.V2;
using Clouder;

namespace GoogleDriveDemo.ViewModel.Box
{
    public class BoxLogin : ILogin
    {
        void ILogin.ClearAuthRecord()
        {
            throw new NotImplementedException();
        }

        async void ILogin.Login(Cloudbase cloudbase)
        {
            AuthBox authBox = new AuthBox();
            BoxClient boxClient = await authBox.GetBoxClient();
            if (boxClient != null)
            {
                ServiceManager.Instence().BoxClient = boxClient;
                BoxDetailViewModel boxDetailVM = new BoxDetailViewModel();
                BoxDetailControl boxDetailControl = new BoxDetailControl();
                boxDetailControl.DataContext = boxDetailVM;
                MainWindowManager.Instence().MainVM.MiddleDataControl = boxDetailControl;
            }
            else
            {
                MessageBox.Show("Auth box api failed!!!");
            }
            
        }
    }
}
