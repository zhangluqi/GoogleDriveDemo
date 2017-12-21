using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleDriveDemo.ViewModel.Common;
using System.Collections.ObjectModel;
using System.Windows;
using GoogleDriveDemo.Util;
using Box.V2;
using Box.V2.Models;
using GoogleDriveDemo.Model.Box;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Box.V2.Utility;

namespace GoogleDriveDemo.ViewModel.Box
{
    public class BoxDetailViewModel : ViewModelBase
    {
        private ObservableCollection<FileEntity> _boxDetailFile;
        public ObservableCollection<FileEntity> BoxDetailFile
        {
            get { return _boxDetailFile; }
            set
            {
                _boxDetailFile = value;
                base.OnPropertyChanged("BoxDetailFile");
            }
        }

        private ObservableCollection<FileEntity> _boxDetailFileChild;
        public ObservableCollection<FileEntity> BoxDetailFileChild
        {
            get { return _boxDetailFileChild; }
            set
            {
                _boxDetailFileChild = value;
                base.OnPropertyChanged("BoxDetailFileChild");
            }
        }

        private ProgrecessVM _progrecess = new ProgrecessVM();
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

        public BoxDetailViewModel()
        {
            var client = ServiceManager.Instence().BoxClient;
            if (client != null)
            {
                try
                {
                    InitBoxRootFolderItems(client);//client.FoldersManager.GetFolderItemsAsync("0", 500);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private BoxDownLoadFile boxDownLoad;

        public async void InitBoxRootFolderItems(BoxClient client)
        {
            BoxCollection<BoxItem> items = null;
            if (BoxDetailFile == null)
            {
                BoxDetailFile = new ObservableCollection<FileEntity>();
            }
            if (BoxDetailFile != null && BoxDetailFile.Count > 0)
            {
                BoxDetailFile.Clear();
            }
            if (BoxDetailFileChild == null)
            {
                BoxDetailFileChild = new ObservableCollection<FileEntity>();
            }
            if (BoxDetailFileChild != null && BoxDetailFile.Count > 0)
            {
                BoxDetailFileChild.Clear();
            }
            try
            {
                if (client != null)
                {
                    items = await client.FoldersManager.GetFolderItemsAsync("0", 500);
                    if (items != null)
                    {
                        foreach (var item in items.Entries)
                        {
                            if (item != null)
                            {

                                var fileEntity = new FileEntity();
                                if (item.Type.ToLower().ToString().Equals("file"))
                                {
                                    var boxfile = await client.FilesManager.GetInformationAsync(item.Id);
                                    fileEntity.FileName = boxfile.Name;
                                    fileEntity.FileId = boxfile.Id;
                                    fileEntity.FileSize = Convert.ToString(boxfile.Size);
                                    fileEntity.ParentID = boxfile.Parent.Id;
                                    fileEntity.IsFile = true;
                                }
                                else if (item.Type.ToLower().ToString().Equals("folder"))
                                {
                                    var boxfolder = await client.FoldersManager.GetInformationAsync(item.Id);
                                    fileEntity.FileName = boxfolder.Name;
                                    fileEntity.FileId = boxfolder.Id;
                                    fileEntity.FileSize = Convert.ToString(boxfolder.Size);
                                    fileEntity.ParentID = boxfolder.Parent.Id;
                                    fileEntity.IsFile = false;
                                }
                                if (fileEntity != null)
                                {
                                    BoxDetailFile.Add(fileEntity);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async void Search(FileEntity selectedfileEntity = null)
        {
            var _boxClient = ServiceManager.Instence().BoxClient;
            if (_boxClient != null)
            {
                if (BoxDetailFileChild != null && BoxDetailFileChild.Count > 0)
                {
                    BoxDetailFileChild.Clear();
                }
                BoxSearchFile boxSearchFile = new BoxSearchFile();
                boxSearchFile.BoxSearchFileOrFolder(_boxClient,
                        t =>
                        {
                            if (t != null)
                            {
                                
                                BoxDetailFileChild.Add(t);
                            }
                        }, selectedfileEntity.FileId, selectedfileEntity.IsFile);
            }
        }

        public void Refresh()
        {
            if (BoxDetailFile != null && BoxDetailFile.Count > 0)
            {
                BoxDetailFile.Clear();
            }
            if (BoxDetailFileChild != null && BoxDetailFileChild.Count > 0)
            {
                BoxDetailFileChild.Clear();
            }
            BoxClient _boxClient = ServiceManager.Instence().BoxClient;
            if (_boxClient != null)
            {
                InitBoxRootFolderItems(_boxClient);
            }
        }

        /// <summary>
        ///上传文件
        /// </summary>
        /// <param name="parentID">todo: describe parentID parameter on Upload</param>
        public async void Upload(string parentID)
        {
            var _boxClient = ServiceManager.Instence().BoxClient;
            if (_boxClient != null)
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
                
                if (!System.IO.File.Exists(fileName))
                {
                    return;
                }
                BoxUploadFile boxUploadFile = new BoxUploadFile();
                bool iSUploadSuccess = await boxUploadFile.BoxUpLoadFileAsunc(_boxClient, fileName, parentID);
                /*System.IO.FileInfo info = new System.IO.FileInfo(fileName);
                long fileSize = info.Length;
                MemoryStream fileInMemoryStream = GetBigFileInMemoryStream(fileSize);
                string loadfileName = System.IO.Path.GetFileName(fileName);
                bool progressReported = false;

                var progress = new Progress<BoxProgress>(val =>
                {
                    Debug.WriteLine("{0}%", val.progress);
                    progressReported = true;
                });
                await _boxClient.FilesManager.UploadUsingSessionAsync(fileInMemoryStream, loadfileName, parentID, null, progress);

                fileInMemoryStream.Close();*/
            }
        }

        

        /// Upload a large file by splitting them up and uploads in a session.
        /// This method is in BETA, not ready for production use yet, but welcome to try and give us feedback.
        ///  
        /*public async Task<BoxFile> UploadUsingSessionAsync(Stream stream, string fileName, string folderId, TimeSpan? timeout = null, IProgress<BoxProgress> progress = null)*/

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="selectedfileEntity">todo: describe selectedfileEntity parameter on Load</param>
        public async void Load(FileEntity selectedfileEntity)
        {
            var _boxClient = ServiceManager.Instence().BoxClient;
            if (_boxClient != null)
            {
                string dir = SettingManager.Instence().SaveDir;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string savePath = Path.Combine(dir, selectedfileEntity.FileName);
                try
                {
                    boxDownLoad = new BoxDownLoadFile();
                    boxDownLoad.BoxProgressEvent += new BoxDownLoadFile.BoxProgressHandler(ProgressEvent);
                    boxDownLoad.BoxFinishedEvent += new BoxDownLoadFile.BoxFinishedHandler(FinishedEvent);
                    Thread thread = new Thread(async () =>
                    {
                        bool isDownloadSuccess = await boxDownLoad.BoxDownLoadFileAsync(_boxClient, savePath, selectedfileEntity.FileId, long.Parse(selectedfileEntity.FileSize));
                    });
                    thread.Start();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async void Delete(FileEntity selectedfileEntity)
        {
            var _boxClient = ServiceManager.Instence().BoxClient;
            bool IsDeleteSuccess = false;
            if (selectedfileEntity != null)
            {
                BoxDeleteFile boxDeleteFile = new BoxDeleteFile();
                IsDeleteSuccess = await boxDeleteFile.DeleteFileOrFolderAsync(_boxClient, selectedfileEntity.FileId, selectedfileEntity.IsFile);
            }
        }

        public async void Create(string name, string parentID = "0")
        {
            BoxClient _boxClient = ServiceManager.Instence().BoxClient;
            BoxCreateFile boxCreateFile = new BoxCreateFile();
            bool isSuccess = await boxCreateFile.CreateFolderAsync(_boxClient, name, parentID);
            if (isSuccess)
            {
                Debug.WriteLine("Create folder on Box success");
            }
            else
            {
                Debug.WriteLine("Create folder on Box faile");
            }
        }

        private void FinishedEvent()
        {
            Progrecess.Result = "Completed";
            Progrecess.Rate = 100;

            if (boxDownLoad != null)
            {
                boxDownLoad.BoxProgressEvent -= ProgressEvent;
                boxDownLoad.BoxFinishedEvent -= FinishedEvent;
            }

            if (boxDownLoad != null)
            {
                boxDownLoad.BoxProgressEvent -= ProgressEvent;
                boxDownLoad.BoxFinishedEvent -= FinishedEvent;
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
