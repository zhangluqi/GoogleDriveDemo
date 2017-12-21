using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Clouder;

using CloudManagerment.Connect2PHP;
using CloudManagerment.DTO;
using CloudManagerment.Model;

using CloudObject;
using CloudObject.EventHandler;

using Newtonsoft.Json;

using OneDrive;

namespace CloudManagerment
{
    public static class CloudManager
    {
        private static readonly IList<Cloudbase> Clouds = new List<Cloudbase>();

        private static UserInfo userInfo = new UserInfo();

        /// <summary>
        /// 向服务器获取自家token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static UserInfo Login(string username, string password)
        {
            try
            {
                Dictionary<string, string> postData = new Dictionary<string, string>
                                                      {
                                                          {"email", username},
                                                          {"password", password}
                                                      };
                string response = ConnectPHP.Instence().GetUserInfo(postData);
                Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                userInfo.UserEmail = dic["email"];
                userInfo.UserToken = dic["token"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return userInfo;
        }

        /// <summary>
        /// 获取账户下所有云的列表，并创建后放在列表里
        /// </summary>
        public static void GetCouldList()
        {
            if (!string.IsNullOrEmpty(userInfo.UserToken))
            {
                try
                {
                    string response = ConnectPHP.Instence().GetCloudList(userInfo.UserToken);
                    List<DriveList> cloudList = JsonConvert.DeserializeObject<List<DriveList>>(response);
                    GetClouds(cloudList);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        /// <summary>
        /// 将云放在列表里
        /// </summary>
        /// <param name="allCoudList"></param>
        public static void GetClouds(List<DriveList> allCoudList)
        {
            if (allCoudList != null && allCoudList.Count > 0)
            {
                try
                {
                    foreach (var item in allCoudList)
                    {
                        Cloudbase cloudclass;
                        cloudclass = CreateCloud(item);
                        Clouds.Add(cloudclass);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// 创建云
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static Cloudbase CreateCloud(DriveList item)
        {
            Cloudbase cloudclass = new OneDriveManager
                                   {
                                       CloudId = Guid.NewGuid(),
                                       Cloud = new Cloud
                                               {
                                                   CloudDriveId = item.Id,
                                                   CloudEmail = item.Email,
                                                   CloudService = item.Service,
                                                   CloudToken = item.Token,
                                                   CloudExpiresAt = item.ExpiresAt,
                                                   CloudExpiresIn = item.ExpiresIn
                                               }
                                   };
            return cloudclass;
        }

        /// <summary>
        /// 获取相对应的云
        /// </summary>
        /// <param name="cloudid"></param>
        /// <returns></returns>
        private static Cloudbase GetCloud(Guid cloudid)
        {
            return Clouds.FirstOrDefault(c => c.CloudId.Equals(cloudid));
        }

        /// <summary>
        /// 提前刷新相对应的云的token，如果没过期，直接返旧token
        /// </summary>
        /// <param name="cloudid"></param>
        /// <returns></returns>
        private static void RefreshCloudToken(Guid cloudid)
        {
            var cloudbase = GetCloud(cloudid);
            if (true) //判断是否过期
            {
                string response = ConnectPHP.Instence().RefreToken(cloudbase.Cloud.CloudDriveId, userInfo.UserToken);
            }
        }

        private static void Cloudclass_Exception(ExceptionEventHandler obj)
        {
            OnException(obj);
        }

        private static void Cloudclass_Progress(ProgressEventhandler obj)
        {
            OnProgress(obj);
        }

        public static Task<bool> Start(Guid cloudid, FileInformation source, string target, OperaType operaType)
        {
            var cloud = GetCloud(cloudid);
            RefreshCloudToken(cloudid);

            var cancel = new CancellationTokenSource();
            var token = cancel.Token;
            var result = Task.Factory.StartNew(() =>
                                               {
                                                   token.ThrowIfCancellationRequested();
                                                   return cloud?.Start(source, target, operaType);
                                               },
                                               token);
            if (result.Result.Result)
            {
                TaskManagement.AddTask(Path.Combine(source.FilePath, source.FileName),
                                       result,
                                       cancel,
                                       target,
                                       0,
                                       cloudid);
                return result.Result;
            }
            return result.Result;
        }

        public static void Pause(Guid cloudid, string taskid)
        {
            var task = TaskManagement.GetOneTask(Guid.Parse(taskid));
            task.CancelSignal.Cancel();
            //var cloud = GetCloud(cloudid);
            //cloud?.Pause(taskid);
        }

        public static Task<bool> Cancel(Guid cloudid, string taskid)
        {
            var cloud = GetCloud(cloudid);
            return cloud.Cancel(taskid);
        }

        public static string CreateFolder(Guid cloudid, string fileid, string foldername)
        {
            var cloud = GetCloud(cloudid);
            RefreshCloudToken(cloudid);
            return cloud.CreateFolder(fileid, foldername);
        }

        public static bool Delete(Guid cloudid, string fileid)
        {
            var cloud = GetCloud(cloudid);
            RefreshCloudToken(cloudid);
            return cloud.Delete(fileid);
        }

        public static IList<FileInformation> Search(Guid cloudid, string fileid)
        {
            var cloud = GetCloud(cloudid);
            RefreshCloudToken(cloudid);
            return cloud.Search(fileid);
        }

        public static event Action<ExceptionEventHandler> Exception;

        public static event Action<ProgressEventhandler> Progress;

        private static void OnException(ExceptionEventHandler obj)
        {
            Exception?.Invoke(obj);
        }

        private static void OnProgress(ProgressEventhandler obj)
        {
            Progress?.Invoke(obj);
        }
    }
}