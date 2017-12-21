using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Google
{
    public class GoogleDeleteFile
    {
        /// <summary>
        /// 删除指定的项目
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public bool DeleteFileOrFolder(DriveService driveService, string fileId)
        {
            bool deleteResult;
            try
            {
                var request = driveService.Files.Delete(fileId);
                var file = request.Execute();
                //之后来确定如何利用这个返回结果值
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
