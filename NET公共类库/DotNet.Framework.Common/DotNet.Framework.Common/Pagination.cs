using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DotNet.Framework.Common
{
    public class Pagination
    {
        private static DBUtility.DbHelperSQLP sqlhelp = new Maticsoft.DBUtility.DbHelperSQLP(Maticsoft.DBUtility.PubConstant.GetConnectionString("ConnectionString"));
        public static string getPageParameterHidden(ArrayList al)
        {
            string str = "";
            for (int i = 0; i < al.Count; i++)
            {
                try
                {
                    string[] strArray = (string[])al[i];
                    string str3 = str;
                    str = str3 + "<input type='hidden' name='" + strArray[0] + "' value='" + strArray[1] + "'>";
                }
                catch
                {
                }
            }
            return str;
        }

        public static int PageCount(int ReCount, int PageSize)
        {
            return (((ReCount % PageSize) > 0) ? ((ReCount / PageSize) + 1) : (ReCount / PageSize));
        }

        public static string PageInfo(int IndexPage, int ReCount, int PageSize, string PageParameter, int Att)
        {
            return PageInfo(IndexPage, ReCount, PageSize, PageParameter, Att, true);
        }

        public static string PageInfo(int IndexPage, int ReCount, int PageSize, string PageParameter, int Att, bool showbutton)
        {
            string str = "";
            if (Att == Config.si_CN)
            {
                return PageInfo_CN(IndexPage, ReCount, PageSize, PageParameter, showbutton);
            }
            if (Att == Config.si_EN)
            {
                str = PageInfo_EN(IndexPage, ReCount, PageSize, PageParameter, showbutton);
            }
            return str;
        }

        public static string PageInfo_CN(int IndexPage, int ReCount, int PageSize, string PageParameter)
        {
            return PageInfo_CN(IndexPage, ReCount, PageSize, PageParameter, true);
        }

        public static string PageInfo_CN(int IndexPage, int ReCount, int PageSize, string PageParameter, bool showbutton)
        {
            string str = "";
            int num = PageCount(ReCount, PageSize);
            int num2 = 10;
            int num3 = 1;
            int num4 = 1;
            if ((num >= num2) && (IndexPage > 6))
            {
                num3 = IndexPage - 5;
                if ((num >= num2) && (num3 > (num - (num2 - 1))))
                {
                    num3 = num - (num2 - 1);
                }
            }
            num4 = num3 + (num2 - 1);
            if (num4 > num)
            {
                num4 = num;
            }
            str = "<table border='0' cellpadding='0' style='border-collapse: collapse' width='100%' height='20'>";
            object obj2 = str + "<tr>" + "<td align='right'>";
            str = string.Concat(new object[] { obj2, "<b>", ReCount, "</b>items&nbsp;Pagesize:<b>", PageSize, "</b>&nbsp;<b>", num, "</b>Page&nbsp;Page:<b>", IndexPage, "</b>/<b>", num, "</b>&nbsp;&nbsp;&nbsp;&nbsp;分页:" });
            if (IndexPage > 1)
            {
                object obj3 = str + "<a href='?Page=1&" + PageParameter + "' title=First><FONT face=webdings>9</FONT></a>";
                str = string.Concat(new object[] { obj3, "<a href='?Page=", IndexPage - 1, "&", PageParameter, "' title=Previous><FONT face=webdings>7</FONT></a>" });
            }
            for (int i = num3; i <= num4; i++)
            {
                if (i == IndexPage)
                {
                    object obj4 = str;
                    str = string.Concat(new object[] { obj4, " <b><font color='red'>", i, "</font></b> " });
                }
                else
                {
                    object obj5 = str;
                    str = string.Concat(new object[] { obj5, " <a href='?Page=", i, "&", PageParameter, "' title=Page", i, ">", i, "</a> " });
                }
            }
            if (IndexPage < num)
            {
                object obj6 = str;
                object obj7 = string.Concat(new object[] { obj6, "<a href='?Page=", IndexPage + 1, "&", PageParameter, "' title=Next><FONT face=webdings>8</FONT></a>" });
                str = string.Concat(new object[] { obj7, "<a href='?Page=", num, "&", PageParameter, "' title=LastPage><FONT face=webdings>:</FONT></a>" });
            }
            str = str + "&nbsp;</td>";
            if (showbutton)
            {
                str = str + "<td><form style='line-height: 150%; margin-top: 0; margin-bottom: 0' name='Select_Page' action='?' mothed='get'><input type='text' name='Page' size='2'><input type='submit' value='GO' name='B1'>" + getPageParameterHidden(splitPageParameter(PageParameter)) + "</form></td>";
            }
            return (str + "</tr></table>");
        }

        public static string PageInfo_EN(int IndexPage, int ReCount, int PageSize, string PageParameter)
        {
            return PageInfo_EN(IndexPage, ReCount, PageSize, PageParameter, true);
        }

        public static string PageInfo_EN(int IndexPage, int ReCount, int PageSize, string PageParameter, bool showbutton)
        {
            string str = "";
            int num = PageCount(ReCount, PageSize);
            int num2 = 10;
            int num3 = 1;
            int num4 = 1;
            if ((num >= num2) && (IndexPage > 6))
            {
                num3 = IndexPage - 5;
                if ((num >= num2) && (num3 > (num - (num2 - 1))))
                {
                    num3 = num - (num2 - 1);
                }
            }
            num4 = num3 + (num2 - 1);
            if (num4 > num)
            {
                num4 = num;
            }
            str = "<table border='0' cellpadding='0' style='border-collapse: collapse' width='100%' height='20'>";
            object obj2 = str + "<tr>" + "<td align='right'>";
            str = string.Concat(new object[] { obj2, "", ReCount, " records&nbsp;", PageSize, "/page pages&nbsp;<b>", IndexPage, "</b>/<b>", num, "</b>&nbsp;&nbsp;&nbsp;&nbsp;page:" });
            if (IndexPage > 1)
            {
                object obj3 = str + "<a href='?Page=1&" + PageParameter + "' title='Page 1'><FONT face=webdings>9</FONT></a>";
                str = string.Concat(new object[] { obj3, "<a href='?Page=", IndexPage - 1, "&", PageParameter, "' title='Prev'><FONT face=webdings>7</FONT></a>" });
            }
            for (int i = num3; i <= num4; i++)
            {
                if (i == IndexPage)
                {
                    object obj4 = str;
                    str = string.Concat(new object[] { obj4, " <b><font color='red'>", i, "</font></b> " });
                }
                else
                {
                    object obj5 = str;
                    str = string.Concat(new object[] { obj5, " <a href='?Page=", i, "&", PageParameter, "' title='Page ", i, "'>", i, "</a> " });
                }
            }
            if (IndexPage < num)
            {
                object obj6 = str;
                object obj7 = string.Concat(new object[] { obj6, "<a href='?Page=", IndexPage + 1, "&", PageParameter, "' title='Next'><FONT face=webdings>8</FONT></a>" });
                str = string.Concat(new object[] { obj7, "<a href='?Page=", num, "&", PageParameter, "' title='Page ", num, "'><FONT face=webdings>:</FONT></a>" });
            }
            str = str + "&nbsp;</td>";
            if (showbutton)
            {
                str = str + "<td><form style='line-height: 150%; margin-top: 0; margin-bottom: 0' name='Select_Page' action='?' mothed='get'><input type='text' name='Page' size='2'><input type='submit' value='Go' name='B1'>" + getPageParameterHidden(splitPageParameter(PageParameter)) + "</form></td>";
            }
            return (str + "</tr></table>");
        }

        public static int ReCordCount(SqlCommand cmd)
        {
            string sql = "select count(*) as total " + cmd.CommandText;
            return Convert.ToInt32(sqlhelp.Query(sql).Tables[0].Rows[0]["total"]);
        }

        public static int ReCordCount(string sql)
        {
            sql = "select count(*) as total " + sql;
            return Convert.ToInt32(sqlhelp.Query(sql).Tables[0].Rows[0]["total"]);
        }

        public static ArrayList splitPageParameter(string PageParameter)
        {
            ArrayList list = new ArrayList();
            if ((PageParameter != null) && (PageParameter != ""))
            {
                string[] strArray = StringPlus.SplitMulti(PageParameter, "&");
                for (int i = 0; i < strArray.Length; i++)
                {
                    if ((strArray[i] != null) && (strArray[i] != ""))
                    {
                        string[] strArray2 = StringPlus.SplitMulti(strArray[i], "=");
                        if ((strArray2 != null) && (strArray2.Length >= 2))
                        {
                            list.Add(strArray2);
                        }
                    }
                }
            }
            return list;
        }

    }
    public class Config
    {
        public static int si_CN = 1;
        public static int si_EN = 2;

    }
}
