namespace System.Web.Util
{
    using System;
    using System.Diagnostics;

    ///<summary>
    ///Source: https://referencesource.microsoft.com/#System.Web/Util/HttpEncoderUtility.cs,1cfba9359f8c05ec
    /// HexToInt: 十六进制字符转换为int
    /// IntToHex: int转换为十六进制字符
    /// IsUrlSafeChar: url合法字符 
    /// UrlEncodeSpaces: 空格转换为%20
    /// </summary>
    internal static class HttpEncoderUtility {
 
        public static int HexToInt(char h) {
            return (h >= '0' && h <= '9') ? h - '0' :
            (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
            (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
            -1;
        }
 
        public static char IntToHex(int n) {
            Debug.Assert(n < 0x10);	// 0x10 为十进制16
 
            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'a');
        }
 
        // Set of safe chars, from RFC 1738.4 minus '+'
        public static bool IsUrlSafeChar(char ch) {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
                return true;
 
            switch (ch) {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
                    return true;
            }
 
            return false;
        }
 
        //  Helper to encode spaces only
        internal static String UrlEncodeSpaces(string str) {
            if (str != null && str.IndexOf(' ') >= 0)
                str = str.Replace(" ", "%20");
            return str;
        }
 
    }
}
