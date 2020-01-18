using NLog;

namespace OperaHtmlParser
{
    public static class HtmlParseLogger
    {
        // default log
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static Logger _mp3TagLogger;
        public static void Info(string info)
        {
            _logger.Info(info);
        }

        public static void Error(string info)
        {
            _logger.Error(info);
        }

        public static void TagError(string info)
        {
            if (_mp3TagLogger == null)
                _mp3TagLogger = LogManager.GetLogger("mp3TagLog", typeof(HtmlParseLogger));
        
            _mp3TagLogger.Error(info);
        }
    }
}