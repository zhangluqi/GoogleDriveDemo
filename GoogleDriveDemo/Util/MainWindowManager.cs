using GoogleDriveDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.Util
{
    public class MainWindowManager
    {
        #region Fileds
        private MainWindowViewModel _mainVM;
        public MainWindowViewModel MainVM
        {
            get
            {
                return _mainVM;
            }

            set
            {
                _mainVM = value;
            }
        }

        #endregion


        #region  Singleton
        private MainWindowManager()
        {

        }
        private static readonly object syncLock = new object();
        private static volatile MainWindowManager _instance = null;
        public static MainWindowManager Instence()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new MainWindowManager();

                    }
                }
            }
            return _instance;
        }
        #endregion
    }
}
