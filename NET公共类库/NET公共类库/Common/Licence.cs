/**********************************************
 * �����ã�   ��Ȩ��
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
using System.Configuration;

namespace Svnhost.Common
{
    /// <summary>
    /// ȷ���Ƿ���Ȩ
    /// </summary>
    public sealed class Licence
    {
        //private static string key = "92f6766f-4b26-40ef-b27c-0b93057d4377";
        public static bool IsLicence(string key)
        {
            string host = HttpContext.Current.Request.Url.Host.ToLower();
            if (host.Equals("localhost"))
                return true;

            string Licence = ConfigurationManager.AppSettings["licence"];
            if (Licence != null && Licence == StringUtil.md5(host + key, 16))
                return true;

            return false;
        }

        /// <summary>
        /// ��ȡ���֤��
        /// </summary>
        /// <param name="host"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLicence(string host, string key)
        {
            return StringUtil.md5(host + key, 16);
        }
    }
}
