using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using log4net;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// Log4Net��־��¼������
    /// </summary>
    public class LogHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Debug(object ex)
        {
            Log.Debug(ex);
        }

        public static void Warn(object ex)
        {
            Log.Warn(ex);
        }

        public static void Error(object ex)
        {
            Log.Error(ex);
        }

        public static void Info(object ex)
        {
            Log.Info(ex);
        }

        public static void Debug(object message, Exception ex)
        {
            Log.Debug(message, ex);
        }

        public static void Warn(object message, Exception ex)
        {
            Log.Warn(message, ex);
        }

        public static void Error(object message, Exception ex)
        {
            Log.Error(message, ex);
        }

        public static void Info(object message, Exception ex)
        {
            Log.Info(message, ex);
        }
    }

    /// <summary>
    /// �ı���־��¼������
    /// </summary>
    public class LogTextHelper
    {                
        static string LogFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
        public static bool RecordLog = true;
        public static bool DebugLog = false;
        static LogTextHelper()
        {
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
        }

        public static void WriteLine(string message)
        {
            string temp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]    ") + message + "\r\n\r\n";
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                if (RecordLog)
                {
                    File.AppendAllText(Path.Combine(LogFolder, fileName), temp, Encoding.GetEncoding("GB2312"));
                }
                if (DebugLog)
                {
                    Console.WriteLine(temp);
                }
            }
            catch
            {
            }
        }

        public static void WriteLine(string className, string funName, string message)
        {
            WriteLine(string.Format("{0}��{1}\r\n{2}", className, funName, message));
        }
    }
}
