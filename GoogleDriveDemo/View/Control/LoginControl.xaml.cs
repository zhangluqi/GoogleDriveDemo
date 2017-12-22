using GoogleDriveDemo.ViewModel;
using GoogleDriveDemo.ViewModel.Dropbox;
using GoogleDriveDemo.ViewModel.Facebook;
using GoogleDriveDemo.ViewModel.Gmail;
using GoogleDriveDemo.ViewModel.Google;
using GoogleDriveDemo.ViewModel.OneDrive;
using GoogleDriveDemo.ViewModel.Box;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Clouder;
//using GoogleDriveDemo.Model.Instagram;
using GoogleDriveDemo.Model.Amazon;
using CloudManagerment;
using CloudObject.EventHandler;

namespace GoogleDriveDemo.View.Control
{
    /// <summary>
    /// LoginControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private ILogin _loginViewModel;
        public LoginControl()
        {
            InitializeComponent();
        }

        private Cloudbase cloudbase = null;
        private void Window_OnLoad(object sender, RoutedEventArgs e)
        {
            CloudManager.Progress += CloudManager_Progress;
            CloudManager.Exception += CloudManager_Exception;
            CloudManager.Login("zhangluqi@outlook.com", "12345678");
            List<Cloudbase> list = CloudManager.GetCloudList();
            foreach (var item in list)
            {
                if (item != null)
                {
                    cloudbase = item;
                }
            }
        }

        /// <summary>
        /// 登陆OneDrive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_OneDrive(object sender, RoutedEventArgs e)
        {
            _loginViewModel = new OneDriveLogin();
            _loginViewModel.Login(cloudbase);
        }

        private void Clear_OneDrive_Auth(object sender, RoutedEventArgs e)
        {
            //if (_loginViewModel == null)
            //{
            //    _loginViewModel = new OneDriveLogin();
            //}
            //_loginViewModel.ClearAuthRecord();   
        }

        private void CloudManager_Exception(ExceptionEventHandler obj)
        {
             
        }

        private void CloudManager_Progress(ProgressEventhandler obj)
        {
            
        }

        private void Login_Google(object sender, RoutedEventArgs e)
        {
            //_loginViewModel = new LoginViewModel();
            //_loginViewModel.Login();
        }

        private void Clear_Auth(object sender, RoutedEventArgs e)
        {
            //_loginViewModel = new LoginViewModel();
            //if (_loginViewModel != null)
            //{
            //    _loginViewModel.ClearAuthRecord();
            //}
        }



        private void Login_DropBox(object sender, RoutedEventArgs e)
        {
            //_loginViewModel = new DropboxLogin();
            //_loginViewModel.Login();
        }

        private void Clear_DropBox_Auth(object sender, RoutedEventArgs e)
        {
            //if(_loginViewModel != null)
            //{
            //    _loginViewModel.ClearAuthRecord();
            //}
        }

        private void Login_Gmail(object sender, RoutedEventArgs e)
        {
            //_loginViewModel = new GmailLogin();
            //_loginViewModel.Login();

        }

        private void Clear_GmailAuth(object sender, RoutedEventArgs e)
        {
            //if (_loginViewModel != null)
            //{
            //    _loginViewModel.ClearAuthRecord();
            //}
        }

        private void Login_Facebook(object sender, RoutedEventArgs e)
        {
            //_loginViewModel = new FacebookLogin();
            //_loginViewModel.Login();
        }

        private void Clear_Facebook(object sender, RoutedEventArgs e)
        {
            //if (_loginViewModel == null)
            //{
            //    _loginViewModel = new BoxLogin();
            //}
            //    _loginViewModel = new FacebookLogin();
            //    _loginViewModel.ClearAuthRecord();
            
        }

        private void Login_box(object sender, RoutedEventArgs e)
        {
            //_loginViewModel = new BoxLogin();
            //_loginViewModel.Login();
        }

        private void Clear_Box(object sender, RoutedEventArgs e)
        {
            //if (_loginViewModel == null)
            //{
            //    _loginViewModel = new BoxLogin();
            //    _loginViewModel.ClearAuthRecord();
            //}
        }

        private void Clear_Instagram(object sender, RoutedEventArgs e)
        {

        }

        private void Login_Instagram(object sender, RoutedEventArgs e)
        {
            //测试
            //AuthInstagram authInstagram = new AuthInstagram();
            //authInstagram.GetInstagramClient();

            //AuthAmazon authAmazon = new AuthAmazon();
            //authAmazon.test();

        }

        private void Login_newOneDrive(object sender, RoutedEventArgs e)
        {

        }
    }
}
