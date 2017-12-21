using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Google
{
    /// <summary>
    /// 获取google认证已经服务
    /// </summary>
    public class AuthGoogle
    {
        private string[] Scopes = new string[] { DriveService.Scope.Drive };
        private string ApplicationName = "GoogleDriveDemo";
        /// <summary>
        /// 认证并且获取服务
        /// </summary>
        /// <returns></returns>
        public DriveService GetService()
        {
            UserCredential credential;
            DriveService driveService=null;
            try
            {
                using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                    credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Drive API service.
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }catch(Exception ex)
            {
                Debug.WriteLine("Auth Error:"+ex.ToString());
            }
            
            return driveService;
        }

        /// <summary>
        /// 删除认证记录
        /// </summary>
        public void ClearAuthRecord()
        {
            string credPath = System.Environment.GetFolderPath(
                       System.Environment.SpecialFolder.Personal);
            credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

            if (Directory.Exists(credPath))
            {
                DeleteFilesAndFolders(credPath);
            }
        }

        /// <summary>  
        /// Recursively delete all the files and folders under the specific path.  
        /// </summary>  
        /// <param name="path">The specific path</param>  
        private  void DeleteFilesAndFolders(string path)
        {
            // Delete files.  
            string[] files = Directory.GetFiles(path);
            if(files != null)
            {
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
          
            // Delete folders.  
            string[] folders = Directory.GetDirectories(path);
            if(folders != null)
            {
                foreach (var folder in folders)
                {
                    DeleteFilesAndFolders(folder);
                    Directory.Delete(folder);
                }
            }   
        }
    }
}
