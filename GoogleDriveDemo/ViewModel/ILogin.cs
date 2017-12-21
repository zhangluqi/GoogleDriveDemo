using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveDemo.ViewModel
{
    interface ILogin
    {
        /// <summary>
        /// 登录
        /// </summary>
        void Login();
        /// <summary>
        /// 清除登陆记录
        /// </summary>
        void ClearAuthRecord();
      
    }
}
