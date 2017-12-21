using GoogleDriveDemo.Model.Google;
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
using System.Windows;
using System.Windows.Forms;

namespace GoogleDriveDemo.ViewModel.Google
{
    public class DetailViewModel:ViewModelBase 
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

        public void Search(FileEntity selectedfileEntity)
        {

            string fileId = selectedfileEntity != null ? selectedfileEntity.FileId : null;
            string parentId = selectedfileEntity != null ? selectedfileEntity.ParentID : null;
            Thread th = new Thread(() => {
                GoogleFileSearch googleFileSearch = new GoogleFileSearch();
                googleFileSearch.SearchFile(ServiceManager.Instence().DriveService,
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
                            ParentID = parentId,
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

                    }, fileId);

            });
            th.Name = "SearchGoogleFileThread";
            th.Start();
        }

        /// <summary>
        ///上传文件
        /// </summary>
        public void Upload(string parentId)
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
            GoogleFileUpload googleFileUpload = new GoogleFileUpload();
            googleFileUpload.ProgressEvent += new GoogleFileUpload.ProgressHandler(GoogleFileLoad_ProgressEvent);
            googleFileUpload.FinishedEvent += new GoogleFileUpload.FinishedHandler(GoogleFileLoad_FinishedEvent);

            googleFileUpload.UpLoadFile(ServiceManager.Instence().DriveService, fileName,parentId);
            googleFileUpload.ProgressEvent -= GoogleFileLoad_ProgressEvent;
            googleFileUpload.FinishedEvent -= GoogleFileLoad_FinishedEvent;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public void Load(string fileId,string fileName,long fileSize)
        {
            GoogleFileLoad googleFileLoad = new GoogleFileLoad();
            googleFileLoad.ProgressEvent += new GoogleFileLoad.ProgressHandler( GoogleFileLoad_ProgressEvent);
            googleFileLoad.FinishedEvent += new GoogleFileLoad.FinishedHandler( GoogleFileLoad_FinishedEvent);
            string dir= SettingManager.Instence().SaveDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string savePath = Path.Combine(dir, fileName);
            Thread th = new Thread(() => {
                googleFileLoad.DownLoadFile(ServiceManager.Instence().DriveService, fileId, fileSize, savePath);
                googleFileLoad.ProgressEvent -= GoogleFileLoad_ProgressEvent;
                googleFileLoad.FinishedEvent -= GoogleFileLoad_FinishedEvent;
            });
            th.Name = "LoadGoogleFileThread";
            th.Start();
        }


        public void Delete(string fileId)
        {
            GoogleDeleteFile googleDeleteFile = new GoogleDeleteFile();
            googleDeleteFile.DeleteFileOrFolder(ServiceManager.Instence().DriveService, fileId);
        }

        public void Create(string parentID)
        {
            GoogleCreateFile googleCreateFile = new GoogleCreateFile();
            googleCreateFile.CreateFolder(ServiceManager.Instence().DriveService, "iMobie" + DateTime.Now.Ticks.ToString(), parentID);
        }

        private void GoogleFileLoad_FinishedEvent()
        {
            //下载完成
            Progrecess.Result = "Completed";
            Progrecess.Rate = 100;
        }

        private void GoogleFileLoad_ProgressEvent(long downloadedSize, long fileSize)
        {
            double rate = (double)downloadedSize / fileSize *100;
            rate = rate > 100 ? 100 : rate;
            Progrecess.Rate = rate;
            Progrecess.Result = string.Format("{0:N2} %", rate);
        }
    }
}
