using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.IO;
using System.Net.Http;
using CloudObject;
using HttpLib;
using Newtonsoft.Json;

namespace OneDrive.OneDriveOperation
{
    class OneDriveFileLoad
    {
        public async void DownLoadFile(GraphServiceClient graphClient, string source, string target, string taskID)
        {
            const long DefaultChunkSize = 50 * 1024; // 50 KB, TODO: change chunk size to make it realistic for a large file.
            long ChunkSize = DefaultChunkSize;
            long offset = 0;         // cursor location for updating the Range header.
            byte[] bytesInStream;                    // bytes in range returned by chunk download.

            // We'll use the file metadata to determine size and the name of the downloaded file
            // and to get the download URL.
            var driveItemInfo = await graphClient.Me.Drive.Items[source].Request().GetAsync();

            // Get the download URL. This URL is preauthenticated and has a short TTL.
            object downloadUrl;
            driveItemInfo.AdditionalData.TryGetValue("@microsoft.graph.downloadUrl", out downloadUrl);

            // Get the number of bytes to download. calculate the number of chunks and determine
            // the last chunk size.
            long size = (long)driveItemInfo.Size;
            int numberOfChunks = Convert.ToInt32(size / DefaultChunkSize);
            // We are incrementing the offset cursor after writing the response stream to a file after each chunk. 
            // Subtracting one since the size is 1 based, and the range is 0 base. There should be a better way to do
            // this but I haven't spent the time on that.
            int lastChunkSize = Convert.ToInt32(size % DefaultChunkSize) - numberOfChunks - 1;
            if (lastChunkSize > 0) { numberOfChunks++; }
            long currentSize = 0;
            // Create a file stream to contain the downloaded file.
            using (FileStream fileStream = new FileStream(target, FileMode.Create))
            {
                for (int i = 0; i < numberOfChunks; i++)
                {
                    // Setup the last chunk to request. This will be called at the end of this loop.
                    if (i == numberOfChunks - 1)
                    {
                        ChunkSize = lastChunkSize;
                    }

                    // Create the request message with the download URL and Range header.
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, (string)downloadUrl);
                    req.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(offset, ChunkSize + offset);

                    // We can use the the client library to send this although it does add an authentication cost.
                    // HttpResponseMessage response = await graphClient.HttpProvider.SendAsync(req);
                    // Since the download URL is preauthenticated, and we aren't deserializing objects, 
                    // we'd be better to make the request with HttpClient.
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
                                ProgressEvent?.Invoke(currentSize, size, taskID);
                            }

                        }
                        while (read > 0);
                    }
                    offset += ChunkSize + 1; // Move the offset cursor to the next chunk.
                }
            }
            FinishedEvent?.Invoke(taskID);
        }

        public async Task<bool> DownloadFile<T>(T t, FileInformation source, string targetPath, string taskId)
        {
            string _token = t as string;
            targetPath = @"C:\Users\zhang\Desktop\asd";
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            string savePath = Path.Combine(targetPath, source.FileName);
            string downLoadUrl = "";
            string host = "graph.microsoft.com";
            string path = "/v1.0/me/drive/items/";
            path = Path.Combine(path, source.FileId);
            NetClient netClient = new NetClient();
            netClient.AddHeader("SampleID", "uwp-csharp-connect-sample");
            netClient.AddHeader("SdkVersion", "Graph-dotnet-1.6.2");
            netClient.AddHeader("Authorization", string.Format("Bearer {0}", _token));
            string result = netClient.GET(host, path, true);
            IDictionary<string, Object> driveItem = JsonConvert.DeserializeObject<IDictionary<string, Object>>(result);
            Object obj = "";
            driveItem.TryGetValue("@microsoft.graph.downloadUrl", out obj);
            downLoadUrl = obj.ToString();

            LoadEntity loadEntity = new LoadEntity()
            {
                Url = downLoadUrl,
                FileSize = source.FileSize,
                ChunkSize = 1024 * 1024,
                TaskId = taskId
            };

            NetUtil netUtil = new NetUtil();
            //return true;
            bool isSuccess = await netUtil.LoadAsync(loadEntity, savePath, null, ProgressEvent);

            return isSuccess;
        }

        #region Events
        public event Action<long, long, string> ProgressEvent;

        public event Action<string> FinishedEvent;

        public event Action<string, string> FailedEvent;
        #endregion
    }
}
