/**********************************************
 * �����ã�   ��֤ʵ����
 * �����ˣ�   abaal
 * ����ʱ�䣺 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Svnhost.Common
{
    public class ValidateUtils
    {
        #region ����������֤


        /// <summary>
        /// �ж��Ƿ�Ϊbase64�ַ���
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }

        /// <summary>
        /// ��֤�Ƿ�Ϊ������
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]*$");
        }

        /// <summary>
        /// �ж϶����Ƿ�ΪInt32���͵�����
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object Expression)
        {
            if (Expression != null)
            {
                string str = Expression.ToString();
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                    {
                        return true;
                    }
                }
            }
            return false;

        }


        public static bool IsDouble(object Expression)
        {
            if (Expression != null)
            {
                return Regex.IsMatch(Expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");
            }
            return false;
        }



        /// <summary>
        /// �жϸ������ַ�������(strNumber)�е������ǲ��Ƕ�Ϊ��ֵ��
        /// </summary>
        /// <param name="strNumber">Ҫȷ�ϵ��ַ�������</param>
        /// <returns>���򷵼�true �����򷵻� false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
            {
                return false;
            }
            if (strNumber.Length < 1)
            {
                return false;
            }
            foreach (string id in strNumber)
            {
                if (!IsNumeric(id))
                {
                    return false;
                }
            }
            return true;

        }

        #endregion


        #region �ַ���������֤

        /// <summary>
        /// �Ƿ�����ƶ����ַ���
        /// </summary>
        /// <param name="str"></param>
        /// <param name="stringarray"></param>
        /// <param name="strsplit"></param>
        /// <returns></returns>
        public static bool IsCompriseStr(string str, string stringarray, string strsplit)
        {
            if (stringarray == "" || stringarray == null)
            {
                return false;
            }

            str = str.ToLower();
            string[] stringArray = StringUtil.SplitString(stringarray.ToLower(), strsplit);
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (str.IndexOf(stringArray[i]) > -1)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return StringUtil.GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ַ�������</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, StringUtil.SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <param name="strsplit">�ָ��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, StringUtil.SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <param name="strsplit">�ָ��ַ���</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�жϽ��</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, StringUtil.SplitString(stringarray, strsplit), caseInsensetive);
        }



        /// <summary>�Ƿ��</summary>
        /// <param name="strInput">�����ַ���</param>
        /// <returns>true/false</returns>
        public static bool isBlank(string strInput)
        {
            if (strInput == null || strInput.Trim() == "")
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        #endregion

        #region IP��ַ SQLע��Ȱ�ȫ��֤


        /// <summary>
        /// �Ƿ�Ϊip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        }


        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");

        }



        /// <summary>
        /// ����Ƿ���SqlΣ���ַ�
        /// </summary>
        /// <param name="str">Ҫ�ж��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// ����Ƿ���Σ�յĿ����������ӵ��ַ���
        /// </summary>
        /// <param name="str">Ҫ�ж��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|�ο�|^Guest");
        }

        /// <summary>
        /// ����ָ��IP�Ƿ���ָ����IP�������޶��ķ�Χ��, IP�����ڵ�IP��ַ����ʹ��*��ʾ��IP������, ����192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {

            string[] userip = StringUtil.SplitString(ip, @".");
            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = StringUtil.SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                    {
                        return true;
                    }

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                if (r == 4)
                {
                    return true;
                }


            }
            return false;

        }

        #endregion


        #region ����������֤

        /// <summary>
        /// �ж��ļ����Ƿ�ΪUTF8�ַ���
        /// </summary>
        /// <param name="sbInputStream">�ļ���</param>
        /// <returns>�жϽ��</returns>
        private static bool IsUTF8(FileStream sbInputStream)
        {
            int i;
            byte cOctets;  // octets to go in this UTF-8 encoded character 
            byte chr;
            bool bAllAscii = true;
            long iLen = sbInputStream.Length;

            cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                chr = (byte)sbInputStream.ReadByte();

                if ((chr & 0x80) != 0) bAllAscii = false;

                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        }
                        while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0) return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    cOctets--;
                }
            }

            if (cOctets > 0)
            {
                return false;
            }

            if (bAllAscii)
            {
                return false;
            }

            return true;

        }       
 

        #endregion


        #region �������͸�ʽ��֤
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// �ж��ַ����Ƿ���yy-mm-dd�ַ���
        /// </summary>
        /// <param name="str">���ж��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool IsDateString(string str)
        {
            return Regex.IsMatch(str, @"(\d{4})-(\d{1,2})-(\d{1,2})");
        }

        /// <summary>�Ƿ�����</summary>
        /// <param name="strInput">�����ַ���</param>
        /// <returns>true/false</returns>
        public static bool isDate(string strInput)
        {
            string datestr = strInput;
            string year, month, day;
            string[] c ={ "/", "-", "." };
            string cs = "";
            for (int i = 0; i < c.Length; i++)
            {
                if (datestr.IndexOf(c[i]) > 0)
                {
                    cs = c[i];
                    break;
                }

            };

            if (cs != "")
            {
                year = datestr.Substring(0, datestr.IndexOf(cs));
                if (year.Length != 4) { return false; };
                datestr = datestr.Substring(datestr.IndexOf(cs) + 1);

                month = datestr.Substring(0, datestr.IndexOf(cs));
                if ((month.Length != 2) || (Convert.ToInt16(month) > 12))
                { return false; };
                datestr = datestr.Substring(datestr.IndexOf(cs) + 1);

                day = datestr;
                if ((day.Length != 2) || (Convert.ToInt16(day) > 31)) { return false; };

                return checkDatePart(year, month, day);
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// ����������Ƿ�Ϸ�
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private static bool checkDatePart(string year, string month, string day)
        {
            int iyear = Convert.ToInt16(year);
            int imonth = Convert.ToInt16(month);
            int iday = Convert.ToInt16(day);
            if (iyear > 2099 || iyear < 1900) { return false; }
            if (imonth > 12 || imonth < 1) { return false; }
            if (iday > DateUtil.GetDaysOfMonth(iyear, imonth) || iday < 1) { return false; };
            return true;
        }

        #endregion
  
    }
}
