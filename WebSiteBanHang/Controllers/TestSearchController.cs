using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using PagedList;
namespace WebSiteBanHang.Controllers
{
    public class TestSearchController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: TestSearch
        public ActionResult Index(string sortOrder,string currentFilter,string searchString,int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TenSortParm = String.IsNullOrEmpty(sortOrder) ? "tenSP_desc" : "";
            ViewBag.NgaySortParm = sortOrder == "Date" ? "ngay_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var lstSP = from s in db.SanPhams
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                lstSP = lstSP.Where(n => n.TenSP.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "tenSP_desc":
                    lstSP = lstSP.OrderByDescending(n => n.TenSP);
                    break;
                case "Date":
                    lstSP = lstSP.OrderBy(n => n.NgayCapNhap);
                    break;
                case "ngay_desc":
                    lstSP = lstSP.OrderByDescending(n => n.NgayCapNhap);
                    break;
                default:
                    lstSP = lstSP.OrderBy(n => n.TenSP);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(lstSP.ToPagedList(pageNumber,pageSize));
        }

        public ActionResult Index2(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var lstSP = from s in db.SanPhams
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                lstSP = lstSP.Where(n => n.TenSP.Contains(searchString));
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(lstSP.OrderBy(n => n.NgayCapNhap).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult testDatetime()
        {
            return View();
        }
        [HttpPost]
        public ActionResult testDatetime(DateTime? start,DateTime? end)
        {
            ViewBag.start = start;
            ViewBag.end = end;
            var orders = db.DonDatHangs.Where(n => n.NgayDat > start && n.NgayDat < end).ToList();
            return View(orders);
        }

    }
}