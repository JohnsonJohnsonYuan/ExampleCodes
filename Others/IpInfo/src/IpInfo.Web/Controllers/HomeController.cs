using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IpInfo.Core;
using IpInfo.Core.Caching;
using System.Diagnostics;
using PagedList;
using IpInfo.Core.Services;
using IpInfo.Web.Models;

namespace IpInfo.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Constants
        /// <summary>
        /// Key for all ip records
        /// </summary>
        private const string IPRECORDS_ALL_KEY = "IpInfo.records.all";
        private const int PAGE_SIZE = 25;
        #endregion

		#region Fields

        private readonly ICacheManager _cacheManager;
        private string _ipResultFilePath;

        #endregion

        #region Constructors

        public HomeController()
            : this(new MemoryCacheManager())
        {
        }

        public HomeController(ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        #endregion

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this._ipResultFilePath = Server.MapPath(@"~/App_Data/data.txt");
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (_cacheManager.IsSet(IPRECORDS_ALL_KEY))
            {
                ViewBag.LoadFromCache = true;
            }
            else
            {
                ViewBag.LoadFromCache = false;
            }

            Stopwatch sw = Stopwatch.StartNew();

            var ipRecords_All = _cacheManager.Get(IPRECORDS_ALL_KEY, () =>
                IpRecordHelper.LoadIpRecordResult(_ipResultFilePath).ToList()
            );

            sw.Stop();

            ViewBag.LoadTime = sw.Elapsed.TotalMilliseconds;

            var ipRecords = from ip in ipRecords_All
                            select ip;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.StartIpSortParm = String.IsNullOrEmpty(sortOrder) ? "StartIp_desc" : "";
            ViewBag.EndIpSortParm = sortOrder == "EndIp" ? "EndIp_desc" : "EndIp";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
            {
                case "EndIp_desc":
                    ipRecords = ipRecords.OrderByDescending(stu => stu.EndIpNumber);
                    break;
                case "EndIp":
                    ipRecords = ipRecords.OrderBy(stu => stu.EndIpNumber);
                    break;
                case "StartIp_desc":
                    ipRecords = ipRecords.OrderByDescending(stu => stu.StartIpNumber);
                    break;
                default:
                    ipRecords = ipRecords.OrderBy(stu => stu.StartIpNumber);
                    break;
            }

            int pageNumber = (page ?? 1);
            return View(ipRecords.ToPagedList(pageNumber, PAGE_SIZE));
        }

        public ActionResult Mmgrid()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult KendoUI()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SearchIp(string ipAddr)
        {
            try
            {
                SearchRecord binary_recorder;
                SearchRecord fibonacci_recorder;

                var ipRecords = _cacheManager.Get(IPRECORDS_ALL_KEY, () =>
                    IpRecordHelper.LoadIpRecordResult(_ipResultFilePath).ToList()
                ).ToArray();

                IpRecordHelper.SearchIpRecord(ipAddr, ipRecords, out binary_recorder, out fibonacci_recorder);

                if (binary_recorder.Index == -1)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Format("没有找到该ip: {0}", ipAddr)
                    });
                }

                SearchResultModel model = new SearchResultModel();
                model.Binary_recorder = binary_recorder;
                model.Fibonacci_recorder = fibonacci_recorder;
                // model.Summary = string.Format("第{0}个找到, ip 信息为\n{1}", binary_recorder.Index + 1, binary_recorder.IpRecord);
                model.Summary = string.Format("第{0}个找到, ip 信息为\n{1}", binary_recorder.IpRecord.Id, binary_recorder.IpRecord);

                return Json(new
                {
                    success = true,
                    message = this.RenderPartialViewToString("SearchResult", model)
                });
            }
            catch (FormatException)
            {
                return Json(new
                {
                    success = false,
                    message = "请输入正确ip地址"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
