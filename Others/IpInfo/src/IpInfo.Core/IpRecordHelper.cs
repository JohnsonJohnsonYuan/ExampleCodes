using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using IpInfo.Core.Extensions;
using IpInfo.Core.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;
using IpInfo.Core.Services;

namespace IpInfo.Core
{
    public static class IpRecordHelper
    {
        private static Regex ipRegex = new Regex(@"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static SearchService searchService = new SearchService();

        #region Utilities

        /// <summary>
        /// 读取处理过的ip数据库
        /// </summary>
        /// <param name="ipFilePath">input文件为处理过的输入</param>
        /// <param name="specialDic"></param>
        /// <returns></returns>
        public static IpRecord[] LoadIpRecordResult(string ipFilePath)
        {
            List<IpRecord> ipRecords = new List<IpRecord>();

            using (StreamReader reader = new StreamReader(ipFilePath, Encoding.Default))
            {
                reader.ReadLine(); // 第一行是标题，不用读取

                int id = 1;
                string lineOfText;
                while ((lineOfText = reader.ReadLine()) != null)
                {
                    var strArray = lineOfText.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    var ipRecord = new IpRecord
                        {
                            Id = id++,
                            StartIpNumber = long.Parse(strArray[0]),
                            EndIpNumber = long.Parse(strArray[1]),
                            StartIp = strArray[2].Trim(),
                            EndIp = strArray[3].Trim(),
                            Address = strArray[4].Trim(),
                            AreaCode = strArray[5].Trim(),
                            FoundResultStr = strArray[6].Trim()
                        };

                    if (strArray.Length == 8)
                    {
                        ipRecord.Note = strArray[7].Trim();
                    }

                    ipRecords.Add(ipRecord);
                }
            }

            return ipRecords.ToArray();
        }

        /// <summary>
        /// 读取ip数据库
        /// </summary>
        /// <param name="ipFilePath">input文件为未处理的输入</param>
        /// <param name="specialDic"></param>
        /// <returns></returns>
        public static IpRecord[] LoadIpDataFile(string ipFilePath, Dictionary<string, string> specialDic = null)
        {
            List<IpRecord> ipRecords = new List<IpRecord>();

            bool useDic = specialDic != null && specialDic.Keys.Count > 0;
            using (StreamReader reader = new StreamReader(ipFilePath, Encoding.Default))
            {
                string lineOfText;
                while ((lineOfText = reader.ReadLine()) != null)
                {
                    var strArray = lineOfText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (strArray.Length >= 3)
                    {
                        var address = strArray[2].Trim();

                        if (useDic && specialDic.ContainsKey(address))
                        {
                            address = specialDic[address];
                        }

                        ipRecords.Add(new IpRecord
                        {
                            StartIp = strArray[0].Trim(),
                            EndIp = strArray[1].Trim(),
                            Address = address
                        });
                    }
                }
            }
            return ipRecords.ToArray();
        }

        /// <summary>
        /// 读取特殊地址对应的地区
        /// 
        /// 太原科技大学,山西省太原市
        /// 长江大学,湖北省荆州市
        /// 长江大学东校区,湖北省荆州市
        /// </summary>
        public static Dictionary<string, string> LoadDicFile(string dictionaryFilePath)
        {
            if (string.IsNullOrEmpty(dictionaryFilePath))
                return null;

            Dictionary<string, string> result = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(dictionaryFilePath, Encoding.Default))
            {
                string lineOfText;
                while ((lineOfText = reader.ReadLine()) != null)
                {
                    var tmpArr = lineOfText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tmpArr.Length != 2)
                        continue;

                    if (!result.ContainsKey(tmpArr[0]))
                        result.Add(tmpArr[0], tmpArr[1]);
                }
            }

            return result;
        }

        /// <summary>
        /// 读取文件编码
        /// 北京市	010000
        /// 北京市昌平区	010100
        /// 北京市朝阳区	010200
        /// 北京市崇文区	010300
        /// 北京市大兴区	010400
        /// 北京市东城区	010500
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> LoadAreaCodeFile(string filePath)
        {
            // 文件包含不是数据的行， 跳过这些内容
            // 地区	编码
            // 北京市	010000
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
            {
                string lineOfText;
                while ((lineOfText = reader.ReadLine()) != null)
                {
                    var tmpArr = lineOfText.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tmpArr.Length == 2)
                    {
                        if (!result.ContainsKey(tmpArr[0]))
                            result.Add(tmpArr[0], tmpArr[1]);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Methods

        public static bool IsStrRegex(string ipAddr)
        {
            return ipRegex.IsMatch(ipAddr);
        }

        /// <summary>
        /// 使用二分法和斐波那契查找ip
        /// </summary>
        /// <param name="ipAddr">要查找的ip</param>
        /// <param name="ipRecords">所有的ip</param>
        public static void SearchIpRecord(string ipAddr, IpRecord[] ipRecords, out SearchRecord binary_recorder, out SearchRecord fibonacci_recorder)
        {
            if (!IsStrRegex(ipAddr))
                throw new FormatException();

            Stopwatch sw = Stopwatch.StartNew();

            searchService.Binary_Search(ipRecords, ipAddr.IpAddressToInt(), out binary_recorder);

            sw.Stop();
            binary_recorder.ElapsedMilliseconds = sw.Elapsed.TotalMilliseconds;


            sw.Restart();
            
            searchService.Fibonacci_Search_Improved(ipRecords, ipAddr.IpAddressToInt(), out fibonacci_recorder);

            sw.Stop();
            fibonacci_recorder.ElapsedMilliseconds = sw.Elapsed.TotalMilliseconds;

            if (binary_recorder.Index != fibonacci_recorder.Index)
                throw new Exception("算法出错, 查找结果不同！");

        }

        #endregion
    }
}
