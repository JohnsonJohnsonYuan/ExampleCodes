using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// ������ʽ������
    /// </summary>
    public partial class CRegex
	{
        #region BaseMethod

        /// <summary>
        /// �����Ƿ�ƥ��ָ���ı��ʽ
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <returns></returns>
        public static bool IsMatch(string sInput, string sRegex)
        {
            if (sRegex == "")
                return true;
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            return mc.Success;
        }


        /// <summary>
        /// ���ƥ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <param name="iGroupIndex">�ڼ�������, ��1��ʼ, 0��������</param>
        public static List<string> GetList(string sInput, string sRegex, int iGroupIndex)
        {
            List<string> list = new List<string>();
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (iGroupIndex > 0)
                {
                    list.Add(mc.Groups[iGroupIndex].Value);
                }
                else
                {
                    list.Add(mc.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// ���ƥ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <param name="sGroupName">������, ""��������</param>
        public static List<string> GetList(string sInput, string sRegex, string sGroupName)
        {
            List<string> list = new List<string>();
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (sGroupName != "")
                {
                    list.Add(mc.Groups[sGroupName].Value);
                }
                else
                {
                    list.Add(mc.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// ����ƥ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <param name="iGroupIndex">�������, ��1��ʼ, 0������</param>
        public static string GetText(string sInput, string sRegex, int iGroupIndex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            string result = "";
            if (mc.Success)
            {
                if (iGroupIndex > 0)
                {
                    result = mc.Groups[iGroupIndex].Value;
                }
                else
                {
                    result = mc.Value;
                }
            }
            return result;
        }

        /// <summary>
        /// ����ƥ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <param name="sGroupName">������, ""��������</param>
        public static string GetText(string sInput, string sRegex, string sGroupName)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            string result = "";
            if (mc.Success)
            {
                if (sGroupName != "")
                {
                    result = mc.Groups[sGroupName].Value;
                }
                else
                {
                    result = mc.Value;
                }
            }
            return result;
        }

        /// <summary>
        /// �滻ָ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <param name="sReplace">�滻ֵ</param>
        /// <param name="iGroupIndex">�������, 0��������</param>
        public static string Replace(string sInput, string sRegex, string sReplace, int iGroupIndex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (iGroupIndex > 0)
                {
                    sInput = sInput.Replace(mc.Groups[iGroupIndex].Value, sReplace);
                }
                else
                {
                    sInput = sInput.Replace(mc.Value, sReplace);
                }
            }
            return sInput;
        }

        /// <summary>
        /// �滻ָ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <param name="sReplace">�滻ֵ</param>
        /// <param name="sGroupName">������, "" ��������</param>
        public static string Replace(string sInput, string sRegex, string sReplace, string sGroupName)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (sGroupName != "")
                {
                    sInput = sInput.Replace(mc.Groups[sGroupName].Value, sReplace);
                }
                else
                {
                    sInput = sInput.Replace(mc.Value, sReplace);
                }
            }
            return sInput;
        }

        /// <summary>
        /// �ָ�ָ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        /// <param name="iStrLen">��С�����ַ�������</param>
        public static List<string> Split(string sInput, string sRegex, int iStrLen)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            string[] sArray = re.Split(sInput);
            List<string> list = new List<string>();
            list.Clear();
            foreach (string s in sArray)
            {
                if (s.Trim().Length < iStrLen)
                    continue;

                list.Add(s.Trim());
            }
            return list;
        }

        #endregion BaseMethod

        #region ����ض�����

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="sInput">��������</param>
        public static List<string> GetLinks(string sInput)
        {
            return GetList(sInput, @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href");
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sInput">��������</param>
        public static string GetLink(string sInput)
        {
            return GetText(sInput, @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href");
        }

        /// <summary>
        /// ͼƬ��ǩ
        /// </summary>
        /// <param name="sInput">��������</param>
        public static List<string> GetImgTag(string sInput)
        {
            return GetList(sInput, "<img[^>]+src=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "");
        }

        /// <summary>
        /// ͼƬ��ַ
        /// </summary>
        /// <param name="sInput">��������</param>
        public static string GetImgSrc(string sInput)
        {
            return GetText(sInput, "<img[^>]+src=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "src");
        }

        /// <summary>
        /// ����URL�������
        /// </summary>
        /// <param name="sUrl">��������</param>
        public static string GetDomain(string sInput)
        {
            return GetText(sInput, @"http(s)?://([\w-]+\.)+(\w){2,}", 0);
        }

        #endregion ����ض�����

        #region ���ݱ��ʽ�������������
        /// <summary>
        /// ���±���
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        public static string GetTitle(string sInput, string sRegex)
        {
            string sTitle = GetText(sInput, sRegex, "Title");
            sTitle = CString.ClearTag(sTitle);
            if (sTitle.Length > 99)
            {
                sTitle = sTitle.Substring(0, 99);
            }
            return sTitle;
        }

        /// <summary>
        /// ��ҳ����
        /// </summary>
        public static string GetTitle(string sInput)
        {
            return GetText(sInput, @"<Title[^>]*>(?<Title>[\s\S]{10,})</Title>", "Title");
        }

        /// <summary>
        /// ��ҳ����
        /// </summary>
        /// <param name="sInput">��������</param>
        public static string GetHtml(string sInput)
        {
            return CRegex.Replace(sInput, @"(?<Head>[^<]+)<", "", "Head");
        }

        /// <summary>
        /// ��ҳBody����
        /// </summary>
        public static string GetBody(string sInput)
        {
            return GetText(sInput, @"<Body[^>]*>(?<Body>[\s\S]{10,})</body>", "Body");
        }

        /// <summary>
        /// ��ҳBody����
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        public static string GetBody(string sInput, string sRegex)
        {
            return GetText(sInput, sRegex, "Body");
        }

        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        public static string GetSource(string sInput, string sRegex)
        {
            string sSource = GetText(sInput, sRegex, "Source");
            sSource = CString.ClearTag(sSource);
            if (sSource.Length > 99)
                sSource = sSource.Substring(0, 99);
            return sSource;
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        public static string GetAuthor(string sInput, string sRegex)
        {
            string sAuthor = GetText(sInput, sRegex, "Author");
            sAuthor = CString.ClearTag(sAuthor);
            if (sAuthor.Length > 99)
                sAuthor = sAuthor.Substring(0, 99);
            return sAuthor;
        }

        /// <summary>
        /// ��ҳ���ӵ�ַ
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        public static List<string> GetPageLinks(string sInput, string sRegex)
        {
            return GetList(sInput, sRegex, "href");
        }

        /// <summary>
        /// �������·���õ�����·��
        /// </summary>
        /// <param name="sUrl">��������</param>
        /// <param name="sInput">ԭʼ��վ��ַ</param>
        /// <param name="sRelativeUrl">������ӵ�ַ</param>
        public static string GetUrl(string sInput, string sRelativeUrl)
        {
            string sReturnUrl = "";
            string sUrl = _GetStandardUrlDepth(sInput);//������http://www.163.com/news/������ʽ

            if (sRelativeUrl.ToLower().StartsWith("http") || sRelativeUrl.ToLower().StartsWith("https"))
            {
                sReturnUrl = sRelativeUrl.Trim();
            }
            else if (sRelativeUrl.StartsWith("/"))
            {
                sReturnUrl = GetDomain(sInput) + sRelativeUrl;
            }
            else if (sRelativeUrl.StartsWith("../"))
            {
                sUrl = sUrl.Substring(0, sUrl.Length - 1);
                while (sRelativeUrl.IndexOf("../") >= 0)
                {
                    string temp = CString.GetPreStrByLast(sUrl, "/");
                    if (temp.Length > 6)
                    {//temp != "http:/"������Ļ���˵���Ѿ����ݵ���ͷ�ˣ�"../"����ַ�Ĳ�ζ�Ӧ���ϡ����������������ҳ����������Ǵ���ģ������������������ʾ
                        sUrl = temp;
                    }
                    sRelativeUrl = sRelativeUrl.Substring(3);
                }
                sReturnUrl = sUrl + "/" + sRelativeUrl.Trim();
            }
            else if (sRelativeUrl.StartsWith("./"))
            {
                sReturnUrl = sUrl + sRelativeUrl.Trim().Substring(2);
            }
            else if (sRelativeUrl.Trim() != "")
            {//2007images/modecss.css
                sReturnUrl = sUrl + sRelativeUrl.Trim();
            }
            else
            {
                sRelativeUrl = sUrl;
            }
            return sReturnUrl;
        }

        /// <summary>
        /// ��ñ�׼��URL·�����
        /// </summary>
        /// <param name="url"></param>
        /// <returns>���ر�׼����ʽ��http://www.163.com/��http://www.163.com/news/��</returns>
        private static string _GetStandardUrlDepth(string url)
        {
            string sheep = url.Trim().ToLower();
            string header = "http://";
            if (sheep.IndexOf("https://") != -1)
            {
                header = "https://";
                sheep = sheep.Replace("https://", "");
            }
            else
            {
                sheep = sheep.Replace("http://", "");
            }

            int p = sheep.LastIndexOf("/");
            if (p == -1)
            {//www.163.com
                sheep += "/";
            }
            else if (p == sheep.Length - 1)
            {//�������ǣ�http://www.163.com/news/
            }
            else if (sheep.Substring(p).IndexOf(".") != -1)
            {//�������ǣ�http://www.163.com/news/hello.htm ������ʽ
                sheep = sheep.Substring(0, p + 1);
            }
            else
            {
                sheep += "/";
            }

            return header + sheep;
        }

        /// <summary>
        /// �ؼ���
        /// </summary>
        /// <param name="sInput">��������</param>
        public static string GetKeyWord(string sInput)
        {
            List<string> list = Split(sInput, "(,|��|\\+|��|��|;|��|��|:|��)|��|��|_|\\(|��|\\)|��", 2);
            List<string> listReturn = new List<string>();
            Regex re;
            foreach (string str in list)
            {
                re = new Regex(@"[a-zA-z]+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                MatchCollection mcs = re.Matches(str);
                string sTemp = str;
                foreach (Match mc in mcs)
                {
                    if (mc.Value.ToString().Length > 2)
                        listReturn.Add(mc.Value.ToString());
                    sTemp = sTemp.Replace(mc.Value.ToString(), ",");
                }
                re = new Regex(@",{1}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                mcs = re.Matches(sTemp);
                foreach (string s in re.Split(sTemp))
                {
                    if (s.Trim().Length <= 2)
                        continue;
                    listReturn.Add(s);
                }
            }
            string sReturn = "";
            for (int i = 0; i < listReturn.Count - 1; i++)
            {
                for (int j = i + 1; j < listReturn.Count; j++)
                {
                    if (listReturn[i] == listReturn[j])
                    {
                        listReturn[j] = "";
                    }
                }
            }
            foreach (string str in listReturn)
            {
                if (str.Length > 2)
                    sReturn += str + ",";
            }
            if (sReturn.Length > 0)
                sReturn = sReturn.Substring(0, sReturn.Length - 1);
            else
                sReturn = sInput;
            if (sReturn.Length > 99)
                sReturn = sReturn.Substring(0, 99);
            return sReturn;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sInput">��������</param>
        /// <param name="sRegex">���ʽ�ַ���</param>
        public static DateTime GetCreateDate(string sInput, string sRegex)
        {
            DateTime dt = System.DateTime.Now;
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            if (mc.Success)
            {
                try
                {
                    int iYear = int.Parse(mc.Groups["Year"].Value);
                    int iMonth = int.Parse(mc.Groups["Month"].Value);
                    int iDay = int.Parse(mc.Groups["Day"].Value);
                    int iHour = dt.Hour;
                    int iMinute = dt.Minute;

                    string sHour = mc.Groups["Hour"].Value;
                    string sMintue = mc.Groups["Mintue"].Value;

                    if (sHour != "")
                        iHour = int.Parse(sHour);
                    if (sMintue != "")
                        iMinute = int.Parse(sMintue);

                    dt = new DateTime(iYear, iMonth, iDay, iHour, iMinute, 0);
                }
                catch { }
            }
            return dt;
        }

        public static string GetContent(string sOriContent, string sOtherRemoveReg, string sPageUrl, DataTable dtAntiLink)
        {
            string sFormartted = sOriContent;

            //ȥ����Σ�յı��
            sFormartted = Regex.Replace(sFormartted, @"<script[\s\S]*?</script>", "", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            sFormartted = Regex.Replace(sFormartted, @"<iframe[^>]*>[\s\S]*?</iframe>", "", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            Regex r = new Regex(@"<input[\s\S]+?>|<form[\s\S]+?>|</form[\s\S]*?>|<select[\s\S]+?>?</select>|<textarea[\s\S]*?>?</textarea>|<file[\s\S]*?>|<noscript>|</noscript>", RegexOptions.IgnoreCase);
            sFormartted = r.Replace(sFormartted, "");
            string[] sOtherReg = sOtherRemoveReg.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sRemoveReg in sOtherReg)
            {
                sFormartted = CRegex.Replace(sFormartted, sRemoveReg, "", 0);
            }

            //ͼƬ·��
            //sFormartted = _ReplaceUrl("<img[^>]+src\\s*=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "src", sFormartted,sPageUrl);
            sFormartted = _ReplaceUrl("<img[\\s\\S]+?src\\s*=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "src", sFormartted, sPageUrl);
            //��������
            string domain = GetDomain(sPageUrl);
            DataRow[] drs = dtAntiLink.Select("Domain='" + domain + "'");
            if (drs.Length > 0)
            {
                foreach (DataRow dr in drs)
                {
                    switch (Convert.ToInt32(dr["Type"]))
                    {
                        case 1://�û�
                            sFormartted = sFormartted.Replace(dr["imgUrl"].ToString(), "http://stat.580k.com/t.asp?url=");
                            break;
                        default://����
                            sFormartted = sFormartted.Replace(dr["imgUrl"].ToString(), "http://stat.580k.com/t.asp?url=" + dr["imgUrl"].ToString());
                            break;
                    }
                }
            }

            //A����
            sFormartted = _ReplaceUrl(@"<a[^>]+href\s*=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href", sFormartted, sPageUrl);

            //CSS
            sFormartted = _ReplaceUrl(@"<link[^>]+href\s*=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href", sFormartted, sPageUrl);

            //BACKGROUND
            sFormartted = _ReplaceUrl(@"background\s*=\s*(?:'(?<img>[^']+)'|""(?<img>[^""]+)""|(?<img>[^>\s]+))", "img", sFormartted, sPageUrl);
            //style��ʽ�ı�����background-image:url(...)
            sFormartted = _ReplaceUrl(@"background-image\s*:\s*url\s*\x28(?<img>[^\x29]+)\x29", "img", sFormartted, sPageUrl);

            //FLASH
            sFormartted = _ReplaceUrl(@"<param\s[^>]+""movie""[^>]+value\s*=\s*""(?<flash>[^"">]+\x2eswf)""[^>]*>", "flash", sFormartted, sPageUrl);

            //XSL
            if (IsXml(sFormartted))
            {
                sFormartted = _ReplaceUrl(@"<\x3fxml-stylesheet\s+[^\x3f>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)"")\s*[^\x3f>]*\x3f>", "href", sFormartted, sPageUrl);
            }

            //script
            //sFormartted = _ReplaceUrl(@"<script[^>]+src\s*=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", "src", sFormartted,sPageUrl);

            return sFormartted;
        }

        /// <summary>
        /// �û�����
        /// </summary>
        private static string _ReplaceUrl(string strRe, string subMatch, string sFormartted, string sPageUrl)
        {
            Regex re = new Regex(strRe, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            MatchCollection mcs = re.Matches(sFormartted);
            string sOriStr = "";
            string sSubMatch = "";
            string sReplaceStr = "";
            foreach (Match mc in mcs)
            {
                sOriStr = mc.Value;
                sSubMatch = mc.Groups[subMatch].Value;
                sReplaceStr = sOriStr.Replace(sSubMatch, CRegex.GetUrl(sPageUrl, sSubMatch));
                sFormartted = sFormartted.Replace(sOriStr, sReplaceStr);
            }

            return sFormartted;
        }

        public static bool IsXml(string sFormartted)
        {
            Regex re = new Regex(@"<\x3fxml\s+", RegexOptions.IgnoreCase);
            MatchCollection mcs = re.Matches(sFormartted);
            return (mcs.Count > 0);
        }

        #endregion ���ݱ��ʽ�������������

	}
}
