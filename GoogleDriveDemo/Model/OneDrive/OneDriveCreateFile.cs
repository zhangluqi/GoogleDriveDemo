using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.OneDrive
{
    public class OneDriveCreateFile
    {
        // Create a folder in the current user's root directory. 
        public async Task<bool> CreateFolder(GraphServiceClient graphClient,string fileID,string folderName)
        {
            // Add the folder.
            DriveItem folder = null;
            if (string.IsNullOrEmpty(fileID))
            {
               folder = await graphClient.Me.Drive.Root.Children.Request().AddAsync(new DriveItem
                {
                    Name = folderName,
                    Folder = new Folder()
                });
            }
            else
            {
               folder = await graphClient.Me.Drive.Items[fileID].Children.Request().AddAsync(new DriveItem
                {
                    Name = folderName,
                    Folder = new Folder()
                });
            }
            if (folder != null)
            {
                //之后进一步看这个返回值有没有作用
                //// Get folder properties.
                //items.Add(new ResultsItem
                //{
                //    Display = folder.Name,
                //    Id = folder.Id,
                //    Properties = new Dictionary<string, object>
                //    {
                //        { Resource.Prop_Created, folder.CreatedDateTime.Value.ToLocalTime() },
                //        { Resource.Prop_ChildCount, folder.Folder.ChildCount },
                //        { Resource.Prop_Id, folder.Id }
                //    }
                //});
            }
            return true;
        }
    }
}
