using Google.Apis.Download;
using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Google
{
    public class GoogleFileLoad
    {

        #region Events
        public delegate void ProgressHandler(long downloadedSize,long fileSize);
        public event ProgressHandler ProgressEvent;

        public delegate void FinishedHandler();
        public event FinishedHandler FinishedEvent;

        public delegate void FailedHandler();
        public event FailedHandler FailedEvent;


        #endregion



        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="savePath">文件保存路径</param>
        public void DownLoadFile(DriveService driveService, string fileId,long fileSize, string savePath)
        {
            if(driveService == null)
            {
                return;
            }
            var request = driveService.Files.Get(fileId);
            var stream = new FileStream(savePath, FileMode.Create);

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            try
            {
                request.MediaDownloader.ChunkSize = 8192;//配置chunk大小
                request.MediaDownloader.ProgressChanged +=
               (IDownloadProgress progress) =>
               {
                   switch (progress.Status)
                   {
                       case DownloadStatus.Downloading:
                           {
                               Console.WriteLine(progress.BytesDownloaded);
                               ProgressEvent?.Invoke(progress.BytesDownloaded,fileSize);
                               break;
                           }
                       case DownloadStatus.Completed:
                           {
                               Console.WriteLine("Download complete.");
                               FinishedEvent?.Invoke();
                               break;
                           }
                       case DownloadStatus.Failed:
                           {
                               Console.WriteLine("Download failed.");
                               FailedEvent?.Invoke();
                               break;
                           }
                   }
               };
                request.Download(stream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                    GC.Collect();
                }

            }
        }

    }
}
