using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Dropbox
{
    public class DropboxFileUnload
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
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="folder"></param>
        /// <param name="sourcePath"></param>
        public async void UpLoadFile(DropboxClient client, string folder, string sourcePath)
        {
            // Chunk size is 128KB.
            const int chunkSize = 128 * 1024;
            if (!System.IO.File.Exists(sourcePath))
            {
                return;
            }
            System.IO.FileInfo info = new System.IO.FileInfo(sourcePath);
            long fileSize = info.Length;
            string fileName = System.IO.Path.GetFileName(sourcePath);
            string path;
            if (string.IsNullOrEmpty(folder))
            {
                path = "/" + fileName;
            }
            else{
                path = folder + "/" + fileName; 
            }
              
            //using (var stream = new MemoryStream(fileContent))
            using (var stream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);

                byte[] buffer = new byte[chunkSize];
                string sessionId = null;
                long currentSize = 0;
                for (var idx = 0; idx < numChunks; idx++)
                {
                    var byteRead = stream.Read(buffer, 0, chunkSize);
                    ProgressEvent?.Invoke(currentSize,fileSize);
                    using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                    {
                        if (idx == 0)
                        {
                            if (numChunks == 1)
                            {
                                //小文件
                                await client.Files.UploadAsync(path, body: memStream);
                            }
                            else
                            {
                                var result = await client.Files.UploadSessionStartAsync(false, memStream);
                                sessionId = result.SessionId;
                            }   
                        }

                        else
                        {
                            UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                            if (idx == numChunks - 1)
                            {
                                await client.Files.UploadSessionFinishAsync(cursor, new CommitInfo(path), memStream);
                            }

                            else
                            {
                                await client.Files.UploadSessionAppendV2Async(cursor, false, memStream);
                            }
                        }
                    }
                }
                FinishedEvent?.Invoke();

            }
        }
    }
}
