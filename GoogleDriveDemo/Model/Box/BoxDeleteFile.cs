using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GoogleDriveDemo.ViewModel.Common;
using Box.V2;

namespace GoogleDriveDemo.Model.Box
{
    public class BoxDeleteFile
    {
        public async Task<bool> DeleteFileOrFolderAsync(BoxClient client, string fileID, bool isFile)
        {
            bool _isDeleteSuccess = false;
            if (client == null)
            {
                return _isDeleteSuccess;
            }
            if (string.IsNullOrEmpty(fileID))
            {
                return _isDeleteSuccess;
            }
            try
            {
                if (isFile)
                {
                    _isDeleteSuccess = await client.FilesManager.DeleteAsync(fileID);
                }
                else
                {
                    _isDeleteSuccess = await client.FoldersManager.DeleteAsync(fileID);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Delete folder or file on Box:" + ex.ToString());
                return _isDeleteSuccess;
            }
            return _isDeleteSuccess;
        }
    }
}
