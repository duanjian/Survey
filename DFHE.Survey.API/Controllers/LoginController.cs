using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;
using DFHE.Survey.Utility;

namespace DFHE.Survey.API.Controllers
{
    public class LoginController : ApiController
    {
        private IUserInfoService _userInfoService;

        public LoginController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>        
        ///[HTTPBasicAuthorizeAttribute]
        public ResultVO Login(UserInfo user)
        {
            var res = _userInfoService.Login(user);

            if (SecurityFactory.CurrentUser == null || SecurityFactory.CurrentUser.UserName != user.UserName)
            {
                SecurityFactory.Login(new UserInfo { UserName = user.UserName }, null);
            }

            return res;
            //return new ResultVO() { Result = 0, Data = SecurityFactory.CurrentUser };    
        }

        /// <summary>
        /// 用户退出
        /// </summary>
        [HttpGet]
        public ResultVO Logout()
        {
            var result = new ResultVO { Result = 1 };
            SecurityFactory.Logout();
            return result;
        }
    }
}