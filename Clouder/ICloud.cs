using System.Threading.Tasks;
using CloudObject;
using System;
using System.Collections.Generic;

namespace Clouder
{
    public enum OperaType
    {
        UpLoad,
        DownLoad
    }

    public interface ICloud
    {
        /// <summary>
        /// 上传/下载
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="operaType"> 开始的地方 </param>
        /// <returns></returns>
        Task<bool> Start(FileInformation source, string target, OperaType operaType);

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        void Pause(string taskid);

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<bool> Cancel(string taskid);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="fileid"></param>
        /// <param name="foldername"></param>
        /// <returns></returns>
        string CreateFolder(string fileid, string foldername);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        bool Delete(string fileid);

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        IList<FileInformation> Search(string fileid);

        /// <summary>
        /// 云云同步
        /// </summary>
        /// <param name="sourceCloud"></param>
        /// <param name="targetCloud"></param>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        Task<bool> SyncCloud(string sourceCloud, string targetCloud, string sourceFile, string targetFile);
    }
}
