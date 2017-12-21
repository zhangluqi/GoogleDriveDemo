using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Util
{
    /// <summary>
    /// 设置管理类
    /// </summary>
    public class SettingManager
    {
        #region Fileds
        private string _saveDir = @"C:\iCloud_Drive";
        public string SaveDir
        {
            get
            {
                if (!Directory.Exists(_saveDir))
                {
                    Directory.CreateDirectory(_saveDir);
                }
                return _saveDir;
            }
            set { _saveDir = value; }
        }
        #endregion




        #region  Singleton
        private SettingManager()
        {

        }
        private static readonly object syncLock = new object();
        private static volatile SettingManager _instance = null;



        public static SettingManager Instence()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new SettingManager();

                    }
                }
            }
            return _instance;
        }
        #endregion

    }
}
