using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Framework.Common
{
    /// <summary>
    /// 显示消息提示对话框。
    /// 李天平
    /// 2005.10.1
    /// </summary>
    public class MessageBox
    {
        private MessageBox()
        {
        }

        /// <summary>
        /// 显示消息提示对话框
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        public static void Show(System.Web.UI.Page page, string msg)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script language='javascript' defer>alert('" + msg.ToString() + "');</script>");
        }

        /// <summary>
        /// 控件点击 消息确认提示框
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        public static void ShowConfirm(System.Web.UI.WebControls.WebControl Control, string msg)
        {
            //Control.Attributes.Add("onClick","if (!window.confirm('"+msg+"')){return false;}");
            Control.Attributes.Add("onclick", "return confirm('" + msg + "');");
        }

        /// <summary>
        /// 显示消息提示对话框，并进行页面跳转(跳出框架)
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="msg">提示信息</param>
        /// <param name="url">跳转的目标URL</param>
        public static void ShowAndRedirect(System.Web.UI.Page page, string msg, string url)
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<script language='javascript' defer>");
            Builder.AppendFormat("alert('{0}');", msg);
            Builder.AppendFormat("top.location.href='{0}'", url);
            Builder.Append("</script>");
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", Builder.ToString());

        }
        /// <summary>
        /// 显示消息提示框，并进行页面跳转（框架内）
        /// </summary>
        /// <param name="page">当前页面指针，一般为this</param>
        /// <param name="values">提示信息</param>
        /// <param name="url">跳转的目标URL</param>
        public static void MessageBoxurl(System.Web.UI.Page page, string values, string url)
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<script language='javascript' defer>");
            Builder.AppendFormat("alert('{0}');", values);
            Builder.AppendFormat("window.location.replace('{0}')", url);
            Builder.Append("</script>");
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", Builder.ToString());
        }
        

    }
}
