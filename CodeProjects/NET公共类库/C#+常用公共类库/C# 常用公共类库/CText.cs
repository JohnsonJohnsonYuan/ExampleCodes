using System;
using System.Collections.Generic;
using System.Text; 
using System.Text.RegularExpressions;
using System.Xml;

namespace WHC.OrderWater.Commons
{
    public class CText
    {       
        #region �������ݻ������
        public static string GetLink(string sContent)
        {
            string strReturn = "";
            Regex re = new Regex(@"<a\s+[^>]*href\s*=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Multiline| RegexOptions.IgnorePatternWhitespace);
            Regex js = new Regex(@"(href|onclick)=[^>]+javascript[^>]+(('(?<href>[\w\d/-]+\.[^']*)')|(&quot;(?<href>[\w\d/-]+\.[^;]*)&quot;))[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            Match mc = js.Match(sContent);//��ȡjavascript�е����ӣ��д��Ľ�
            if (mc.Success)
            {
                strReturn = mc.Groups["href"].Value;
            }
            else
            {
                Match me = re.Match(sContent);
                if (me.Success)
                {
                    strReturn = System.Web.HttpUtility.HtmlDecode(me.Groups["href"].Value);
                    //strReturn = RemoveByReg(strReturn, @";.*|javascript:.*");
                    strReturn = RemoveByReg(strReturn, @";[^?&]*|javascript:.*");
                }
            }

            return strReturn;
        }
        public static string GetTextByLink(string sContent)
        {
            Regex re = new Regex(@"<a(?:\s+[^>]*)?>([\s\S]*)?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Regex email = new Regex(@"(href|onclick)=[^>]+mailto[^>]+@[^>]+>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match me = email.Match(sContent);
            if (me.Success)
                return "";

            Match mc = re.Match(sContent);
            if (mc.Success)
                return mc.Groups[1].Value;
            else
                return "";
        }

        /// <summary>
        /// ��ȡ������Ч���ӣ����˹��
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetLinks(string sContent, string sUrl)
        {
            Dictionary<string, string> lisDes = new Dictionary<string, string>();
            return GetLinks(sContent, sUrl,ref lisDes);
        }

        public static Dictionary<string, string> GetLinks(string sContent, string sUrl, ref Dictionary<string, string> lisDes)
        {
            Dictionary<string, string> lisA = new Dictionary<string, string>();

            _GetLinks(sContent, sUrl, ref lisA);

            string domain = CRegex.GetDomain(sUrl).ToLower();

            //ץȡ�ű����������
            Regex re = new Regex(@"<script[^>]+src\s*=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            MatchCollection mcs = re.Matches(sContent);
            //foreach (Match mc in mcs)
            for( int i = mcs.Count - 1;i >= 0; i--)
            {
                Match mc = mcs[i];
                string subUrl = CRegex.GetUrl(sUrl, mc.Groups["src"].Value);
                if (domain.CompareTo(CRegex.GetDomain(subUrl).ToLower()) != 0)
                {
                    //ͬһ��Ĳ�����
                    continue;
                }
                string subContent = CSocket.GetHtmlByUrl(subUrl);
                if (subContent.Length == 0)
                {
                    continue;
                }
                _GetLinks(subContent, subUrl, ref lisA);
            }

            if (lisA.Count == 0)
            {
                return GetLinksFromRss(sContent,sUrl,ref lisDes);
            }

            return lisA;
        }

        private static void _GetLinks(string sContent, string sUrl, ref Dictionary<string, string> lisA)
        {
            const string sFilter =
@"��ҳ|����|����|English|����|������|Ͷ��|����|��ϵ|����|about|����|����|���|����|��Ӱ|���
|��¼|ע��|ע��|ʹ��|����|����|�ղؼ�|�ղ�|���|����
|����|more|ר��|��ѡ|����|����|�Ƽ�|����
|����|����|����|����|���
|����|�Ķ���|RSS
|����|����|����|�ҵ�|����|��֯|�ſ�|����|���|��˾|����|����|���|��ͼ|��˽
|��|��|��|��|��|��|��|��|��|��|\.";

            Regex re = new Regex(@"<a\s+[^>]*href\s*=\s*[^>]+>[\s\S]*?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Regex re2 = new Regex(@"""|'", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sContent);
            //foreach (Match mc in mcs)
            for (int i = mcs.Count - 1; i >= 0; i--)
            {
                Match mc = mcs[i];
                string strHref = GetLink(mc.Value).Trim();
                
                strHref = strHref.Replace("\\\"", "");//���JS�������
                strHref = strHref.Replace("\\\'", "");
                
                string strTemp = RemoveByReg(strHref, @"^http.*/$");//�����ԡ�http����ͷ��/����β�����ӵ�ַ
                if (strTemp.Length < 2)
                {
                    continue;
                }

                //���˹��������������
                string strText = CString.ClearTag(GetTextByLink(mc.Value)).Trim();
                strTemp = RemoveByReg(strText, sFilter);
                if (CString.GetLength(strTemp) < 9)
                {
                    continue;
                }
                if (re2.IsMatch(strText))
                {
                    continue;
                }

                //���Ͼ��Ե�ַ
                strHref = CText.GetUrlByRelative(sUrl, strHref);
                if (strHref.Length <= 18)//���磬http://www.163.com = 18
                {
                    continue;
                }

                //����#�ַ����ֵ�λ�ã��Ƴ������������
                //�����������ַ��������
                int charIndex = strHref.IndexOf('#');
                if (charIndex > -1)
                {
                    strHref = strHref.Substring(0, charIndex);
                }
                strHref = strHref.Trim(new char[]{'/','\\'});
                string tmpDomainURL = CRegex.GetDomain(strHref);
                if (strHref.Equals(tmpDomainURL, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }                

                if (!lisA.ContainsKey(strHref) && !lisA.ContainsValue(strText))
                {
                    lisA.Add(strHref, strText);
                }
            }
        }

        public static bool IsExistsScriptLink(string sHtml)
        {
            Regex re = new Regex(@"<script[^>]+src\s*=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            return re.IsMatch(sHtml);
        }

        /// <summary>
        /// �������������ùؼ��ֹ���
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listKey"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetLinksByKey(Dictionary<string, string> listA, List<string> listKey)
        {
            if (listKey == null)
            {
                return listA;
            }

            Dictionary<string, string> listNeed = new Dictionary<string, string>();

            //׼���ùؼ���������ʽ
            string sKey = "";
            foreach (string s in listKey)
            {
                sKey += "([\\s\\S]*" + _ForReguSpeciChar(s) + "[\\s\\S]*)|";
            }
            sKey = (sKey != "") ? sKey.Substring(0, sKey.Length - 1) : "[\\s\\S]+";
            Regex reKey = new Regex(sKey, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            foreach (KeyValuePair<string, string> kvp in listA)
            {
                if (reKey.Match(kvp.Value).Success)
                {
                    if (!listNeed.ContainsKey(kvp.Key))
                    {
                        listNeed.Add(kvp.Key, kvp.Value); 
                    }
                }
            }

            return listNeed;
        }

        private static string _ForReguSpeciChar(string txtRegular)
        {
            string[] arSpecial = new string[]{".","$","^","{","[","(","|",")","*","+","?","#"};
            string txtTranRegular = txtRegular;

            foreach (string s in arSpecial)
            {
                txtTranRegular = txtTranRegular.Replace(s, "\\" + s);
            }

            return txtTranRegular;
        }


        /// <summary>
        /// ��RSS FEED�ж�ȡ
        /// </summary>
        /// <param name="sContent"></param>
        /// <param name="listKey"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetLinksFromRss(string sContent, string sUrl)
        {
            Dictionary<string, string> lisDes = new Dictionary<string, string>();
            return GetLinksFromRss(sContent, sUrl, ref lisDes);
        }
        
        public static Dictionary<string, string> GetLinksFromRss(string sContent, string sUrl, ref Dictionary<string, string> lisDes)
        {
            Dictionary<string, string> listResult = new Dictionary<string, string>();

            XmlDocument xml = new XmlDocument();

            //RSS2.0
            try
            {
                xml.LoadXml(sContent.Trim());
                XmlNodeList nodes = xml.SelectNodes("/rss/channel/item");
                if (nodes.Count > 0)
                {
                    //for (int i = 0; i < nodes.Count; i++)
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            string sLink = GetUrlByRelative(sUrl, nodes[i].SelectSingleNode("link").InnerText);
                            listResult.Add(sLink, nodes[i].SelectSingleNode("title").InnerText);
                            lisDes.Add(sLink, nodes[i].SelectSingleNode("description").InnerText);
                        }
                        catch { }
                    }
                    return listResult;
                }
                
            }
            catch { }

            //RSS1.0��RDF��
            try
            {
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                nsMgr.AddNamespace("rss", "http://purl.org/rss/1.0/"); 
                XmlNodeList nodes = xml.SelectNodes("/rdf:RDF//rss:item", nsMgr);
                if (nodes.Count > 0)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            string sLink = GetUrlByRelative(sUrl, nodes[i].SelectSingleNode("rss:link", nsMgr).InnerText);
                            listResult.Add(sLink, nodes[i].SelectSingleNode("rss:title", nsMgr).InnerText);
                            lisDes.Add(sLink, nodes[i].SelectSingleNode("rss:description",nsMgr).InnerText);
                        }
                        catch { }
                        //listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("rss:link",nsMgr).InnerText + "\">" + nodes[i].SelectSingleNode("rss:title",nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch{}

            //RSS ATOM
            try
            {
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("atom", "http://purl.org/atom/ns#");
                XmlNodeList nodes = xml.SelectNodes("/atom:feed/atom:entry",nsMgr);
                if (nodes.Count > 0)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            string sLink = GetUrlByRelative(sUrl, nodes[i].SelectSingleNode("atom:link", nsMgr).Attributes["href"].InnerText);
                            listResult.Add(sLink, nodes[i].SelectSingleNode("atom:title", nsMgr).InnerText);
                            lisDes.Add(sLink, nodes[i].SelectSingleNode("atom:content", nsMgr).InnerText);
                        }
                        catch { }
                        //listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("atom:link",nsMgr).Attributes["href"].InnerText + "\">" + nodes[i].SelectSingleNode("atom:title",nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch { }

            return listResult;
        }
        public static string GetTitleFromRss(string sContent)
        {
            string title = "";
            XmlDocument xml = new XmlDocument();

            //RSS2.0
            try
            {
                xml.LoadXml(sContent.Trim());
                title = xml.SelectSingleNode("/rss/channel/title").InnerText;
            }
            catch { }

            return title;
        }

        #region �ѹ�ʱ�ķ���
        [Obsolete("�ѹ�ʱ�ķ�����")]
        public static List<string> GetLinksByKey(string sContent, /*string sUrl,*/ List<string> listKey)
        {
            List<string> listResult = new List<string>();
            List<string> list = new List<string>();
            string sKey = "";
            string strKey;

            //��ȡ����
            Regex re = new Regex(@"<a\s+[^>]*href\s*=\s*[^>]+>[\s\S]*?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sContent);
            foreach (Match mc in mcs)
            {
                strKey = RemoveByReg(GetLink(mc.Value), @"^http.*/$");//�����ԡ�http����ͷ��/����β�����ӵ�ַ
                if (strKey.Length > 0)
                {
                    list.Add(mc.Value);
                }
            }

            //׼���ùؼ���
            foreach (string s in listKey)
            {
                sKey += "([\\s\\S]*" + s + "[\\s\\S]*)|";
            }
            if (sKey != "")
                sKey = sKey.Substring(0, sKey.Length - 1);
            if (sKey == "")
                sKey = "[\\s\\S]+";
            Regex reKey = new Regex(sKey, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            Match tmpmc;
            //���ӵ�����һ��Ҫ5�������ϲ�����Ч��
            re = new Regex(@"<a\s+[^>]+>([\s\S]{5,})?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            foreach (string s in list)
            {
                tmpmc = re.Match(s);
                if (tmpmc.Success)
                {
                    strKey = CString.ClearTag(tmpmc.Groups[1].Value.Trim());
                    strKey = RemoveByReg(strKey, @"����|��¼|���|�Ƽ�|�ղؼ�|����|����|����|�Ķ���|�ҵ�|����|���|��˾|more|RSS|about|\.");
                    if (CString.GetLength(strKey) > 8)//��������5����Ϊ������������Ϣ��
                    {
                        if (reKey.Match(strKey).Success)
                        {
                            listResult.Add(s);
                        }
                    }
                }
            }

            #region ��RSS��֧��
            if (listResult.Count == 0)
            {
                return GetLinksByKeyFromRss(sContent, listKey);
            }
            #endregion

            return listResult;
        }

        /// <summary>
        /// ��RSS FEED�ж�ȡ
        /// </summary>
        /// <param name="sContent"></param>
        /// <param name="listKey"></param>
        /// <returns></returns>
        [Obsolete("�ѹ�ʱ�ķ�����")]
        public static List<string> GetLinksByKeyFromRss(string sContent, List<string> listKey)
        {
            List<string> listResult = new List<string>();

            string sKey = "";
            foreach (string s in listKey)
            {
                sKey += "([\\s\\S]*" + s + "[\\s\\S]*)|";
            }
            if (sKey != "")
                sKey = sKey.Substring(0, sKey.Length - 1);
            if (sKey == "")
                sKey = "[\\s\\S]+";
            Regex reKey = new Regex(sKey, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            XmlDocument xml = new XmlDocument();

            //RSS2.0
            try
            {
                xml.LoadXml(sContent.Trim());
                XmlNodeList nodes = xml.SelectNodes("/rss/channel/item");
                if (nodes.Count > 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("link").InnerText + "\">" + nodes[i].SelectSingleNode("title").InnerText + "</a>");
                        //listResult.Add(nodes[i].SelectSingleNode("link").InnerText, nodes[i].SelectSingleNode("title").InnerText);
                    }
                    return listResult;
                }

            }
            catch { }

            //RSS1.0��RDF��
            try
            {
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                nsMgr.AddNamespace("rss", "http://purl.org/rss/1.0/");
                XmlNodeList nodes = xml.SelectNodes("/rdf:RDF//rss:item", nsMgr);
                if (nodes.Count > 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        //listResult.Add(nodes[i].SelectSingleNode("rss:link", nsMgr).InnerText, nodes[i].SelectSingleNode("rss:title", nsMgr).InnerText);
                        listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("rss:link",nsMgr).InnerText + "\">" + nodes[i].SelectSingleNode("rss:title",nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch { }

            //RSS ATOM
            try
            {
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("atom", "http://purl.org/atom/ns#");
                XmlNodeList nodes = xml.SelectNodes("/atom:feed/atom:entry", nsMgr);
                if (nodes.Count > 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        //listResult.Add(nodes[i].SelectSingleNode("atom:link", nsMgr).Attributes["href"].InnerText, nodes[i].SelectSingleNode("atom:title", nsMgr).InnerText);
                        listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("atom:link",nsMgr).Attributes["href"].InnerText + "\">" + nodes[i].SelectSingleNode("atom:title",nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch { }

            return listResult;
        }
        #endregion

        public static string RemoveByReg(string sContent, string sRegex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sContent);
            foreach (Match mc in mcs)
            {
                sContent = sContent.Replace(mc.Value, "");
            }
            return sContent;
        }

        public static string ReplaceByReg(string sContent,string sReplace, string sRegex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            sContent = re.Replace(sContent, sReplace);
            return sContent;
        }

        public static string GetBody(string sContent)
        {
            Regex re = new Regex(@"[\s\S]*?<\bbody\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            sContent = re.Replace(sContent, "");

            re = new Regex(@"</\bbody\b[^>]*>\s*</html>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace|RegexOptions.RightToLeft );
            sContent = re.Replace(sContent, "");
            return sContent;
        } 
        #endregion ���ݳ����ӵ�ַ��ȡҳ������

        #region �����������ַ�������
        public static string GetTextByReg( string sContent, string sRegex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sContent);
            string str = "";
            if (mc.Success)
                str = mc.Groups[0].Value;
            while (str.EndsWith("_"))
            {
                str = CString.RemoveEndWith(str, "_");
            }
            return str;
        }

        // charset=[\s]*(?<Coding>[^'"]+)[\s]*['"]?[\s]*[/]?>
        public static string GetTextByReg(string sContent, string sRegex, string sGroupName)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sContent);
            string str = "";
            if (mc.Success)
                str = mc.Groups[sGroupName].Value; 
            return str; 
        }

        /// <summary>
        /// ������ӵľ���·��
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sRUrl"></param>
        /// <returns></returns>
        public static string  GetUrlByRelative(string sUrl,  string sRUrl)
        {
            try
            {
                //http://q.yesky.com/grp/dsc/view.do;jsessionid=A6324FD46B4893303124F70C0B2AAC1E?grpId=201595&rvId=8215876
                Uri baseUri = new Uri(sUrl);
                if (!sUrl.EndsWith("/"))
                {
                    int i = baseUri.Segments.Length - 1;
                    if (i > 0)
                    {
                        string file = baseUri.Segments[i];
                        if (file.IndexOf('.') < 1)
                        {

                            baseUri = new Uri(sUrl + "/");
                        }
                    }
                }

                Uri myUri = new Uri(baseUri, sRUrl);

                return myUri.AbsoluteUri;
            }
            catch
            {
                return sUrl;
            }
        }

        public static List<string> GetListByReg(string sContent, string sRegex)
        {
            List<string> list = new List<string>();
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            MatchCollection mcs = re.Matches(sContent);
            foreach(Match mc in mcs)
            {
                list.Add( mc.Groups["href"].Value);
            }
            return list;
        }

        public static string GetDomainUrl(string sUrl)
        {
            try
            {
                Uri baseUri = new Uri(sUrl);

                return baseUri.Scheme+ "://" + baseUri.Authority;
            }
            catch
            {
                return sUrl;
            }
            
            //Regex re = new Regex(@"http(s)?://([\w-]+\.)+(\w){2,}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            //Match mc1 = re.Match(sUrl);
            //if (mc1.Success)
            //{
            //    return mc1.Value;
            //}
            //else
            //    return "";
        }

        public static List<string> GetKeys(string sOri)
        {
            if (sOri.Trim().Length == 0)
            {
                return null;
            }

            //Regex re = new Regex("(,{1})|(��{1})", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            //string[] sArray = re.Split(sOri);
            string[] sArray = sOri.Split(new char[] {',','��','\\','/','��'});
            List<string> listStr = new List<string>();
            foreach (string sContent in sArray)
            {
                if (sContent.Length == 0)
                    continue;
                listStr.Add(sContent);                
            }
            return listStr;
        }

        public static string Split(string sOri)
        {  
            Regex re = new Regex("(,{1})|(��{1})|(\\+{1})|(��{1})|(��{1})|(;{1})|(��{1})|(��{1})|(:{1})|(��{1})|(��{1})|(��{1})|(_{1})",
                                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            string[] sArray = re.Split(sOri);
            List<string> listStr = new List<string>();
            listStr.Clear();
            foreach (string sContent in sArray)
            {
                if (sContent.Length <= 2)
                    continue;  
                re = new Regex(@"[a-zA-z]+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                MatchCollection mcs = re.Matches(sContent);
                string sTemp = sContent;
                foreach (Match mc in mcs)
                {
                    if(mc.Value.ToString().Length > 2)
                        listStr.Add(mc.Value.ToString());
                    sTemp = sTemp.Replace(mc.Value.ToString(), ",");
                }
                re=new Regex(@",{1}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                mcs = re.Matches(sTemp);
                foreach(string s in re.Split(sTemp))
                {
                    if(s.Trim().Length <=2)
                        continue;
                    listStr.Add(s);
                } 
            }
            string sReturn = "";
            for( int i=0; i<listStr.Count-1; i++ )
            {
                for (int j = i+1; j < listStr.Count; j++)
                {
                    if (listStr[i] == listStr[j])
                    {
                        listStr[j] = "";
                    }
                }
            }

            foreach (string str in listStr)
            {
                if(str.Length>2)
                    sReturn += str + ",";
            }
            if (sReturn.Length > 0)
                return sReturn.Substring(0, sReturn.Length - 1);
            else
                return sReturn ;
        }
        #endregion

        #region ��÷������ڡ�����
        public static DateTime GetCreateDate(string sContent, string sRegex)
        {
            DateTime dt = System.DateTime.Now;
            
            Regex re = new Regex(sRegex,  RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sContent);
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
                        iMinute  = int.Parse(sMintue);

                    dt = new DateTime(iYear, iMonth, iDay, iHour, iMinute, 0);                
                }
                catch { }
            }
            return dt;
        } 
        #endregion ��÷�������

        #region ����

        public static string GetTxtFromHtml(string sHtml)
        {
            string del = @"<head[^>]*>[\s\S]*?</head>";
            string content = CText.RemoveByReg(sHtml, del);

            del = @"(<script[^>]*>[\s\S]*?</script>)|(<IFRAME[^>]*>[\s\S]*?</IFRAME>)|(<style[^>]*>[\s\S]*?</style>|<title[^>]*>[\s\S]*?</title>|<meta[^>]*>|<option[^>]*>[\s\S]*?</option>)";
            content = CText.RemoveByReg(content, del);

            del = @"(&nbsp;)|([\n\t]+)";
            content = CText.RemoveByReg(content, del);

            string re = @"(<table(\s+[^>]*)*>)|(<td(\s+[^>]*)*>)|(<tr(\s+[^>]*)*>)|(<p(\s+[^>]*)*>)|(<div(\s+[^>]*)*>)|(<ul(\s+[^>]*)*>)|(<li(\s+[^>]*)*>)|</table>|</td>|</tr>|</p>|<br>|</div>|</li>|</ul>|<p />|<br />";
            content = CText.ReplaceByReg(content, "", re);
            content = CText.ReplaceByReg(content, "", @"[\f\n\r\v]+");

            content = CText.RemoveByReg(content, @"<a(\s+[^>]*)*>[\s\S]*?</a>");
            content = CText.RemoveByReg(content, "<[^>]+>");//ȥ������HTML��ǣ���ô�����

            content = content.Replace("\n", "");
            content = content.Replace("\r", "");
            content = content.Trim();
            return content;
        }

        /// <summary>
        /// ��GetTxtFromHtml����һ���������������з���
        /// </summary>
        /// <param name="sHtml"></param>
        /// <returns></returns>
        public static string GetTxtFromHtml2(string sHtml)
        {
            string del = @"<head[^>]*>[\s\S]*?</head>";
            string content = CText.RemoveByReg(sHtml, del);

            del = @"(<script[^>]*>[\s\S]*?</script>)|(<IFRAME[^>]*>[\s\S]*?</IFRAME>)|(<style[^>]*>[\s\S]*?</style>|<title[^>]*>[\s\S]*?</title>|<meta[^>]*>|<option[^>]*>[\s\S]*?</option>)";
            content = CText.RemoveByReg(content, del);

            del = @"(&nbsp;)|([\t]+)";//del = @"(&nbsp;)|([\n\t]+)";
            content = CText.RemoveByReg(content, del);

            string re = @"(<table(\s+[^>]*)*>)|(<td(\s+[^>]*)*>)|(<tr(\s+[^>]*)*>)|(<p(\s+[^>]*)*>)|(<div(\s+[^>]*)*>)|(<ul(\s+[^>]*)*>)|(<li(\s+[^>]*)*>)|</table>|</td>|</tr>|</p>|<br>|</div>|</li>|</ul>|<p />|<br />";
            content = CText.ReplaceByReg(content, "", re);
            //content = CText.ReplaceByReg(content, "", @"[\f\n\r\v]+");

            content = CText.RemoveByReg(content, @"<a(\s+[^>]*)*>[\s\S]*?</a>");
            content = CText.RemoveByReg(content, "<[^>]+>");//ȥ������HTML��ǣ���ô�����
            content = content.Trim();

            return content;
        }
        #endregion
    }
}
