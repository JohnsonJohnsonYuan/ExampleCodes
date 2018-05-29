using System.Configuration;

namespace DotNet.Framework.Common.Helper
{
    /// <summary>
    /// 部分FTP类代码来自网络.版式权归原作者所有
    /// </summary>
    public static class FTPHelper
    {
        /// <summary>
        /// 获取上一层目录
        /// </summary>
        /// <returns></returns>
        public static string getParentDirectory()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string path = context.Session["Path"].ToString();
            if (path == "./")
                return ("../");
            else if (path == "/")
                return (ConfigurationManager.AppSettings["rootPath"].ToString());
            else
            {
                if (path.LastIndexOf("/") == path.Length - 1)
                {
                    path = path.Remove(path.LastIndexOf("/"), (path.Length - path.LastIndexOf("/")));
                }
                try
                {//path = path.Remove(path.LastIndexOf("/"), (path.Length - path.LastIndexOf("/")));
                    return (path + "/");
                }
                catch
                {
                    return (ConfigurationManager.AppSettings["rootPath"]);	// default to root;
                }
            }

        }


        /// <summary>
        /// 错误报告
        /// </summary>
        /// <param name="problem">问题</param>
        /// <param name="tech">技术</param>
        /// <param name="suggestion">建议</param>
        public static void ReportError(string problem, string tech, string suggestion)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string output = "<font color=red><BIG>问题:</BIG> " + problem + "</font><hr>";
            output += "建议: " + suggestion + "<hr>";
            output += "<small>Technical details: " + tech + "</small><hr>";
            context.Response.Write(output);
        }

    }
}
