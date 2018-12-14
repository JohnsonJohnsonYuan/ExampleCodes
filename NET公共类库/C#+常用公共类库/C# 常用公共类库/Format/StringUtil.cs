using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.VisualBasic;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// Summary description for TextHelper.
    /// </summary>
    public class StringUtil
    {
        private StringUtil()
        {
        }

        public static string ToCamel(string name)
        {
            string clone = name.TrimStart('_');
            clone = RemoveSpaces(ToProperCase(clone));
            return String.Format("{0}{1}", Char.ToLower(clone[0]),
                                 clone.Substring(1, clone.Length - 1));
        }

        public static string ToCapit(String name)
        {
            string clone = name.TrimStart('_');
            return RemoveSpaces(ToProperCase(clone));
        }

        public static string RemoveFinalChar(string s)
        {
            if (s.Length > 1)
            {
                s = s.Substring(0, s.Length - 1);
            }
            return s;
        }

        public static string RemoveFinalComma(string s)
        {
            if (s.Trim().Length > 0)
            {
                int c = s.LastIndexOf(",");
                if (c > 0)
                {
                    s = s.Substring(0, s.Length - (s.Length - c));
                }
            }
            return s;
        }

        public static string RemoveSpaces(string s)
        {
            s = s.Trim();
            s = s.Replace(" ", "");
            return s;
        }

        public static string ToProperCase(string s)
        {
            string revised = "";
            if (s.Length > 0)
            {
                if (s.IndexOf(" ") > 0)
                {
                    revised = Strings.StrConv(s, VbStrConv.ProperCase, 1033);
                }
                else
                {
                    string firstLetter = s.Substring(0, 1).ToUpper(new CultureInfo("en-US"));
                    revised = firstLetter + s.Substring(1, s.Length - 1);
                }
            }
            return revised;
        }

        public static string ToTrimmedProperCase(string s)
        {
            return RemoveSpaces(ToProperCase(s));
        }

        public static string ToString(Object o)
        {
            StringBuilder sb = new StringBuilder();
            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties();

            sb.Append("Properties for: " + o.GetType().Name + Environment.NewLine);
            foreach (PropertyInfo i in pi)
            {
                try
                {
                    sb.Append("\t" + i.Name + "(" + i.PropertyType.ToString() + "): ");
                    if (null != i.GetValue(o, null))
                    {
                        sb.Append(i.GetValue(o, null).ToString());
                    }
                }
                catch
                {
                }
                sb.Append(Environment.NewLine);
            }

            FieldInfo[] fi = t.GetFields();

            foreach (FieldInfo i in fi)
            {
                try
                {
                    sb.Append("\t" + i.Name + "(" + i.FieldType.ToString() + "): ");
                    if (null != i.GetValue(o))
                    {
                        sb.Append(i.GetValue(o).ToString());
                    }
                }
                catch
                {
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public static ArrayList ExtractInnerContent(string content, string start, string end)
        {
            int sindex = -1, eindex = -1;
            int msindex = -1, meindex = -1;
            int span = 0;

            ArrayList al = new ArrayList();

            sindex = content.IndexOf(start);
            msindex = sindex + start.Length;

            eindex = content.IndexOf(end, msindex);

            span = eindex - msindex;

            if (sindex >= 0 && eindex > sindex)
            {
                al.Add(content.Substring(msindex, span));
            }

            while (sindex >= 0 && eindex > 0)
            {
                sindex = content.IndexOf(start, eindex);
                if (sindex > 0)
                {
                    eindex = content.IndexOf(end, sindex);
                    msindex = sindex + start.Length;

                    span = eindex - msindex;

                    if (msindex > 0 && eindex > 0)
                    {
                        al.Add(content.Substring(msindex, span));
                    }
                }
            }

            return al;
        }

        public static ArrayList ExtractOuterContent(string content, string start, string end)
        {
            int sindex = -1, eindex = -1;

            ArrayList al = new ArrayList();

            sindex = content.IndexOf(start);
            eindex = content.IndexOf(end);

            if (sindex >= 0 && eindex > sindex)
            {
                al.Add(content.Substring(sindex, eindex + end.Length - sindex));
            }

            while (sindex >= 0 && eindex > 0)
            {
                sindex = content.IndexOf(start, eindex);

                if (sindex > 0)
                {
                    eindex = content.IndexOf(end, sindex);

                    if (sindex > 0 && eindex > 0)
                    {
                        al.Add(content.Substring(sindex, eindex + end.Length - sindex));
                    }
                }
            }

            return al;
        }

        /// <summary>
        /// ȥ��ָ���ַ���ǰ׺���㷨
        /// </summary>
        /// <param name="content">����ȥ�ض��ַ���������</param>
        /// <param name="prefixString">�ض��ַ����б�(�Զ���,�ֺ�,�ո�ȱ�ʶ)</param>
        /// <returns></returns>
        public static string RemovePrefix(string content, string prefixString)
        {
            if (string.IsNullOrEmpty(prefixString) || prefixString.Trim() == string.Empty)
            {
                return content;
            }

            char[] splitChars = new char[] {',', ';', ' '};
            // No case sensative
            string strReturn = content;
            prefixString = prefixString.Trim(splitChars); //����ǰ�����ķָ�����,�������׳���

            string[] suffixArray = prefixString.Split(splitChars);
            foreach (string suffix in suffixArray)
            {
                int sindex = strReturn.IndexOf(suffix, StringComparison.OrdinalIgnoreCase);
                if (sindex == 0)
                {
                    strReturn = strReturn.Substring(suffix.Length);
                    break; //ƥ��һ�ξ�Ӧ�ó���
                }
            }

            return strReturn;
        }
    }
}