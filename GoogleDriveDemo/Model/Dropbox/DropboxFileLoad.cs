using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Dropbox
{
    public class DropboxFileLoad
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
        /// <param name="dropboxSourcePath">folder + "/" + file,这个/一定不能省略</param>
        /// <param name="savePath"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public async Task<bool> DownLoadFile(DropboxClient client, string folder,string fileName, string savePath, long fileSize)
        {
            string dropboxSourcePath = "";
            if (string.IsNullOrEmpty(folder))
            {
                dropboxSourcePath = "/" + fileName;
            }
            else
            {
                dropboxSourcePath =folder+"/"+ fileName;
            }
            using (var response = await client.Files.DownloadAsync(dropboxSourcePath))
            {
                ulong aFileSize = response.Response.Size;
                const int aBufferSize = 4 * 1024 * 1024;

                var aBuffer = new byte[aBufferSize];
                using (var aDropboxContentStream = await response.GetContentAsStreamAsync())
                {
                    int aLengthOfBytesRead = 0;
                    using (FileStream fs = new FileStream(savePath, FileMode.Create))
                    {
                        long currentSize = 0;
                        while ((aLengthOfBytesRead = aDropboxContentStream.Read(aBuffer, 0, aBufferSize))>0)
                        {
                           
                            fs.Write(aBuffer, 0, aLengthOfBytesRead);
                            currentSize += aLengthOfBytesRead;
                            ProgressEvent?.Invoke(currentSize, fileSize);
                        }
                    }

                }
                FinishedEvent?.Invoke();
            }
            return true;
        }
    }
}
