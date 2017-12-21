using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoogleDriveDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainWindowViewModel mainVM = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            MainWindowManager.Instence().MainVM = mainVM;
            this.DataContext = mainVM;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
