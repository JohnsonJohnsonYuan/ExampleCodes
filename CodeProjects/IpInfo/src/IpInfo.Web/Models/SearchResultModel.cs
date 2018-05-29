using IpInfo.Core.Services;
using IpInfo.Core.Models;

namespace IpInfo.Web.Models
{
    /// <summary>
    /// 
    // 查找方式: {0}, 查找次数: {1}, 用时: {2} ms
    /// </summary>
    public class SearchResultModel
    {
        public string Summary { get; set; }
        public SearchRecord Binary_recorder { get; set; }
        public SearchRecord Fibonacci_recorder { get; set; }


        public IpRecord IpRecord
        {
            get
            {
                return Binary_recorder != null ? Binary_recorder.IpRecord : null;
            }
        }
    }
}