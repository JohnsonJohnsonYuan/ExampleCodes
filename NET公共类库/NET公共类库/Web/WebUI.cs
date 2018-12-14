/**********************************************
 * 类作用：   网页界面功能类
 * 建立人：   abaal
 * 建立时间： 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/

using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Web.UI;
using System.Collections;

/* 采用此命名空间是为再Aspx页面中无需导入特定命名空间 */
namespace System.Web
{
    /// <summary>
    /// 网页界面功能函数
    /// 作 者: abaal
    /// 日 期: 2008-09-03
    /// </summary>
    public static class WebUI
    {
        /// <summary>
        /// 错误消息输出
        /// </summary>
        private static void page_Error(object sender, EventArgs e)
        {
            Page page1 = (Page)sender;
            Exception exception1 = page1.Server.GetLastError();
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("<div style=\"font-size:10pt;font-family:verdana;line-height:150%;\">");
            builder1.AppendFormat("<strong>\u9519\u8bef\u6d88\u606f\uff1a</strong>{0} \n", exception1.Message);
            builder1.AppendFormat("<strong>\u5bfc\u81f4\u9519\u8bef\u7684\u5e94\u7528\u7a0b\u5e8f\u6216\u5bf9\u8c61\u7684\u540d\u79f0</strong>\uff1a{0} \n", exception1.Source);
            builder1.AppendFormat("<div style=\"background-color:yellow;color:red;padding:12px;\"><strong>\u5806\u6808\u5185\u5bb9</strong>\uff1a{0} </div>\n", exception1.StackTrace);
            builder1.AppendFormat("<strong>\u5f15\u53d1\u5f02\u5e38\u7684\u65b9\u6cd5</strong>\uff1a{0} \n", exception1.TargetSite);
            builder1.AppendFormat("<strong>\u9519\u8bef\u9875\u9762</strong>\uff1a{0} \n", page1.Request.RawUrl);
            builder1.Append("</div>");
            page1.Server.ClearError();
            page1.Response.Write(builder1.ToString().Replace("\n", "<br/>"));
        }

        /// <summary>
        /// 注册在线输出错误消息模式
        /// </summary>
        public static void RegisterOnlineDebug(Page page)
        {
            Uri uri1 = page.Request.UrlReferrer;
            if (((page.Request["DEBUG"] != null) && (page.Request["DEBUG"] == "true")) || ((uri1 != null) && (uri1.ToString().IndexOf("DEBUG=") != -1)))
            {
                page.Error += new EventHandler(WebUI.page_Error);
            }
        }

