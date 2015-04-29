using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using LoginDemo.API;

namespace DFHE.Survey.API
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AutofacConfig.AutofacConfiguration();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //GlobalConfiguration.Configuration.Filters.Add(new ActionFilter());
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new DirectUrlReqHandler());

        }

        protected void Application_BeginRequest()
        {
            var tmpUrl = Request.RawUrl;
            var urlArray = Request.Url.OriginalString.Split('/');
            var basicUrl = string.Join("/", urlArray[0], urlArray[1], urlArray[2]);

            if (!tmpUrl.ToString().Contains("api"))
            {
                Response.Redirect(basicUrl + "/index.html#" + tmpUrl);
            }
        }
    }
}