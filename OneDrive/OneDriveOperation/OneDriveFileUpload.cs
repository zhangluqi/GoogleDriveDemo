using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Graph;
using LogLib;
using CloudObject;
using HttpLib;
using Newtonsoft.Json;

namespace OneDrive.OneDriveOperation
{
    class OneDriveFileUpload
    {
        public async Task<bool> UpLoadFile<T>(T t, FileInformation source, string taskId)
        {
            string auth = t as string;
            UploadEntity uploadEntity = new UploadEntity
            {
                FileSize = source.FileSize,
                TaskId = taskId,
                SourcePath = source.FilePath,
                SourceName = source.FileName,

                //之后从配置文件读取
                ChunkSize = 320 * 1024
            };

            //-:获取上传地址
            Log.WriteLog("Request uploadurl");
            uploadEntity.Url = GetUploadUrl(source.ParentId, source.FileName, auth);
            Log.WriteLog("Request uploadurl success");

            //二：正式上传
            NetUtil netUtil = new NetUtil();
            uploadEntity.Method = "PUT";
            IDictionary<string, string> fixedHeaders = new Dictionary<string, string>
            {
                //给固定头部赋值
                { "SdkVersion", "Graph-dotnet-1.6.2" },
                { "Cache-Control", "no-store, no-cache" }
            };

            bool result = await netUtil.UploadAsync(uploadEntity, fixedHeaders,
                (netClient, unfixedHeadInfo) => {
                    //这里给随着块数变化而变化的头部赋值
                    if (unfixedHeadInfo.IsAdd)
                    {
                        //添加头部
                        string range = string.Format("bytes {0}-{1}/{2}",
                            unfixedHeadInfo.Offset, unfixedHeadInfo.Offset + unfixedHeadInfo.BlockSize - 1, unfixedHeadInfo.FileSize);
                        Log.WriteLog("range:" + range);
                        netClient.AddHeader("Content-Range", range);
                    }
                    else
                    {
                        //移除上面添加的头部，必须跟上面对应
                        netClient.Headers.Remove("Content-Range");
                    }
                },
                (offset, fileSize, des) => {
                    Log.WriteLog("process:" + offset + ":" + fileSize + ":" + des);
                    //这里处理进度
                    if (string.IsNullOrEmpty(des))
                    {

                    }
                    else
                    {
                        ProgressEvent?.Invoke(offset, fileSize, des);
                    }
                });
            FinishedEvent("");
            return result;
        }

        /// <summary>
        /// 获取下载路径
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="fileName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetUploadUrl(string parentId, string fileName, string token)
        {
            string uploadUrl = "";
            string host = "graph.microsoft.com";
            string path = null; //string.Format("/v1.0/me/drive/{0}:/{1}:/microsoft.graph.createUploadSession", parentId, fileName);
            if ("root" == parentId || string.IsNullOrEmpty(parentId))
            {
                path = string.Format("/v1.0/me/drive/{0}:/{1}:/microsoft.graph.createUploadSession", parentId,
                    fileName);
            }
            else
            {
                path = string.Format("/v1.0/me/drive/items/{0}:/{1}:/microsoft.graph.createUploadSession", parentId,
                    fileName);
            }
            NetClient netClient = new NetClient();
            try
            {
                netClient.AddHeader("SampleID", "uwp-csharp-connect-sample");
                netClient.AddHeader("SdkVersion", "Graph-dotnet-1.6.2");
                netClient.AddHeader("Cache-Control", "no-store, no-cache");
                netClient.AddHeader("Content-Type", "application/json");
                netClient.AddHeader("Host", "graph.microsoft.com");

                netClient.AddHeader("Authorization", string.Format("Bearer {0}", token));
                string data = "{}";
                string uploadPreper = netClient.POST(data, host, path, false, true);
                IDictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(uploadPreper);
                if (dic != null && dic.ContainsKey("uploadUrl"))
                {
                    uploadUrl = dic["uploadUrl"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return uploadUrl;
        }

        #region Events
        public event Action<long, long, string> ProgressEvent;

        public event Action<string> FinishedEvent;

        public event Action<string, string> FailedEvent;
        #endregion
    }
}
