using GoogleDriveDemo.Model.Instagram;
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

namespace GoogleDriveDemo.ViewModel.Instamgram
{
    public class InstamGramDetailViewModel:ViewModelBase
    {

        private ObservableCollection<ImageEntity> _detailVM = new ObservableCollection<ImageEntity>();
        public ObservableCollection<ImageEntity> DetailVM
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

        private InstamgramFileLoad _instamgramFileLoad;

        public void Search()
        {
            Thread th = new Thread(() => {
                InstagramSearch instagramSearch = new InstagramSearch();
                instagramSearch.SearchFile(ServiceManager.Instence().InstamgramClient,
                    t =>
                    {

                        ImageEntity imageEntity = new ImageEntity()
                        {
                            Url = t.Url

                        };
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            _detailVM.Add(imageEntity);
                        }));

                    });

            });
            th.Name = "SearchFacebookThread";
            th.Start();
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        public void Load(string url, string fileName)
        {
            _instamgramFileLoad = new InstamgramFileLoad();
            _instamgramFileLoad.ProgressEvent += new InstamgramFileLoad.ProgressHandler(ProgressEvent);
            _instamgramFileLoad.FinishedEvent += new InstamgramFileLoad.FinishedHandler(FinishedEvent);
            string dir = SettingManager.Instence().SaveDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string savePath = System.IO.Path.Combine(dir, string.Format("{0}.jpg", DateTime.Now.Ticks.ToString()));
            // string savePath = Path.Combine(dir, fileName);
            _instamgramFileLoad.DownLoadFile(url, savePath);
        }

        private void FinishedEvent()
        {
            //下载完成
            Progrecess.Result = "Completed";
            Progrecess.Rate = 100;


            if (_instamgramFileLoad != null)
            {
                _instamgramFileLoad.ProgressEvent -= ProgressEvent;
                _instamgramFileLoad.FinishedEvent -= FinishedEvent;
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
