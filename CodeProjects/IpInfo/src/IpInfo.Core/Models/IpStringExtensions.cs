using System;

namespace IpInfo.Core.Extensions
{
    public static class IpStringExtensions
    {
        public static long IpAddressToInt(this string ipAddr)
        {
            if (string.IsNullOrEmpty(ipAddr))
            {
                throw new ArgumentNullException("ipAddr");
            }

            string[] ipArr = ipAddr.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (ipArr.Length != 4)
            {
                throw new FormatException("ipAddr format error: " + ipArr);
            }

            return Int64.Parse(ipArr[0]) * 256 * 256 * 256
                + Int64.Parse(ipArr[1]) * 256 * 256
                + Int64.Parse(ipArr[2]) * 256
                + Int64.Parse(ipArr[3]);
        }
    }
}
