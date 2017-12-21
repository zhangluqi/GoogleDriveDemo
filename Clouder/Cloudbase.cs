using System;
using System.Threading.Tasks;
using CloudObject;
using CloudObject.EventHandler;
using System.Collections.Generic;

namespace  Clouder
{
    public abstract class Cloudbase : ICloud
    {
        public abstract event Action<ExceptionEventHandler> Exception;

        public abstract event Action<ProgressEventhandler> Progress;

        public abstract Guid CloudId { get; set; }

        public abstract Cloud Cloud { get; set; }

        #region Implementation of ICloud
        public abstract Task<bool> Start(FileInformation source, string target, OperaType operaType);

        public abstract void Pause(string taskid);

        public abstract Task<bool> Cancel(string taskid);

        public abstract string CreateFolder(string fileid, string foldername);

        public abstract bool Delete(string fileid);

        public abstract IList<FileInformation> Search(string fileid);

        public abstract Task<bool> SyncCloud(string sourceCloud, string targetCloud, string sourceFile, string targetFile);
        #endregion
    }
}
