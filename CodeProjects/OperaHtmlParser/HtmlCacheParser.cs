using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace OperaHtmlParser
{
    public static class HtmlCacheParser
    {
        private static string _CACHE_FOLDER = "_htmlCache";
        private static HashSet<string> _allCacheKeys;
        private static HtmlWeb _web;

        static HtmlCacheParser()
        {
            _allCacheKeys = new HashSet<string>();

            if (Directory.Exists(_CACHE_FOLDER))
                Directory.CreateDirectory(_CACHE_FOLDER);
        }

        /// <summary>
        /// Create a static instance of the Nop engine.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static HtmlWeb Create(Encoding encoding)
        {
            var web = new HtmlWeb();
            //web.AutoDetectEncoding = false;
            web.OverrideEncoding = encoding;
            return web;
        }

        public static HtmlNode LoadHtmlNode(string key)
        {
            var html = LoadHtmlFromCache(key);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode;
        }

        public static string LoadHtmlFromCache(string url)
        {
            // remove hostname prefix
            var cacheKey = GetCacheKey(url);

            var cacheDir = Path.Combine(Directory.GetCurrentDirectory(), _CACHE_FOLDER);

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            if (!cacheKey.Contains(cacheKey))
                _allCacheKeys.Add(cacheKey);

            var filePath = Path.Combine(cacheDir, cacheKey);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath, StaticVariables.GB2312Encoding);
            }

            var htmlResult = Current.Load(url);
            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
                htmlResult.Save(fs, StaticVariables.GB2312Encoding);
            }

            return File.ReadAllText(filePath, StaticVariables.GB2312Encoding);
        }

        public static string LoadHtmlNodeFromBrowser(string url)
        {
            // remove hostname prefix
            var cacheKey = GetCacheKey(url);

            var cacheDir = Path.Combine(Directory.GetCurrentDirectory(), _CACHE_FOLDER);

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            if (!cacheKey.Contains(cacheKey))
                _allCacheKeys.Add(cacheKey);

            var filePath = Path.Combine(cacheDir, cacheKey);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath, StaticVariables.GB2312Encoding);
            }
            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
                //htmlResult.Save(fs, StaticVariables.GB2312Encoding);
            }

            return File.ReadAllText(filePath, StaticVariables.GB2312Encoding);
        }

        private static HtmlWeb Current
        {
            get
            {
                if (_web == null)
                {
                    _web = Create(StaticVariables.GB2312Encoding);

                    // default timeout: 100,000 milliseconds (100 seconds).
                    // _web.PreRequest = (webRequest) =>
                    // {
                    //     webRequest.Timeout = 4;
                    //     return true;
                    // };
                }

                return _web;
            }
        }
        private static string GetCacheKey(string url)
        {
            // remove hostname prefix
            var cacheKey = url;
            var hostNameIndex = url.IndexOf(StaticVariables.HOST_NAME, StringComparison.InvariantCultureIgnoreCase);
            if (hostNameIndex > -1)
            {
                cacheKey = url.Substring(StaticVariables.HOST_NAME.Length + 1);
            }

            cacheKey = RemoveIllegalFileNameChars(cacheKey);

            return cacheKey;
        }
        public static string RemoveIllegalFileNameChars(string input, string replacement = "")
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(input, replacement);
        }
    }
}