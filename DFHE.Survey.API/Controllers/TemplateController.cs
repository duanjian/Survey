using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;
using DFHE.Survey.Model.ViewModel;
using DFHE.Survey.Utility;
using Ionic.Zip;
using System.Net.Http.Headers;

namespace DFHE.Survey.API.Controllers
{
    public class TemplateController : ApiController
    {
        private ITemplateService _templateService;
        private IUserInfoService _userInfoService;

        public TemplateController(ITemplateService templateService,IUserInfoService userInfoService)
        {
            _templateService = templateService;
            _userInfoService = userInfoService;
        }

        public string RootPath
        {
            get
            {
                string str = HttpContext.Current.Server.MapPath("~/");
                return str;
            }
        }       

        public ResultVO GetTemplateList()
        {
            var ret = _templateService.GetTemplateList();
            return ret;
        }

        [HttpPost]
        public ResultVO UploadTemplate()
        {
            ResultVO result = new ResultVO() { Result = 0 };
            try
            {
                var files = HttpContext.Current.Request.Files;
                // 配置比较好
                string targetPath = "Upload\\~temp\\";
                string serverPath = RootPath + targetPath;
                if (FileHelper.InitDirectory(serverPath))
                {
                    var file = files[0];
                    string newFileName = FileHelper.GetFileNameByTime(file.FileName);
                    #region --- 保存到临时目录 ---
                    string targetName = string.Format("{0}{1}", serverPath, newFileName);
                    Stream stream = file.InputStream;
                    using (FileStream fileStream = File.OpenWrite(targetName))
                    {
                        stream.CopyTo(fileStream);                                               
                    }
                    #endregion
                    result.Data = new { tempPath = targetName };
                    result.Result = 1;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 插入新模版
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>        
        [HttpPost]
        public ResultVO InsertTemplate(TemplateVO template)
        {
            var tempPath = template.StoredName;

            var staticFolderPath = "statics\\tmpl\\";

            var tmplRootPath = string.Format("{0}{1}", RootPath,staticFolderPath);


            var targetTmplPath = FileHelper.InitTmplPath(tmplRootPath);

            ZipHelper.Decompression(tempPath, targetTmplPath, true);

            //var tmpUser = _userInfoService.GetUserInfoById()

            var tmpl = new TemplateInfo()
            {
                TmplTitle = template.TmplTitle,
                TmplDescription = template.TmplDescription,
                PreviewUrl = template.PreviewUrl,
                StoredName = targetTmplPath,
                CreateTime = DateTime.Now,
                Deleted = false,
                Remark = string.Empty
            };            

            var ret = _templateService.InsertTemplate(tmpl);
            return ret;
        }

        [HttpDelete]
        public ResultVO DeleteTemplateById(int primaryID)
        {
            var ret = _templateService.DeleteTemplateById(primaryID);
            return ret;
        }

        [HttpGet]
        public HttpResponseMessage DownloadTemplate(int id)
        {
            //下载示例
            if (id == 0)
            {
                var examplePath = string.Format("{0}{1}{2}", RootPath, "statics\\tmplExample\\", "example.zip");
                #region --- 读取文件并返回 ---
                byte[] byteArr = File.ReadAllBytes(examplePath);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(byteArr);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("attachment");
                contentDisposition.FileName = "example.zip";
                result.Content.Headers.ContentDisposition = contentDisposition;
                #endregion
                return result;
            }

            var tmp = _templateService.GetTemplateById(id);
            var tmpl = tmp.Data as TemplateInfo;
            if (tmpl != null && !string.IsNullOrWhiteSpace(tmpl.StoredName))
            {
                //string fileName = string.Format("{0}{1}", this.RootPath, tmpl.StoredName);
                //fileName = @"C:\Users\Administrator\Desktop\version\temp\A123.jpg";
                var fileName = tmpl.StoredName;

                if (Directory.Exists(fileName))
                {
                    var plist = new List<string>();
                    plist.Add(string.Format(fileName));
                    //plist.Add(string.Format("{0}{1}", fileName, "\\base.css"));
                    //plist.Add(string.Format("{0}{1}", fileName, "\\common.css"));
                    var zipPath = string.Format("{0}{1}{2}", RootPath,"statics\\tmpl\\", "template.zip");
                    ZipHelper.CompressMulti(plist, zipPath, true);

                    #region --- 读取文件并返回 ---
                    byte[] byteArr = File.ReadAllBytes(zipPath);
                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(byteArr);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("attachment");
                    contentDisposition.FileName = string.Format("{0}.zip", tmpl.TmplTitle);
                    result.Content.Headers.ContentDisposition = contentDisposition;
                    #endregion                    
                    return result;
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

    }
}
