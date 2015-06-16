using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.Utility
{
    public class FileHelper
    {
        /// <summary>
        /// 初始化文件目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool InitDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return true;
        }

        /// <summary>
        /// 初始模版文件夹路径
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        public static string InitTmplPath(string tmplRootPath)
        {
            var newTmplFolder = string.Format("tmpl{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            var newTmplPath = string.Format("{0}{1}",tmplRootPath, newTmplFolder);
            InitDirectory(newTmplPath);
            return newTmplPath;
        }

        /// <summary>
        /// 根据时间获取文件名称
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public static string GetFileNameByTime(string sourceName)
        {
            string result = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string suffix = "";
            string[] strArr = sourceName.Split('.');
            if (strArr.Length >= 2)
            {
                suffix = strArr[strArr.Length - 1];
            }
            if (string.IsNullOrWhiteSpace(suffix))
                return result;
            return string.Format("{0}.{1}", result, suffix);
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public static bool MoveFile(string sourceFile, string targetFile)
        {
            File.Move(sourceFile, targetFile);
            return true;
        }
    }

}
