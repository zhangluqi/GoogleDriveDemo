using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box.V2;
using Box.V2.Models;
using System.Diagnostics;

namespace GoogleDriveDemo.Model.Box
{
    class BoxCreateFile
    {
        public async Task<bool> CreateFolderAsync(BoxClient client, string folderName, string parentID = "0")
        {
            bool _isCreateSuccess = false;
            if (client == null)
            {
                return _isCreateSuccess;
            }
            if (string.IsNullOrEmpty(folderName))
            {
                return _isCreateSuccess;
            }
            try
            {
                BoxFolderRequest fr = new BoxFolderRequest()
                {
                    Name = folderName,
                    Parent = new BoxRequestEntity { Id = parentID }
                };
                BoxFolder boxFolder = await client.FoldersManager.CreateAsync(fr);
                if (boxFolder != null)
                {
                    _isCreateSuccess = true;
                }
                else
                {
                    _isCreateSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Create folder on Box:" + ex.ToString());
                _isCreateSuccess = false;
            }
            return _isCreateSuccess;
        }
    }
}
