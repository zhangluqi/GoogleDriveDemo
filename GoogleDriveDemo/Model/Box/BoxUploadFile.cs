using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Box.V2;
using Box.V2.Utility;
using Box.V2.Models;

namespace GoogleDriveDemo.Model.Box
{
    class BoxUploadFile
    {
        public async Task<bool> BoxUpLoadFileAsunc(BoxClient client, string filePath, string parentID)
        {
            bool _isSuccessUpload = false;
            if(client == null)
            {
                return _isSuccessUpload;
            }
            if (!System.IO.File.Exists(filePath))
            {
                return _isSuccessUpload;
            }

            FileInfo info = new System.IO.FileInfo(filePath);
            long fileSize = info.Length;
            MemoryStream fileInMemoryStream = GetBigFileInMemoryStream(fileSize);
            string loadfileName = System.IO.Path.GetFileName(filePath);
            string fileName = info.Name;
            bool progressReported = false;
            long minSize = 20 * 1024 * 1024;
            try
            {
                if (fileSize >= minSize)
                {
                    var progress = new Progress<BoxProgress>(val =>
                    {
                        Debug.WriteLine("{0}%", val.progress);
                        progressReported = true;
                    });
                    await client.FilesManager.UploadUsingSessionAsync(fileInMemoryStream, loadfileName, parentID, null, progress);
                    fileInMemoryStream.Close();
                }
                else
                {
                    using (FileStream fs = new FileStream(loadfileName, FileMode.Open))
                    {
                        BoxFileRequest req = new BoxFileRequest
                        {
                            Name = fileName,
                            Parent = new BoxRequestEntity { Id = parentID }
                        };
                        await client.FilesManager.UploadAsync(req, fs);
                    }
                }
            }
            catch (Exception ex)
            {
                //throw;
                return false;
            }
            return true;
        }

        private static MemoryStream GetBigFileInMemoryStream(long fileSize)
        {
            // Create random data to write to the file.
            byte[] dataArray = new byte[fileSize];
            new Random().NextBytes(dataArray);
            MemoryStream memoryStream = new MemoryStream(dataArray);
            return memoryStream;
        }
    }
}
