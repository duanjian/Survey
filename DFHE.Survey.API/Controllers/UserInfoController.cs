using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;

namespace DFHE.Survey.API.Controllers
{
    public class UserInfoController : ApiController
    {
        private IUserInfoService _userInfoService;

        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public ResultVO GetUserList()
        {
            var res = _userInfoService.GetUserList();
            return res;
        }

        /// <summary>
        /// 根据id获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultVO GetUserInfoById(int id)
        {
            var res = _userInfoService.GetUserInfoById(id);
            return res;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultVO DeleteUserInfo(int primaryID)
        {
            var res = _userInfoService.DeleteUserInfo(primaryID);
            return res;
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultVO InsertUserInfo(UserInfo user)
        {
            var res = _userInfoService.InsertUserInfo(user);
            return res;
        }
    }
}
