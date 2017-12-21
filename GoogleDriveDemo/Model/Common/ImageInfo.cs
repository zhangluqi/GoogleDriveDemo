using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Model.Common
{
    public class ImageInfo
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
