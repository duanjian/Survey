using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.Utility
{
    public class StaticPageHelper
    {


        #region 静态页生成相关

        public static void GenerateHtml(Object survey, string tmplName, string staticPageName)
        {

            try
            {
                VelocityHelper helper = new VelocityHelper("/statics/tmpl/");
                NVTools tools = new NVTools();
                helper.Put("tools", tools);
                helper.Put("Survey", survey);
                helper.GenerateHtml(tmplName, string.Format("/statics/{0}.html", staticPageName));              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GenerateExampleHtml(Object survey, string tmplName, string staticPageFolder)
        {

            try
            {
                VelocityHelper helper = new VelocityHelper("/statics/tmpl/");
                NVTools tools = new NVTools();
                helper.Put("tools", tools);
                helper.Put("Survey", survey);
                helper.GenerateHtml(tmplName, string.Format("/statics/tmpl/{0}/example.html", staticPageFolder));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private class NVTools
        {
            public string OptNumToChar(string source)
            {
                return Convert.ToChar(Convert.ToInt32(source) + 64).ToString();
            }
        }

        #endregion
    }
}
