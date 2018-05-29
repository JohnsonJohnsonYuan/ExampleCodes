using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IpInfo.Core;
using IpInfo.Core.Caching;

namespace IpInfo.Web.Controllers
{
    public class IpRecordController : ApiController
    {
        #region Constants
        /// <summary>
        /// Key for all ip records
        /// </summary>
        private const string IPRECORDS_ALL_KEY = "IpInfo.records.all";
        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private string _ipResultFilePath;

        #endregion

        #region Constructors

        public IpRecordController()
            : this(new MemoryCacheManager())
        {
        }

        public IpRecordController(ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
            this._ipResultFilePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/data.txt");
        }

        #endregion

        // GET api/iprecord
        [HttpGet]
        [ActionName("MmGridGet")]
        public object Get([FromUri]IpInfo.Web.Models.MmGrid.DataSourceRequest command)
        {
            var ipRecords_All = _cacheManager.Get(IPRECORDS_ALL_KEY, () =>
                   IpRecordHelper.LoadIpRecordResult(_ipResultFilePath).ToList()
            );

            var gridModel = new IpInfo.Web.Models.MmGrid.DataSourceResult
            {
                Data = ipRecords_All.Skip((command.Page - 1) * command.Limit).Take(command.Limit),
                totalCount = ipRecords_All.Count
            };

            return gridModel;
        }


        // GET api/iprecord
        [HttpGet]
        [ActionName("KendoUIGet")]
        public object Get([FromUri]IpInfo.Web.Models.Kendoui.DataSourceRequest command)
        {
            var ipRecords_All = _cacheManager.Get(IPRECORDS_ALL_KEY, () =>
                   IpRecordHelper.LoadIpRecordResult(_ipResultFilePath).ToList()
            );

            var gridModel = new IpInfo.Web.Models.Kendoui.DataSourceResult
            {
                Data = ipRecords_All.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize),
                Total = ipRecords_All.Count
            };

            return gridModel;
        }
    }
}
