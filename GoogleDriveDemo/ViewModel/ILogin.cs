using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  Clouder;

namespace GoogleDriveDemo.ViewModel
{
    interface ILogin
    {
        /// <summary>
        /// 登录
        /// </summary>
        void Login(Cloudbase cloudbase);
        /// <summary>
        /// 清除登陆记录
        /// </summary>
        void ClearAuthRecord();
      
    }
}
