using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Graph;
using CloudObject;
using System.Net;
using HttpLib;
using LogLib;
using OneDrive.OnedriveModel;
using Newtonsoft.Json;

namespace OneDrive.OneDriveOperation
{
    class OneDriveFileSearch
    {
        public IList<FileInformation> SearchFile<T>(T t, string fileId)
        {
            Log.WriteLog("Search onedrive folder content.");
            /*
             * Request URL: https://graph.microsoft.com/v1.0/me/drive/items/1C1370877CDEA235!104/children
             * Authorization bearer token
             */
            if (string.IsNullOrEmpty(fileId))
            {
                fileId = "root";
            }
            string token = t as string;

            IList<FileInformation> fileList = null;
            //bearer之后的空格需要保留
            string authContent = "bearer " + token;
            List<HeaderEntity> headers = new List<HeaderEntity>
            {
                new HeaderEntity("SdkVersion", "Graph-dotnet-1.6.2"),
                new HeaderEntity("Authorization", authContent),
                new HeaderEntity("SampleID", "uwp-csharp-connect-sample"),
                new HeaderEntity("Cache-Control", "no-store, no-cache")
            };
            NetClient netclient = new NetClient(headers);
            string requestUrl = "";
            if (fileId.Equals("root"))
            {
                requestUrl = "/v1.0/me/drive/" + fileId + "/children"; 
            }
            else
            {
                requestUrl = "/v1.0/me/drive/items/" + fileId + "/children";
            }
            string jsonContent = "";
            try
            {
                jsonContent = netclient.GET("graph.microsoft.com", requestUrl, true);
            }
            catch (WebException ex)
            {
                jsonContent = "";
                Log.WriteLog("Create OneDrive folder webexception:" + ex.Message);
            }
            catch (Exception ex)
            {
                jsonContent = "";
                Log.WriteLog("Search OneDrive folder content exception:" + ex.Message);
            }
            if (!string.IsNullOrEmpty(jsonContent))
            {
                CatalogData catalogData = null;
                try
                {
                    catalogData = JsonConvert.DeserializeObject<CatalogData>(jsonContent);
                }
                catch (Exception ex)
                {
                    catalogData = null;
                    Log.WriteLog("Search OneDrive folder content deserialize exception:" + ex.Message);
                }
                if (catalogData != null && catalogData.value != null)
                {
                    fileList = new List<FileInformation>();
                    foreach (ValueItem item in catalogData.value)
                    {
                        FileInformation fileEntity = new FileInformation
                        {
                            FileId = item.id,
                            FileName = item.name,
                            FileSize = item.size
                        };
                        if (item.parentReference != null)
                        {
                            fileEntity.ParentId = item.parentReference.id;
                        }
                        if (item.folder != null)
                        {
                            fileEntity.IsFolder = true;
                        }
                        else if (item.file != null)
                        {
                            fileEntity.IsFolder = false;
                        }
                        fileList.Add(fileEntity);
                    }
                }
            }
            else
            {
                Log.WriteLog("Search OneDrive folder content failed.");
            }
            return fileList;
        }
    }
}
