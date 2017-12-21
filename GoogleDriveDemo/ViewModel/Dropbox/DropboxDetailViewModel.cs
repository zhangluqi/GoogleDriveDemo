using GoogleDriveDemo.Model.Dropbox;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveDemo.ViewModel.Dropbox
{
    public class DropboxDetailViewModel:ViewModelBase
    {
        private ObservableCollection<FileEntity> _detailVM = new ObservableCollection<FileEntity>();
        public ObservableCollection<FileEntity> DetailVM
        {
            get
            {
                return _detailVM;
            }

            set
            {
                _detailVM = value;
            }
        }

        public ProgrecessVM Progrecess
        {
            get
            {
                return _progrecess;
            }

            set
            {
                _progrecess = value;
                base.OnPropertyChanged("Progrecess");
            }
        }

        private ProgrecessVM _progrecess = new ProgrecessVM();

        private DropboxFileUnload _dropboxFileUpload;
        private DropboxFileLoad _dropboxFileLoad;
        public void Search(FileEntity selectedfileEntity=null)
        {
            string parentId = selectedfileEntity == null ? null : selectedfileEntity.ParentID+"/"+ selectedfileEntity.FileName; //这里只认/方向的斜线，所有不能用Path.Comebine
            Thread th = new Thread(() => {
                DropboxFileSearch dropboxFileSearch = new DropboxFileSearch();
                dropboxFileSearch.SearchFile(ServiceManager.Instence().DropboxClient,
                    t => {
                        //防止重复加载已经请求的数据
                        if (selectedfileEntity != null && selectedfileEntity.ChildFileList != null)
                        {
                            if (selectedfileEntity.ChildFileList.FirstOrDefault(f => f.FileId == t.FileId) != null)
                            {
                                return;
                            }
                        }
                        FileEntity fileEntity = new FileEntity()
                        {
                            ParentID = t.ParentId,
                            FileId = t.FileId,
                            FileName = t.FileName,
                            FileSize = t.FileSize,
                            IsFile = t.IsFile
                        };
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //root目录
                            if (selectedfileEntity == null)
                            {
                                _detailVM.Add(fileEntity);
                            }
                            else
                            {
                                if (selectedfileEntity.ChildFileList == null)
                                {
                                    selectedfileEntity.ChildFileList = new ObservableCollection<FileEntity>();
                                }
                                ((ObservableCollection<FileEntity>)selectedfileEntity.ChildFileList).Add(fileEntity);
                            }

                        }));

                    },parentId);

            });
            th.Name = "SearchDropboxFileThread";
            th.Start();
        }
        
        /// <summary>
        ///上传文件
        /// </summary>
        public void Upload(string parentID)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "zip";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string fileName = openFileDialog.FileName;
            _dropboxFileUpload = new DropboxFileUnload();
            _dropboxFileUpload.ProgressEvent += new DropboxFileUnload.ProgressHandler(ProgressEvent);
            _dropboxFileUpload.FinishedEvent += new DropboxFileUnload.FinishedHandler(FinishedEvent);
            //目前直接传到root目录下
            _dropboxFileUpload.UpLoadFile(ServiceManager.Instence().DropboxClient,parentID, fileName);

        }
        
        /// <summary>
        /// 下载文件
        /// </summary>
        public void Load(FileEntity selectedfileEntity)//string fileId, string fileName, long fileSize)
        {
            _dropboxFileLoad = new DropboxFileLoad();
            _dropboxFileLoad.ProgressEvent += new DropboxFileLoad.ProgressHandler(ProgressEvent);
            _dropboxFileLoad.FinishedEvent += new DropboxFileLoad.FinishedHandler(FinishedEvent);
            string dir = SettingManager.Instence().SaveDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string savePath = Path.Combine(dir, selectedfileEntity.FileName);
            _dropboxFileLoad.DownLoadFile(ServiceManager.Instence().DropboxClient,selectedfileEntity.ParentID, selectedfileEntity.FileName,  savePath,long.Parse(selectedfileEntity.FileSize));
        }


        public void Delete(FileEntity selectedfileEntity)
        {
            DropboxDeleteFile dropboxDeleteFile = new DropboxDeleteFile();
            dropboxDeleteFile.DeleteFileOrFolder(ServiceManager.Instence().DropboxClient,selectedfileEntity.ParentID,selectedfileEntity.FileName);
        }

        public void Create(string parentID)
        {
            DropboxCreateFile dropboxCreateFile = new DropboxCreateFile();
            dropboxCreateFile.CreateFolder(ServiceManager.Instence().DropboxClient, parentID, "iMobie" + DateTime.Now.Ticks.ToString());
        }



        private void FinishedEvent()
        {
            //下载完成
            Progrecess.Result = "Completed";
            Progrecess.Rate = 100;
            if (_dropboxFileUpload != null)
            {
                _dropboxFileUpload.ProgressEvent -= ProgressEvent;
                _dropboxFileUpload.FinishedEvent -= FinishedEvent;
            }

            if (_dropboxFileLoad != null)
            {
                _dropboxFileLoad.ProgressEvent -= ProgressEvent;
                _dropboxFileLoad.FinishedEvent -= FinishedEvent;
            }
        }

        private void ProgressEvent(long downloadedSize, long fileSize)
        {
            double rate = (double)downloadedSize / fileSize * 100;
            rate = rate > 100 ? 100 : rate;
            Progrecess.Rate = rate;
            Progrecess.Result = string.Format("{0:N2} %", rate);
        }
    }
}
