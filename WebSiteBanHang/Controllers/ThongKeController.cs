using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
namespace WebSiteBanHang.Controllers
{
    public class ThongKeController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: ThongKe
        [HttpGet]
        [Authorize(Roles ="QuanTri")]
        public ActionResult Index()
        {
            ViewBag.LoiNhuanBanTinDang = LoiNhuanBanTinDang();
            ViewBag.TongSoTinDaBan = TongSoTinDaBan();
            ViewBag.TongDDH = ThongKeDonHang();
            ViewBag.TongThanhVien = TongThanhVien();
            return View();
        }
        public decimal? LoiNhuanBanTinDang()
        {
            decimal? loiNhuanBanTinDang = db.HoaDonMuaTins.Sum(n => n.SoLuong * n.DonGia);
            return loiNhuanBanTinDang;
        }
        public decimal? LoiNhuanBanTinDang(DateTime date1, DateTime date2)
        {
            var list = db.HoaDonMuaTins.Where(n => n.NgayMua > date1 && n.NgayMua < date2);
            if (!list.Any())
            {
                return 0;
            }
            decimal? loiNhuanBanTinDang = list.Sum(n => n.SoLuong * n.DonGia);
            return loiNhuanBanTinDang;
        }
        public int? TongSoTinDaBan()
        {
            int? tongTin = db.HoaDonMuaTins.Sum(n => n.SoLuong);
            return tongTin;
        }
        public int? TongSoTinDaBan(DateTime date1, DateTime date2)
        {
            var list = db.HoaDonMuaTins.Where(n => n.NgayMua > date1 && n.NgayMua < date2);
            if (!list.Any())
            {
                return 0;
            }
            int? tongTin = list.Sum(n => n.SoLuong);
            return tongTin;
        }
        public double ThongKeDonHang()
        {
            
            //Đếm đơn đặt hàng
            double sl = db.DonDatHangs.Count();
            return sl;
        }
        public double ThongKeDonHang(DateTime date1, DateTime date2)
        {
            var list = db.DonDatHangs.Where(n => n.NgayDat > date1 && n.NgayDat < date2);
            if (!list.Any())
            {
                return 0;
            }
            //Đếm đơn đặt hàng
            double sl = list.Count();
            return sl;
        }
        public double TongThanhVien()
        {
            double slTV = db.ThanhViens.Count();
            return slTV;
        }
        [HttpGet]
        public ActionResult ThongKeTheoThoiGian()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThongKeTheoThoiGian(DateTime date,DateTime date2)
        {
            date = date.AddDays(1);
            date2 = date2.AddDays(1);

            ViewBag.LoiNhuanBanTinDang = LoiNhuanBanTinDang(date,date2).Value.ToString("#,##") + " VNĐ";
            ViewBag.TongSoTinDaBan = TongSoTinDaBan(date,date2) + " (tin)";
            ViewBag.TongDDH = ThongKeDonHang(date,date2) + " (đơn)";
            ViewBag.TongThanhVien = TongThanhVien() + "(thành viên)";
            return PartialView("_Partial");
        }
        
        // Thông kê theo merchant
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult DoanhThuMerchant()
        {
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            var lstSPBanThanhCong = db.ChiTietDonDatHangMerchants.Where(n => n.SanPham.MaThanhVien == tv.MaThanhVien && n.MaTrangThai==3);
            ViewBag.Data = lstSPBanThanhCong;
            List<loiNhuanMV> kq = lstSPBanThanhCong
                .GroupBy(n => n.SanPham.TenSP)
                .Select(cl => new loiNhuanMV
                {
                    NguoiBan = cl.FirstOrDefault().SanPham.TenSP,
                    TongTien = cl.Sum(c => c.DonGia * c.SoLuong),
                    TongSoLuong = cl.Sum(n=>n.SoLuong)
                }).ToList();
            ViewBag.TongSL = kq.Sum(n => n.TongSoLuong);
            ViewBag.TongDoanhThu = kq.Sum(n => n.TongTien);
            return View(kq);
        }

        // Thống kê tất cả merchant
        [Authorize(Roles ="QuanTri")]
        public ActionResult DoanhThuAllMerchant()
        {
            var lstSPBanThanhCong = db.ChiTietDonDatHangMerchants.Where(n => n.MaTrangThai == 3);
            List<loiNhuanMV> kq = lstSPBanThanhCong
                .GroupBy(n => n.SanPham.ThanhVien.TaiKhoan)
                .Select(cl => new loiNhuanMV
                {
                    NguoiBan = cl.FirstOrDefault().SanPham.ThanhVien.TaiKhoan,
                    TongTien = cl.Sum(c => c.DonGia * c.SoLuong),
                    TongSoLuong = cl.Sum(n => n.SoLuong)
                }).ToList();
            ViewBag.TongSL = kq.Sum(n => n.TongSoLuong);
            ViewBag.TongDoanhThu = kq.Sum(n => n.TongTien);
            return View(kq);
        }
        [Authorize(Roles = "QuanTri")]
        public ActionResult XemChiTietMerchant(string name)
        {
            var lstSPBanThanhCong = db.ChiTietDonDatHangMerchants.Where(n => n.SanPham.ThanhVien.TaiKhoan == name && n.MaTrangThai == 3);
            ViewBag.TaiKhoanMerchant = name;
            List<loiNhuanMV> kq = lstSPBanThanhCong
                .GroupBy(n => n.SanPham.TenSP)
                .Select(cl => new loiNhuanMV
                {
                    NguoiBan = cl.FirstOrDefault().SanPham.TenSP,
                    TongTien = cl.Sum(c => c.DonGia * c.SoLuong),
                    TongSoLuong = cl.Sum(n => n.SoLuong)
                }).ToList();
            ViewBag.TongSL = kq.Sum(n => n.TongSoLuong);
            ViewBag.TongDoanhThu = kq.Sum(n => n.TongTien);
            return View(kq);
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