using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleDriveDemo.ViewModel.Common;
using Box.V2;
using GoogleDriveDemo.Model.Google;

namespace GoogleDriveDemo.Model.Box
{
    class BoxSearchFile
    {
        public async void BoxSearchFileOrFolder(BoxClient client, Action<FileEntity> searchResult, string fileID, bool isFile)
        {
            if (client == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(fileID))
            {
                return;
            }
            try
            {
                if (!isFile)
                {
                    var items = await client.FoldersManager.GetFolderItemsAsync(fileID, 500);
                    if (items != null)
                    {
                        foreach (var item in items.Entries)
                        {
                            if (item != null)
                            {
                                var fileEntity = new FileEntity();
                                if (item.Type.ToLower().ToString().Equals("file"))
                                {
                                    var boxfile = await client.FilesManager.GetInformationAsync(item.Id);
                                    fileEntity.FileName = boxfile.Name;
                                    fileEntity.FileId = boxfile.Id;
                                    fileEntity.FileSize = Convert.ToString(boxfile.Size);
                                    fileEntity.ParentID = boxfile.Parent.Id;
                                    fileEntity.IsFile = true;
                                }
                                else if (item.Type.ToLower().ToString().Equals("folder"))
                                {
                                    var boxfolder = await client.FoldersManager.GetInformationAsync(item.Id);
                                    fileEntity.FileName = boxfolder.Name;
                                    fileEntity.FileId = boxfolder.Id;
                                    fileEntity.FileSize = Convert.ToString(boxfolder.Size);
                                    fileEntity.ParentID = boxfolder.Parent.Id;
                                    fileEntity.IsFile = false;
                                }
                                searchResult(fileEntity);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
