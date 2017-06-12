using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace IpInfo.Web.Models.MmGrid
{
    public class DataSourceResult
    {
        public object ExtraData { get; set; }

        public IEnumerable Data { get; set; }

        public object Errors { get; set; }

        /// <summary>
        /// mmGrid返回json, 必须为totalCount, 否则无法正常分页
        /// </summary>
        public int totalCount { get; set; }
    }
}