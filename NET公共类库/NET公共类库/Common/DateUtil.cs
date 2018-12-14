/**********************************************
 * �����ã�   ���ڸ�ʽ������
 * �����ˣ�   abaal
 * ����ʱ�䣺 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Svnhost.Common
{
    public class DateUtil
    {

        /// <summary>
        /// ���ر�׼���ڸ�ʽstring
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// ����ָ�����ڸ�ʽ
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
            {
                return replacestr;
            }

            if (datetimestr.Equals(""))
            {
                return replacestr;
            }

            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;

        }


        /// <summary>
        /// ���ر�׼ʱ���ʽstring
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// ���ر�׼ʱ���ʽstring
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// ��������ڵ�ǰʱ����������
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// ���ر�׼ʱ���ʽstring
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// ���ر�׼ʱ�� 
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
            {

                return fDateTime;
            }
            DateTime s = Convert.ToDateTime(fDateTime);
            return s.ToString(formatStr);
        }

        /// <summary>
        /// ���ر�׼ʱ�� yyyy-MM-dd HH:mm:ss
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }
        
        public static string AdDeTime(int times)
        {
            string newtime = (DateTime.Now).AddMinutes(times).ToString();
            return newtime;

        }


        /// <summary>���ر����ж�����</summary>
        /// <param name="iYear">���</param>
        /// <returns>���������</returns>
        public static int GetDaysOfYear(int iYear)
        {
            int cnt = 0;
            if (IsRuYear(iYear))
            {
                //����� 1 �� ����2 ��Ϊ 29 ��
                cnt = 366;

            }
            else
            {
                //--��������1�� ����2 ��Ϊ 28 ��
                cnt = 365;
            }
            return cnt;
        }


        /// <summary>�����ж�����</summary>
        /// <param name="dt">����</param>
        /// <returns>�����ڵ��������</returns>
        public static int GetDaysOfYear(DateTime idt)
        {
            int n;

            //ȡ�ô����������ݲ��֣������ж��Ƿ�������

            n = idt.Year;
            if (IsRuYear(n))
            {
                //����� 1 �� ����2 ��Ϊ 29 ��
                return 366;
            }
            else
            {
                //--��������1�� ����2 ��Ϊ 28 ��
                return 365;
            }

        }


        /// <summary>�����ж�����</summary>
        /// <param name="iYear">��</param>
        /// <param name="Month">��</param>
        /// <returns>����</returns>
        public static int GetDaysOfMonth(int iYear, int Month)
        {
            int days = 0;
            switch (Month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    if (IsRuYear(iYear))
                    {
                        //����� 1 �� ����2 ��Ϊ 29 ��
                        days = 29;
                    }
                    else
                    {
                        //--��������1�� ����2 ��Ϊ 28 ��
                        days = 28;
                    }

                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }

            return days;


        }


        /// <summary>�����ж�����</summary>
        /// <param name="dt">����</param>
        /// <returns>����</returns>
        public static int GetDaysOfMonth(DateTime dt)
        {
            //--------------------------------//
            //--��dt��ȡ�õ�ǰ���꣬����Ϣ  --//
            //--------------------------------//
            int year, month, days = 0;
            year = dt.Year;
            month = dt.Month;

            //--����������Ϣ���õ���ǰ�µ�������Ϣ��
            switch (month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    if (IsRuYear(year))
                    {
                        //����� 1 �� ����2 ��Ϊ 29 ��
                        days = 29;
                    }
                    else
                    {
                        //--��������1�� ����2 ��Ϊ 28 ��
                        days = 28;
                    }

                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }

            return days;

        }


        /// <summary>���ص�ǰ���ڵ���������</summary>
        /// <param name="dt">����</param>
        /// <returns>��������</returns>
        public static string GetWeekNameOfDay(DateTime idt)
        {
            string dt, week = "";

            dt = idt.DayOfWeek.ToString();
            switch (dt)
            {
                case "Mondy":
                    week = "����һ";
                    break;
                case "Tuesday":
                    week = "���ڶ�";
                    break;
                case "Wednesday":
                    week = "������";
                    break;
                case "Thursday":
                    week = "������";
                    break;
                case "Friday":
                    week = "������";
                    break;
                case "Saturday":
                    week = "������";
                    break;
                case "Sunday":
                    week = "������";
                    break;

            }
            return week;
        }


        /// <summary>���ص�ǰ���ڵ����ڱ��</summary>
        /// <param name="dt">����</param>
        /// <returns>�������ֱ��</returns>
        public static string GetWeekNumberOfDay(DateTime idt)
        {
            string dt, week = "";

            dt = idt.DayOfWeek.ToString();
            switch (dt)
            {
                case "Mondy":
                    week = "1";
                    break;
                case "Tuesday":
                    week = "2";
                    break;
                case "Wednesday":
                    week = "3";
                    break;
                case "Thursday":
                    week = "4";
                    break;
                case "Friday":
                    week = "5";
                    break;
                case "Saturday":
                    week = "6";
                    break;
                case "Sunday":
                    week = "7";
                    break;

            }

            return week;


        }


        /// <summary>������������֮����������</summary>
        /// <param name="dt">�������ڲ���</param>
        /// <returns>����</returns>
        public static int DiffDays(DateTime dtfrm, DateTime dtto)
        {
            int diffcnt = 0;
            //diffcnt = dtto- dtfrm ;

            return diffcnt;
        }


        /// <summary>�жϵ�ǰ��������������Ƿ������꣬˽�к���</summary>
        /// <param name="dt">����</param>
        /// <returns>�����꣺True ���������꣺False</returns>
        private static bool IsRuYear(DateTime idt)
        {
            //��ʽ����Ϊ�������� 
            //���磺2003-12-12
            int n;
            n = idt.Year;

            if ((n % 400 == 0) || (n % 4 == 0 && n % 100 != 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>�жϵ�ǰ����Ƿ������꣬˽�к���</summary>
        /// <param name="dt">���</param>
        /// <returns>�����꣺True ���������꣺False</returns>
        private static bool IsRuYear(int iYear)
        {
            //��ʽ����Ϊ���
            //���磺2003
            int n;
            n = iYear;

            if ((n % 400 == 0) || (n % 4 == 0 && n % 100 != 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// ��������ַ���ת��Ϊ���ڡ�����ַ����ĸ�ʽ�Ƿ����򷵻ص�ǰ���ڡ�
        /// </summary>
        /// <param name="strInput">�����ַ���</param>
        /// <returns>���ڶ���</returns>
        public static DateTime ConvertStringToDate(string strInput)
        {
            DateTime oDateTime;

            try
            {
                oDateTime = DateTime.Parse(strInput);
            }
            catch (Exception)
            {
                oDateTime = DateTime.Today;
            }

            return oDateTime;
        }


        /// <summary>
        /// �����ڶ���ת��Ϊ��ʽ�ַ���
        /// </summary>
        /// <param name="oDateTime">���ڶ���</param>
        /// <param name="strFormat">
        /// ��ʽ��
        ///		"SHORTDATE"===������
        ///		"LONGDATE"==������
        ///		����====�Զ����ʽ
        /// </param>
        /// <returns>�����ַ���</returns>
        public static string ConvertDateToString(DateTime oDateTime, string strFormat)
        {
            string strDate = "";

            try
            {
                switch (strFormat.ToUpper())
                {
                    case "SHORTDATE":
                        strDate = oDateTime.ToShortDateString();
                        break;
                    case "LONGDATE":
                        strDate = oDateTime.ToLongDateString();
                        break;
                    default:
                        strDate = oDateTime.ToString(strFormat);
                        break;
                }
            }
            catch (Exception)
            {
                strDate = oDateTime.ToShortDateString();
            }

            return strDate;
        }



        /// <summary>
        /// �ж��Ƿ�Ϊ�Ϸ����ڣ��������1800��1��1��
        /// </summary>
        /// <param name="strDate">���������ַ���</param>
        /// <returns>True/False</returns>
        public static bool IsDateTime(string strDate)
        {
            try
            {
                DateTime oDate = DateTime.Parse(strDate);
                if (oDate.CompareTo(DateTime.Parse("1800-1-1")) > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
