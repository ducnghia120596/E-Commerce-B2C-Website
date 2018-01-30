using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    
    public class KhachHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        [Authorize(Roles ="QLKhachHang")]
        public ActionResult Index()
        {
            //Cach 1:
            //var lstKH = from KH in db.KhachHangs select KH;
            //Cach 2:
            var lstKH = db.KhachHangs;
            return View(lstKH);
        }
        [Authorize(Roles ="QLDonHang")]
        public ActionResult Index1()
        {
            var lstKH = from KH in db.KhachHangs select KH;
            return View(lstKH);
        }

        public ActionResult TruyVan1DoiTuong()
        {
            //Cach1: Cau truy van
            //var lstKH = from KH in db.KhachHangs where KH.MaKH == 2  select KH;
            //KhachHang khang = new KhachHang();
            //khang = lstKH.FirstOrDefault();
            //Cach 2: Phuong thuc ho tro
            KhachHang khang = db.KhachHangs.SingleOrDefault(n => n.MaKH == 2);
            return View(khang);
        }
        public ActionResult SortDuLieu()
        {
            List<KhachHang> lstKh = db.KhachHangs.OrderByDescending(n => n.TenKH).ToList();
            return View(lstKh);
        }
        public ActionResult GroupDuLieu()
        {
            List<ThanhVien> lstTV = new List<ThanhVien>();
            lstTV = db.ThanhViens.OrderByDescending(n => n.TaiKhoan).ToList();
            return View(lstTV);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}