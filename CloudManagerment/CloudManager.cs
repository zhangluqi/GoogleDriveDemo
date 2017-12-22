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
        public static void Login(string username, string password)
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
                GetCouldList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
        private static void GetClouds(List<DriveList> allCoudList)
        {
            if (allCoudList != null && allCoudList.Count > 0)
            {
                if (Clouds != null && Clouds.Count > 0)
                {
                    Clouds.Clear();
                }
                try
                {
                    foreach (var item in allCoudList)
                    {
                        Cloudbase cloudclass;
                        cloudclass = CreateCloud(item);
                        if (cloudclass != null)
                        {
                            Clouds.Add(cloudclass);
                        }
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
            Cloudbase cloudclass = null;
            switch (item.Service)
            {
                case "microsoft":
                    cloudclass = new OneDriveManager();
                    break;
            }
            if (cloudclass != null)
            {
                cloudclass.CloudId = Guid.NewGuid();
                cloudclass.Cloud = new Cloud
                {
                   CloudDriveId = item.Id,
                   CloudEmail = item.Email,
                   CloudService = item.Service,
                   CloudToken = item.Token,
                   CloudExpiresAt = item.ExpiresAt,
                   CloudExpiresIn = item.ExpiresIn
                };
            }
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
                Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            }
        }

        /// <summary>
        /// 获取所有云的列表
        /// </summary>
        /// <returns></returns>
        public static List<Cloudbase> GetCloudList()
        {
            List<Cloudbase> list = new List<Cloudbase>();
            foreach (var item in Clouds)
            {
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 搜索文件
        /// </summary>
        /// <param name="cloudid"></param>
        /// <param name="fileid"></param>
        /// <returns></returns>
        public static IList<FileInformation> Search(Guid cloudid, string fileid)
        {
            var cloud = GetCloud(cloudid);
            RefreshCloudToken(cloudid);
            return cloud.Search(fileid);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="cloudid"></param>
        /// <param name="fileid"></param>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public static string CreateFolder(Guid cloudid, string fileid, string foldername)
        {
            var cloud = GetCloud(cloudid);
            RefreshCloudToken(cloudid);
            return cloud.CreateFolder(fileid, foldername);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="cloudid"></param>
        /// <param name="fileid"></param>
        /// <returns></returns>
        public static bool Delete(Guid cloudid, string fileid)
        {
            var cloud = GetCloud(cloudid);
            RefreshCloudToken(cloudid);
            return cloud.Delete(fileid);
        }

        /// <summary>
        /// 开始下载 和 上传
        /// </summary>
        /// <param name="cloudid"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="operaType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 暂停下载
        /// </summary>
        /// <param name="cloudid"></param>
        /// <param name="taskid"></param>
        public static void Pause(Guid cloudid, string taskid)
        {
            var task = TaskManagement.GetOneTask(Guid.Parse(taskid));
            task.CancelSignal.Cancel();
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        /// <param name="cloudid"></param>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public static Task<bool> Cancel(Guid cloudid, string taskid)
        {
            var cloud = GetCloud(cloudid);
            return cloud.Cancel(taskid);
        }

        public static event Action<ExceptionEventHandler> Exception;

        public static event Action<ProgressEventhandler> Progress;

        private static void Cloudclass_Exception(ExceptionEventHandler obj)
        {
            OnException(obj);
        }

        private static void Cloudclass_Progress(ProgressEventhandler obj)
        {
            OnProgress(obj);
        }

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