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
using GoogleDriveDemo.ViewModel.Box;
using GoogleDriveDemo.ViewModel.Common;
using System.Collections.ObjectModel;

namespace GoogleDriveDemo.View.Control.Box
{
    /// <summary>
    /// BoxDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class BoxDetailControl : UserControl
    {
        private FileEntity selectFile;

        public BoxDetailControl()
        {
            InitializeComponent();
        }

        private void Window_Load(object sender, RoutedEventArgs e)
        {

        }

        private void Load(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BoxDetailViewModel)
            {
                BoxDetailViewModel _detailVM = this.DataContext as BoxDetailViewModel;
                if (_detailVM != null)
                {
                    if (selectFile != null)
                    {
                        if (selectFile.IsFile)
                        {
                            _detailVM.Load(selectFile);
                        }
                    }
                }
            }
        }

        private void Upload(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BoxDetailViewModel)
            {
                BoxDetailViewModel _detailVM = this.DataContext as BoxDetailViewModel;
                if (_detailVM != null)
                {
                    if (selectFile == null)
                    {
                        _detailVM.Upload("0");
                    }
                    else
                    {
                        if (selectFile.IsFile)
                        {
                            _detailVM.Upload(selectFile.ParentID);
                        }
                        else
                        {
                            _detailVM.Upload(selectFile.FileId);
                        }
                    }
                }
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BoxDetailViewModel)
            {
                BoxDetailViewModel _detailVM = this.DataContext as BoxDetailViewModel;
                if (_detailVM != null)
                {
                    _detailVM.Refresh();
                }
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BoxDetailViewModel)
            {
                BoxDetailViewModel _detailVM = this.DataContext as BoxDetailViewModel;
                if (_detailVM != null)
                {
                    if (selectFile != null)
                    {
                        _detailVM.Delete(selectFile);
                    }
                }
            }
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BoxDetailViewModel)
            {
                BoxDetailViewModel _detailVM = this.DataContext as BoxDetailViewModel;
                if (_detailVM != null)
                {
                    if (selectFile == null)
                    {
                        _detailVM.Create("test");
                    }
                    else
                    {
                        if (selectFile.IsFile)
                        {
                            _detailVM.Create("zhang", selectFile.ParentID);
                        }
                        else
                        {
                            _detailVM.Create("zhangluqi", selectFile.FileId);
                        }
                    }
                }
            }
        }

        private void Select(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is FileEntity)
            {
                FileEntity fileEntity = e.NewValue as FileEntity;
                selectFile = fileEntity;
                if (this.DataContext is BoxDetailViewModel)
                {
                    BoxDetailViewModel _detailVM = this.DataContext as BoxDetailViewModel;
                    if (_detailVM != null)
                    {
                        _detailVM.Search(fileEntity);
                    }
                }

            }
        }

        private void Select(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                FileEntity fileEntity = e.AddedItems[0] as FileEntity;
                selectFile = fileEntity;
                if (this.DataContext is BoxDetailViewModel)
                {
                    BoxDetailViewModel _detailVM = this.DataContext as BoxDetailViewModel;
                    if (_detailVM != null)
                    {
                        //_detailVM.Search(fileEntity);
                    }
                }
            }
        }
    }
}
