using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clouder;
using CloudObject;
using CloudObject.EventHandler;
using Microsoft.Graph;
using System.Diagnostics;
using ChacheLib;
using Microsoft.SqlServer.Server;
using OneDrive.OneDriveOperation;


namespace OneDrive
{
    public sealed class OneDriveManager : Cloudbase
    {
        private void OnProgress(ProgressEventhandler obj)
        {
            Progress?.Invoke(obj);
        }

        private void OnException(ExceptionEventHandler obj)
        {
            Exception?.Invoke(obj);
        }

        #region cloudBase override 
        public override event Action<ExceptionEventHandler> Exception;
        public override event Action<ProgressEventhandler> Progress;

        public override Guid CloudId { get; set; }

        public override Cloud Cloud { get; set; }
       
        /// <summary>
        /// 创建文件夹，返回创建的文件夹ID
        /// </summary>
        /// <param name="fileid"></param>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public override string CreateFolder(string fileid, string foldername)
        {
            string createId = "";
            try
            {
                OneDriveCreateFile createFolder = new OneDriveCreateFile();
                createId = createFolder.CreateFolder(Cloud.CloudToken, fileid, foldername);
            }
            catch (Exception e)
            {
                Console.WriteLine();
            }
            return createId;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        public override bool Delete(string fileid)
        {
            bool isSuccess = false;
            try
            {
                OnedriveDeleteFile deletefile = new OnedriveDeleteFile();
                isSuccess = deletefile.DeleteFileOrFolder(Cloud.CloudToken, fileid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                isSuccess = false;
            }
            return isSuccess;
        }


        public override IList<FileInformation> Search(string fileid)
        {
            List<FileInformation> fileInfoList = new List<FileInformation>();
            OneDriveFileSearch oneDriveFileSearch = new OneDriveFileSearch();
            try
            {
                fileInfoList = (List<FileInformation>)oneDriveFileSearch.SearchFile(Cloud.CloudToken, fileid);
            }
            catch (Exception ex)
            {
                OnException(new ExceptionEventHandler(CloudId.ToString(), "", ex.ToString()));
                throw;
            }
            return fileInfoList;
        }

        public override async Task<bool> Start(FileInformation source, string target, OperaType operaType)
        {
            bool isSuccess = false;
            switch (operaType)
            {
                case OperaType.UpLoad:
                    try
                    {
                        var o = new OneDriveFileUpload();
                        o.ProgressEvent += O_ProgressEvent;
                        isSuccess = await o.UpLoadFile(Cloud.CloudToken, source, Guid.NewGuid().ToString());
                    }
                    catch (Exception e)
                    {
                        OnException(new ExceptionEventHandler(CloudId.ToString(), "", e.ToString()));
                    }
                    break;

                case OperaType.DownLoad:
                    try
                    {
                        var o = new OneDriveFileLoad();
                        o.ProgressEvent += O_ProgressEvent;
                        isSuccess = await o.DownloadFile(Cloud.CloudToken, source, target, Guid.NewGuid().ToString());
                        
                    }
                    catch (Exception e)
                    {
                        OnException(new ExceptionEventHandler(CloudId.ToString(), "", e.ToString()));
                    }
                    break;
            }
            return isSuccess;
        }

        public override Task<bool> Cancel(string taskid)
        {
            throw new NotImplementedException();
        }
        
        public override void Pause(string taskid)
        {
            OneDrivePause oneDrivePause = new OneDrivePause();
        }

        private void O_ProgressEvent(long arg1, long arg2, string arg3)
        {
            if (arg2 > 0)
            {
                Debug.WriteLine($"{arg1}  / {arg2}" + "=======" + arg3);
                OnProgress(new ProgressEventhandler(CloudId.ToString(), arg3, (decimal)arg1 / arg2, 0));
            }        }
 
        public override Task<bool> SyncCloud(string sourceCloud, string targetCloud, string sourceFile, string targetFile)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
