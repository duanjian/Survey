using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DFHE.Survey.Model;

namespace DFHE.Survey.BLL
{
    public abstract class BaseService
    {
        /// <summary>
        /// 当前服务器时间
        /// </summary>
        public DateTime CurrentServerTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public string CurrentUser
        {
            get { return "admin"; }
        }        

        /// <summary>
        /// 根目录路径
        /// </summary>
        public string RootPath
        {
            get
            {
                string str = HttpContext.Current.Server.MapPath("~/");
                return str;
            }
        }
    }
}
