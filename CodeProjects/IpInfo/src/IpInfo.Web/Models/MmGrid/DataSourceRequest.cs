using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IpInfo.Web.Models.MmGrid
{
    public class DataSourceRequest
    {
        public int Page { get; set; }

        // page size
        public int Limit { get; set; }

        public DataSourceRequest()
        {
            this.Page = 1;
            this.Limit = 10;
        }
    }
}