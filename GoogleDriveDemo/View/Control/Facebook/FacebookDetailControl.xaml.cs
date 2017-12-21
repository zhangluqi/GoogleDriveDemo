using GoogleDriveDemo.ViewModel.Common;
using GoogleDriveDemo.ViewModel.Facebook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GoogleDriveDemo.View.Control.Facebook
{
    /// <summary>
    /// FacebookDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class FacebookDetailControl : UserControl
    {
        private FacebookDetailViewModel _detailViewModel;
        public FacebookDetailControl()
        {
            InitializeComponent();
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            _detailViewModel = new FacebookDetailViewModel();
            fileListbox.ItemsSource = _detailViewModel.DetailVM;
            Progrecess.DataContext = _detailViewModel.Progrecess;
            _detailViewModel.Search();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            ObservableCollection<FacebookEntity> facebookList = _detailViewModel.DetailVM;
            if (facebookList != null)
            {
                foreach (var facebookEntity in facebookList)
                {
                    if (facebookEntity.IsChecked)
                    {
                        _detailViewModel.Load(facebookEntity.Url,"");
                    }
                }
            }
        }

        private void Refer(object sender, RoutedEventArgs e)
        {

        }
    }
}
