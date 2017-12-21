using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Dropbox
{
     public class DropboxCreateFile
    {
   
        public async Task<bool> CreateFolder(DropboxClient client, string folder, string fileName)
        {
            if(client == null)
            {
                return false;
            }

            string newPath= "";
            if (string.IsNullOrEmpty(folder))
            {
                newPath = "/" + fileName;
            }
            else
            {
                newPath = folder + "/" + fileName;
            }
            try
            {
                CreateFolderResult createFolderResult = await client.Files.CreateFolderV2Async(newPath);
            }catch(Exception ex)
            {
                Debug.WriteLine("Create folder on Dropbox:"+ex.ToString());
                return false;
            }
         

            return true;
        }
    }
}