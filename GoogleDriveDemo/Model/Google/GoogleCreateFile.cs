using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GoogleDriveDemo.Model.Google
{
    public class GoogleCreateFile
    {
        public void CreateFolder(DriveService service, string folderName,string parentId)
        {
            var fileMetadata =new File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            if (!string.IsNullOrEmpty(parentId))
            {
                fileMetadata.Parents = new List<string>() { parentId };
            }
            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            Console.WriteLine("Folder ID: " + file.Id);
        }
    }
}
