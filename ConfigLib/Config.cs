using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigLib
{ /// <summary>
  /// 配置参数类，之后配置的东西从文件读取
  /// </summary>
    public class Config
    {
        #region  Singleton
        private Config()
        {

        }
        private static readonly object syncLock = new object();
        private static volatile Config _instance = null;
        public static Config Instence()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new Config();
                        _instance.InitUp();
                    }
                }
            }
            return _instance;
        }
        #endregion


        #region Fieds
        private int _greenTaskCount = 0;

        public int GreenTaskCount
        {
            get { return _greenTaskCount; }
            set { _greenTaskCount = value; }
        }

        private int _maxTaskCount = 0;

        public int MaxTaskCount
        {
            get { return _maxTaskCount; }
            set { _maxTaskCount = value; }
        }

        /// <summary>
        /// 下载文件的任务数
        /// </summary>
        private int _loadTaskCount = 0;

        public int LoadTaskCount
        {
            get { return _loadTaskCount; }
            set { _loadTaskCount = value; }
        }

        /// <summary>
        /// 上传文件的任务数
        /// </summary>
        private int _uploadTaskCount = 0;

        public int UploadTaskCount
        {
            get { return _uploadTaskCount; }
            set { _uploadTaskCount = value; }
        }

        //上传块大小
        private Dictionary<string, int> _blockUpSize;
        public Dictionary<string, int> BlockUpSize
        {
            get { return _blockUpSize; }
            set { _blockUpSize = value; }
        }

        private string _logPath;

        //日志文件路径
        public string LogPath
        {
            get { return _logPath; }
            set { _logPath = value; }
        }


        private int _chacheBlockCout = 1;

        public int ChacheBlockCout
        {
            get { return _chacheBlockCout; }
            set { _chacheBlockCout = value; }
        }

        private string _processUrl;

        public string ProcessUrl
        {
            get { return _processUrl; }
            set { _processUrl = value; }
        }


        private void InitUp()
        {
            try
            {
                //日志路径
                string xmlLogPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"bin\ConfigAll\MainConfig.xml");
                LogPath = XmlUtil.XmlHelper.Read(xmlLogPath, "LogPath", "key", "value")["LogPath"]["LogPath"];
                Log.SetLogPath(LogPath);

                //进度上传地址
                ProcessUrl = XmlUtil.XmlHelper.Read(xmlLogPath, "ProcessUrl", "key", "value")["ProcessUrl"]["ProcessUrl"];


                //缓冲区块数大小
                ChacheBlockCout = int.Parse(XmlUtil.XmlHelper.Read(xmlLogPath, "ChacheBlockCout", "key", "value")["ChacheBlockCout"]["ChacheBlockCout"]);

                //任务列表及参数
                string xmlTaskPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"bin\ConfigAll\TaskConfig.xml");
                Dictionary<string, Dictionary<string, string>> taskConfigDic = XmlUtil.XmlHelper.Read(xmlTaskPath, "Task", "key", "value");
                GreenTaskCount = int.Parse(taskConfigDic["Task"]["GreenTaskCount"]);
                MaxTaskCount = int.Parse(taskConfigDic["Task"]["GreenTaskCount"]);
                LoadTaskCount = int.Parse(taskConfigDic["Task"]["GreenTaskCount"]);
                UploadTaskCount = int.Parse(taskConfigDic["Task"]["GreenTaskCount"]);
                Log.WriteLog("LoadTaskCount:" + LoadTaskCount);
                Log.WriteLog("UploadTaskCount:" + UploadTaskCount);
                //上传块大小
                BlockUpSize = new Dictionary<string, int>();
                string xmlUploadInfoPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"bin\ConfigAll\UploadInfo.xml");
                Dictionary<string, Dictionary<string, string>> dic = XmlUtil.XmlHelper.Read(xmlUploadInfoPath, "upload", "key", "value");
                foreach (var key in dic["upload"].Keys)
                {
                    //之后从配置文件读取
                    BlockUpSize.Add(key, int.Parse(dic["upload"][key]));
                    Log.WriteLog("Read from Config key:" + key);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Read from Config:" + ex.ToString());
            }

        }

        #endregion

    }
}
