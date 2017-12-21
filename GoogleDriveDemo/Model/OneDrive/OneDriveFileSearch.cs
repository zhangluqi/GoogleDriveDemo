using GoogleDriveDemo.Model.Google;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.OneDrive
{
    public class OneDriveFileSearch
    {
       /// <summary>
       /// 获取文件列表
       /// </summary>
       /// <param name="graphClient"></param>
       /// <param name="searchResult"></param>
       /// <param name="fileID">fileID为null时候，返回root根目录下对的文件，fileID传入文件夹id时候，就返回这个文件夹下的子项</param>
   
        public async void SearchFile(GraphServiceClient graphClient, Action<FileInfo> searchResult,string fileID=null)
        {
            if (graphClient == null)
            {
                return;
            }
            try {
                //得到root根目录下的所有文件
                IDriveItemChildrenCollectionPage files = null;
                if (string.IsNullOrEmpty(fileID))
                {
                    files = await graphClient.Me.Drive.Root.Children.Request().GetAsync();
                }
                else
                {
                    files = await graphClient.Me.Drive.Items[fileID].Children.Request().GetAsync();
                }
             
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        Console.WriteLine("{0} ({1})", file.Name, file.Id);
                        FileInfo fileInfo = new FileInfo();
                        fileInfo.FileName = file.Name;
                        fileInfo.FileId = file.Id;
                        fileInfo.FileSize = file.Size.ToString();//之后转换
                        //判断是文件还是文件夹File跟 Forder属性是否为null判断
                        if(file.File != null)
                        {
                            fileInfo.IsFile = true;
                        }
                        else
                        {
                            fileInfo.IsFile = false;
                          
                        }
                        searchResult(fileInfo);
                    }
                }
                else
                {
                    Console.WriteLine("No files found.");
                }
            } catch(Exception ex)
            {
                Debug.WriteLine("Get files frome onedrive:"+ex.ToString());
            } 
        }
    }
}
