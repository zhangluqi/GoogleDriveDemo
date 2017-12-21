
using ConfigLib;
using LogLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChacheLib
{
    /// <summary>
    /// 数据缓存区
    /// </summary>
    public class MemoryCache
    {
        #region  Singleton
        private MemoryCache()
        {

        }
        private static readonly object syncLock = new object();
        private static volatile MemoryCache _instance = null;
        public static MemoryCache Instence()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new MemoryCache();
                        _itemTaskChache = new Dictionary<string, ConcurrentQueue<byte[]>>();
                        _chache = new Dictionary<string, byte[]>();
                        _offset = new Dictionary<string, int>();
                        _numBlocks = new Dictionary<string, int>();

                        _productSemaphore = new Dictionary<string, Semaphore>();
                        _customSemphore = new Dictionary<string, Semaphore>();

                    }
                }
            }
            return _instance;
        }
        #endregion
        #region Fields
        private static Dictionary<string, ConcurrentQueue<byte[]>> _itemTaskChache;
        private static Dictionary<string, Semaphore> _productSemaphore;
        private static Dictionary<string, Semaphore> _customSemphore;

        private static Dictionary<string, byte[]> _chache;
        private static Dictionary<string, int> _offset;
        private static Dictionary<string, int> _numBlocks;//块数


        /// <summary>
        /// 准备资源
        /// </summary>
        /// <param name="taskId"></param>
        public void Prepare(string taskId)
        {
            int N = Config.Instence().ChacheBlockCout;
            Semaphore productSem = new Semaphore(N, N);
            Semaphore customSem = new Semaphore(0, N);

            _productSemaphore.Add(taskId, productSem);
            _customSemphore.Add(taskId, customSem);
        }

        public void internalEnqueue(string taskId, byte[] bytes, int len)
        {
            _productSemaphore[taskId].WaitOne();

            Log.WriteLog("---------input block into chache------------");
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }
            if (!_itemTaskChache.ContainsKey(taskId))
            {
                ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
                EventWaitHandle enventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                queue.Enqueue(bytes);
                _itemTaskChache.Add(taskId, queue);
            }
            else
            {
                _itemTaskChache[taskId].Enqueue(bytes);
            }
            _numBlocks[taskId] += 1;
            //成功就释放
            try
            {
                _customSemphore[taskId].Release();
            }
            catch (Exception ex)
            {
                Log.WriteLog("Release CustomSemEx:" + ex.Message);
            }
        }

        public void InputItemTaskChache(string taskId, byte[] tmpbytes, int len, UploadInfo uploadInfo)
        {
            //手动抛出异常，结束过本次任务
            ChacheManager.Instence().ThrowException(taskId, ExceptionEnum.STOPEX.ToString());

            int blockSize = uploadInfo.BlockSize;
            long fileSize = uploadInfo.FileSize;
            int NUMBLOCKS = uploadInfo.NumBlocks;
            int LastBlockSize = uploadInfo.LastBlockSize;
            //构建块
            try
            {
                if (!_chache.ContainsKey(taskId))
                {
                    byte[] _chacheBytes = new byte[blockSize];
                    _chache.Add(taskId, _chacheBytes);
                    _offset.Add(taskId, 0);
                    _numBlocks.Add(taskId, 0);
                }

                if (_offset.ContainsKey(taskId))
                {
                    //剩余的字节
                    int needSize = blockSize - _offset[taskId];
                    if (len < needSize)
                    {
                        Array.Copy(tmpbytes, 0, _chache[taskId], _offset[taskId], len);
                        _offset[taskId] += len;
                    }
                    else
                    {
                        int leftSize = len - needSize;
                        //第一块
                        Array.Copy(tmpbytes, 0, _chache[taskId], _offset[taskId], needSize);
                        internalEnqueue(taskId, _chache[taskId], blockSize);

                        _chache[taskId] = new byte[blockSize];
                        _offset[taskId] = 0;

                        //剩余的整块数
                        int countBlock = leftSize / blockSize;
                        int fanilSize = leftSize % blockSize;
                        for (int i = 0; i < countBlock; i++)
                        {
                            //多余的块
                            byte[] newBlockBytes = new byte[blockSize];
                            Array.Copy(tmpbytes, needSize + i * blockSize, newBlockBytes, 0, blockSize);
                            internalEnqueue(taskId, newBlockBytes, blockSize);
                        }
                        //最后剩余的块
                        int newoffset = needSize + countBlock * blockSize;
                        Array.Copy(tmpbytes, newoffset, _chache[taskId], 0, leftSize - countBlock * blockSize);
                        _offset[taskId] = len - newoffset;
                    }

                    //如果是最后一块
                    if (_numBlocks.ContainsKey(taskId) && _numBlocks[taskId] == NUMBLOCKS - 1)
                    {
                        Log.WriteLog("last block-------------"+"m:"+ _offset[taskId]+"n:"+LastBlockSize);
                        if (_offset[taskId] == LastBlockSize)
                        {
                            Log.WriteLog("finished-------------");
                            byte[] newBytes = new byte[LastBlockSize];
                            Array.Copy(_chache[taskId], newBytes, LastBlockSize);
                            internalEnqueue(taskId, newBytes, LastBlockSize);
                            _chache.Remove(taskId);
                            _offset.Remove(taskId);
                            _numBlocks.Remove(taskId);
                            Log.WriteLog("download finished .....");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("build block:" + ex.ToString());
            }
        }

        public byte[] OutItemchache(string taskId)
        {
            _customSemphore[taskId].WaitOne();
            //手动抛出异常，结束过本次任务
            ChacheManager.Instence().ThrowException(taskId, ExceptionEnum.STOPEX.ToString());
            byte[] bytes = null;
            if (_itemTaskChache.ContainsKey(taskId))
            {
                bool result = _itemTaskChache[taskId].TryDequeue(out bytes);
            }
            try
            {
                _productSemaphore[taskId].Release();
            }
            catch (Exception ex)
            {
                Log.WriteLog("Release ProductSemEx:" + ex.Message);
            }
            return bytes;
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="taskId"></param>
        public void Recycle(string taskId)
        {
            Log.WriteLog("Recycle rources");
            if (_itemTaskChache.ContainsKey(taskId))
            {
                _itemTaskChache.Remove(taskId);
            }
            if (_offset.ContainsKey(taskId))
            {
                _offset.Remove(taskId);
            }
            if (_numBlocks.ContainsKey(taskId))
            {
                _numBlocks.Remove(taskId);
            }
            if (_productSemaphore.ContainsKey(taskId))
            {
                _productSemaphore[taskId].Close();
                _productSemaphore.Remove(taskId);
            }

            if (_customSemphore.ContainsKey(taskId))
            {
                _customSemphore[taskId].Close();
                _customSemphore.Remove(taskId);
            }
        }

        public void Set(string taskId, HandEnum handEnum)
        {
            try
            {
                if (handEnum == HandEnum.UPLOAD)
                {
                    Log.WriteLog("Release semophore UPLOAD");
                    if (!_productSemaphore[taskId].WaitOne(0))
                    {
                        _productSemaphore[taskId].Release();
                    }
                  
                }
                else if (handEnum == HandEnum.LOAD)
                {
                    Log.WriteLog("Release semophore Load");
                    if (!_customSemphore[taskId].WaitOne(0))
                    {
                        _customSemphore[taskId].Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Release semophore:" + handEnum.ToString() + ex.ToString());
            }
        }

        #endregion



    }
}
