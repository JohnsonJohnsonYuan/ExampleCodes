using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using HtmlAgilityPack;

namespace BiliBiliHtmlParser
{
    public class Program
    {
        // {0}: userid, {1}: page number
        static string VIDEO_LIST_URL = "https://space.bilibili.com/{0}/video?tid=0&page={1}&keyword=&order=pubdate";
        //static string VIDEO_LIST_URL = "https://space.bilibili.com/{0}/video";

        static string HEADER = "标题\t时长\tURL\t上传时间\t当前页码";

        static HtmlWeb _web;
        static string _activeUserId;

        // 没有 STAThread 属性LoadFromBrowser会报错
        [STAThread]
        static void Main(string[] args)
        {
            // 348050423: 剑如长虹气如玉
            _activeUserId = ConfigurationManager.AppSettings["UserId"];
            if (_activeUserId == null)
                throw new Exception("AppSettings未配置UserId");

            var firstPageUrl = GetVideoUrl(_activeUserId, 1);

            _web = new HtmlWeb();
            var document = _web.LoadFromBrowser(firstPageUrl, o =>
            {
                var webBrowser = (WebBrowser)o;

                // WAIT until the dynamic text is set
                return webBrowser.Document.GetElementById("submit-video-list") != null && webBrowser.Document.GetElementById("submit-video-list").Children[1].Children.Count > 0;
            });


            var pageNumNode = document.DocumentNode.SelectSingleNode(".//div[@id='submit-video-list']/ul[@class='be-pager']/span[@class='be-pager-total']");
            // 总页数
            var pageCount = GetPageCount(pageNumNode);
            var totalCountNode = document.DocumentNode.SelectSingleNode(".//div[@id='submit-video-type-filter']/a[1]/span");
            var totalCount = totalCountNode.InnerText;

            Console.WriteLine($"总页数: { pageCount}, 总个数: {totalCount}");

            if (pageCount <= 0)
            {
                return;
            }

            StringBuilder result = new StringBuilder(500);
            result.AppendLine($"总数: {totalCount}, 生成时间{DateTime.Now}");
            result.AppendLine(HEADER);
            for (int i = 1; i <= pageCount; i++)
            {
                Console.WriteLine($"第{i}页读取中...");
                var videoInfo = GetVideosFromNode(i);
                result.Append(videoInfo);
            }


            System.IO.File.WriteAllText($"export-{_activeUserId}.csv", result.ToString());

            Console.WriteLine("Load Complete");
        }

        static string GetVideosFromNode(int pageNum)
        {
            var videoUrl = GetVideoUrl(_activeUserId, pageNum);
            //var document = _web.LoadFromBrowser(videoUrl);

            var document = _web.LoadFromBrowser(videoUrl, o =>
            {
                var webBrowser = (WebBrowser)o;

                // WAIT until the dynamic text is set
                return webBrowser.Document.GetElementById("submit-video-list") != null && webBrowser.Document.GetElementById("submit-video-list").Children[1].Children.Count > 0;
            });

            // 第一个ul是详细模式, 第二个ul是表格模式
            var videoNodes = document.DocumentNode.SelectNodes(".//div[@id='submit-video-list']/ul[2]/li");

            if (videoNodes == null)
            {
                System.Console.WriteLine("no nodes");
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();
            foreach (var item in videoNodes)
            {
                var titleNode = item.SelectSingleNode(".//a[@class='title']");
                var title = titleNode.Attributes["title"].Value;
                var url = "https:" + titleNode.Attributes["href"].Value;

                var videoTimeNode = item.SelectSingleNode(".//a[@class='cover']/span[@class='length']");
                var videoTime = videoTimeNode.InnerText;

                var metaNode = item.SelectSingleNode(".//div[@class='meta']/span[@class='time']");
                var createTime = metaNode.InnerText.TrimEnd(' ', '\n');

                result.AppendLine($"{title}\t{videoTime}\t{url}\t{createTime}\t{pageNum}");
            }

            return result.ToString();
        }

        /// <summary>
        /// 返回当前页码url
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        static string GetVideoUrl(string userId, int pageNumber = 1)
        {
            return string.Format(VIDEO_LIST_URL, userId, pageNumber);
        }

        /// <summary>
        /// 返回总页数
        /// </summary>
        /// <param name="pageNumNode"></param>
        /// <returns></returns>
        static int GetPageCount(HtmlNode pageNumNode)
        {
            if (pageNumNode == null)
                return -1;

            StringBuilder num = new StringBuilder();
            using (StringReader reader = new StringReader(pageNumNode.InnerText))
            {
                int curr;
                while ((curr = reader.Read()) != -1)
                {
                    var ch = (char)curr;
                    if (char.IsDigit(ch))
                    {
                        num.Append(ch);
                    }
                }
            }

            return Int32.Parse(num.ToString());
        }
    }
}
