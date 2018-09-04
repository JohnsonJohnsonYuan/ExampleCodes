/**********************************************
 * �����ã�   �ͻ��˴��빦����
 * �����ˣ�   abaal
 * ����ʱ�䣺 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Svnhost.Common
{
    /// <summary>
    /// �ṩ��ҳ������ͻ��˴���ʵ�����⹦�ܵķ���
    /// </summary>
    /// <remarks>
    /// </remarks>

    public class JScript
    {
        public static void AlertAndRedirect(string message, string toURL)
        {
            string js = "<script language=javascript>alert('{0}');window.location.replace('{1}')</script>";
            HttpContext.Current.Response.Write(string.Format(js, message, toURL));
        }
        /// <summary>
        /// ��ͻ��˷��ͺ���KendoPostBack(eventTarget, eventArgument)
        /// �������˿ɽ���__EVENTTARGET,__EVENTARGUMENT��ֵ
        /// </summary>
        /// <param name="page">System.Web.UI.Page һ��Ϊthis</param>
        public static void JscriptSender(System.Web.UI.Page page)
        {

            page.RegisterHiddenField("__EVENTTARGET", "");
            page.RegisterHiddenField("__EVENTARGUMENT", "");
            string s = @"		
<script language=Javascript>
      function KendoPostBack(eventTarget, eventArgument) 
      {
				var theform = document.forms[0];
				theform.__EVENTTARGET.value = eventTarget;
				theform.__EVENTARGUMENT.value = eventArgument;
				theform.submit();
			}
</script>";

            page.RegisterStartupScript("sds", s);
        }
        /// <summary>
        /// ����JavaScriptС����
        /// </summary>
        /// <param name="js">������Ϣ</param>
        public static void Alert(string message)
        {
            message = StringUtil.DeleteUnVisibleChar(message);
            string js = @"<Script language='JavaScript'>
                    alert('" + message + "');</Script>";
            HttpContext.Current.Response.Write(js);
        }
        public static void Alert(object message)
        {
            string js = @"<Script language='JavaScript'>
                    alert('{0}');  
                  </Script>";
            HttpContext.Current.Response.Write(string.Format(js, message.ToString()));
        }

        public static void RtnRltMsgbox(object message, string strWinCtrl)
        {
            string js = @"<Script language='JavaScript'>
					 strWinCtrl = true;
                     strWinCtrl = if(!confirm('" + message + "'))return false;</Script>";
            HttpContext.Current.Response.Write(string.Format(js, message.ToString()));
        }

        /// <summary>
        /// �ص���ʷҳ��
        /// </summary>
        /// <param name="value">-1/1</param>
        public static void GoHistory(int value)
        {
            string js = @"<Script language='JavaScript'>
                    history.go({0});  
                  </Script>";
            HttpContext.Current.Response.Write(string.Format(js, value));
        }

        /// <summary>
        /// �رյ�ǰ����
        /// </summary>
        public static void CloseWindow()
        {
            string js = @"<Script language='JavaScript'>
                    window.close();  
                  </Script>";
            HttpContext.Current.Response.Write(js);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// ˢ�¸�����
        /// </summary>
        public static void RefreshParent()
        {
            string js = @"<Script language='JavaScript'>
                    parent.location.reload();
                  </Script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>
        /// ��ʽ��ΪJS�ɽ��͵��ַ���
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string JSStringFormat(string s)
        {
            return s.Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\\"");
        }

        /// <summary>
        /// ˢ�´򿪴���
        /// </summary>
        public static void RefreshOpener()
        {
            string js = @"<Script language='JavaScript'>
                    opener.location.reload();
                  </Script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>
        /// �򿪴���
        /// </summary>
        /// <param name="url"></param>
        public static void OpenWebForm(string url)
        {

            /*��������������������������������������������������������������������*/
            /*�޸���Ա:		sxs						*/
            /*�޸�ʱ��:	2003-4-9	*/
            /*�޸�Ŀ��:	�¿�ҳ��ȥ��ie�Ĳ˵�������						*/
            /*ע������:								*/
            /*��ʼ*/
            string js = @"<Script language='JavaScript'>
			//window.open('" + url + @"');
			window.open('" + url + @"','','height=0,width=0,top=0,left=0,location=no,menubar=no,resizable=yes,scrollbars=yes,status=yes,titlebar=no,toolbar=no,directories=no');
			</Script>";
            /*����*/
            /*��������������������������������������������������������������������*/


            HttpContext.Current.Response.Write(js);
        }
        public static void OpenWebForm(string url, string name, string future)
        {
            string js = @"<Script language='JavaScript'>
                     window.open('" + url + @"','" + name + @"','" + future + @"')
                  </Script>";
            HttpContext.Current.Response.Write(js);
        }
        public static void OpenWebForm(string url, string formName)
        {
            /*��������������������������������������������������������������������*/
            /*�޸���Ա:		sxs						*/
            /*�޸�ʱ��:	2003-4-9	*/
            /*�޸�Ŀ��:	�¿�ҳ��ȥ��ie�Ĳ˵�������						*/
            /*ע������:								*/
            /*��ʼ*/
            string js = @"<Script language='JavaScript'>
			//window.open('" + url + @"','" + formName + @"');
			window.open('" + url + @"','" + formName + @"','height=0,width=0,top=0,left=0,location=no,menubar=no,resizable=yes,scrollbars=yes,status=yes,titlebar=no,toolbar=no,directories=no');
			</Script>";
            /*����*/
            /*��������������������������������������������������������������������*/

            HttpContext.Current.Response.Write(js);
        }

        /// <summary>		
        /// ������:OpenWebForm	
        /// ��������:��WEB����	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-04-29 17:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="url">WEB����</param>
        /// <param name="isFullScreen">�Ƿ�ȫ��Ļ</param>
        public static void OpenWebForm(string url, bool isFullScreen)
        {
            string js = @"<Script language='JavaScript'>";
            if (isFullScreen)
            {
                js += "var iWidth = 0;";
                js += "var iHeight = 0;";
                js += "iWidth=window.screen.availWidth-10;";
                js += "iHeight=window.screen.availHeight-50;";
                js += "var szFeatures ='width=' + iWidth + ',height=' + iHeight + ',top=0,left=0,location=no,menubar=no,resizable=yes,scrollbars=yes,status=yes,titlebar=no,toolbar=no,directories=no';";
                js += "window.open('" + url + @"','',szFeatures);";
            }
            else
            {
                js += "window.open('" + url + @"','','height=0,width=0,top=0,left=0,location=no,menubar=no,resizable=yes,scrollbars=yes,status=yes,titlebar=no,toolbar=no,directories=no');";
            }
            js += "</Script>";
            HttpContext.Current.Response.Write(js);
        }
        /// <summary>
        /// ת��Url�ƶ���ҳ��
        /// </summary>
        /// <param name="url"></param>
        public static void JavaScriptLocationHref(string url)
        {
            string js = @"<Script language='JavaScript'>
                    window.location.replace('{0}');
                  </Script>";
            js = string.Format(js, url);
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>
        /// ָ���Ŀ��ҳ��ת��
        /// </summary>
        /// <param name="FrameName"></param>
        /// <param name="url"></param>
        public static void JavaScriptFrameHref(string FrameName, string url)
        {
            string js = @"<Script language='JavaScript'>
					
                    @obj.location.replace(""{0}"");
                  </Script>";
            js = js.Replace("@obj", FrameName);
            js = string.Format(js, url);
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>
        ///����ҳ��
        /// </summary>
        public static void JavaScriptResetPage(string strRows)
        {
            string js = @"<Script language='JavaScript'>
                    window.parent.CenterFrame.rows='" + strRows + "';</Script>";
            HttpContext.Current.Response.Write(js);
        }
        /// <summary>
        /// ������:JavaScriptSetCookie
        /// ��������:�ͻ��˷�������Cookie
        /// ����:sxs
        /// ���ڣ�2003-4-9
        /// �汾��1.0
        /// </summary>
        /// <param name="strName">Cookie��</param>
        /// <param name="strValue">Cookieֵ</param>
        public static void JavaScriptSetCookie(string strName, string strValue)
        {
            string js = @"<script language=Javascript>
			var the_cookie = '" + strName + "=" + strValue + @"'
			var dateexpire = 'Tuesday, 01-Dec-2020 12:00:00 GMT';
			//document.cookie = the_cookie;//д��Cookie<BR>} <BR>
			document.cookie = the_cookie + '; expires='+dateexpire;			
			</script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>		
        /// ������:GotoParentWindow	
        /// ��������:���ظ�����	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-04-30 10:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="parentWindowUrl">������</param>		
        public static void GotoParentWindow(string parentWindowUrl)
        {
            string js = @"<Script language='JavaScript'>
                    this.parent.location.replace('" + parentWindowUrl + "');</Script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>		
        /// ������:ReplaceParentWindow	
        /// ��������:�滻������	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-04-30 10:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="parentWindowUrl">������</param>
        /// <param name="caption">������ʾ</param>
        /// <param name="future">������������</param>
        public static void ReplaceParentWindow(string parentWindowUrl, string caption, string future)
        {
            string js = "";
            if (future != null && future.Trim() != "")
            {
                js = @"<script language=javascript>this.parent.location.replace('" + parentWindowUrl + "','" + caption + "','" + future + "');</script>";
            }
            else
            {
                js = @"<script language=javascript>var iWidth = 0 ;var iHeight = 0 ;iWidth=window.screen.availWidth-10;iHeight=window.screen.availHeight-50;
							var szFeatures = 'dialogWidth:'+iWidth+';dialogHeight:'+iHeight+';dialogLeft:0px;dialogTop:0px;center:yes;help=no;resizable:on;status:on;scroll=yes';this.parent.location.replace('" + parentWindowUrl + "','" + caption + "',szFeatures);</script>";
            }

            HttpContext.Current.Response.Write(js);
        }


        /// <summary>		
        /// ������:ReplaceOpenerWindow	
        /// ��������:�滻��ǰ����Ĵ򿪴���	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-04-30 16:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="openerWindowUrl">��ǰ����Ĵ򿪴���</param>		
        public static void ReplaceOpenerWindow(string openerWindowUrl)
        {
            string js = @"<Script language='JavaScript'>
                    window.opener.location.replace('" + openerWindowUrl + "');</Script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>		
        /// ������:ReplaceOpenerParentWindow	
        /// ��������:�滻��ǰ����Ĵ򿪴��ڵĸ�����	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-07-03 19:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="openerWindowUrl">��ǰ����Ĵ򿪴��ڵĸ�����</param>		
        public static void ReplaceOpenerParentFrame(string frameName, string frameWindowUrl)
        {
            string js = @"<Script language='JavaScript'>
                    window.opener.parent." + frameName + ".location.replace('" + frameWindowUrl + "');</Script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>		
        /// ������:ReplaceOpenerParentWindow	
        /// ��������:�滻��ǰ����Ĵ򿪴��ڵĸ�����	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-07-03 19:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="openerWindowUrl">��ǰ����Ĵ򿪴��ڵĸ�����</param>		
        public static void ReplaceOpenerParentWindow(string openerParentWindowUrl)
        {
            string js = @"<Script language='JavaScript'>
                    window.opener.parent.location.replace('" + openerParentWindowUrl + "');</Script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>		
        /// ������:CloseParentWindow	
        /// ��������:�رմ���	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-04-30 16:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        public static void CloseParentWindow()
        {
            string js = @"<Script language='JavaScript'>
                    window.parent.close();  
                  </Script>";
            HttpContext.Current.Response.Write(js);
        }

        public static void CloseOpenerWindow()
        {
            string js = @"<Script language='JavaScript'>
                    window.opener.close();  
                  </Script>";
            HttpContext.Current.Response.Write(js);
        }

        /// <summary>
        /// ������:ShowModalDialogJavascript	
        /// ��������:���ش�ģʽ���ڵĽű�	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-04-30 15:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="webFormUrl"></param>
        /// <returns></returns>
        public static string ShowModalDialogJavascript(string webFormUrl)
        {
            string js = @"<script language=javascript>
							var iWidth = 0 ;
							var iHeight = 0 ;
							iWidth=window.screen.availWidth-10;
							iHeight=window.screen.availHeight-50;
							var szFeatures = 'dialogWidth:'+iWidth+';dialogHeight:'+iHeight+';dialogLeft:0px;dialogTop:0px;center:yes;help=no;resizable:on;status:on;scroll=yes';
							showModalDialog('" + webFormUrl + "','',szFeatures);</script>";
            return js;
        }

        public static string ShowModalDialogJavascript(string webFormUrl, string features)
        {
            string js = @"<script language=javascript>							
							showModalDialog('" + webFormUrl + "','','" + features + "');</script>";
            return js;
        }

        /// <summary>
        /// ������:ShowModalDialogWindow	
        /// ��������:��ģʽ����	
        /// ��������:
        /// �㷨����:
        /// �� ��: ����
        /// �� ��: 2003-04-30 15:00
        /// �� ��:
        /// �� ��:
        /// �� ��:
        /// </summary>
        /// <param name="webFormUrl"></param>
        /// <returns></returns>
        public static void ShowModalDialogWindow(string webFormUrl)
        {
            string js = ShowModalDialogJavascript(webFormUrl);
            HttpContext.Current.Response.Write(js);
        }

        public static void ShowModalDialogWindow(string webFormUrl, string features)
        {
            string js = ShowModalDialogJavascript(webFormUrl, features);
            HttpContext.Current.Response.Write(js);
        }
        public static void ShowModalDialogWindow(string webFormUrl, int width, int height, int top, int left)
        {
            string features = "dialogWidth:" + width.ToString() + "px"
                + ";dialogHeight:" + height.ToString() + "px"
                + ";dialogLeft:" + left.ToString() + "px"
                + ";dialogTop:" + top.ToString() + "px"
                + ";center:yes;help=no;resizable:no;status:no;scroll=no";
            ShowModalDialogWindow(webFormUrl, features);
        }

        public static void SetHtmlElementValue(string formName, string elementName, string elementValue)
        {
            string js = @"<Script language='JavaScript'>if(document." + formName + "." + elementName + "!=null){document." + formName + "." + elementName + ".value =" + elementValue + ";}</Script>";
            HttpContext.Current.Response.Write(js);
        }

    }
}
