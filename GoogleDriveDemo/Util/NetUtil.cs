using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Util
{
    public class NetUtil
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
        /// 分块下载文件
        /// </summary>
        /// <param name="url">下载的url</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="fileSize">文件的大小</param>
        public async void Download(string url, string savePath, long fileSize = 0)
        {
            //获取文件大小
            if (fileSize == 0)
            {
                fileSize = Size(url);
            }
            const long DefaultChunkSize = 50 * 1024; // 50 KB, TODO: change chunk size to make it realistic for a large file.
            long ChunkSize = DefaultChunkSize;
            long offset = 0;         // cursor location for updating the Range header.
            byte[] bytesInStream;                    // bytes in range returned by chunk download.
            int numberOfChunks = Convert.ToInt32(fileSize / DefaultChunkSize);
            // We are incrementing the offset cursor after writing the response stream to a file after each chunk. 
            // Subtracting one since the size is 1 based, and the range is 0 base. There should be a better way to do
            // this but I haven't spent the time on that.
            int lastChunkSize = Convert.ToInt32(fileSize % DefaultChunkSize) - numberOfChunks - 1;
            if (lastChunkSize > 0) { numberOfChunks++; }
            long currentSize = 0;
            // Create a file stream to contain the downloaded file.
            using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
            {
                for (int i = 0; i < numberOfChunks; i++)
                {
                    // Setup the last chunk to request. This will be called at the end of this loop.
                    if (i == numberOfChunks - 1)
                    {
                        ChunkSize = lastChunkSize;
                    }

                    // Create the request message with the download URL and Range header.
                    HttpRequestMessage req = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                    req.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(offset, ChunkSize + offset);
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.SendAsync(req);
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        bytesInStream = new byte[ChunkSize];
                        int read;
                        do
                        {
                            read = responseStream.Read(bytesInStream, 0, (int)bytesInStream.Length);
                            if (read > 0)
                            {
                                fileStream.Write(bytesInStream, 0, read);
                                currentSize += read;
                                ProgressEvent?.Invoke(currentSize, fileSize);
                                Debug.WriteLine(i + " : " + currentSize + " : " + fileSize);
                            }

                        }
                        while (read > 0);
                    }
                    offset += ChunkSize + 1; // Move the offset cursor to the next chunk.
                }
            }
            FinishedEvent?.Invoke();
        }
        /// <summary>
        /// 根据url获取文件的大小
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public long Size(string url)
        {
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Method = "HEAD";
            System.Net.WebResponse resp = req.GetResponse();
            resp.Close();
            return resp.ContentLength;

        }

    }
}
