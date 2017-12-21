using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using System.Reflection;

namespace LogLib.SysLog
{
    public class LogMessager
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogMessageType
        {
            /// <summary>
            /// 调试
            /// </summary>
            Debug,
            /// <summary>
            /// 信息
            /// </summary>
            Info,
            /// <summary>
            /// 警告
            /// </summary>
            Warn,
            /// <summary>
            /// 错误
            /// </summary>
            Error,
            /// <summary>
            /// 致命错误
            /// </summary>
            Fatal
        }

        private static ILog m_log;
        private static bool IsLoaded = false;


        /// <summary>
        /// 动态改变日志文件的纪录路径
        /// </summary>
        public static void ChangeRollingFilePath(string logPath)
        {
            if (IsLoaded)
            {
                return;
            }
            try
            {
                if (!string.IsNullOrEmpty(logPath))
                {
                    logPath = Path.Combine(logPath, "ErrorLog");
                    if (!Directory.Exists(logPath)) { Directory.CreateDirectory(logPath); }
                    log4net.Repository.Hierarchy.Logger root = ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root;
                    root.Level = log4net.Core.Level.Debug;
                    log4net.Appender.RollingFileAppender appender = new log4net.Appender.RollingFileAppender();
                    string strFilePath = Path.Combine(logPath, "log_system.log");
                    appender.File = strFilePath;
                    appender.AppendToFile = true;
                    appender.MaxSizeRollBackups = 5;
                    appender.MaximumFileSize = "1MB";
                    appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
                    appender.StaticLogFileName = false;
                    appender.Encoding = Encoding.UTF8;
                    appender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
                    appender.DatePattern = "yyyy-MM-dd";
                    appender.Layout = new log4net.Layout.PatternLayout("%date [%thread] %-5level - %message%newline");
                    appender.ActivateOptions();
                    log4net.Config.BasicConfigurator.Configure(appender);
                    IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                string TempPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (System.IO.Directory.Exists(TempPath))
                {
                    TempPath = Path.Combine(TempPath, "imobie");
                    TempPath = Path.Combine(TempPath, "ErrorLog");
                    if (!System.IO.Directory.Exists(TempPath))
                    {
                        Directory.CreateDirectory(TempPath);
                    }
                    TempPath = Path.Combine(TempPath, "logfile.txt");
                    if (File.Exists(TempPath))
                    {
                        File.Delete(TempPath);
                    }
                    FileStream fs1 = File.Create(TempPath);
                    fs1.Close();
                    StreamWriter sw = new StreamWriter(TempPath, true);
                    sw.WriteLine("Create LogFile fail!");
                    sw.WriteLine("Create Path:" + (logPath != "" ? logPath : "Null"));
                    sw.WriteLine("Failed Messsage:" + ex.Message);
                    sw.Close();
                }
            }

        }

        /// <summary>
        /// 初始化日志系统
        /// </summary>
        public static void Init(string filePath)
        {
            try
            {
                FileInfo fileLog = new FileInfo(@filePath);
                log4net.Config.XmlConfigurator.ConfigureAndWatch(fileLog);
            }
            catch (Exception ex)
            {
                throw new Exception("加载log4net文件" + filePath + "时发生错误：" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        public static void Write(Type type, Exception ex, LogMessageType messageType)
        {
            DoLog("", messageType, ex, type);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        public static void Write(string message, LogMessageType messageType)
        {
            DoLog(message, messageType, null, MethodInfo.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        public static void WriteInfo(string message)
        {
            DoLog(message, LogMessageType.Info, null, MethodInfo.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void WriteDebug(string message)
        {
            DoLog(message, LogMessageType.Debug, null, MethodInfo.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="type"></param>
        public static void Write(string message, LogMessageType messageType, Type type)
        {
            DoLog(message, messageType, null, type);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="ex">异常</param>
        public static void Write(string message, LogMessageType messageType, Exception ex)
        {
            DoLog(message, messageType, ex, MethodInfo.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="ex">异常</param>
        /// <param name="type"></param>
        public static void Write(string message, LogMessageType messageType, Exception ex,
        Type type)
        {
            DoLog(message, messageType, ex, type);
        }

        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">日志信息</param>
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, MethodInfo.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">日志信息</param>
        /// <param name="type">日志类型</param>
        public static void Assert(bool condition, string message, Type type)
        {
            if (condition == false)
                Write(message, LogMessageType.Info);
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="ex">异常</param>
        /// <param name="type">日志类型</param>
        private static void DoLog(string message, LogMessageType messageType, Exception ex,
        Type type)
        {
            m_log = LogManager.GetLogger(type);
            switch (messageType)
            {
                case LogMessageType.Debug:
                    LogMessager.m_log.Debug(message, ex);
                    break;
                case LogMessageType.Info:
                    LogMessager.m_log.Info(message, ex);
                    break;
                case LogMessageType.Warn:
                    LogMessager.m_log.Warn(message, ex);
                    break;
                case LogMessageType.Error:
                    LogMessager.m_log.Error(message, ex);
                    break;
                case LogMessageType.Fatal:
                    LogMessager.m_log.Fatal(message, ex);
                    break;
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="messageType">日志类型</param>
        /// <param name="ex">异常</param>
        /// <param name="type">日志类型</param>
        public static void DoSendMesaggeLog(string message, Type type)
        {
            m_log = LogManager.GetLogger(type);
            LogMessager.m_log.Info(message);
        }

    }
}
