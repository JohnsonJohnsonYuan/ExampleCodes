using System.IO;
using System.Text;

namespace OperaHtmlParser
{
    public static class StaticVariables
    {
        public static string MP4_FOLDER_NAME = "mp4";
        // 专辑原封面(下载自网站)
        public static string ALBUM_COVER_NAME = "_cover.jpg";
        // 手动裁剪之后封面名称
        // public static string ALBUM_MP3_COVER_NAME = "_avatar.jpg";
        public static string ALBUM_MP3_COVER_NAME = "FormatFactory_avatar.jpg";
        
        public static string HOST_NAME = "http://www.bavc.com.cn";
        public static Encoding GB2312Encoding = Encoding.GetEncoding("GB2312");

        public static string GetDownloadDir()
        {
            var downloadDir = Path.Combine(Directory.GetCurrentDirectory(), "_downloads");
            
            if (!Directory.Exists(downloadDir))
                Directory.CreateDirectory(downloadDir);
            return downloadDir;
        }
    }
}