using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChacheLib
{
    public class UploadInfo
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


        private int _numBlocks;

        public int NumBlocks
        {
            get { return _numBlocks; }
            set { _numBlocks = value; }
        }

        private int _lastBlockSize;

        public int LastBlockSize
        {
            get { return _lastBlockSize; }
            set { _lastBlockSize = value; }
        }

    }
}
