using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.ViewModel.Common
{
    public class FileEntity : ViewModelBase
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
                base.OnPropertyChanged("FileName");
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
                base.OnPropertyChanged("FileId");
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
                base.OnPropertyChanged("FileSize");
            }
        }


        /// <summary>
        /// 父节点ID
        /// </summary>
        private string _parentID="root";
        public string ParentID
        {
            get
            {
                return _parentID;
            }

            set
            {
                _parentID = value;
                base.OnPropertyChanged("ParentID");
            }
        }



        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                _isChecked = value;
                base.OnPropertyChanged("IsChecked");
            }
        }

        private bool _isChecked = false;


        /// <summary>
        /// 是不是文件，ture为文件，false为文件夹
        /// </summary>
        public bool IsFile
        {
            get
            {
                return _isFile;
            }

            set
            {
                _isFile = value;
                base.OnPropertyChanged("IsFile");
            }
        }


        private bool _isFile = true;



        private IEnumerable<FileEntity> _childFileList;
        public IEnumerable<FileEntity> ChildFileList
        {
            get
            {
                return _childFileList;
            }

            set
            {
                _childFileList = value;
                base.OnPropertyChanged("ChildFileList");
            }
        }
    }
}
