/**********************************************
 * �����ã�   �����������
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
    public class Request
    {
        /// <summary>
        /// �жϵ�ǰҳ���Ƿ���յ���Post����
        /// </summary>
        /// <returns>�Ƿ���յ���Post����</returns>
        public static bool IsPost()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("POST");
        }
        /// <summary>
        /// �жϵ�ǰҳ���Ƿ���յ���Get����
        /// </summary>
        /// <returns>�Ƿ���յ���Get����</returns>
        public static bool IsGet()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("GET");
        }

        /// <summary>
        /// ����ָ���ķ�����������Ϣ
        /// </summary>
        /// <param name="strName">������������</param>
        /// <returns>������������Ϣ</returns>
        public static string GetServerString(string strName)
        {
            //
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
            {
                return "";
            }
            return HttpContext.Current.Request.ServerVariables[strName].ToString();
        }

        /// <summary>
        /// ������һ��ҳ��ĵ�ַ
        /// </summary>
        /// <returns>��һ��ҳ��ĵ�ַ</returns>
        public static string GetUrlReferrer()
        {
            string retVal = null;

            try
            {
                retVal = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            catch { }

            if (retVal == null)
                return "";

            return retVal;

        }

        /// <summary>
        /// �õ���ǰ��������ͷ
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
            {
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());
            }
            return request.Url.Host;
        }

        /// <summary>
        /// �õ�����ͷ
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }


        /// <summary>
        /// ��ȡ��ǰ�����ԭʼ URL(URL ������Ϣ֮��Ĳ���,������ѯ�ַ���(�������))
        /// </summary>
        /// <returns>ԭʼ URL</returns>
        public static string GetRawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }

        /// <summary>
        /// �жϵ�ǰ�����Ƿ�������������
        /// </summary>
        /// <returns>��ǰ�����Ƿ�������������</returns>
        public static bool IsBrowserGet()
        {
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            string curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (curBrowser.IndexOf(BrowserName[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ���ص�ǰҳ���Ƿ��ǿ�վ�ύ
        /// </summary>
        /// <returns>��ǰҳ���Ƿ��ǿ�վ�ύ</returns>
        public static bool IsCrossSitePost()
        {

            // ��������ύ��Ϊtrue
            if (!Request.IsPost())
            {
                return true;
            }
            return IsCrossSitePost(Request.GetUrlReferrer(), Request.GetHost());
        }

        /// <summary>
        /// �ж��Ƿ��ǿ�վ�ύ
        /// </summary>
        /// <param name="urlReferrer">�ϸ�ҳ���ַ</param>
        /// <param name="host">��̳url</param>
        /// <returns></returns>
        public static bool IsCrossSitePost(string urlReferrer, string host)
        {
            if (urlReferrer.Length < 7)
            {
                return true;
            }
            Uri u = new Uri(urlReferrer);
            return u.Host != host;
        }

        /// <summary>
        /// �ж��Ƿ�����������������
        /// </summary>
        /// <returns>�Ƿ�����������������</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpContext.Current.Request.UrlReferrer == null)
            {
                return false;
            }
            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
            string tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
            for (int i = 0; i < SearchEngine.Length; i++)
            {
                if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ��õ�ǰ����Url��ַ
        /// </summary>
        /// <returns>��ǰ����Url��ַ</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }


        /// <summary>
        /// ���ָ��Url������ֵ
        /// </summary>
        /// <param name="strName">Url����</param>
        /// <returns>Url������ֵ</returns>
        public static string GetQueryString(string strName)
        {

            if (HttpContext.Current.Request.QueryString[strName] == null)
            {
                return String.Empty;
            }
            return HttpContext.Current.Request.QueryString[strName];

        }
        /// <summary>
        /// ��õ�ǰҳ�������
        /// </summary>
        /// <returns>��ǰҳ�������</returns>
        public static string GetPageName()
        {
            string[] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
            return urlArr[urlArr.Length - 1].ToLower();
        }

        /// <summary>
        /// ���ر���Url�������ܸ���
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount()
        {
            return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
        }


        /// <summary>
        /// ���ָ����������ֵ
        /// </summary>
        /// <param name="strName">������</param>
        /// <returns>��������ֵ</returns>
        public static string GetFormString(string strName)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
            {
                return "";
            }
            return HttpContext.Current.Request.Form[strName];
        }

        /// <summary>
        /// ���Url���������ֵ, ���ж�Url�����Ƿ�Ϊ���ַ���, ��ΪTrue�򷵻ر�������ֵ
        /// </summary>
        /// <param name="strName">����</param>
        /// <returns>Url���������ֵ</returns>
        public static string GetString(string strName)
        {
            if ("".Equals(GetQueryString(strName)))
            {
                return GetFormString(strName);
            }
            else
            {
                return GetQueryString(strName);
            }
        }


        /// <summary>
        /// ���ָ��Url������int����ֵ
        /// </summary>
        /// <param name="strName">Url����</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>Url������int����ֵ</returns>
        public static int GetQueryInt(string strName, int defValue)
        {
            return TypeParse.StrToInt(HttpContext.Current.Request.QueryString[strName], defValue);
        }


        /// <summary>
        /// ���ָ����������int����ֵ
        /// </summary>
        /// <param name="strName">������</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>��������int����ֵ</returns>
        public static int GetFormInt(string strName, int defValue)
        {
            return TypeParse.StrToInt(HttpContext.Current.Request.Form[strName], defValue);
        }

        /// <summary>
        /// ���ָ��Url���������int����ֵ, ���ж�Url�����Ƿ�Ϊȱʡֵ, ��ΪTrue�򷵻ر�������ֵ
        /// </summary>
        /// <param name="strName">Url�������</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>Url���������int����ֵ</returns>
        public static int GetInt(string strName, int defValue)
        {
            if (GetQueryInt(strName, defValue) == defValue)
            {
                return GetFormInt(strName, defValue);
            }
            else
            {
                return GetQueryInt(strName, defValue);
            }
        }

        /// <summary>
        /// ���ָ��Url������float����ֵ
        /// </summary>
        /// <param name="strName">Url����</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>Url������int����ֵ</returns>
        public static float GetQueryFloat(string strName, float defValue)
        {
            return TypeParse.StrToFloat(HttpContext.Current.Request.QueryString[strName], defValue);
        }


        /// <summary>
        /// ���ָ����������float����ֵ
        /// </summary>
        /// <param name="strName">������</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>��������float����ֵ</returns>
        public static float GetFormFloat(string strName, float defValue)
        {
            return TypeParse.StrToFloat(HttpContext.Current.Request.Form[strName], defValue);
        }

        /// <summary>
        /// ���ָ��Url���������float����ֵ, ���ж�Url�����Ƿ�Ϊȱʡֵ, ��ΪTrue�򷵻ر�������ֵ
        /// </summary>
        /// <param name="strName">Url�������</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>Url���������int����ֵ</returns>
        public static float GetFloat(string strName, float defValue)
        {
            if (GetQueryFloat(strName, defValue) == defValue)
            {
                return GetFormFloat(strName, defValue);
            }
            else
            {
                return GetQueryFloat(strName, defValue);
            }
        }

        /// <summary>
        /// ��õ�ǰҳ��ͻ��˵�IP
        /// </summary>
        /// <returns>��ǰҳ��ͻ��˵�IP</returns>
        public static string GetIP()
        {


            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty || !ValidateUtils.IsIP(result))
            {
                return "0.0.0.0";
            }

            return result;

        }


        /// <summary>
        /// �ж��Ƿ����ϴ����ļ�
        /// </summary>
        /// <returns>�Ƿ����ϴ����ļ�</returns>
        public static bool IsPostFile()
        {
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                if (HttpContext.Current.Request.Files[i].FileName != "")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// �����û��ϴ����ļ�
        /// </summary>
        /// <param name="path">����·��</param>
        public static void SaveRequestFile(string path)
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpContext.Current.Request.Files[0].SaveAs(path);
            }
        }

        /// <summary>
        /// �����ϴ����ļ�
        /// </summary>
        /// <param name="MaxAllowFileCount">���������ϴ��ļ�����</param>
        /// <param name="MaxAllowFileSize">���������ļ�����(��λ: KB)</param>
        /// <param name="AllowFileExtName">������ļ���չ��, ��string[]��ʽ�ṩ</param>
        /// <param name="AllowFileType">������ļ�����, ��string[]��ʽ�ṩ</param>
        /// <param name="Dir">Ŀ¼</param>
        /// <returns></returns>
        public static string[] SaveRequestFiles(int MaxAllowFileCount, int MaxAllowFileSize, string[] AllowFileExtName, string[] AllowFileType, string Dir)
        {
            string[] attachmentinfo = new string[MaxAllowFileCount];
            int fcount = Math.Min(MaxAllowFileCount, HttpContext.Current.Request.Files.Count);
            for (int i = 0; i < fcount; i++)
            {
                string filename = HttpContext.Current.Request.Files[i].FileName;
                string fileextname = filename.Substring(filename.LastIndexOf("."));
                string filetype = HttpContext.Current.Request.Files[i].ContentType;
                int filesize = HttpContext.Current.Request.Files[i].ContentLength;
                // �ж� �ļ���չ��/�ļ���С/�ļ����� �Ƿ����Ҫ��
                if (ValidateUtils.InArray(fileextname, AllowFileExtName) && (filesize <= MaxAllowFileSize * 1024) && ValidateUtils.InArray(filetype, AllowFileType))
                {
                    HttpContext.Current.Request.Files[i].SaveAs(Dir + DateUtil.GetDateTime() + Environment.TickCount.ToString() + fileextname);
                    attachmentinfo[i] = Dir + DateUtil.GetDateTime() + Environment.TickCount.ToString() + fileextname;
                }
            }
            return attachmentinfo;

        }



    }
}
