using GoogleDriveDemo.Model.Gmail;
using GoogleDriveDemo.Util;
using GoogleDriveDemo.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveDemo.ViewModel.Gmail
{
    public class GmailDetailViewModel:ViewModelBase
    {
        private ObservableCollection<MailEntity> _detailVM = new ObservableCollection<MailEntity>();
        public ObservableCollection<MailEntity> DetailVM
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
        private GmailLoad _gmailLoad = new GmailLoad();

        public void Search()
        {
            Thread th = new Thread(() => {
                GmailSearch gmailSearch = new GmailSearch();
                gmailSearch.SearchMail(ServiceManager.Instence().GmailServive,
                    t => {

                        MailEntity mailEntity = new MailEntity()
                        {
                            Id = t.Id,
                            Snippet = t.Snippet,

                        };
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            _detailVM.Add(mailEntity);
                        }));

                    });

            });
            th.Name = "SearchGmailBoxThread";
            th.Start();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public void Load(string Id)
        {
            String outputDir = SettingManager.Instence().SaveDir;
            
            //这里是获取附近
            _gmailLoad.GetAttachments(ServiceManager.Instence().GmailServive, "me", Id, outputDir);
            //_dropboxFileLoad = new DropboxFileLoad();
            //_dropboxFileLoad.ProgressEvent += new DropboxFileLoad.ProgressHandler(ProgressEvent);
            //_dropboxFileLoad.FinishedEvent += new DropboxFileLoad.FinishedHandler(FinishedEvent);
            //string dir = @"D:\iCloud_Drive";
            //if (!Directory.Exists(dir))
            //{
            //    Directory.CreateDirectory(dir);
            //}
            //string savePath = Path.Combine(dir, fileName);
            //string forder = "";

            //_dropboxFileLoad.DownLoadFile(ServiceManager.Instence().DropboxClient, forder, fileName, savePath, fileSize);
        }

        private void FinishedEvent()
        {
            //下载完成
            Progrecess.Result = "Completed";
            Progrecess.Rate = 100;
            //if (_dropboxFileUpload != null)
            //{
            //    _dropboxFileUpload.ProgressEvent -= ProgressEvent;
            //    _dropboxFileUpload.FinishedEvent -= FinishedEvent;
            //}

            //if (_dropboxFileLoad != null)
            //{
            //    _dropboxFileLoad.ProgressEvent -= ProgressEvent;
            //    _dropboxFileLoad.FinishedEvent -= FinishedEvent;
            //}
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
