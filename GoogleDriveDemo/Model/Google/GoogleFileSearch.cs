using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Google
{
    public class GoogleFileSearch
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="driveService"></param>
       /// <param name="searchResult"></param>
       /// <param name="fileId">fileId为null时候，检索根目录</param>
        public void SearchFile(DriveService driveService,Action<FileInfo>searchResult,string fileId=null)
        {
            if (driveService == null)
            {
                return;
            }
         
            //搜索根目录
            if (string.IsNullOrEmpty(fileId))
            {
                // Define parameters of request.
                FilesResource.ListRequest listRequest = driveService.Files.List();
                listRequest.PageSize = 10;
                listRequest.Fields = "nextPageToken, files(id,kind, name,size,mimeType)";
                // List files.
                IList<File> files = listRequest.Execute()
                    .Files;
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo();
                        //用MimeType区分是文件还是文件夹
                        if (file.MimeType == "application/vnd.google-apps.folder")
                        {
                            fileInfo.IsFile = false;
                        }
                        else
                        {
                            fileInfo.IsFile = true;
                        }
                        fileInfo.FileName = file.Name;
                        fileInfo.FileId = file.Id;
                        fileInfo.FileSize = file.Size.ToString();//之后转换
                        searchResult(fileInfo);
                    }
                }
                else
                {
                    Console.WriteLine("No files found.");
                }
            }
            //搜索文件夹
            else
            {
                ResourceFromFolder(driveService, searchResult, fileId);
            }
            
        }

        private void ResourceFromFolder(DriveService driveService, Action<FileInfo> searchResult, string folderId)
        {
            var request = driveService.Files.List();
            request.PageSize = 10;
            request.Q = "'" + folderId + "'" + " in parents and trashed = false";
            request.Fields = "nextPageToken, files(modifiedTime,id,parents,name,webContentLink,mimeType,size)";// "files(modifiedTime,id,parents,name,webContentLink,mimeType,size)";
            do
            {
                var children = request.Execute();
                foreach (var file in children.Files)
                {
                    FileInfo fileInfo = new FileInfo();
                    //用MimeType区分是文件还是文件夹
                    if (file.MimeType == "application/vnd.google-apps.folder")
                    {
                        fileInfo.IsFile = false;
                    }
                    else
                    {
                        fileInfo.IsFile = true;
                    }
                    fileInfo.FileName = file.Name;
                    fileInfo.FileId = file.Id;
                    if(file.Size != null)
                    {
                       fileInfo.FileSize = file.Size.ToString();//之后转换
                    }
                    searchResult(fileInfo);
                }
                request.PageToken = children.NextPageToken;
            } while (!String.IsNullOrEmpty(request.PageToken));
  
        }
    }
}
