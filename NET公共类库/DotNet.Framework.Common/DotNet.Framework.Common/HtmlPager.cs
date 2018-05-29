using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Framework.Common
{
    /// <summary>
    /// HTMl 分页
    /// </summary>
    public static class HtmlPager
    {
        /// <summary>
        /// 写出分页
        /// </summary>
        /// <param name="pageCount">页数</param>
        /// <param name="currentPage">当前页</param>
        public static string GetPager(int pageCount, int currentPage)
        {
            return GetPager(pageCount, currentPage, new string[] { }, new string[] { });
        }

        /// <summary>
        /// 写出分页
        /// </summary>
        /// <param name="pageCount">页数</param>
        /// <param name="currentPage">当前页</param>
        /// <param name="FieldName">地址栏参数</param>
        /// <param name="FieldValue">地址栏参数值</param>
        /// <returns></returns>
        public static string GetPager(int pageCount, int currentPage, string[] FieldName, string[] FieldValue)
        {
            string pString = "";
            for (int i = 0; i < FieldName.Length; i++)
            {
                pString += "&" + FieldName[i].ToString() + "=" + FieldValue[i].ToString();
            }
            int stepNum = 4;
            int pageRoot = 1;
            pageCount = pageCount == 0 ? 1 : pageCount;
            currentPage = currentPage == 0 ? 1 : currentPage;

            StringBuilder sb = new StringBuilder();
            sb.Append("<table cellpadding=0 cellspacing=1 class=\"pager\">\r<tr>\r");
            sb.Append("<td class=pagerTitle>&nbsp;分页&nbsp;</td>\r");
            sb.Append("<td class=pagerTitle>&nbsp;" + currentPage.ToString() + "/" + pageCount.ToString() + "&nbsp;</td>\r");
            if (currentPage - stepNum < 2)
                pageRoot = 1;
            else
                pageRoot = currentPage - stepNum;
            int pageFoot = pageCount;
            if (currentPage + stepNum >= pageCount)
                pageFoot = pageCount;
            else
                pageFoot = currentPage + stepNum;
            if (pageRoot == 1)
            {
                if (currentPage > 1)
                {
                    sb.Append("<td>&nbsp;<a href='?page=1" + pString + "' title='首页'>首页</a>&nbsp;</td>\r");
                    sb.Append("<td>&nbsp;<a href='?page=" + Convert.ToString(currentPage - 1) + pString + "' title='上页'>上页</a>&nbsp;</td>\r");
                }
            }
            else
            {
                sb.Append("<td>&nbsp;<a href='?page=1" + pString + "' title='首页'>首页</a>&nbsp;</td>");
                sb.Append("<td>&nbsp;<a href='?page=" + Convert.ToString(currentPage - 1) + pString + "' title='上页'>上页</a>&nbsp;</td>\r");
            }
            for (int i = pageRoot; i <= pageFoot; i++)
            {
                if (i == currentPage)
                {
                    sb.Append("<td class='current'>&nbsp;" + i.ToString() + "&nbsp;</td>\r");
                }
                else
                {
                    sb.Append("<td>&nbsp;<a href='?page=" + i.ToString() + pString + "' title='第" + i.ToString() + "页'>" + i.ToString() + "</a>&nbsp;</td>\r");
                }
                if (i == pageCount)
                    break;
            }
            if (pageFoot == pageCount)
            {
                if (pageCount > currentPage)
                {
                    sb.Append("<td>&nbsp;<a href='?page=" + Convert.ToString(currentPage + 1) + pString + "' title='下页'>下页</a>&nbsp;</td>\r");
                    sb.Append("<td>&nbsp;<a href='?page=" + pageCount.ToString() + pString + "' title='尾页'>尾页</a>&nbsp;</td>\r");
                }
            }
            else
            {
                sb.Append("<td>&nbsp;<a href='?page=" + Convert.ToString(currentPage + 1) + pString + "' title='下页'>下页</a>&nbsp;</td>\r");
                sb.Append("<td>&nbsp;<a href='?page=" + pageCount.ToString() + pString + "' title='尾页'>尾页</a>&nbsp;</td>\r");
            }
            sb.Append("</tr>\r</table>");
            return sb.ToString();
        }



        /// <summary>
        /// 写出分页
        /// </summary>
        /// <param name="pageCount">总页数</param>
        /// <param name="currentPage">当前页</param>
        /// <param name="prefix">上一页</param>
        /// <param name="suffix">下一页</param>
        /// <returns></returns>
        public static string GetHtmlPager(int pageCount, int currentPage, string prefix, string suffix)
        {
            int stepNum = 4;
            int pageRoot = 1;
            pageCount = pageCount == 0 ? 1 : pageCount;
            currentPage = currentPage == 0 ? 1 : currentPage;

            StringBuilder sb = new StringBuilder();
            sb.Append("<table cellpadding=0 cellspacing=1 class=\"pager\">\r<tr>\r");
            sb.Append("<td class=pagerTitle>&nbsp;分页&nbsp;</td>\r");
            sb.Append("<td class=pagerTitle>&nbsp;" + currentPage.ToString() + "/" + pageCount.ToString() + "&nbsp;</td>\r");
            if (currentPage - stepNum < 2)
                pageRoot = 1;
            else
                pageRoot = currentPage - stepNum;
            int pageFoot = pageCount;
            if (currentPage + stepNum >= pageCount)
                pageFoot = pageCount;
            else
                pageFoot = currentPage + stepNum;
            if (pageRoot == 1)
            {
                if (currentPage > 1)
                {
                    sb.Append("<td>&nbsp;<a href='" + prefix + "1" + suffix + "' title='首页'>首页</a>&nbsp;</td>\r");
                    sb.Append("<td>&nbsp;<a href='" + prefix + Convert.ToString(currentPage - 1) + suffix + "' title='上页'>上页</a>&nbsp;</td>\r");
                }
            }
            else
            {
                sb.Append("<td>&nbsp;<a href='" + prefix + "1" + suffix + "' title='首页'>首页</a>&nbsp;</td>");
                sb.Append("<td>&nbsp;<a href='" + prefix + Convert.ToString(currentPage - 1) + suffix + "' title='上页'>上页</a>&nbsp;</td>\r");
            }
            for (int i = pageRoot; i <= pageFoot; i++)
            {
                if (i == currentPage)
                {
                    sb.Append("<td class='current'>&nbsp;" + i.ToString() + "&nbsp;</td>\r");
                }
                else
                {
                    sb.Append("<td>&nbsp;<a href='" + prefix + i.ToString() + suffix + "' title='第" + i.ToString() + "页'>" + i.ToString() + "</a>&nbsp;</td>\r");
                }
                if (i == pageCount)
                    break;
            }
            if (pageFoot == pageCount)
            {
                if (pageCount > currentPage)
                {
                    sb.Append("<td>&nbsp;<a href='" + prefix + Convert.ToString(currentPage + 1) + suffix + "' title='下页'>下页</a>&nbsp;</td>\r");
                    sb.Append("<td>&nbsp;<a href='" + prefix + pageCount.ToString() + suffix + "' title='尾页'>尾页</a>&nbsp;</td>\r");
                }
            }
            else
            {
                sb.Append("<td>&nbsp;<a href='" + prefix + Convert.ToString(currentPage + 1) + suffix + "' title='下页'>下页</a>&nbsp;</td>\r");
                sb.Append("<td>&nbsp;<a href='" + prefix + pageCount.ToString() + suffix + "' title='尾页'>尾页</a>&nbsp;</td>\r");
            }
            sb.Append("</tr>\r</table>");
            return sb.ToString();
        }
    }
}
