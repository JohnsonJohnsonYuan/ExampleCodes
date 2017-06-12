using System;
using System.Reflection;
using IpInfo.Core.Extensions;

namespace IpInfo.Core.Models
{
    public static class FoundResultExtensions
    {
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length == 1)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }

    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    public enum FoundResult
    {
        [Description("没找到")]
        None,
        [Description("完全匹配")]
        ExactMatch,
        [Description("模糊查找")]
        Match,
        [Description("字典中找到")]
        Dictionary
    }

    /// <summary>
    /// 数据中读取的数据
    /// </summary>
    public class IpRecord : IComparable<long>
    {
        private long _startIpNumber;
        private long _endIpNumber;
        private string _foundResultStr;

        public long Id { get; set; }
        public string StartIp { get; set; }
        public string EndIp { get; set; }
        public string Address { get; set; }
        public string AreaCode { get; set; }
        public FoundResult FoundResult { get; set; }
        public string Note { get; set; }

        public string FoundResultStr
        {
            get { return string.IsNullOrEmpty(_foundResultStr) ? FoundResult.GetDescription() : _foundResultStr; }
            set { _foundResultStr = value; }
        }

        /// <summary>
        /// 如果已经赋值（_startIpNumber ！= -1）, 直接返回值
        /// </summary>
        public long StartIpNumber
        {
            get { return _startIpNumber == 0 ? StartIp.IpAddressToInt() : _startIpNumber; }
            set { _startIpNumber = value; }
        }

        public long EndIpNumber
        {
            get { return _endIpNumber == 0 ? EndIp.IpAddressToInt() : _endIpNumber; }
            set { _endIpNumber = value; }
        }

        public override string ToString()
        {
            return string.Format("{1}({2}){0}{3}({4}){0}{5}{0}{6}{0}{7}{0}{8}",
                   " ", StartIp, StartIpNumber, EndIp, EndIpNumber, Address, AreaCode, FoundResult.GetDescription<FoundResult>(), Note);

            //return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
            //    "\t", StartIpNumber, EndIpNumber, StartIp, EndIp, Address, AreaCode, FoundResult.GetDescription<FoundResult>(), Note);
        }

        /// <summary>
        /// 与ip地址转换为int的值比较
        /// 相等则该ip在当前ip记录[StartIpNumber, EndIpNumber]之间
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(long other)
        {
            if (other < StartIpNumber)
                return 1;
            else if (other > EndIpNumber)
                return -1;
            else
                return 0;
        }
    }
}
