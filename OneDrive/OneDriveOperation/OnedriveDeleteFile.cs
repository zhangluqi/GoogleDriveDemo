using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogLib;
using Microsoft.Graph;
using HttpLib;
using System.Net;

namespace OneDrive.OneDriveOperation
{
    public class OnedriveDeleteFile
    {
        /// <summary>
        /// 删除指定的项目
        /// </summary>
        /// <param name="t"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public bool DeleteFileOrFolder<T>( T t, string fileId)
        {
            bool deleteResult = false;
            Log.WriteLog("Search onedrive folder content.");
            string token = t as string;
            string authContent = "bearer " + token;
            List<HeaderEntity> headers = new List<HeaderEntity>
            {
                new HeaderEntity("SdkVersion", "Graph-dotnet-1.6.2"),
                new HeaderEntity("Authorization", authContent),
                new HeaderEntity("SampleID", "uwp-csharp-connect-sample"),
                new HeaderEntity("Cache-Control", "no-store, no-cache")
            };
            NetClient netclient = new NetClient(headers);
            string requestUrl = "/v1.0/me/drive/items/" + fileId;
            string jsonContent = "";
            try
            {
                jsonContent = netclient.DELETE("graph.microsoft.com", requestUrl);
                deleteResult = true;
            }
            catch (WebException ex)
            {
                jsonContent = "";
                Log.WriteLog("delete OneDrive folder webexception:" + ex.Message);
            }
            catch (Exception ex)
            {
                jsonContent = "";
                Log.WriteLog("Search OneDrive folder content exception:" + ex.Message);
            }
            return deleteResult;
        }
    }
}
