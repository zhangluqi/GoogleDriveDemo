using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLib
{
    public class UnfixedHeadInfo
    {
        private int _blockSize;

        public int BlockSize
        {
            get { return _blockSize; }
            set { _blockSize = value; }
        }

        private long _fileSize;

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        private long _offset;

        public long  Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        private bool _isAdd;

        public bool IsAdd
        {
            get { return _isAdd; }
            set { _isAdd = value; }
        }


    }
}
