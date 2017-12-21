using LogLib.SysLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLib
{
    public class Log
    {
        private static object _lock = new object();
        private static string LogPath = "";
        public static void SetLogPath(string path)
        {
            LogPath = path;
            LogMessager.ChangeRollingFilePath(path);
        }

        public static string GetLogPath()
        {
            return LogPath;
        }


        public static void WriteLog(string msg)
        {
            lock (_lock)
            {
                LogMessager.Write(msg, LogMessager.LogMessageType.Info);
            }
        }
    }
}
