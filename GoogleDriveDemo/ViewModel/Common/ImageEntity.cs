using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.ViewModel.Common
{
    public class ImageEntity:ViewModelBase
    {
        public string Url
        {
            get
            {
                return _url;
            }

            set
            {
                _url = value;
                base.OnPropertyChanged("Url");
            }
        }

        private string _url;




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
    }
}
