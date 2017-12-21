using GoogleDriveDemo.Util;
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

namespace GoogleDriveDemo.View.Control
{
    /// <summary>
    /// TopControl.xaml 的交互逻辑
    /// </summary>
    public partial class TopControl : UserControl
    {
        public TopControl()
        {
            InitializeComponent();
        }

        private void Back_Home(object sender, RoutedEventArgs e)
        {
            UserControl detailControl = new LoginControl();
            MainWindowManager.Instence().MainVM.MiddleDataControl = detailControl;
        }
    }
}
