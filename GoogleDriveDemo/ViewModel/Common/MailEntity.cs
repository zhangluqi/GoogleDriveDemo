using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.ViewModel.Common
{
    public class MailEntity:ViewModelBase
    {
        /// <summary>
        /// 文件名字
        /// </summary>
        private string _snippet;
        public string Snippet
        {
            get
            {
                return _snippet;
            }

            set
            {
                _snippet = value;
                base.OnPropertyChanged("Snippet");
            }
        }

     

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                base.OnPropertyChanged("Id");
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


    }
}
