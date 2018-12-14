using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections;

/// <summary>
/// GeneralHelper 的摘要说明
/// </summary>
public class GeneralHelper
{
	public GeneralHelper()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
    }

    #region 返回不重复随机数数组
    /// <summary>
    /// 返回不重复随机数数组
    /// </summary>
    /// <param name="Num">随机数个数</param>
    /// <param name="minNum">随机数下限</param>
    /// <param name="maxNum">随机数上限</param>
    /// <returns></returns>
    public int[] GetRandomArray(int Number, int minNum, int maxNum)
    {
        int j;
        int[] b = new int[Number];
        Random r = new Random();
        for (j = 0; j < Number; j++)
        {
            int i = r.Next(minNum, maxNum + 1);
            int num = 0;
            for (int k = 0; k < j; k++)
            {
                if (b[k] == i)
                {
                    num = num + 1;
                }
            }
            if (num == 0)
            {
                b[j] = i;
            }
            else
            {
                j = j - 1;
            }
        }
        return b;
    }
    #endregion

    #region 计算字符串字节
    ///   <summary>   
    ///   判断一个字符串的字节数量   
    ///   </summary>   
    ///   <param   name="theString"></param>   
    ///   <returns></returns>   
    ///   
    public static int StringLength(string theString)
    {
        int theLength = 0;
        for (int i = 0; i < theString.Length; i++)
        {
            if ((short)(Convert.ToChar(theString.Substring(i, 1))) > 255 || (short)(Convert.ToChar(theString.Substring(i, 1))) < 0)
            {
                theLength += 2;

            }
            else
            {
                theLength += 1;
            }
        }
        return theLength;
    }
    #endregion


    #region MD5加密
    public static string MD5(string str)
    {
        return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().ToLower().Substring(8, 16); //16位
        //return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().ToLower(); //32位
    }
    #endregion


    #region 提取中文首字母
    //需引用using System.Text;
    static public string GetChineseSpell(string strText)
    {
        int len = strText.Length;
        string myStr = "";
        for (int i = 0; i < len; i++)
        {
            myStr += getSpell(strText.Substring(i, 1));
        }
        return myStr;
    }

    static public string getSpell(string cnChar)
    {
        byte[] arrCN = Encoding.Default.GetBytes(cnChar);
        if (arrCN.Length > 1)
        {
            int area = (short)arrCN[0];
            int pos = (short)arrCN[1];
            int code = (area << 8) + pos;
            int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
            for (int i = 0; i < 26; i++)
            {
                int max = 55290;
                if (i != 25) max = areacode[i + 1];
                if (areacode[i] <= code && code < max)
                {
                    return Encoding.Default.GetString(new byte[] { (byte)(97 + i) });
                }
            }
            return "*";
        }
        else return cnChar;
    }
    #endregion


    #region 文本框格式化(value=?使用)
    ///   <summary>   
    ///   过滤输出字符串   
    ///   </summary>   
    ///   <param   name="inputString">要过滤的字符串</param>   
    ///   <returns>过滤后的字符串</returns>   
    public static string Output(object inputString)
    {
        if (inputString == null)
            return string.Empty;

        string str1 = HttpContext.Current.Server.HtmlEncode(inputString.ToString());
        str1 = str1.Replace("&amp;", "&");
        str1 = str1.Replace("&lt;", "<");
        str1 = str1.Replace("&gt;", ">");
        str1 = str1.Replace("&quot;", ((char)34).ToString());
        return str1.ToString();

        //前台显示DataBinder.Eval(Container.DataItem,   "Content").ToString().Replace("<","&lt;").Replace(">","&gt;").Replace("\r\n","<br>").Replace("   ","&nbsp;")   
    }
    #endregion


    #region 文本框格式化(前台显示=?使用)
    ///   <summary>   
    ///   过滤输出字符串   
    ///   </summary>   
    ///   <param   name="inputString">要过滤的字符串</param>   
    ///   <returns>过滤后的字符串</returns>   
    public static string Outhtml(object htmlString)
    {
        if (htmlString == null)
            return string.Empty;

        string str2 = HttpContext.Current.Server.HtmlEncode(htmlString.ToString());
        str2 = str2.Replace("&", "&amp;");
        str2 = str2.Replace("<", "&lt;");
        str2 = str2.Replace(">", "&gt;");
        str2 = str2.Replace(((char)34).ToString(), "&quot;");
        str2 = str2.Replace("\r\n", "<br>");
        return str2.ToString();

        //前台显示DataBinder.Eval(Container.DataItem,   "Content").ToString().Replace("<","&lt;").Replace(">","&gt;").Replace("\r\n","<br>").Replace("   ","&nbsp;")   
    }
    #endregion


    #region 半角转全角(SBC case)
    /// <summary> 
    /// 转全角的函数(SBC case) 
    /// </summary> 
    /// <param name="input">任意字符串</param> 
    /// <returns>全角字符串</returns> 
    ///<remarks> 
    ///全角空格为12288，半角空格为32 
    ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
    ///</remarks> 
    public static string ToSBC(string input)
    {
        //半角转全角： 
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new string(c);
    }
    #endregion


    #region 全角转半角(DBC case)
    /// <summary> 
    /// 转半角的函数(DBC case) 
    /// </summary> 
    /// <param name="input">任意字符串</param> 
    /// <returns>半角字符串</returns> 
    ///<remarks> 
    ///全角空格为12288，半角空格为32 
    ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
    ///</remarks> 
    public static string ToDBC(string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c);
    }
    #endregion


    #region 字符串截取函数，截取左边指定的字节数
    /// <summary> 
    /// 字符串截取函数，截取左边指定的字节数 
    /// </summary> 
    /// <param name="text">输入字符串</param> 
    /// <param name="CutLength">截取长度</param> 
    /// <returns>返回处理后的字符串</returns> 
    public static string cutStr(string text, int CutLength)
    {
        if (text == null || text.Length == 0 || CutLength <= 0)
            return "";
        int iCount = System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(text);
        if (iCount > CutLength)
        {
            int iLength = 0;
            for (int i = 0; i < text.Length; i++)
            {
                int iCharLength = System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(new char[] { text[i] });
                iLength += iCharLength;
                if (iLength == CutLength)
                {
                    text = text.Substring(0, i + 1);
                    break;
                }
                else if (iLength > CutLength)
                {
                    text = text.Substring(0, i);
                    break;
                }
            }
        }
        return text;
    }
    #endregion


    #region Rss日期时间转换，将时间全部转换为GMT时间
    /// <summary> 
    /// Rss日期时间转换，将时间全部转换为GMT时间 
    /// </summary> 
    /// <param name="strDateTime">Rss中读取的时间</param> 
    /// <returns>处理后的标准时间格式</returns> 
    public static string dateConvert(string strDateTime)
    {
        strDateTime = strDateTime.Replace("+0000", "GMT");
        strDateTime = strDateTime.Replace("+0100", "GMT");
        strDateTime = strDateTime.Replace("+0200", "GMT");
        strDateTime = strDateTime.Replace("+0300", "GMT");
        strDateTime = strDateTime.Replace("+0400", "GMT");
        strDateTime = strDateTime.Replace("+0500", "GMT");
        strDateTime = strDateTime.Replace("+0600", "GMT");
        strDateTime = strDateTime.Replace("+0700", "GMT");
        strDateTime = strDateTime.Replace("+0800", "GMT");
        strDateTime = strDateTime.Replace("-0000", "GMT");
        strDateTime = strDateTime.Replace("-0100", "GMT");
        strDateTime = strDateTime.Replace("-0200", "GMT");
        strDateTime = strDateTime.Replace("-0300", "GMT");
        strDateTime = strDateTime.Replace("-0400", "GMT");
        strDateTime = strDateTime.Replace("-0500", "GMT");
        strDateTime = strDateTime.Replace("-0600", "GMT");
        strDateTime = strDateTime.Replace("-0700", "GMT");
        strDateTime = strDateTime.Replace("-0800", "GMT");
        DateTime dt = DateTime.Parse(strDateTime, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        return dt.ToString();
    }
    #endregion


    #region 使用正则表达式删除用户输入中的html内容
    /// <summary> 
    /// 使用正则表达式删除用户输入中的html内容 
    /// </summary> 
    /// <param name="text">输入内容</param> 
    /// <returns>清理后的文本</returns> 
    public static string clearHtml(string text)
    {
        string pattern;

        if (text.Length == 0)
            return text;

        pattern = @"(<[a-zA-Z].*?>)|(<[\/][a-zA-Z].*?>)";
        text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);
        text = text.Replace("<", "<");
        text = text.Replace(">", ">");

        return text;
    }

    public static string reHtml(string text)
    {
        text = text.Replace(" ", " ");
        text = text.Replace("<br />", "\n");
        return text;
    }
    #endregion


    #region 使用正则表达式删除用户输入中的JS脚本内容
    /// <summary> 
    /// 使用正则表达式删除用户输入中的JS脚本内容 
    /// </summary> 
    /// <param name="text">输入内容</param> 
    /// <returns>清理后的文本</returns> 
    public static string clearScript(string text)
    {
        string pattern;

        if (text.Length == 0)
            return text;

        pattern = @"(?i)<script([^>])*>(\w|\W)*</script([^>])*>";
        text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);

        pattern = @"<script([^>])*>";
        text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);

        pattern = @"</script>";
        text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);

        return text;
    }
    #endregion


    #region 过滤SQL,所有涉及到输入的用户直接输入的地方都要使用
    /// <summary> 
    /// 过滤SQL,所有涉及到输入的用户直接输入的地方都要使用。 
    /// </summary> 
    /// <param name="text">输入内容</param> 
    /// <returns>过滤后的文本</returns> 
    public static string filterSQL(string text)
    {
        text = text.Replace("'", "''");
        text = text.Replace("{", "{");
        text = text.Replace("}", "}");

        return text;
    }
    #endregion


    #region 过滤SQL,将SQL字符串里面的(')转换成('')，再在字符串的两边加上(')
    /// <summary> 
    /// 将SQL字符串里面的(')转换成('')，再在字符串的两边加上(')。 
    /// </summary> 
    /// <param name="text">输入内容</param> 
    /// <returns>过滤后的文本</returns> 
    public static String GetQuotedString(String text)
    {
        return ("'" + filterSQL(text) + "'");
    }
     #endregion


    #region 判断字符串是否为数字
    /// <summary> 
    /// 判断字符串是否为数字 
    /// </summary> 
    /// <param name="str">输入字符串</param> 
    /// <returns>true为是，false为否</returns> 
    public static bool isNumeric(string str)
    {
        if (str == null || str.Length == 0)
            return false;
        foreach (char c in str)
        {
            if (!Char.IsNumber(c))
            {
                return false;
            }
        }
        return true;
    }
    #endregion


    #region 空格转换为逗号
    /// <summary> 
    /// 空格转换为逗号 
    /// </summary> 
    /// <param name="strTags"></param> 
    /// <returns></returns> 
    public static string space2comma(string strIn)
    {
        strIn = strIn.Replace(" ", ",");
        return strIn;
    }

    public static string comma2space(string strIn)
    {
        strIn = strIn.Replace(",", " ");
        return strIn;
    }
    #endregion


    #region 1位计数转换为2位
    /// <summary> 
    /// 1位计数转换为2位 
    /// </summary> 
    /// <param name="strIn"></param> 
    /// <returns></returns> 
    public static string one2two(int strIn)
    {
        if (strIn.ToString().Length < 2)
        {
            return "0" + strIn.ToString();
        }
        else
        {
            return strIn.ToString();
        }
    }
    #endregion


    #region 自动识别文本中的URL
    /// <summary> 
    /// 自动识别文本中的URL 
    /// 可以识别 www.，http://， ftp://， xx@xx.xx， mms:// 
    /// </summary> 
    /// <param name="input">输入数据</param> 
    /// <returns>自动识别URL后的数据</returns> 
    public static string autoConvertToURL(object input)
    {
        string str = input.ToString();

        Regex Reg;
        Reg = new Regex("([^\\]=>])(http://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "$1<a href=\"$2\" target=\"_blank\">$2</a>");
        Reg = new Regex("^(http://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "<a href=\"$1\" target=\"_blank\">$1</a>");
        Reg = new Regex("(http://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)$");
        str = Reg.Replace(str, "<a href=\"$1\" target=\"_blank\">$1</a>");
        Reg = new Regex("([^\\]=>])(ftp://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "$1<a href=\"$2\" target=\"_blank\">$2</a>");
        Reg = new Regex("^(ftp://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "<a href=\"$1\" target=\"_blank\">$1</a>");
        Reg = new Regex("(ftp://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)$");
        str = Reg.Replace(str, "<a href=\"$1\" target=\"_blank\">$1</a>");
        Reg = new Regex("([^\\]=>])(mms://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "$1<a href=\"$2\" target=\"_blank\">$2</a>");
        Reg = new Regex("^(mms://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "<a href=\"$1\" target=\"_blank\">$1</a>");
        Reg = new Regex("(mms://[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)$");
        str = Reg.Replace(str, "<a href=\"$1\" target=\"_blank\">$1</a>");
        Reg = new Regex("([a-z0-9_A-Z\\-\\.]{1,20})@([a-z0-9_\\-]{1,15})\\.([a-z]{2,4})");
        str = Reg.Replace(str, "<a href=\"mailto:$1@$2.$3\" target=\"_blank\">$1@$2.$3</a>");
        Reg = new Regex("([^/])(www.[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "$1<a href=\"http://$2\" target=\"_blank\">$2</a>");
        Reg = new Regex("^(www.[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)");
        str = Reg.Replace(str, "<a href=\"http://$1\" target=\"_blank\">$1</a>");
        Reg = new Regex("(www.[A-Za-z0-9\\./=\\?%\\-&_~`@':+!]+)$");
        str = Reg.Replace(str, "<a href=\"http://$1\" target=\"_blank\">$1</a>");

        return str;
    }
    #endregion


    #region 把IP地址转换为数字格式
    /// <summary> 
    /// 把IP地址转换为数字格式 
    /// </summary> 
    /// <param name="strIp">IP地址</param> 
    /// <returns>数字</returns> 
    public static int IPtoNum(string strIp)
    {
        string[] temp = strIp.Split('.');
        return (int.Parse(temp[0])) * 256 * 256 * 256 + (int.Parse(temp[1])) * 256 * 256 * 256 + (int.Parse(temp[2])) * 256 * 256 * 256;
    }
    #endregion


    #region 检查邮件正确性
    /// <summary> 
    /// 检查邮件正确性 
    /// </summary> 
    /// <param name="inputEmail">输入的邮件地址</param> 
    /// <returns>返回BOOL值</returns> 
    public static bool isEmail(string inputEmail)
    {
        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(inputEmail))
            return (true);
        else
            return (false);
    }
    #endregion


    #region 替换XML文档不接受的字符
    /// <summary> 
    /// 替换XML文档不接受的字符 
    /// </summary> 
    /// <param name="input">传入值</param> 
    /// <returns>替换后的字符</returns> 
    public static string formatForXML(object input)
    {
        string str = input.ToString();

        //替换XML文档不接受的字符 
        str = str.Replace(" ", " ");
        str = str.Replace("&", "&");
        str = str.Replace("\"", "''");
        str = str.Replace("'", "&apos;");

        return str;
    }
    #endregion

    #region Split数组处理
    static public string[] SplitString(string str, string separator)
    {
        string tmp = str;
        Hashtable ht = new Hashtable();
        int i = 0;
        int pos = tmp.IndexOf(separator);
        while (pos != -1)
        {
            ht.Add(i, tmp.Substring(0, pos));
            tmp = tmp.Substring(pos + separator.Length);
            pos = tmp.IndexOf(separator);
            i++;
        }
        ht.Add(i, tmp);
        string[] array = new string[ht.Count];
        for (int j = 0; j < ht.Count; j++)
            array[j] = ht[j].ToString();

        return array;
    }
    #endregion


    #region AjaxPro使用的分页方法
    /// <summary> 
    /// 使用AjaxPro时候使用的方法
    /// </summary> 
    /// <param name="pageIndex">第几页</param> 
    /// <param name="pageIndex">总共多少页</param> 
    /// <param name="pageIndex">当前页条数</param> 
    /// <param name="pageIndex">总条数</param> 
    /// <returns>分页导航</returns> 
    public static string AjaxPages(int pageIndex, int pageCount,int roscount,int counts)
    {
        StringBuilder text = new StringBuilder();

        //新的分页
        text.Append("<br><TABLE class='tableborder' cellSpacing='1' cellPadding='3' width='98%'  border='0' align='center'>");
        text.Append("<td align='left' width='250px'>第" + pageIndex + "页/总" + pageCount + "页　本页" + roscount + "条/总" + counts + "条 </td>");
        text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(1)'>首页</a></td>");
        if (pageIndex < pageCount)
        {
            text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(" + (pageIndex + 1) + ")'>下一页</a></td>");
        }
        else
        {
            text.Append("<td align='left' width='40px'>下一页</a></td>");
        }
        if (pageIndex > 1)
        {
            text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(" + (pageIndex - 1) + ")'>上一页</a></td>");
        }
        else
        {
            text.Append("<td align='left' width='40px'>上一页</a></td>");
        }
        text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(" + pageCount + ")'>尾页</a><td>");

        int BasePage = (pageIndex / 10) * 10;
        if (BasePage > 0)
        {
            text.Append("<td align='left' width='20px'><a href='javascript:JumpPage(" + (BasePage - 9) + ")'>&lt;&lt;</a></td>");
        }
        for (int j = 1; j < 11; j++)
        {
            int PageNumber = BasePage + j;
            if (PageNumber > pageCount)
            {
                break;
            }
            if (PageNumber == Convert.ToInt32(pageIndex))
            {
                text.Append("<td align='left' width='20px'><font color='#FF0000'>" + PageNumber + "</font></td>");
            }
            else
            {
                text.Append("<td align='left' width='20px'><a href='javascript:JumpPage(" + PageNumber + ")'>" + PageNumber + "</a></td>");
            }

        }
        if (pageCount - 1 > BasePage)
        {
            text.Append("<td align='left' width='20px'><a href='javascript:JumpPage(" + (BasePage + 11) + ")'>&gt;&gt;</a></td>");
        }
        text.Append("</table>");

        return text.ToString();
    }
    #endregion
}
