using GoogleDriveDemo.Model.Google;
using GoogleDriveDemo.ViewModel.Common;
using GoogleDriveDemo.ViewModel.Google;
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

namespace GoogleDriveDemo.View.Control.Google
{
    /// <summary>
    /// GoogleDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class GoogleDetailControl : UserControl
    {
        private DetailViewModel _detailViewModel;
        private string _parentID;
        public GoogleDetailControl()
        {
            InitializeComponent();
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            _detailViewModel = new DetailViewModel();
            fileTree.ItemsSource = _detailViewModel.DetailVM;
            fileListbox.ItemsSource = _detailViewModel.DetailVM;
            Progrecess.DataContext = _detailViewModel.Progrecess;
            _detailViewModel.Search(null);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Upload(object sender, RoutedEventArgs e)
        {

            _detailViewModel.Upload(_parentID);

        }

        private void Refer(object sender, RoutedEventArgs e)
        {
            _detailViewModel.Search(null);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load(object sender, RoutedEventArgs e)
        {
            ObservableCollection<FileEntity> fileList = fileListbox.ItemsSource as ObservableCollection<FileEntity>;
            if (fileList != null)
            {
                foreach (var fileEntity in fileList)
                {
                    if (fileEntity.IsFile && fileEntity.IsChecked)
                    {
                        long fileSize = 1;
                        long.TryParse(fileEntity.FileSize,out fileSize);
                        //未拿到文件长度的，之后要进行特殊处理
                        _detailViewModel.Load(fileEntity.FileId, fileEntity.FileName, fileSize);
                    }
                }

            }
        }


        private void Delete(object sender, RoutedEventArgs e)
        {
            ObservableCollection<FileEntity> fileList = fileListbox.ItemsSource as ObservableCollection<FileEntity>;
            if (fileList != null)
            {
                foreach (var fileEntity in fileList)
                {
                    if (fileEntity.IsChecked)
                    {
                        _detailViewModel.Delete(fileEntity.FileId);
                    }
                }
            }
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            _detailViewModel.Create(_parentID);
        }


        private void Select(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is FileEntity)
            {
                FileEntity fileEntity = e.NewValue as FileEntity;
                _parentID = fileEntity.FileId;
                if (!fileEntity.IsFile)
                {
                    fileListbox.ItemsSource = fileEntity.ChildFileList;
                }
                else
                {
                    fileListbox.ItemsSource = new ObservableCollection<FileEntity>() { fileEntity };
                }

                if (!fileEntity.IsFile)
                {
                    _detailViewModel.Search(fileEntity);
                }

            }
        }
    }
}
