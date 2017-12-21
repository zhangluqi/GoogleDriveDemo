using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GoogleDriveDemo.Model.Google
{
    public class GoogleFileUpload
    {

        #region Events
        public delegate void ProgressHandler(long downloadedSize, long fileSize);
        public event ProgressHandler ProgressEvent;

        public delegate void FinishedHandler();
        public event FinishedHandler FinishedEvent;

        public delegate void FailedHandler();
        public event FailedHandler FailedEvent;


        #endregion


        /// <summary>
        ///上传文件到Cloud,如果传递同名的文件有什么影响
        /// </summary>
        /// <param name="sourcePath"></param>
        public void UpLoadFile(DriveService driveService,string sourcePath,string parentId=null)
        {
            if (driveService == null)
            {
                return;
            }
            if (!System.IO.File.Exists(sourcePath))
            {
                return;
            }
            System.IO.FileInfo info = new System.IO.FileInfo(sourcePath);
            long fileSize = info.Length;
            string fileName = System.IO.Path.GetFileName(sourcePath);
            var contentType = MimeMapping.GetMimeMapping(fileName);
            var fileMetadata = new File()
            { 
                Name = fileName
            };
            if (!string.IsNullOrEmpty(parentId))
            {
                fileMetadata.Parents = new List<string>(){parentId};
            }
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(sourcePath,
                                    System.IO.FileMode.Open))
            {
                request = driveService.Files.Create(
                    fileMetadata, stream, contentType);
                request.Fields = "id";
                request.ChunkSize = 262144;//配置chunk大小,must be a multiple of Google.Apis.Upload.ResumableUpload.MinimumChunkSize
                request.ProgressChanged +=
                    (IUploadProgress progress) =>
                    {
                        switch (progress.Status)
                        {
                            case UploadStatus.Uploading:
                                {
                                    // Console.WriteLine(progress.BytesSent);
                                    ProgressEvent?.Invoke(progress.BytesSent, fileSize);
                                    break;
                                }
                            case UploadStatus.Completed:
                                {
                                    Console.WriteLine("Upload complete.");
                                    FinishedEvent?.Invoke();
                                    break;
                                }
                            case UploadStatus.Failed:
                                {
                                    Console.WriteLine("Upload failed.");
                                    FailedEvent?.Invoke();
                                    break;
                                }
                        }
                    };
                request.Upload();
            }
            var file = request.ResponseBody;
            Console.WriteLine("File ID: " + file.Id);
        }
    }
}