        #region 控件类
        /// <summary>
        /// 绑定数据表格到服务器下拉列表控件
        /// </summary>
        /// <param name="dpt">控件对象</param>
        /// <param name="strValField">值字段</param>
        /// <param name="strTxtField">文本字段</param>
        /// <param name="dTab">数据表格源</param>
        /// <param name="strValSel">默认选择值</param>
        public static void BindDptData(DropDownList dpt, string strValField, string strTxtField, DataTable dTab, string strValSel)
        {
            dpt.DataSource = dTab;
            dpt.DataValueField = strValField;
            dpt.DataTextField = strTxtField;
            dpt.DataBind();
            try
            {
                dpt.SelectedValue = strValSel;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 根据表单的记录行设置相应服务器文本标签控件的文本
        /// </summary>
        /// <param name="dRow">记录行</param>
        /// <param name="Columns">记录行集参数</param>
        /// <param name="labels">相关文本标签控件</param>
        public static void SetLabelText(ref DataRow dRow, object[] Columns,
            params Label[] labels)
        {
            if (Columns.Length < labels.Length)
            {
                throw new Exception("记录集参数和相关文本标签控件参数不匹配！");
            }
            for (int i = 0; i < labels.Length; i++)
            {
                if (Columns[i].GetType() == typeof(object[]))
                {
                    labels[i].Text = GetFormatStr((object[])Columns[i], ref dRow);
                }
                else
                {
                    labels[i].Text = dRow[Columns[i].ToString()].ToString();
                }
            }
        }

        /// <summary>
        /// 根据表单的记录行设置相应服务器文本框控件的文本
        /// </summary>
        /// <param name="dRow">记录行</param>
        /// <param name="Columns">记录行集参数</param>
        /// <param name="tbxes">相关文本框控件</param>
        public static void SetTextBoxText(ref DataRow dRow, object[] Columns,
            params TextBox[] tbxes)
        {
            if (Columns.Length < tbxes.Length)
            {
                throw new Exception("记录集参数和相关文本标签控件参数不匹配！");
            }
            for (int i = 0; i < tbxes.Length; i++)
            {
                if (Columns[i].GetType() == typeof(object[]))
                {
                    tbxes[i].Text = GetFormatStr((object[])Columns[i], ref dRow);
                }
                else
                {
                    tbxes[i].Text = dRow[Columns[i].ToString()].ToString();
                }
            }
        }

        /// <summary>
        /// 强制客户端控件自动屏蔽HTML标签的 &lt; 和 &gt;
        /// </summary>
        /// <param name="tbxes">相关TextBox服务端控件</param>
        public static void ForbiddenHtmlTag(params TextBox[] tbxes)
        {
            for (int i = 0; i < tbxes.Length; i++)
            {
                tbxes[i].Attributes.Add("onchange", @"this.value=this.value.replace(/\<.[^\<]*\>/gi,'')");
            }
        }

        /// <summary>
        /// 强制客户端控件的值为相关数字
        /// </summary>
        /// <param name="EnableNegative">是否允许负数</param>
        /// <param name="iDefault">默认数字</param>
        /// <param name="tbxes">相关TextBox服务端控件</param>
        public static void ForceNumberValue(bool EnableNegative, int iDefault, params TextBox[] tbxes)
        {
            string strJs = (EnableNegative) ? "javascript:{if (!/^\\-?\\d+$/.test(this.value)) { this.value='" + iDefault.ToString() + "';}}" : "javascript:{if (!/^\\d+$/.test(this.value)) { this.value='" + iDefault.ToString() + "';}}";
            for (int i = 0; i < tbxes.Length; i++)
            {
                tbxes[i].Attributes.Add("onchange", strJs);
            }
        }

        /// <summary>
        /// 根据键值集合设置相应服务器文本框控件的文本
        /// </summary>
        /// <param name="nCol">键值集合</param>
        /// <param name="Columns">键集</param>
        /// <param name="tbxes">相关文本框控件</param>
        public static void SetTextBoxText(ref NameValueCollection nCol, object[] Columns,
            params TextBox[] tbxes)
        {
            if (Columns.Length < tbxes.Length)
            {
                throw new Exception("记录集参数和相关文本标签控件参数不匹配！");
            }
            for (int i = 0; i < tbxes.Length; i++)
            {
                if (Columns[i].GetType() == typeof(object[]))
                {
                    tbxes[i].Text = GetFormatStr((object[])Columns[i], ref nCol);
                }
                else
                {
                    tbxes[i].Text = nCol[Columns[i].ToString()].ToString();
                }
            }
        }

        /// <summary>
        /// 去异常选中服务端下拉列表值
        /// </summary>
        /// <param name="dpt">下拉列表控件</param>
        /// <param name="sValue">下拉列表选中的值</param>
        public static void SetDropDownlistSelect(ref DropDownList dpt, string sValue)
        {
            try
            {
                dpt.SelectedValue = sValue;
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// 根据表单的记录行设置相应服务器超级链接控件的文本、图片链接、锚属性
        /// </summary>
        /// <param name="dRow">记录行</param>
        /// <param name="Columns">记录行集参数</param>
        /// <param name="links">超级链接控件</param>
        public static void SetHyperLink(ref DataRow dRow, object[] Columns,
            params HyperLink[] links)
        {
            if (Columns.Length < links.Length)
            {
                throw new Exception("记录行参数和相关超级链接控件参数不匹配！");
            }
            for (int i = 0; i < links.Length; i++)
            {
                if (Columns[i].GetType() == typeof(object[]))
                {
                    links[i].Text = GetFormatStr((object[])Columns[i], ref dRow);
                }
                else
                {
                    links[i].Text = dRow[Columns[i].ToString()].ToString();
                }

                // 图片地址
                string strCheck = links[i].ImageUrl;
                if (IsMatchBinding(strCheck))
                {
                    links[i].ImageUrl = GetMatchBinding(strCheck, ref dRow);
                }

                // 链接
                strCheck = links[i].NavigateUrl;
                if (IsMatchBinding(strCheck))
                {
                    links[i].NavigateUrl = GetMatchBinding(strCheck, ref dRow);
                }
            }
        }

        /// <summary>
        /// 查找是否有匹配类似于{"列名"}或{'列名'}项
        /// </summary>
        /// <param name="strBindObject">需要检测的文本串</param>
        /// <returns>是否绑定相关列数据</returns>
        private static bool IsMatchBinding(string strBindObject)
        {
            if (strBindObject == string.Empty)
            {
                return false;
            }
            else
            {
                // 匹配字符中的{"列名"}或{'列名'}项
                // Source: \{(\"|\')[^\"\']+(\"|\')\}
                // Escaped: \\{(\\\"|\\')[^\\\"\\']+(\\\"|\\')\\}
                return Regex.IsMatch(strBindObject, "\\{(\\\"|\\')[^\\\"\\']+(\\\"|\\')\\}", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// 根据行集转化相应的列数据绑定
        /// </summary>
        /// <param name="strBindObject">需要装化的文本串</param>
        /// <param name="dRow">记录行集参数</param>
        /// <returns>绑定文本输出</returns>
        private static string GetMatchBinding(string strBindObject, ref DataRow dRow)
        {
            // Source:  \{(\"|\')([^\"\']+)(\"|\')\}
            // Escaped: \\{(\\\"|\\')([^\\\"\\']+)(\\\"|\\')\\}
            string strPattern = "\\{(\\\"|\\')([^\\\"\\']+)(\\\"|\\')\\}";
            MatchCollection mc = Regex.Matches(strBindObject, strPattern, RegexOptions.IgnoreCase);
            string strColumnName = string.Empty;
            int iTotal = mc.Count;
            for (int i = 0; i < iTotal; i++)
            {
                if (mc[i].Groups.Count == 4)
                {
                    strColumnName = mc[i].Groups[2].Value;
                    strBindObject = strBindObject.Replace(mc[i].Groups[0].Value, dRow[strColumnName].ToString());
                }
            }
            return strBindObject;
        }

        /// <summary>
        /// 获取相应记录行的格式化字符
        /// </summary>
        /// <param name="objParams">第1个参数为格式化字符参数，其余参数为列名称或其他数据类型</param>
        /// <param name="dRow">记录行引用</param>
        /// <returns>符合相关参数的字符</returns>
        public static string GetFormatStr(object[] objParams, ref System.Data.DataRow dRow)
        {
            string strFmt = objParams[0].ToString();
            object[] paramValue = new object[objParams.Length - 1];
            for (int i = 1; i < objParams.Length; i++)
            {
                if (objParams[i].GetType() == typeof(string))
                {
                    paramValue[i - 1] = dRow[objParams[i].ToString()].ToString();
                }
                else
                {
                    paramValue[i - 1] = objParams[i];
                }
            }
            return String.Format(strFmt, paramValue);
        }

        /// <summary>
        /// 获取相应记录行的格式化字符
        /// </summary>
        /// <param name="objParams">第1个参数为格式化字符参数，其余参数为列名称或其他数据类型</param>
        /// <param name="nCol">键值访问集合</param>
        /// <returns>符合相关参数的字符</returns>
        public static string GetFormatStr(object[] objParams, ref NameValueCollection nCol)
        {
            string strFmt = objParams[0].ToString();
            object[] paramValue = new object[objParams.Length - 1];
            for (int i = 1; i < objParams.Length; i++)
            {
                if (objParams[i].GetType() == typeof(string))
                {
                    paramValue[i - 1] = nCol[objParams[i].ToString()].ToString();
                }
                else
                {
                    paramValue[i - 1] = objParams[i];
                }
            }
            return String.Format(strFmt, paramValue);
        }

        #endregion  控件类

        #region 字符文本类

        /// <summary>
        /// 获取下拉列表的选项HTML
        /// </summary>
        /// <param name="dTab">数据表格源</param>
        /// <param name="strValField">值字段</param>
        /// <param name="strTxtField">文本字段</param>
        /// <param name="strValSel">默认选择值</param>
        /// <returns>所有option标签的构建</returns>
        public static string GetOptionData(DataTable dTab, string strValField, string strTxtField, string strValSel)
        {
            if (dTab == null)
            {
                return string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                string optfmt = "<option value=\"{0}\"{2}>{1}</option>";
                for (int i = 0; i < dTab.Rows.Count; i++)
                {
                    sb.AppendFormat(optfmt, dTab.Rows[i][strValField],
                        dTab.Rows[i][strTxtField],
                        ((dTab.Rows[i][strValField].ToString() == strValSel) ? " selected" : "")
                        );
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 时间显示
        /// </summary>
        /// <param name="objTime">时间对象</param>
        /// <param name="strFmt">格式化字符</param>
        /// <returns>相应时间文本</returns>
        public static string GetTime(object objTime, string strFmt)
        {
            return Convert.ToDateTime(objTime).ToString(strFmt);
        }

        /// <summary>
        /// 取得文件的大小，从Byte到M
        /// </summary>
        /// <param name="tsize">文件大小的数字</param>
        /// <returns>文件大小的格式,如1.44M</returns>
        public static string GetSize(object tsize)
        {
            int filesize = 0;
            if (tsize == null)
            {
                filesize = 0;
            }
            else
            {
                filesize = Convert.ToInt32(tsize);
            }

            if (filesize > 0)
            {
                if (filesize > 1048576) { return (filesize / 1048576).ToString() + "M"; }
                else if (filesize > 1024) { return (filesize / 1024).ToString() + "K"; }
                else return filesize.ToString() + "B";
            }
            else
            {
                return "未知";
            }
        }

        public static string GetCheckedValue(CheckBoxList cbxList, string Separator)
        {
            ArrayList objList = new ArrayList();
            foreach (ListItem item in cbxList.Items)
            {
                if (item.Selected == true) objList.Add(item.Value);
            }
            return string.Join(Separator, (string[])objList.ToArray(typeof(string)));
        }

        public static void SetCheckedValue(CheckBoxList cbxList, string chkedValue, string Separator)
        {
            string strSearchSource = Separator + chkedValue + Separator;
            foreach (ListItem item in cbxList.Items)
            {
                if (strSearchSource.IndexOf(Separator + item.Value + Separator) != -1) item.Selected = true;
            }
        }

        /// <summary>
        /// HTML代码生成
        /// </summary>
        /// <param name="dTab">数据表格源</param>
        /// <param name="objModel">模版数据(Item,AlterItem,LiteralCount,Lieteral,Header,Footer)</param>
        /// <returns>根据相应Repeater模版生成的字符集</returns>
        /// <remarks>ST:ToPractice More, Fixed 2006-4-4</remarks>
        public static string GeneralHtmlBind(DataTable dTab, params string[] objModel)
        {
            StringBuilder sb = new StringBuilder(5000);
            DataRow dRow = null;
            int k = 1;

            if (dTab != null)
            {
                for (int i = 0; i < dTab.Rows.Count; i++)
                {
                    dRow = dTab.Rows[i];
                    if (k == 1 && objModel.Length > 4)
                    {
                        sb.Append(String.Format(objModel[4] + "\n", dRow.ItemArray));
                    }

                    if (objModel.Length > 1 && objModel[1] != string.Empty && k % 2 == 0)
                    {
                        sb.Append(String.Format(objModel[1].Replace("$", k.ToString()), dRow.ItemArray));
                    }
                    else
                    {
                        sb.Append(String.Format(objModel[0].Replace("$", k.ToString()), dRow.ItemArray));
                    }

                    if (objModel.Length > 3 && objModel[3] != string.Empty && IsNumerical(objModel[2]))
                    {
                        if (k % int.Parse(objModel[2]) == 0 && k < dTab.Rows.Count)
                        {
                            sb.Append(objModel[3]);
                        }
                    }

                    if (k == dTab.Rows.Count && objModel.Length > 5)
                    {
                        sb.Append(String.Format(objModel[5] + "\n", dRow.ItemArray));
                    }
                    k++;
                }
                dTab.Dispose();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 判断输入对象是否为数字类型
        /// </summary>
        /// <param name="strInput">输入对象</param>
        /// <returns>是否为不含小数点的数字类型</returns>
        private static bool IsNumerical(object strInput)
        {
            if (strInput == null) return false;
            bool bValue = true;
            string strCheck = strInput.ToString();
            if (strCheck.Length == 0) return false;
            for (int i = 0; i < strCheck.Length; i++)
            {
                if (!char.IsDigit(strCheck, i))
                {
                    bValue = false;
                    break;
                }
            }
            return bValue;
        }

        /// <summary>
        /// 返回指定数据的重复次数
        /// </summary>
        /// <param name="objInt">int型数据</param>
        /// <param name="strRepeat">重复的字符片段</param>
        /// <returns>Example:ReplicateObject(5,"*")="*****"</returns>
        public static string ReplicateObject(object objInt, string strRepeat)
        {
            string strRet = string.Empty;
            if (objInt != null)
            {
                int Count = Convert.ToInt32(objInt);
                strRet = (Count > 0) ? (new string('*', Count)) : "";
            }
            else
            {
                strRet = "";
            }
            return strRet.Replace("*", strRepeat);
        }

        /// <summary>
        /// 截取HTML左边字符
        /// </summary>
        /// <param name="objString">截取对象</param>
        /// <param name="length">长度</param>
        /// <returns>相应的普通文本</returns>
        public static string HTMLLeft(object objString, int length)
        {
            if (objString == null) return string.Empty;
            string strGet = objString.ToString();
            string strPattern = "<.[^<]*>";
            if (Regex.IsMatch(strGet, strPattern, RegexOptions.IgnoreCase))
            {
                strGet = Regex.Replace(strGet, strPattern, "", RegexOptions.IgnoreCase);
            }
            return (strGet.Length >= length) ? strGet.Substring(0, length) + "..." : strGet;
        }

        /// <summary>
        /// 智能格式化文本\判断文本还是HTML文本
        /// </summary>
        /// <param name="objString">要格式化排版的对象</param>
        /// <returns>如果是文本则输出文本的相应HTML格式，若为HTML格式则不变。</returns>
        public static string SmartFormat(object objString)
        {
            if (objString == null) return string.Empty;
            string strGet = objString.ToString();
            string strPattern = "<.[^<]*>";
            if (!Regex.IsMatch(strGet, strPattern, RegexOptions.IgnoreCase))
            {
                strGet = Text2Html(strGet);
            }
            return strGet;
        }

        /// <summary>
        /// 普通文本转换为HTML格式
        /// </summary>
        /// <param name="objInput">转换文本对象</param>
        /// <returns>格式化的HTML格式</returns>
        public static string Text2Html(object objInput)
        {
            if (objInput == null) return string.Empty;
            string strResult = objInput.ToString();
            // xml escape
            strResult = strResult.Replace("&", "&amp;");
            strResult = strResult.Replace("<", "&lt;");
            strResult = strResult.Replace(">", "&gt;");
            strResult = strResult.Replace("'", "&#39;");
            strResult = strResult.Replace("\"", "&#34;");

            // Simple Transfer
            strResult = strResult.Replace("\r\n", "<br>");
            strResult = strResult.Replace("\n", "<br>");
            strResult = strResult.Replace("\r", "<br>");
            strResult = strResult.Replace(" ", "&nbsp;");
            return strResult;
        }

        /// <summary>
        /// HTML格式文本转换为普通文本
        /// </summary>
        /// <param name="objInput">转换HTML文本对象</param>
        /// <returns>格式化的普通文本</returns>
        public static string Html2Text(object objInput)
        {
            if (objInput == null) return string.Empty;
            string strResult = objInput.ToString();
            // &nbsp; <p> <br>
            strResult = Regex.Replace(strResult, "&nbsp;?", " ", RegexOptions.IgnoreCase);
            strResult = Regex.Replace(strResult, "<p\\s?[^>]*>", "\n\n", RegexOptions.IgnoreCase);
            strResult = Regex.Replace(strResult, "<BR\\s?/?>", "\n", RegexOptions.IgnoreCase);
            strResult = Regex.Replace(strResult, "</p>", "", RegexOptions.IgnoreCase);
            // Other < > wrapped
            strResult = Regex.Replace(strResult, "<.[^<]*>", "", RegexOptions.IgnoreCase);
            // xml unescape
            strResult = strResult.Replace("&lt;", "<").Replace("&gt;", ">");
            strResult = strResult.Replace("&#39;", "'").Replace("&#34;", "\"");
            strResult = strResult.Replace("&amp;", "&");
            return strResult;
        }

        /// <summary>
        /// 获取定长像素的网页片段，多余部分用省略号代替。(ellipsis)
        /// </summary>
        /// <param name="htmlText">片段内容</param>
        /// <param name="width">片段长度</param>
        /// <returns>相关属性样式的DIV片段</returns>
        /// <remarks>2008-1-29 by Ridge Wong</remarks>
        public static string GetFixedDivEllipsis(string htmlText, int width)
        {
            return string.Format("<div style=\"width:{0}px;overflow:hidden;float:left;text-overflow:ellipsis;white-space:nowrap;\">{1}</div>", width, htmlText);
        }

        /// <summary>
        /// 去掉小数位后无意义的0，如21.50结尾为21.5。
        /// </summary>
        /// <param name="pointNum">金额绑定对象</param>
        /// <returns>相关小数点金额</returns>
        public static string StripZeroEnd(object pointNum)
        {
            if (pointNum == null) { return "0"; }
            return Regex.Replace(pointNum.ToString(), @"(\.)?0{1,}$", "");
        }

        #endregion 字符文本类

        #region 逻辑判断类

        /// <summary>
        /// 空值显示替换
        /// </summary>
        /// <param name="objOrigin">来源对象</param>
        /// <param name="strOriginal">来源对象拷贝或处理对象</param>
        /// <param name="strReplace">替换对象</param>
        /// <returns>相应文本输出</returns>
        public static string GetEmptyShow(object objOrigin, object strOriginal, string strReplace)
        {
            string strRet = string.Empty;
            strRet = (objOrigin != null && objOrigin.ToString().Trim() != string.Empty) ? strOriginal.ToString() : strReplace;
            return strRet;
        }

        /// <summary>
        /// 获取满足条件的相应值
        /// </summary>
        /// <param name="Expression">布尔表达式</param>
        /// <param name="strTrue">为真时返回的字符</param>
        /// <param name="strFalse">为假时返回的字符</param>
        /// <returns>相应条件的字符值</returns>
        public static string GetWhich(bool Expression, string strTrue, string strFalse)
        {
            return (Expression == true) ? strTrue : strFalse;
        }

        /// <summary>
        /// 表单选择判断
        /// </summary>
        /// <param name="objQuery">来源对象</param>
        /// <param name="strMatch">匹配项</param>
        /// <param name="allowNull">可以为Null对象</param>
        /// <param name="strValue">返回文本输出</param>
        /// <returns>相应文本输出</returns>
        public static string GetDefaultValue(object objQuery, string strMatch, bool allowNull, string strValue)
        {
            string strRet = string.Empty;
            if (objQuery == null)
            {
                if (allowNull == true) strRet = strValue;
            }
            else
            {
                if ((objQuery.ToString() == strMatch) ||
                    (objQuery.ToString() == string.Empty && allowNull == true))
                {
                    strRet = strValue;
                }
            }
            return strRet;
        }

        #endregion 逻辑判断类

    }
}
