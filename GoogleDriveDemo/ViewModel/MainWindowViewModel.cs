using GoogleDriveDemo.View.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GoogleDriveDemo.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {

             MiddleDataControl = new LoginControl();
             TopDataControl = new TopControl();
        }


        private UserControl _middleDataControl = null;
        /// <summary>
        /// Middle Control View
        /// </summary>
        public UserControl MiddleDataControl
        {
            get { return _middleDataControl; }
            set
            {
                _middleDataControl = value;
                base.OnPropertyChanged("MiddleDataControl");
            }
        }

        private UserControl _topDataControl = null;
        /// <summary>
        /// Middle Control View
        /// </summary>
        public UserControl TopDataControl
        {
            get { return _topDataControl; }
            set
            {
                _topDataControl = value;
                base.OnPropertyChanged("TopDataControl");
            }
        }

    }
}
