using Dropbox.Api;
using GoogleDriveDemo.Model.Google;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Dropbox
{
    public class DropboxFileSearch
    {
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="dropboxClient"></param>
        /// <param name="searchResult"></param>
        /// <param name="path">文件夹全路径,为空的时候获取跟目录下的文件列表</param>
        public async void SearchFile(DropboxClient dropboxClient, Action<FileInfo> searchResult,string path=null)
        {
            if (dropboxClient == null)
            {
                return;
            }
            //修正文件请求路径
            if(string.IsNullOrEmpty(path))
            {
                path = string.Empty;
            }
            else
            {
                if (!path.StartsWith("/"))
                {
                    path = string.Format("/{0}", path);
                }
            }
            try
            {
              
                var list = await dropboxClient.Files.ListFolderAsync(path);
                if (list != null)
                {
                    // show folders then files
                    foreach (var item in list.Entries.Where(t => t.IsFolder))
                    {
                        FileInfo fileInfo = new FileInfo();
                        fileInfo.ParentId = path;
                        fileInfo.IsFile = false;
                        fileInfo.FileName = item.Name;
                        searchResult(fileInfo);
                    }
                    foreach (var item in list.Entries.Where(t => t.IsFile))
                    {
                        FileInfo fileInfo = new FileInfo();
                        fileInfo.ParentId = path;
                        fileInfo.IsFile = true;
                        fileInfo.FileName = item.Name;
                        fileInfo.FileSize = item.AsFile.Size.ToString();
                        searchResult(fileInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get file list form dropbox exception:"+ex.ToString());
            }
            
        }

    }
}
