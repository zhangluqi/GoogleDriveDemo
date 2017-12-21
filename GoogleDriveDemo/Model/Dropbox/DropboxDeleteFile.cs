using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Dropbox
{
    public class DropboxDeleteFile
    {
        /// <summary>
        /// 删除指定的项目
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileOrFolder(DropboxClient dropboxClient, string folder, string fileName)
        {
            bool opResult;
            string deletePath = "";
            if (string.IsNullOrEmpty(folder))
            {
                deletePath = "/" + fileName;
            }
            else
            {
                deletePath = folder + "/" + fileName;
            }
            try
            {
                DeleteResult deleteResult = await dropboxClient.Files.DeleteV2Async(deletePath);
                //之后来分析这个删除结果中有用的参数
                opResult = true;
            }
            catch (Exception ex)
            {
                opResult = false;
            }

            return opResult;
        }
    }
}
