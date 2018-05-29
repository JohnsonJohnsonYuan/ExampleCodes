using System;
using System.Collections.Generic;
using System.IO;
using IpInfo.Core.Extensions;
using IpInfo.Core.Models;

namespace IpInfo.Core.Services
{
    public class IpRecordService
    {
        #region Methods

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="ipFilePath"></param>
        /// <param name="dictionaryFilePath"></param>
        /// <param name="areaAndCodeFilePath"></param>
        /// <returns></returns>
        public IpRecord[] LoadIpRecord(string ipFilePath, string dictionaryFilePath, string areaAndCodeFilePath, string specialReplaceFilePath = null)
        {
            if (string.IsNullOrEmpty(ipFilePath))
                throw new ArgumentNullException("ipFilePath");

            if (string.IsNullOrEmpty(dictionaryFilePath))
                throw new ArgumentNullException("dictionaryFilePath");

            var specialDic = IpRecordHelper.LoadDicFile(specialReplaceFilePath);

            var ipRecords = IpRecordHelper.LoadIpDataFile(ipFilePath, specialDic);

            var areaAndCode = IpRecordHelper.LoadAreaCodeFile(areaAndCodeFilePath);
            var dictionary = IpRecordHelper.LoadDicFile(dictionaryFilePath);

            Console.WriteLine("total: " + ipRecords.Length);
            int count = 0;

            foreach (var ipRecord in ipRecords)
            {
                ++count;
                if (count % 5000 == 0)
                {
                    Console.WriteLine(count);
                }

                bool found = false;
                // 1. 地区编码查找
                if (areaAndCode.ContainsKey(ipRecord.Address))
                {
                    ipRecord.AreaCode = areaAndCode[ipRecord.Address];
                    ipRecord.FoundResult = FoundResult.ExactMatch;
                    found = true;
                    continue;
                }

                // 2. 特殊地址查找(字典表)
                if (!found)
                {
                    if (dictionary.ContainsKey(ipRecord.Address))
                    {
                        // 编码表中查找
                        if (areaAndCode.ContainsKey(dictionary[ipRecord.Address]))
                        {
                            ipRecord.AreaCode = areaAndCode[dictionary[ipRecord.Address]];
                            ipRecord.FoundResult = FoundResult.Dictionary;
                            ipRecord.Note = ipRecord.Address;
                            ipRecord.Address = dictionary[ipRecord.Address];
                            found = true;
                            continue;
                        }
                    }
                }

                // 3. 模糊查找
                // 甘肃省天水市秦安县
                // 找到编码为:
                // 甘肃省天水市	281200
                if (!found)
                {
                    foreach (string key in areaAndCode.Keys)
                    {
                        if (ipRecord.Address.StartsWith(key))
                        {
                            ipRecord.AreaCode = areaAndCode[key];
                            ipRecord.FoundResult = FoundResult.Match;
                            ipRecord.Note = ipRecord.Address;

                            if (specialDic != null && specialDic.ContainsKey(key))
                            {
                                ipRecord.Address = specialDic[key];
                            }
                            else
                            {
                                ipRecord.Address = key;
                            }

                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    ipRecord.AreaCode = string.Empty;
                    ipRecord.FoundResult = FoundResult.None;
                }
            }

            return ipRecords;
        }

        /// <summary>
        /// 把ip段重合的记录合并为一条
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public IpRecord[] MergeIpRecord(IpRecord[] records)
        {
            if (records == null || records.Length == 0)
                return new IpRecord[0];

            if (records.Length == 1)
                return records;

            List<IpRecord> result = new List<IpRecord>();

            IpRecord currVal = records[0];
            string startIP = records[0].StartIp;
            string endIP = records[0].EndIp;

            for (int i = 1; i < records.Length; i++)
            {
                IpRecord nextVal = records[i];
                // 如果不可以合并 就增加该记录到结果
                bool addElement = false;

                if (string.Equals(currVal.Address, nextVal.Address, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (nextVal.StartIpNumber == (endIP.IpAddressToInt() + 1))
                    {
                        // 可以合并
                        endIP = nextVal.EndIp;

                        if (i == records.Length - 1)
                        {
                            // 如果到了最后一条记录, 且可以合并
                            addElement = true;
                        }
                    }
                    else
                    {
                        addElement = true;
                    }
                }
                else
                {
                    // 不可以合并
                    addElement = true;
                }

                if (addElement)
                {
                    currVal.StartIp = startIP;
                    currVal.EndIp = endIP;
                    result.Add(currVal);

                    currVal = nextVal;
                    startIP = nextVal.StartIp;
                    endIP = nextVal.EndIp;
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 保存ip信息
        /// </summary>
        /// <param name="ipRecords"></param>
        /// <param name="savePath"></param>
        public void SaveIpRecord(Models.IpRecord[] ipRecords, string savePath)
        {
            using (StreamWriter writer = new StreamWriter(savePath, false, System.Text.Encoding.Unicode))
            {
                writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
                "\t", "StartNumber", "EndNumber", "StartIp", "EndIp", "地址", "编码", "查找结果", "备注"));

                if (ipRecords == null)
                    return;

                foreach (var record in ipRecords)
                {
                    writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
                    "\t",
                    record.StartIpNumber,
                    record.EndIpNumber,
                    record.StartIp,
                    record.EndIp,
                    record.Address,
                    record.AreaCode,
                    record.FoundResultStr,
                    record.Note));
                }
            }
        }

        #endregion
    }
}