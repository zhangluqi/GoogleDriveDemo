using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLib
{
    public class UploadEntity
    {
        /// <summary>
        /// 上传地址
        /// </summary>
        private string _url;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// 源文件路径
        /// </summary>
        private string _sourcePath = "";

        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }

        /// <summary>
        /// 源文件名字
        /// </summary>
        private string _sourceName = "";

        public string SourceName
        {
            get { return _sourceName; }
            set { _sourceName = value; }
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
        /// 块的大小
        /// </summary>
        private int _chunkSize;
        public int ChunkSize
        {
            get { return _chunkSize; }
            set { _chunkSize = value; }
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
        /// 标示是Post方法还是put方法,默认为post方法
        /// </summary>
        private string _method;

        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }


    }
}
