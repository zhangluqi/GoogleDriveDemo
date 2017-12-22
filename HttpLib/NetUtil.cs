using ChacheLib;
using LogLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpLib
{
    public class NetUtil
    {
        public async Task<bool> LoadAsync(LoadEntity loadEntity, string target, IDictionary<string, string> fixedHeader, Action<long, long, string> process)
        {
            long offset = 0;         // cursor location for updating the Range header.
            long newFileSize = 0;
            long fileSize = loadEntity.FileSize;
            Log.WriteLog("fileSize:" + fileSize);
            string url = loadEntity.Url;
            string taskId = loadEntity.TaskId;

            try
            {
                //获取文件大小
                if (fileSize == 0){ fileSize = Size(url); }
                
                if (File.Exists(target))
                {
                    FileInfo fileInfo = new FileInfo(target);
                    offset = fileInfo.Length;
                    newFileSize = fileSize -  fileInfo.Length;
                }

                long chunkSize = loadEntity.ChunkSize;
                Log.WriteLog("ChunkSize:" + chunkSize);
                byte[] bytesInStream;                    // bytes in range returned by chunk download
                int numberOfChunks = (int)(newFileSize / chunkSize + 1);
                int lastChunkSize = (int)(newFileSize % chunkSize);
                Log.WriteLog("lastChunkSize:" + lastChunkSize);
                long currentSize = offset;
                
                using (FileStream fs = new FileStream(target, FileMode.OpenOrCreate))
                {
                    for (int i = 0; i < numberOfChunks; i++)
                    {
                        // Setup the last chunk to request. This will be called at the end of this loop.
                        if (i == numberOfChunks - 1)
                        {
                            chunkSize = lastChunkSize;
                        }

                        // Create the request message with the download URL and Range header.
                        HttpRequestMessage req = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                        req.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(offset, chunkSize + offset - 1);
                        if (fixedHeader != null)
                        {
                            foreach (var key in fixedHeader.Keys)
                            {
                                req.Headers.Add(key, fixedHeader[key]);
                            }
                        }
                        var client = new HttpClient();
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        HttpResponseMessage response = await client.SendAsync(req);
                        using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            watch.Stop();
                            bytesInStream = new byte[chunkSize];
                            int len;
                            while ((len = responseStream.Read(bytesInStream, 0, bytesInStream.Length)) > 0)
                            {
                                fs.Position = fs.Length;
                                fs.Write(bytesInStream,0,len);
                                currentSize += len;
                                long time = watch.ElapsedMilliseconds / 1000;
                                long progress = (currentSize / fileSize);
                                long speed = 0;
                                if (time > 0)
                                {
                                   speed = (chunkSize / 1024) / time;
                                   Console.WriteLine(speed + "KB/S");
                                }
                                Console.WriteLine(progress + "B");
                                process?.Invoke(progress, speed, taskId);
                            }
                        }
                        offset += chunkSize; // Move the offset cursor to the next chunk.
                    }
                }
            }
            catch(Exception ex)
            {
                process?.Invoke(0,0,ex.Message);
                Log.WriteLog("load googledrive file:"+ex.Message);
                throw new Exception(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="uploadEntity">上传信息</param>
        /// <param name="fixedHeader">固定头部，每次不变</param>
        /// <param name="unfixed">可变头部</param>
        /// <param name="process">进度</param>
        /// <returns></returns>
        public async Task<bool> UploadAsync(UploadEntity uploadEntity, IDictionary<string, string> fixedHeader, Action<NetClient, UnfixedHeadInfo> unfixed, Action<long, long, string> process)
        {
            try
            {
                // 要上传地址
                string address = uploadEntity.Url;
                // 要上传的文件
                long fileSize = uploadEntity.FileSize;
                // 每次上传块大小
                int chunkSize = uploadEntity.ChunkSize;
                // 文件路径
                string sourcePath = Path.Combine(uploadEntity.SourcePath, uploadEntity.SourceName);
                // 上传块数
                int blockIndex = 0;

                string method;
                if (string.IsNullOrEmpty(uploadEntity.Method))
                {
                    method = "POST";
                }
                else
                {
                    method = uploadEntity.Method;
                }
                string taskId = uploadEntity.TaskId;

                // 根据uri创建HttpWebRequest对象
                NetClient netClient = new NetClient();
                UnfixedHeadInfo unfixedHeadInfo = new UnfixedHeadInfo { FileSize = fileSize };
                if (fixedHeader != null)
                {
                    foreach (var key in fixedHeader.Keys)
                    {
                        netClient.AddHeader(key, fixedHeader[key]);
                    }
                }
                int numChunks = (int)Math.Ceiling((double)fileSize / chunkSize);

                //已上传的字节数
                long offset = 0;
                using (FileStream fs = new FileStream(sourcePath, FileMode.OpenOrCreate))
                {
                    while (blockIndex < numChunks)
                    {
                        //从缓冲区获取数据
                        Log.WriteLog("Get bytes");
                        byte[] bytes = MemoryCache.Instence().OutItemchache(taskId);
                        Log.WriteLog("Get bytes end");
                        if (bytes == null || bytes.Length <= 0)
                        {
                            continue;
                        }
                        int size = bytes.Length;
                        unfixedHeadInfo.Offset = offset;
                        unfixedHeadInfo.IsAdd = true;
                        unfixedHeadInfo.BlockSize = size;
                        unfixed?.Invoke(netClient, unfixedHeadInfo);

                        Log.WriteLog("upload start 1:" + size);
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        await netClient.UploadDataTaskAsync(address, method, bytes);
                        watch.Stop();
                        
                        ///计算速度
                        long time = watch.ElapsedMilliseconds / 1000;
                        long progress = (offset / fileSize);
                        long speed = 0;
                        if (time > 0)
                        {
                            speed = (offset / 1024) / time;
                        }
                        Log.WriteLog("upload end 1");

                        process?.Invoke(progress, speed, taskId);
                        offset += size;
                        unfixedHeadInfo.IsAdd = false;
                        unfixed?.Invoke(netClient, unfixedHeadInfo);

                        Debug.WriteLine("size:" + offset);
                        blockIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                process?.Invoke(0, 0, ex.Message);
                throw new Exception(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 根据url获取文件的大小
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public long Size(string url)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            return response.ContentLength;//获得文件的总大小 
        }
    }
}
