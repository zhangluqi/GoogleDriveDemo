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

        ///临时的
        private string str = "EwA4A8l6BAAU7p9QDpi/D7xJLwsTgCg3TskyTaQAASPWPeXzTHvQAstGz3F8e2MQm9u7LW1hpGIj/toiIADXrzgjqLbndi9TD3IIujyarCiClUqWuNjzHqzmeRjxZj4kXKSn0MNOvw6bdLucperYkz7AcusSEttvZK5N3NZA7C3T62T51uzOnewSUNpq4Z9VIzAldwAfd/xQNd3KinFgEv8Luv+3ybMszp1r+TXauBsP2PzU8NaYRqC9Cr2Ee/v+ohIh49R1oiIDydlZ+wd7SVXTIuneXKH26VMaMIferN5zsckdmmVQxOd+WSDYspUqzVA+sj00D2E/NSpGaPzGIC7zmfEMoblUuCtxFwL/aeTUATA7cgkf6gdE9VwtqJMDZgAACI4AzOBSNgnfCAILYl3XIn35usx3I0/umZdR73Vk7W+F109cP2L+7nUhbKRQxnPTkcM36lMteTB0pdye+VaJ/EcjODySg89GMcDj5Ixl8+8nc96kY/ekF8pc2Pu9pQpZEmg8/s2OFufjy81zsTB+FHCEAhGXZCsjjW5vphUiPjKOARzk27n6Chh2mchIGqQ2c259T7G+sr7c26xbU0MwMzQq5J/eNdGt6VNcjHk+ezHe/1dP5f0//Vh2RuAqRKnHC46h9O33Detoap6osIcXahzZ+xirWAd4HW5rH6cHzufJcPNgbrK/CoiHQCoVmE0Sb6UAXCzIzh6jQpG41fvujesp1l2LqqXx8FEc/lOVyRLhORjJlFvHITktnxDhdA29bAPmXtsOyo+arzV1Ul7DjQ8VA45sdJgsD16WjL37jxKIZqLmD5Q1eU8MYl4SZQYUO+L5UNgpqZ8tJw0Fkj5Ff2z6SR4pUB91oYCaWMsMKXgcOYsuLMQAXliSAltTXPdKcbkqeTDPeBzVaAyo9/RvD7diJhSnlxSmddrT7Z6mHuyXoLS4PuBMTscxjGGLBVF7PEAz50Wy5MDFhSWBbMpN/HIZT6Tz5hBW7XQxinKbfGcsg0wyma7OeKlgB4WC0mU7OHKP1wRGz1mzAA1I9z7j+tiwCAJBwZKD60l08sx57c4Gm5COI1ET5DBs9pSokWeqBxPwOQI=";
       
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
                fileInfoList = (List<FileInformation>)oneDriveFileSearch.SearchFile(str, fileid);
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
