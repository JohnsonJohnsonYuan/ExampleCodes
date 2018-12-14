using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace WHC.OrderWater.Commons
{
	/// <summary>
	/// clsPublic ��ժҪ˵����
    /// </summary>
  
    public class CSocket
	{
        #region ���ݳ����ӵ�ַ��ȡҳ������
        public static string GetHtmlByUrl(string sUrl)
        {
            return GetHtmlByUrl(sUrl, "auto");
        }

        public static string GetHtmlByUrl(string sUrl, string sCoding)
        {
            return GetHtmlByUrl(ref sUrl, sCoding);
        }
		public static string  GetHtmlByUrl(ref string sUrl, string sCoding)
		{
            string content = "";

            try
            {
                HttpWebResponse response = _MyGetResponse(sUrl);
                if (response == null)
                {
                    return content;
                }

                sUrl = response.ResponseUri.AbsoluteUri;

                Stream stream = response.GetResponseStream();
                byte[] buffer = GetContent(stream);
                stream.Close();
                stream.Dispose();

                string charset = "";
                if (sCoding == null || sCoding == "" || sCoding.ToLower() == "auto")
                {//�����ָ�����룬��ôϵͳ��Ϊָ��

                    //���ȣ��ӷ���ͷ��Ϣ��Ѱ��
                    string ht = response.GetResponseHeader("Content-Type");
                    response.Close();
                    string regCharSet = "[\\s\\S]*charset=(?<charset>[\\S]*)";
                    Regex r = new Regex(regCharSet, RegexOptions.IgnoreCase);
                    Match m = r.Match(ht);
                    charset = (m.Captures.Count != 0) ? m.Result("${charset}") : "";
                    if (charset == "-8") charset = "utf-8";

                    if (charset == "")
                    {//�Ҳ����������ļ���Ϣ�����в���

                        //�Ȱ�gb2312����ȡ�ļ���Ϣ
                        content = System.Text.Encoding.GetEncoding("gb2312").GetString(buffer);

                        regCharSet = "(<meta[^>]*charset=(?<charset>[^>'\"]*)[\\s\\S]*?>)|(xml[^>]+encoding=(\"|')*(?<charset>[^>'\"]*)[\\s\\S]*?>)";
                        r = new Regex(regCharSet, RegexOptions.IgnoreCase);
                        m = r.Match(content);
                        if (m.Captures.Count == 0)
                        {//û�취�����Ҳ������룬ֻ�ܷ��ذ�"gb2312"��ȡ����Ϣ
                            //content = CText.RemoveByReg(content, @"<!--[\s\S]*?-->");
                            return content;
                        }
                        charset = m.Result("${charset}");
                    }
                }
                else
                {
                    response.Close();
                    charset = sCoding.ToLower();
                }

                try
                {
                    content = System.Text.Encoding.GetEncoding(charset).GetString(buffer);
                }
                catch (ArgumentException)
                {//ָ���ı��벻��ʶ��
                    content = System.Text.Encoding.GetEncoding("gb2312").GetString(buffer);
                }

                //content = CText.RemoveByReg(content, @"<!--[\s\S]*?-->");
            }
            catch
            {
                content = "";
            }

            return content;
        }
        private static HttpWebResponse _MyGetResponse(string sUrl)
        {
            int iTimeOut = 10000;
            //try
            //{
            //    //iTimeOut = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SocketTimeOut"]);
            //}
            //catch { iTimeOut = 10000; }

            bool bCookie = false;
            bool bRepeat = false;
            Uri target = new Uri(sUrl);

            ReCatch:
            try
            {
                HttpWebRequest resquest = (HttpWebRequest)WebRequest.Create(target);
                resquest.MaximumResponseHeadersLength = -1;
                resquest.ReadWriteTimeout = 120000;//120��ͳ�ʱ
                resquest.Timeout = iTimeOut;
                resquest.MaximumAutomaticRedirections = 50;
                resquest.MaximumResponseHeadersLength = 5;
                resquest.AllowAutoRedirect = true;
                if (bCookie)
                {
                    resquest.CookieContainer = new CookieContainer();
                }
                resquest.UserAgent = "Mozilla/6.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                //resquest.UserAgent = @"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.1) Web-Sniffer/1.0.24";
                //resquest.KeepAlive = true;
                return (HttpWebResponse)resquest.GetResponse();
            }
            catch (WebException we)
            {
                if (!bRepeat)
                {
                    bRepeat = true;
                    bCookie = true;
                    goto ReCatch;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        private static byte[] GetContent(Stream stream)
        {
            ArrayList arBuffer = new ArrayList();
            const int BUFFSIZE = 4096;

            try
            {
                byte[] buffer = new byte[BUFFSIZE];
                int count = stream.Read(buffer, 0, BUFFSIZE);
                while (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        arBuffer.Add(buffer[i]);
                    }
                    count = stream.Read(buffer, 0, BUFFSIZE);
                }
            }
            catch {}

            return (byte[])arBuffer.ToArray(System.Type.GetType("System.Byte"));
        }

        public static string GetHttpHead(string sUrl)
        {
            string sHead = "";
            Uri uri = new Uri(sUrl);
            try
            {
                WebRequest req = WebRequest.Create(uri);
                WebResponse resp = req.GetResponse();
                WebHeaderCollection headers = resp.Headers;
                string[] sKeys = headers.AllKeys;
                foreach (string sKey in sKeys)
                {
                    sHead += sKey + ":" + headers[sKey] + "\r\n";
                }
            }
            catch
            {
            }
            return sHead;
        }

        /// <summary>
        /// ������ҳ�����⡣�����ҳ���ǿ�ܽṹ�Ļ������ظÿ��
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] DealWithFrame(string url,string content)
        {
            string regFrame = @"<frame\s+[^>]*src\s*=\s*(?:""(?<src>[^""]+)""|'(?<src>[^']+)'|(?<src>[^\s>""']+))[^>]*>";
            return DealWithFrame(regFrame,url,content);
        }

        /// <summary>
        /// �����������⡣�����ҳ����ڸ����壬���ظ�����
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] DealWithIFrame(string url,string content)
        {
            string regiFrame = @"<iframe\s+[^>]*src\s*=\s*(?:""(?<src>[^""]+)""|'(?<src>[^']+)'|(?<src>[^\s>""']+))[^>]*>";
            return DealWithFrame(regiFrame, url, content);
        }

        private static string[] DealWithFrame(string strReg, string url,string content)
        {
            ArrayList alFrame = new ArrayList();
            Regex r = new Regex(strReg, RegexOptions.IgnoreCase);
            Match m = r.Match(content);
            while (m.Success)
            {
                alFrame.Add(CRegex.GetUrl(url, m.Groups["src"].Value));
                m = m.NextMatch();
            }

            return (string[])alFrame.ToArray(System.Type.GetType("System.String"));
        }

        #endregion ���ݳ����ӵ�ַ��ȡҳ������

        #region ��ö��ҳ��
        public static List<KeyValuePair<int, string>>  GetHtmlByUrlList( List<KeyValuePair<int, string>>  listUrl, string sCoding)
        {
            int iTimeOut = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SocketTimeOut"]);
            StringBuilder sbHtml = new StringBuilder();
            List<KeyValuePair<int, string>> listResult = new List<KeyValuePair<int,string>>();
            int nBytes = 0;
            Socket sock = null;
            IPHostEntry ipHostInfo = null;
            try
            {                
                // ��ʼ��				
                Uri site = new Uri(listUrl[0].Value.ToString());              
                try
                {
                    ipHostInfo = System.Net.Dns.GetHostEntry(site.Host);
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, site.Port);
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.SendTimeout = iTimeOut;
                sock.ReceiveTimeout = iTimeOut;
                try
                {
                    sock.Connect(remoteEP);
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
                foreach (KeyValuePair<int,string>  kvUrl in listUrl)
                {
                    site = new Uri(kvUrl.Value);
                    string sendMsg = "GET " + HttpUtility.UrlDecode(site.PathAndQuery) + " HTTP/1.1\r\n" +
                        "Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/msword, application/vnd.ms-powerpoint, */*\r\n" +
                        "Accept-Language:en-us\r\n" +
                        "Accept-Encoding:gb2312, deflate\r\n" +
                        "User-Agent: Mozilla/4.0\r\n" +
                        "Host: " + site.Host + "\r\n\r\n" + '\0';
                    // ����
                    byte[] msg = Encoding.GetEncoding(sCoding).GetBytes(sendMsg);
                    if ((nBytes = sock.Send(msg)) == 0)
                    {
                        sock.Shutdown(SocketShutdown.Both);
                        sock.Close();
                        return listResult;
                    }
                    // ����
                    byte[] bytes = new byte[2048];
                    byte bt = Convert.ToByte('\x7f');
                    do
                    {    
                        int count = 0;
                        try
                        {
                            nBytes = sock.Receive(bytes, bytes.Length - 1, 0);
                        }
                        catch (Exception Ex)
                        {
                            string str = Ex.Message;
                            nBytes = -1;
                        }
                        if (nBytes <= 0) break;
                        if (bytes[nBytes - 1] > bt)
                        {
                            for (int i = nBytes - 1; i >= 0; i--)
                            {
                                if (bytes[i] > bt) 
                                    count++;
                                else
                                    break;
                            }
                            if (count % 2 == 1)
                            {
                                count = sock.Receive(bytes, nBytes, 1, 0);
                                if (count < 0) 
                                    break;
                                nBytes = nBytes + count;
                            }
                        }
                        else
                            bytes[nBytes] = (byte)'\0';
                        string s = Encoding.GetEncoding(sCoding).GetString(bytes, 0, nBytes);
                        sbHtml.Append(s);
                    } while (nBytes > 0);

                    listResult.Add(new KeyValuePair<int, string>(kvUrl.Key,  sbHtml.ToString()));
                    sbHtml = null;
                    sbHtml = new StringBuilder();
                }
            }
            catch (Exception Ex)
            {
                string s = Ex.Message;
                try
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
                catch { }
            }
            return listResult;          
        }
        #endregion ���ݳ����ӵ�ַ��ȡҳ������

        public enum PageType : int { HTML = 0, RSS };
        public static PageType GetPageType(string sUrl,ref string sHtml)
        {
            PageType pt = PageType.HTML;

            //����û��RSS FEED
            string regRss = @"<link\s+[^>]*((type=""application/rss\+xml"")|(type=application/rss\+xml))[^>]*>";
            Regex r = new Regex(regRss, RegexOptions.IgnoreCase);
            Match m = r.Match(sHtml);
            if (m.Captures.Count != 0)
            {//�У���ת���RSS FEED��ץȡ
                string regHref = @"href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))";
                r = new Regex(regHref, RegexOptions.IgnoreCase);
                m = r.Match(m.Captures[0].Value);
                if (m.Captures.Count > 0)
                {
                    //�п��������·�������Ͼ���·��
                    string rssFile = CRegex.GetUrl(sUrl, m.Groups["href"].Value);
                    sHtml = GetHtmlByUrl(rssFile);
                    pt = PageType.RSS;
                }
            }
            else
            {//�������ַ�����ǲ���һ��Rss feed
                r = new Regex(@"<rss\s+[^>]*>", RegexOptions.IgnoreCase);
                m = r.Match(sHtml);
                if (m.Captures.Count > 0)
                {
                    pt = PageType.RSS;
                }
            }

            return pt;
        }
    }
}
