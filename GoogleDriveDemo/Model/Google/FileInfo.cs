using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Google
{
    public class FileInfo
    {
        /// <summary>
        /// 文件名字
        /// </summary>
        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
            }
        }

        /// <summary>
        /// 文件ID,这个非常重要，在文件请求的时候需要用到
        /// </summary>
        private string _fileId;
        public string FileId
        {
            get
            {
                return _fileId;
            }

            set
            {
                _fileId = value;
            }
        }


        /// <summary>
        /// 文件大小
        /// </summary>
        private string _fileSize;
        public string FileSize
        {
            get
            {
                return _fileSize;
            }

            set
            {
                _fileSize = value;

            }
        }

       /// <summary>
       /// 是不是文件
       /// </summary>
        private bool _isFile;
        public bool IsFile
        {
            get { return _isFile; }
            set { _isFile = value; }
        }

        private string _parentId;
        public string ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }



    }
}
