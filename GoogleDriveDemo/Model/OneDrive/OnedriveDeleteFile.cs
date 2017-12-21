using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.OneDrive
{
    public class OnedriveDeleteFile
    {
  
        /// <summary>
        /// 删除指定的项目
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileOrFolder(GraphServiceClient graphClient,string fileId)
        {
            bool deleteResult;
            try
            {
                await graphClient.Me.Drive.Items[fileId].Request().DeleteAsync();
                deleteResult = true;
            }
            catch (Exception ex)
            {
                deleteResult = false;
            }

            return deleteResult;
        }
    }
}
