using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLib
{
    public class LoadEntity
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        private string _url;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        private long _fileSize;

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        /// <summary>
        /// 任务Id
        /// </summary>
        private string _taskId;

        public string TaskId
        {
            get { return _taskId; }
            set { _taskId = value; }
        }

        /// <summary>
        /// 上传块大小
        /// </summary>
        private int _chunkSize;

        public int ChunkSize
        {
            get { return _chunkSize; }
            set { _chunkSize = value; }
        }

    }
}
