using GoogleDriveDemo.Model.OneDrive;
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
using CloudObject;
using OneDrive;
using OnedriveDeleteFile = OneDrive.OneDriveOperation.OnedriveDeleteFile;
using CloudManagerment;
using CloudObject.EventHandler;
using Clouder;

namespace GoogleDriveDemo.ViewModel.OneDrive
{
    public class OneDriveDetailViewModel : ViewModelBase
    {
        private ObservableCollection<FileEntity> _detailVM = new ObservableCollection<FileEntity>();
        public ObservableCollection<FileEntity> DetailVM
        {
            get { return _detailVM; }
            set { _detailVM = value; }
        }

        public Cloudbase oneDriveCloud = new OneDriveManager();

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

        private OneDriveFileUpload _oneDriveFileUpload;
        private OneDriveFileLoad _oneDriveFileLoad;


        public void Search(FileEntity selectedfileEntity)
        {
            string fileId = selectedfileEntity != null ? selectedfileEntity.FileId : null;
            string parentId = selectedfileEntity != null ? selectedfileEntity.ParentID : "root";
            
            IList<FileInformation> listFileInfo = null;
            listFileInfo = CloudManager.Search(oneDriveCloud.CloudId,parentId);
            if (listFileInfo != null && listFileInfo.Count > 0)
            {
                foreach (var item in listFileInfo)
                {
                    if (item != null)
                    {
                        FileEntity fileEntity = new FileEntity
                        {
                            ParentID = item.ParentId,
                            FileId = item.FileId,
                            FileName = item.FileName,
                            FileSize = item.FileSize.ToString(),
                            IsFile = item.IsFolder
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
                    }
                }
            }
        }

        public void Create(string parentID)
        {
            CloudManager.CreateFolder(oneDriveCloud.CloudId, parentID, "aosdiosaido");//.CreateFolder(parentID, "testtttt");
        }

        public void Delete(string fileId)
        {
            CloudManager.Delete(oneDriveCloud.CloudId, fileId);
        }

        public async void Load(FileEntity fileId, string fileName, long fileSize)
        {
            OneDriveManager oneDrive = new OneDriveManager();
            oneDrive.Progress += OneDrive_Progress;
            oneDrive.Exception += OneDrive_Exception;
            CloudObject.FileInformation f = new CloudObject.FileInformation
            {
                FileName = fileId.FileName,
                FileId = fileId.FileId,
                FileSize = long.Parse(fileId.FileSize),
                IsFolder = fileId.IsFile,
                ParentId = fileId.ParentID
            };
            bool isSuccess = await oneDrive.Start(f, @"E:\asd", Clouder.OperaType.DownLoad);
        }



        public void Stop()
        {
            OneDriveManager oneDrive = new OneDriveManager();
            //bool isSuccess = oneDrive.Pause("");
        }

        /// <summary>
        ///上传文件
        /// </summary>
        public async void Upload(string parentID)
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

            System.IO.FileInfo info = new System.IO.FileInfo(openFileDialog.FileName);
            OneDriveManager oneDrive = new OneDriveManager();
            oneDrive.Progress += OneDrive_Progress;
            oneDrive.Exception += OneDrive_Exception;

            CloudObject.FileInformation f = new CloudObject.FileInformation
            {
                ParentId = parentID,
                FileName = info.Name,
                FileSize = info.Length,
                FilePath = info.Directory.ToString()
            };
            bool isSuccess = await oneDrive.Start(f, parentID, Clouder.OperaType.UpLoad);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        

        private void OneDrive_Exception(CloudObject.EventHandler.ExceptionEventHandler obj)
        {
            
        }

        private void OneDrive_Progress(CloudObject.EventHandler.ProgressEventhandler obj)
        {
            double rate = (double)obj.Progress * 100 ;
            rate = rate > 100 ? 100 : rate;
            Progrecess.Rate = rate;
            Progrecess.Result = string.Format("{0:N2} %", rate);
        }



       

        private void FinishedEvent()
        {
            //下载完成
            Progrecess.Result = "Completed";
            Progrecess.Rate = 100;
            if(_oneDriveFileLoad != null)
            {
                _oneDriveFileLoad.ProgressEvent -= ProgressEvent;
                _oneDriveFileLoad.FinishedEvent -= FinishedEvent;
            }
           
            if(_oneDriveFileUpload != null)
            {
                _oneDriveFileUpload.ProgressEvent -= ProgressEvent;
                _oneDriveFileUpload.FinishedEvent -= FinishedEvent;
            }  
        }

        private void ProgressEvent(long downloadedSize, long fileSize)
        {
            double rate = (double)downloadedSize / fileSize * 100;
            rate = rate > 100 ? 100 : rate;
            Progrecess.Rate = rate;
            Progrecess.Result = string.Format("{0:N2} %", rate);
        }


        private void CloudManager_Exception(ExceptionEventHandler obj)
        {
            throw new NotImplementedException();
        }

        private void CloudManager_Progress(ProgressEventhandler obj)
        {
            throw new NotImplementedException();
        }
    }
}


