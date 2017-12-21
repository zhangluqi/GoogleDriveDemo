using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpLib;
using LogLib;
using Microsoft.Graph;
using Newtonsoft.Json;
using OneDrive.OnedriveModel;
using OneDrive.Tools;

namespace OneDrive.OneDriveOperation
{
    public class OneDriveCreateFile
    {
        public  string CreateFolder<T>(T t, string fileId, string folderName)
        {
            Log.WriteLog("Create OneDrive folder content.");
            string token = t as string;
            /*
             * Request URL: 
             * https://graph.microsoft.com/v1.0/me/drive/root/children [Root]
             * https://graph.microsoft.com/v1.0/me/drive/items/1C1370877CDEA235!105/children
             * Authorization bearer token
             */
            if (string.IsNullOrEmpty(fileId))
            {
                Log.WriteLog("OneDrive :  Create target folder parent is null.");
                return null;
            }
            if (string.IsNullOrEmpty(token))
            {
                Log.WriteLog("OneDrive :  token is null.");
                return null;
            }
            
            string hostURL = "graph.microsoft.com";
            string authContent = "bearer " + token;
            List<HeaderEntity> headers = new List<HeaderEntity>
            {
                new HeaderEntity("SdkVersion", "Graph-dotnet-1.6.2"),
                new HeaderEntity("Authorization", authContent),
                new HeaderEntity("SampleID", "uwp-csharp-connect-sample"),
                new HeaderEntity("Cache-Control", "no-store, no-cache"),
                new HeaderEntity("Content-Type", "application/json"),
                new HeaderEntity("Host", hostURL)
            };

            //Check Request is Root folder
            string requestURL = "";
            if (fileId.ToLower() == "root" || fileId == "/")
            {
                requestURL = "/v1.0/me/drive/root/children";
            }
            else
            {
                requestURL = "/v1.0/me/drive/items/" + fileId + "/children";
            }
            string postData = "";
            CreateFolderModel createFolderItem = new CreateFolderModel();
            createFolderItem.name = FileNameTool.GetValidFileName(folderName);
            createFolderItem.folder = new FolderData();
            try
            {
                postData = JsonConvert.SerializeObject(createFolderItem);
            }
            catch (Exception ex)
            {
                postData = "";
                Log.WriteLog("Create OneDriver exception:" + ex.Message);
            }
            if (string.IsNullOrEmpty(postData))
            {
                return null;
            }
            string folderId = "";
            NetClient netclient = new NetClient(headers);
            try
            {
                string jsonContent = netclient.POST(postData, hostURL, requestURL, false, true);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    ValueItem valueItem = JsonConvert.DeserializeObject<ValueItem>(jsonContent);
                    if (valueItem != null)
                    {
                        folderId = valueItem.id;
                    }
                }
            }
            catch (WebException ex)
            {
                folderId = "";
                Log.WriteLog("Search OneDrive folder content webexception:" + ex.Message);
            }
            catch (Exception ex)
            {
                folderId = "";
                Log.WriteLog("Search OneDrive folder content exception:" + ex.Message);
            }
            finally
            {
                if (netclient != null)
                {
                    netclient.Dispose();
                    netclient = null;
                }
            }
            return folderId;
        }
    }
}
