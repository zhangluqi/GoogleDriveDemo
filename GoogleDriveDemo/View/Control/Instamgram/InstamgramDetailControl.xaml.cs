using GoogleDriveDemo.ViewModel.Common;
using GoogleDriveDemo.ViewModel.Instamgram;
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

namespace GoogleDriveDemo.View.Control.Instamgram
{
    /// <summary>
    /// InstamgramDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class InstamgramDetailControl : UserControl
    {
        private InstamGramDetailViewModel _detailViewModel;
        public InstamgramDetailControl()
        {
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e)
        {

        }

        private void Refer(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ImageEntity> instamgramList = _detailViewModel.DetailVM;
            if (instamgramList != null)
            {
                foreach (var instamgramEntity in instamgramList)
                {
                    if (instamgramEntity.IsChecked)
                    {
                        _detailViewModel.Load(instamgramEntity.Url, "");
                    }
                }
            }
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            _detailViewModel = new InstamGramDetailViewModel();
            fileListbox.ItemsSource = _detailViewModel.DetailVM;
            Progrecess.DataContext = _detailViewModel.Progrecess;
            _detailViewModel.Search();
        }
    }
}
