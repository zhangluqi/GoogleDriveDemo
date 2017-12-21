using LogLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChacheLib
{   /// <summary>
    /// 管理缓冲区
    /// </summary>
    public class ChacheManager
    {
        #region  Singleton
        private ChacheManager()
        {

        }
        private static readonly object syncLock = new object();
        private static volatile ChacheManager _instance = null;
        public static ChacheManager Instence()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ChacheManager();
                        IsStopDic = new ConcurrentDictionary<string, bool>();
                        ReferenceCount = new Dictionary<string, int>();
                        _lockDic = new Dictionary<string, object>();
                    }
                }
            }
            return _instance;
        }
        private static Dictionary<string, object> _lockDic;
        private static ConcurrentDictionary<string, bool> _isStopDic;

        public static ConcurrentDictionary<string, bool> IsStopDic
        {
            get { return _isStopDic; }
            set { _isStopDic = value; }
        }

        //初始值为2，数量代表上传跟下载
        private static Dictionary<string, int> _referenceCount;

        public static Dictionary<string, int> ReferenceCount
        {
            get { return _referenceCount; }
            set { _referenceCount = value; }
        }

        //准备资源
        public void Prepare(string taskId)
        {
            _lockDic.Add(taskId, new object());
            AddIsStopDic(taskId);
            ReferenceCount.Add(taskId, 2);
            MemoryCache.Instence().Prepare(taskId);
        }

        private void AddIsStopDic(string taskId)
        {
            IsStopDic.AddOrUpdate(taskId, false, (key, vale) => {
                key = taskId;
                vale = false;
                return true;
            });
        }


        /// <summary>
        /// 手动抛出异常结束本次任务
        /// 完成资源清理
        /// </summary>
        public void ThrowException(string taskId, string msg)
        {
            bool isStop = false;
            if (IsStopDic.TryGetValue(taskId, out isStop))
            {
                if (isStop)
                {
                    //回收资源
                    Log.WriteLog("Manul stop Exception");
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// 回收资源，必须在upload跟load方法finally中调用
        /// </summary>
        /// <param name="taskId"></param>
        public void RecycleResource(string taskId)
        {
            if (_lockDic.ContainsKey(taskId))
            {
                lock (_lockDic[taskId])
                {
                    if (ReferenceCount.ContainsKey(taskId))
                    {
                        ReferenceCount[taskId] -= 1;
                        if (ReferenceCount[taskId] == 0)
                        {
                            bool result = IsStopDic.TryRemove(taskId, out result);
                            if (!result)
                            {
                                Log.WriteLog("Recycel isStop failed");
                            }
                            MemoryCache.Instence().Recycle(taskId);
                            _lockDic.Remove(taskId);
                            ReferenceCount.Remove(taskId);
                        }
                    }
                }
            }

        }
        public void ChangeValueIsStop(string taskId, HandEnum handEnum)
        {
            if (IsStopDic.ContainsKey(taskId))
            {
                try
                {
                    Log.WriteLog("Change IsStop to true");
                    IsStopDic[taskId] = true;
                    //放开等待锁
                    MemoryCache.Instence().Set(taskId, handEnum);
                }
                catch (Exception ex)
                {

                }

            }
        }

        #endregion

    }
}
