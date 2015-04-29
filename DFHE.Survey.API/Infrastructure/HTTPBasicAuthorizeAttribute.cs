using System;
using System.Text;
using System.Web;
using Autofac;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;

namespace DFHE.Survey.API
{
    public class HTTPBasicAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private readonly IUserInfoService _loginService = AuthenticationFactory.GetLocalAuthenticationService();

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                string userInfo = Encoding.Default.GetString(Convert.FromBase64String(actionContext.Request.Headers.Authorization.Parameter));
                //用户验证逻辑
                var tmpuname = userInfo.Split(':')[0];
                var tmppwd = userInfo.Split(':')[1];

                var tmpUser = new UserInfo()
                {
                    UserName = tmpuname,
                    Password = tmppwd
                };

                if (_loginService.Login(tmpUser) != null)
                {
                    //var tmptt = EncryptHelper.EncodeBase64(string.Format("{0}:{1}", employee.UserName, employee.Password));

                    if (string.Equals(userInfo, string.Format("{0}:{1}", tmpuname, tmppwd)))
                    {
                        // 当前登陆用户信息
                        if (SecurityFactory.CurrentUser == null || SecurityFactory.CurrentUser.UserName != tmpuname)
                            SecurityFactory.Login(new UserInfo() { UserName = tmpuname }, null);

                        IsAuthorized(actionContext);
                    }
                    else
                    {
                        HandleUnauthorizedRequest(actionContext);
                    }
                }
            }
            else
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
            //throw new System.Web.Http.HttpResponseException(challengeMessage);
        }
    }

    public class AuthenticationFactory
    {
        private static IUserInfoService _loginService;

        public static void InitAuthenticationFactory(IUserInfoService loginService)
        {
            _loginService = loginService;
        }

        public static IUserInfoService GetLocalAuthenticationService()
        {
            return _loginService;
        }
        }
    }
