using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
namespace WebSiteBanHang.Controllers
{
    public class TinhTrangGiaoHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // Danh sách đơn đặt hàng của khách hàng
        [Authorize(Roles ="QuanTri")]
        public ActionResult DanhSachDonHangCuaKhachHang()
        {
            var lst = db.DonDatHangs.OrderBy(n => n.NgayDat);
            return View(lst);
        }
        //// GET: TinhTrangGiaoHang
        //public ActionResult XacNhan()
        //{
        //    var lst = db.DonDatHangs.Where(n => n.MaTrangThai == 1).OrderBy(n => n.NgayDat);
        //    return View(lst);
        //}
        //public ActionResult GiaoHang()
        //{
        //    var lst = db.DonDatHangs.Where(n => n.MaTrangThai == 2).OrderBy(n => n.NgayDat);
        //    return View(lst);
        //}
        //public ActionResult ThanhCong()
        //{
        //    var lst = db.DonDatHangs.Where(n => n.MaTrangThai==3).OrderBy(n => n.NgayDat);
        //    return View(lst);
        //}
        //public ActionResult Huy()
        //{
            
        //    var lst = db.DonDatHangs.Where(n => n.MaTrangThai==4).OrderBy(n => n.NgayDat);
        //    return View(lst);
        //}

        [HttpGet]
        [Authorize(Roles = "QuanTri")]
        public ActionResult DuyetDonHang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonDatHang model = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTrangThai = new SelectList(db.TrangThaiGiaoHangs.OrderBy(n => n.MaTrangThai), "MaTrangThai", "TenTrangThai", model.MaTrangThai);
            // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(model);
        }
        
        [HttpPost]
        [Authorize(Roles = "QuanTri")]
        public ActionResult DuyetDonHang(DonDatHang ddh)
        {
            
            // Lấy dữ liệu của đơn hàng đó
            DonDatHang ddhUpdate = db.DonDatHangs.Single(n => n.MaDDH == ddh.MaDDH);

            ViewBag.MaTrangThai = new SelectList(db.TrangThaiGiaoHangs.OrderBy(n => n.MaTrangThai), "MaTrangThai", "TenTrangThai", ddhUpdate.MaTrangThai);

            ddhUpdate.DaHuy = ddh.DaHuy;
            ddhUpdate.TinhTrangGiaoHang = ddh.TinhTrangGiaoHang;
            ddhUpdate.MaTrangThai =ddh.MaTrangThai;
            db.SaveChanges();

            // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddh.MaDDH);
            ViewBag.ListChiTietDH = lstChiTietDH;

            

            //GuiEmail("Xác nhận đơn hàng", "ducnghia1205@gmail.com", "kiembtcmp@gmail.com", "zewang.help", "Đơn hàng của bạn đã được đặt thành công");
            return View(ddhUpdate);
        }

        public void GuiEmail(string Title, string ToEmail, string FromEmail, string PassWord, string Content)
        {
            // goi email
            MailMessage mail = new MailMessage();
            mail.To.Add(ToEmail); // Địa chỉ nhận
            mail.From = new MailAddress(ToEmail); // Địa chửi gửi
            mail.Subject = Title;  // tiêu đề gửi
            mail.Body = Content;                 // Nội dung
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com"; // host gửi của Gmail
            smtp.Port = 587;               //port của Gmail
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential
            (FromEmail, PassWord);//Tài khoản password người gửi
            smtp.EnableSsl = true;   //kích hoạt giao tiếp an toàn SSL
            smtp.Send(mail);   //Gửi mail đi
        }
        [Authorize(Roles = "QuanTri")]
        public ActionResult dsddhAllMerchant()
        {
            var lstDDHMerchant = db.DonDatHangMerchants;
            return View(lstDDHMerchant.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QuanTri")]
        public ActionResult dsddhXacNhan()
        {
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaTrangThai == 1);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QuanTri")]
        public ActionResult dsddhGiaoHang()
        {
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaTrangThai == 2);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QuanTri")]
        public ActionResult dsddhThanhCong()
        {
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaTrangThai == 3);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QuanTri")]
        public ActionResult dsddhHuy()
        {
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaTrangThai == 4);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QuanTri")]
        public ActionResult dsddhDuyetDonHang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonDatHangMerchant model = db.DonDatHangMerchants.SingleOrDefault(n => n.MaDDHMerchant == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTrangThai = new SelectList(db.TrangThaiGiaoHangs.OrderBy(n => n.MaTrangThai), "MaTrangThai", "TenTrangThai", model.MaTrangThai);
            // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
            var lstChiTietDHMerchant = db.ChiTietDonDatHangMerchants.Where(n => n.MaDDHMerchant == id);
            ViewBag.ListChiTietDH = lstChiTietDHMerchant;
            return View(model);
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
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult DanhSachDDHMerchant()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "merchantLogin");
            }
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            //var lstHoaDon = db.ChiTietDonDatHangs.Where(n => n.SanPham.MaThanhVien == tv.MaThanhVien);
            //return View(lstHoaDon.OrderByDescending(n => n.DonDatHang.NgayDat));
            var lstDDHMerchant = db.DonDatHangMerchants.Where(n => n.MaMerchant == tv.MaThanhVien);
            return View(lstDDHMerchant.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult merchantXacNhan()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "merchantLogin");
            }
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            //var lstHoaDon = db.ChiTietDonDatHangs.Where(n => n.SanPham.MaThanhVien == tv.MaThanhVien && n.MaTrangThai == 1);
            //return View(lstHoaDon.OrderByDescending(n => n.DonDatHang.NgayDat));
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaMerchant == tv.MaThanhVien && n.MaTrangThai == 1);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult merchantGiaoHang()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "merchantLogin");
            }
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            //var lstHoaDon = db.ChiTietDonDatHangs.Where(n => n.SanPham.MaThanhVien == tv.MaThanhVien && n.MaTrangThai == 2);
            //return View(lstHoaDon.OrderByDescending(n => n.DonDatHang.NgayDat));
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaMerchant == tv.MaThanhVien && n.MaTrangThai == 2);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult merchantThanhCong()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "merchantLogin");
            }
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            //var lstHoaDon = db.ChiTietDonDatHangs.Where(n => n.SanPham.MaThanhVien == tv.MaThanhVien && n.MaTrangThai == 3);
            //return View(lstHoaDon.OrderByDescending(n => n.DonDatHang.NgayDat));
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaMerchant == tv.MaThanhVien && n.MaTrangThai == 3);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult merchantHuy()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "merchantLogin");
            }
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            //var lstHoaDon = db.ChiTietDonDatHangs.Where(n => n.SanPham.MaThanhVien == tv.MaThanhVien && n.MaTrangThai == 4);
            //return View(lstHoaDon.OrderByDescending(n => n.DonDatHang.NgayDat));
            var lstHoaDon = db.DonDatHangMerchants.Where(n => n.MaMerchant == tv.MaThanhVien && n.MaTrangThai == 4);
            return View(lstHoaDon.OrderByDescending(n => n.NgayDat));
        }
        //// --- Duyệt đơn hàng 1 test
        //[HttpGet]
        //public ActionResult merchantDuyetDonHang(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ChiTietDonDatHang model = db.ChiTietDonDatHangs.SingleOrDefault(n => n.MaChiTietDDH == id);
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.MaTrangThai = new SelectList(db.TrangThaiGiaoHangs.OrderBy(n => n.MaTrangThai), "MaTrangThai", "TenTrangThai", model.MaTrangThai);
        //    // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
        //    //var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
        //    //ViewBag.ListChiTietDH = lstChiTietDH;
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult merchantDuyetDonHang(ChiTietDonDatHang ctddh)
        //{

        //    // Lấy dữ liệu của đơn hàng đó
        //    ChiTietDonDatHang ctddhUpdate = db.ChiTietDonDatHangs.Single(n => n.MaChiTietDDH == ctddh.MaChiTietDDH);

        //    ViewBag.MaTrangThai = new SelectList(db.TrangThaiGiaoHangs.OrderBy(n => n.MaTrangThai), "MaTrangThai", "TenTrangThai", ctddhUpdate.MaTrangThai);

        //    ctddhUpdate.MaTrangThai = ctddh.MaTrangThai;
        //    db.SaveChanges();

        //    //GuiEmail("Xác nhận đơn hàng", "ducnghia1205@gmail.com", "kiembtcmp@gmail.com", "zewang.help", "Đơn hàng của bạn đã được đặt thành công");
        //    return View(ctddhUpdate);
        //}

        [HttpGet]
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult merchantDuyetDonHang2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonDatHangMerchant model = db.DonDatHangMerchants.SingleOrDefault(n => n.MaDDHMerchant == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTrangThai = new SelectList(db.TrangThaiGiaoHangs.OrderBy(n => n.MaTrangThai), "MaTrangThai", "TenTrangThai", model.MaTrangThai);
            // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
            var lstChiTietDHMerchant = db.ChiTietDonDatHangMerchants.Where(n => n.MaDDHMerchant == id);
            ViewBag.ListChiTietDH = lstChiTietDHMerchant;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult merchantDuyetDonHang2(DonDatHangMerchant ddh)
        {
            // Lấy dữ liệu của đơn hàng đó
            DonDatHangMerchant ddhUpdate = db.DonDatHangMerchants.Single(n => n.MaDDHMerchant == ddh.MaDDHMerchant);

            ViewBag.MaTrangThai = new SelectList(db.TrangThaiGiaoHangs.OrderBy(n => n.MaTrangThai), "MaTrangThai", "TenTrangThai", ddhUpdate.MaTrangThai);

            ddhUpdate.DaHuy = ddh.DaHuy;
            ddhUpdate.TinhTrangGiaoHang = ddh.TinhTrangGiaoHang;
            ddhUpdate.MaTrangThai = ddh.MaTrangThai;
            
            // list chi tiet ChiTietDonHang
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddh.MaDDH && n.SanPham.MaThanhVien == ddh.MaMerchant);
            foreach(var item in lstChiTietDH)
            {
                item.MaTrangThai = ddh.MaTrangThai;
            }
            var duyetChiTietDHMerchant = db.ChiTietDonDatHangMerchants.Where(n => n.MaDDHMerchant == ddh.MaDDHMerchant && n.SanPham.MaThanhVien == ddh.MaMerchant);
            foreach (var item in duyetChiTietDHMerchant)
            {
                item.MaTrangThai = ddh.MaTrangThai;
            }
            db.SaveChanges();
            // Lấy ds chi tiết đơn hàng để hiển thị cho người dùng thấy
            var lstChiTietDHMerchant = db.ChiTietDonDatHangMerchants.Where(n => n.MaDDHMerchant == ddh.MaDDHMerchant);
            ViewBag.ListChiTietDH = lstChiTietDHMerchant;



            //GuiEmail("Xác nhận đơn hàng", "ducnghia1205@gmail.com", "kiembtcmp@gmail.com", "zewang.help", "Đơn hàng của bạn đã được đặt thành công");
            return View(ddhUpdate);
        }
    }
}