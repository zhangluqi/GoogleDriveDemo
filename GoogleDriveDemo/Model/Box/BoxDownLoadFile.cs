using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Box.V2;

namespace GoogleDriveDemo.Model.Box
{
    class BoxDownLoadFile
    {
        #region Events
        public delegate void BoxProgressHandler(long downloadedSize, long fileSize);
        public event BoxProgressHandler BoxProgressEvent;

        public delegate void BoxFinishedHandler();
        public event BoxFinishedHandler BoxFinishedEvent;

        public delegate void BoxFailedHandler();
        public event BoxFailedHandler BoxFailedEvent;
        #endregion

        public async Task<bool> BoxDownLoadFileAsync(BoxClient client, string savePath, string fileID, long fileSize)
        {
            bool _isDownLoadFiled = false;
            if (client == null)
            {
                return _isDownLoadFiled;
            }
            if (string.IsNullOrEmpty(savePath) && string.IsNullOrEmpty(fileID))
            {
                return _isDownLoadFiled;
            }
            try
            {
                using (var aBoxContentStream = await client.FilesManager.DownloadStreamAsync(fileID))
                {
                    const int aBufferSize = 4 * 1024 * 1024;
                    var aBuffer = new byte[aBufferSize];
                    int aLengthOfBytesRead = 0;
                    using (FileStream fs = new FileStream(savePath, FileMode.Create))
                    {
                        long currentSize = 0;
                        try
                        {
                            while ((aLengthOfBytesRead = aBoxContentStream.Read(aBuffer, 0, aBufferSize)) > 0)
                            {
                                fs.Write(aBuffer, 0, aLengthOfBytesRead);
                                currentSize += aLengthOfBytesRead;
                                BoxProgressEvent?.Invoke(currentSize, fileSize);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        BoxFinishedEvent?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
    }
}
