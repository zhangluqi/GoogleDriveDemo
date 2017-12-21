using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.ViewModel.Common
{
    public class ProgrecessVM : ViewModelBase
    {
        /// <summary>
        /// 进度值
        /// </summary>
        private double _rate;
        public double Rate
        {
            get
            {
                return _rate;
            }

            set
            {
                _rate = value;
                base.OnPropertyChanged("Rate");
            }
        }

        /// <summary>
        /// 下载过程描述
        /// </summary>
        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }

            set
            {
                _result = value;
                base.OnPropertyChanged("Result");
            }
        }

    }
}
