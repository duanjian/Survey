using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Providers.Entities;
using DFHE.Survey.DAL;
using DFHE.Survey.IBLL;
using DFHE.Survey.IDAL;
using DFHE.Survey.Model;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Security.Principal;

namespace DFHE.Survey.API
{
    public class SecurityFactory
    {

        private static IUserInfoRepository _userInfoRepository;

        public static void InitSecurityFactory(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }   
        /// <summary>
        /// 获取当前认证信息
        /// </summary>
        private static IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        /// <summary>
        /// 保存当前用户的认证信息
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="context"></param>
        public static void Login(UserInfo user, HttpContextBase context)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            var clailms = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var identity = new ClaimsIdentity(clailms, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(identity);

        }

        /// <summary>
        /// 清除当前用户的认证信息
        /// </summary>
        public static void Logout()
        {
            AuthenticationManager.SignOut();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        }

        private static UserInfo _employeeInfo;
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        public static UserInfo CurrentUser
        {
            get
            {
                //if (_employeeInfo == null)
                //{
                _employeeInfo = GetCurrentEmployee(HttpContext.Current.User);
                // }
                return _employeeInfo;
            }
        }


        private static UserInfo GetCurrentEmployee(IPrincipal principal)
        {
            string account = principal.Identity.Name;
            if (string.IsNullOrWhiteSpace(account))
            {
                return null;
            }
            UserInfo user = _userInfoRepository.LoadEntities(u => u.UserName == account).FirstOrDefault();
            return user;
        }
    }
}