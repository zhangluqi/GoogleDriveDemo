using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.OneDrive
{
    public class OneDriveFileUpload
    {
        
        #region Events
        public delegate void ProgressHandler(long downloadedSize, long fileSize);
        public event ProgressHandler ProgressEvent;

        public delegate void FinishedHandler();
        public event FinishedHandler FinishedEvent;

        public delegate void FailedHandler();
        public event FailedHandler FailedEvent;


        #endregion
        

        public async Task<bool> UpLoadFile(GraphServiceClient graphClient, string sourcePath, string fileID = null)
        {
            try
            {
                if (!System.IO.File.Exists(sourcePath))
                {
                    return false;
                }
                System.IO.FileInfo info = new System.IO.FileInfo(sourcePath);
                long fileSize = info.Length;
                string fileName = System.IO.Path.GetFileName(sourcePath);
                long currentSize = 0;
                using (FileStream fileStream = new FileStream(sourcePath, FileMode.Open))
                {
                    // Create the upload session. The access token is no longer required as you have session established for the upload.  
                    // POST /v1.0/drive/root:/UploadLargeFile.bmp:/microsoft.graph.createUploadSession
                    UploadSession uploadSession;
                    if (string.IsNullOrEmpty(fileID))
                    {
                        uploadSession = await graphClient.Me.Drive.Root.ItemWithPath(fileName).CreateUploadSession().Request().PostAsync();
                    }
                    else
                    {
                        uploadSession = await graphClient.Me.Drive.Items[fileID].ItemWithPath(fileName).CreateUploadSession().Request().PostAsync();
                    }
                 
                    var maxChunkSize = 320 * 1024; // 320 KB - Change this to your chunk size. 5MB is the default.
                    var provider = new ChunkedUploadProvider(uploadSession, graphClient, fileStream, maxChunkSize);

                    // Setup the chunk request necessities
                    var chunkRequests = provider.GetUploadChunkRequests();
                    var readBuffer = new byte[maxChunkSize];
                    var trackedExceptions = new List<Exception>();
                    DriveItem itemResult = null;

                    //upload the chunks
                    foreach (var request in chunkRequests)
                    {
                        // Do your updates here: update progress bar, etc.
                        // ...
                        // Send chunk request
                        var result = await provider.GetChunkRequestResponseAsync(request, readBuffer, trackedExceptions);
                        //这里时最终的一个结果
                        if (result.UploadSucceeded)
                        {
                            itemResult = result.ItemResponse;
                            currentSize = (long)result.ItemResponse.Size;

                        }
                        else
                        {
                            currentSize += maxChunkSize;

                        }
                        ProgressEvent?.Invoke(currentSize, fileSize);

                    }

                    // Check that upload succeeded
                    if (itemResult == null)
                    {
                        // Retry the upload
                        // ...
                        FailedEvent?.Invoke();
                    }
                    else
                    {
                        FinishedEvent?.Invoke();
                    }

                    return true;
                }

            }

            catch (ServiceException e)
            {
                System.Diagnostics.Debug.WriteLine("We could not upload the file: " + e.Error.Message);
                FailedEvent?.Invoke();
                return false;
            }
        }

    }
}
