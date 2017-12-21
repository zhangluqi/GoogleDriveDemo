using GoogleDriveDemo.ViewModel.Common;
using GoogleDriveDemo.ViewModel.Gmail;
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

namespace GoogleDriveDemo.View.Control.Gmail
{
    /// <summary>
    /// GmailDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class GmailDetailControl : UserControl
    {
        private GmailDetailViewModel _detailViewModel;
        public GmailDetailControl()
        {
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            ObservableCollection<MailEntity> mailList = _detailViewModel.DetailVM;
            if (mailList != null)
            {
                foreach (var mailEntity in mailList)
                {
                    if (mailEntity.IsChecked)
                    {
                        _detailViewModel.Load(mailEntity.Id);
                    }
                }
            }
        }

        private void Refer(object sender, RoutedEventArgs e)
        {
            _detailViewModel.Search();
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            _detailViewModel = new GmailDetailViewModel();
            fileListbox.ItemsSource = _detailViewModel.DetailVM;
            Progrecess.DataContext = _detailViewModel.Progrecess;
            _detailViewModel.Search();
        }
    }
}
