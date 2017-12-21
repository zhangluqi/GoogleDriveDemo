using GoogleDriveDemo.Model.Dropbox;
using GoogleDriveDemo.Model.Facebook;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveDemo.ViewModel.Facebook
{
    public class FacebookDetailViewModel:ViewModelBase
    {
        private ObservableCollection<FacebookEntity> _detailVM = new ObservableCollection<FacebookEntity>();
        public ObservableCollection<FacebookEntity> DetailVM
        {
            get
            {
                return _detailVM;
            }

            set
            {
                _detailVM = value;
            }
        }

        public ProgrecessVM Progrecess
        {
            get
            {
                return _progrecess;
            }

            set
            {
                _progrecess = value;
                base.OnPropertyChanged("Progrecess");
            }
        }

        private ProgrecessVM _progrecess = new ProgrecessVM();

        private FacebookLoad _facebookFileLoad;
 
        public void Search()
        {
            Thread th = new Thread(() => {
                FacebookSearch facebookSearch = new FacebookSearch();
                facebookSearch.SearchFile(ServiceManager.Instence().FacebookClient,
                    t => {

                        FacebookEntity facebookEntity = new FacebookEntity()
                        {
                            Url = t.Url
                          
                        };
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            _detailVM.Add(facebookEntity);
                        }));

                    });

            });
            th.Name = "SearchFacebookThread";
            th.Start();
        }

      
        /// <summary>
        /// 下载文件
        /// </summary>
        public void Load(string url,string fileName)
        {
            _facebookFileLoad = new FacebookLoad();
            _facebookFileLoad.ProgressEvent += new FacebookLoad.ProgressHandler(ProgressEvent);
            _facebookFileLoad.FinishedEvent += new FacebookLoad.FinishedHandler(FinishedEvent);
            string dir = SettingManager.Instence().SaveDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string savePath = System.IO.Path.Combine(dir, string.Format("{0}.jpg",DateTime.Now.Ticks.ToString()));
           // string savePath = Path.Combine(dir, fileName);
            _facebookFileLoad.DownLoadFile(url,savePath);
        }

        private void FinishedEvent()
        {
            //下载完成
            Progrecess.Result = "Completed";
            Progrecess.Rate = 100;
           

            if (_facebookFileLoad != null)
            {
                _facebookFileLoad.ProgressEvent -= ProgressEvent;
                _facebookFileLoad.FinishedEvent -= FinishedEvent;
            }
        }

        private void ProgressEvent(long downloadedSize, long fileSize)
        {
            double rate = (double)downloadedSize / fileSize * 100;
            rate = rate > 100 ? 100 : rate;
            Progrecess.Rate = rate;
            Progrecess.Result = string.Format("{0:N2} %", rate);
        }
    }
}
