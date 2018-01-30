using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class GioHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // Lây danh sách giỏ hàng
        public List<itemGioHang> LayGioHang()
        {
            List<itemGioHang> lstGioHang = Session["GioHang"] as List<itemGioHang>;
            if(lstGioHang == null)
            {
                //Nếu session bằng  null thì khởi tạo gio hàng
                lstGioHang = new List<itemGioHang>();
                // Gán lại giỏ hàng cho session
                Session["GioHang"] = lstGioHang;
            }
            // nếu giỏ hàng khác null ( đã có sản phẩm trong giỏ ) thì trả về  list
            return lstGioHang;
        }
        //  Thêm sản phẩm bằng cách thông thường ( Load lại trang bằng URL)
        public ActionResult ThemGioHang(int MaSP,string strURL)
        {
            // Kiểm tra trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trả về trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            // nếu != null thì Lấy giỏ hàng
            List<itemGioHang> lstGioHang = LayGioHang();
            // Xét trường hợp sản phẩm được chọn đã có trong giỏ hàng -> tăng số lượng và cập nhật thành tiền
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(spCheck != null)
            {
                // Kiểm tra số lượng tồn kho
                if(spCheck.SoLuong > sp.SoLuongTon)
                {
                    // trả về thông báo hết hàng
                    return View("ThongBao");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                // trả về trang URL hiện tại
                return Redirect(strURL);
            }
            // nếu sp không có trong giỏ hàng -> tạo sp theo MaSP mới rồi add vào giỏ hàng hiện tại
            itemGioHang itemGH = new itemGioHang(MaSP);
            // Kiểm tra số lượng tồn kho
            if (itemGH.SoLuong > sp.SoLuongTon)
            {
                // trả về thông báo hết hàng
                return View("ThongBao");
            }
            lstGioHang.Add(itemGH);
            return Redirect(strURL);

        }
        // Tính tổng số lượng
        public double TinhTongSoLuong()
        {
            // Lấy giỏ hàng từ Session 
            List<itemGioHang> lstGioHang = Session["GioHang"] as List<itemGioHang>;
            if(lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.SoLuong);
        }

        // Tính tổng tiền
        public decimal TinhTongTien()
        {
            List<itemGioHang> lstGioHang = Session["GioHang"] as List<itemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.ThanhTien);
        }

        public ActionResult GioHangPartial()
        {
            //Kiểm tra nếu tổng số lượng = 0 thì trả về View 0
            if (TinhTongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            // Gán trả về ViewBag
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();

            return PartialView();
        }


        // GET: GioHang
        public ActionResult XemGioHang()
        {
            List<itemGioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }


        //Chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang(int maSP)
        {
            // Kiểm tra giỏ hàng tồn tại hay chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Kiểm tra sp có trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Lấy list giỏ hàng từ Session
            List<itemGioHang> lstGioHang = LayGioHang();
            // Kiểm tra sp sửa có tồn tại trong list hay không
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == maSP);
            if(spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Gán lstGioHang qua ViewBag để tạo giao diện chỉnh sửa
            ViewBag.GioHang = lstGioHang;


            //Nếu tồn tại rồi
            return View(spCheck);
        }

        // Chức năng xử lý cập nhật giỏ hàng CapNhatGioHang
        // nhận 1 biến itemGioHang
        [HttpPost]
        public ActionResult CapNhatGioHang(itemGioHang itemGH)
        {
            // Kiểm tra tồn kho
            SanPham spCheck = db.SanPhams.Single(n => n.MaSP == itemGH.MaSP);
            if(spCheck.SoLuongTon< itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            // Cập nhật số lượng trong session giỏ hàng
            List<itemGioHang> lstGioHang = LayGioHang();
            // tìm itemGH trong lstGioHang
            itemGioHang itemGHUpdate = lstGioHang.Find(n => n.MaSP == itemGH.MaSP);
            itemGHUpdate.SoLuong = itemGH.SoLuong;
            // Cập nhật số lượng --> cập nhật thành tiền
            itemGHUpdate.ThanhTien = itemGHUpdate.DonGia * itemGHUpdate.SoLuong;


            //return RedirectToAction("SuaGioHang",new { @maSP = itemGHUpdate.MaSP});
            return RedirectToAction("XemGioHang");
        }

        public ActionResult XoaGioHang(int maSP)
        {
            // Kiểm tra giỏ hàng tồn tại hay chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Kiểm tra sp có trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Lấy list giỏ hàng từ Session
            List<itemGioHang> lstGioHang = LayGioHang();
            // Kiểm tra sp sửa có tồn tại trong list hay không
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == maSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xóa item trong giỏ hàng
            lstGioHang.Remove(spCheck);

            return RedirectToAction("XemGioHang");

        }

        //Xây dựng chức năng đặt hàng
        public ActionResult DatHang(KhachHang kh)
        {
            // Kiểm tra giỏ hàng tồn tại hay chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KhachHang khang = new KhachHang();
            if(Session["TaiKhoan"] == null)
            {
                //Thêm kh vào bảng KhachHang ...khi chưa đăng nhập
                khang = kh;
                db.KhachHangs.Add(khang);
                db.SaveChanges();
            }
            else
            {
                // Thêm kh bằng session Taikhoan
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                khang.TenKH = tv.HoTen;
                khang.DiaChi = tv.DiaChi;
                khang.Email = tv.Email;
                khang.SoDienThoai = tv.SoDienThoai;
                khang.MaThanhVien = tv.MaThanhVien;
                db.KhachHangs.Add(khang);
                db.SaveChanges();
            }
            //Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = khang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.MaTrangThai = 1;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            ddh.DaHuy = false;
            ddh.DaXoa = false;
            db.DonDatHangs.Add(ddh);
            db.SaveChanges();
            // Thêm chi tiết đơn hàng
            List<itemGioHang> lstGioHang = LayGioHang();
            foreach(var item in lstGioHang)
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.MaTrangThai = 1;
                ctdh.TenSP = item.TenSP;
                ctdh.MaSP = item.MaSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia;
                db.ChiTietDonDatHangs.Add(ctdh);
                // trừ tồn
                SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == item.MaSP);
                sp.SoLuongTon -= item.SoLuong;
            }
            db.SaveChanges();
            Session["GioHang"] = null;
            var lstSP = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddh.MaDDH);
            var lstMerchant = db.ThanhViens.Where(n => n.MaLoaiTV == 2).ToList();
            foreach(var merchant in lstMerchant)
            {
                var find = lstSP.Where(n => n.SanPham.MaThanhVien == merchant.MaThanhVien);
                if (find.Any())
                {
                    DonDatHangMerchant ddhMerchant = new DonDatHangMerchant();
                    ddhMerchant.MaDDH = ddh.MaDDH;
                    ddhMerchant.MaKH = ddh.MaKH;
                    ddhMerchant.MaMerchant = merchant.MaThanhVien;
                    ddhMerchant.NgayDat = ddh.NgayDat;
                    ddhMerchant.MaTrangThai = 1;
                    db.DonDatHangMerchants.Add(ddhMerchant);
                    db.SaveChanges();
                    foreach (var item in find)
                    {
                        ChiTietDonDatHangMerchant ctdhMerchant = new ChiTietDonDatHangMerchant();
                        ctdhMerchant.MaDDHMerchant = ddhMerchant.MaDDHMerchant;
                        ctdhMerchant.MaTrangThai = 1;
                        
                        ctdhMerchant.MaSP = item.MaSP;
                        ctdhMerchant.TenSP = item.TenSP;
                        ctdhMerchant.SoLuong = item.SoLuong;
                        ctdhMerchant.DonGia = item.DonGia;
                        db.ChiTietDonDatHangMerchants.Add(ctdhMerchant);
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("XemGioHang");

        }


        // Thêm giỏ hàng Ajax
        public ActionResult ThemGioHangAjax(int MaSP, string strURL)
        {
            // Kiểm tra trong csdl 
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trả về trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            // nếu != null thì Lấy giỏ hàng
            List<itemGioHang> lstGioHang = LayGioHang();
            // Xét trường hợp sản phẩm được chọn đã có trong giỏ hàng -> tăng số lượng và cập nhật thành tiền
            itemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                // Kiểm tra số lượng tồn kho
                if (spCheck.SoLuong >= sp.SoLuongTon)
                {
                    // trả về thông báo hết hàng
                    //return Content("<script>alert(\"Sản phẩm đã hết hàng\")</script>");
                    ViewBag.TongTien = TinhTongTien();
                    ViewBag.TongSoLuong = TinhTongSoLuong();
                    TempData["msg"] = "<script>alert('Sản phẩm đã hết hàng');</script>";
                    //ViewBag.ThongBao = "< script > alert(\"Sản phẩm đã hết hàng\")</script>";
                    return PartialView("GioHangPartial");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                // trả về trang URL hiện tại
                //return Redirect(strURL);
                // Trả về PartialView GioHangPartial --> cập nhật lại ViewBag
                ViewBag.TongTien = TinhTongTien();
                ViewBag.TongSoLuong = TinhTongSoLuong();
                return PartialView("GioHangPartial");
            } 
            // nếu sp không có trong giỏ hàng -> tạo sp theo MaSP mới rồi add vào giỏ hàng hiện tại
            itemGioHang itemGH = new itemGioHang(MaSP);
            // Kiểm tra số lượng tồn kho
            if (itemGH.SoLuong >= sp.SoLuongTon)
            {
                // trả về thông báo hết hàng
                ViewBag.TongTien = TinhTongTien();
                ViewBag.TongSoLuong = TinhTongSoLuong();
                TempData["msg"] = "<script>alert('Sản phẩm đã hết hàng');</script>";
                return PartialView("GioHangPartial");
                //return Content("<script>alert(\"Sản phẩm đã hết hàng\")</script>");
            }
            lstGioHang.Add(itemGH);
            ViewBag.TongTien = TinhTongTien();
            ViewBag.TongSoLuong = TinhTongSoLuong();
            return PartialView("GioHangPartial");

        }

        public ActionResult LichSuMuaHang()
        {
            
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            List<DonDatHang> lstDDH = db.DonDatHangs.Where(n => n.KhachHang.MaThanhVien == tv.MaThanhVien).ToList();
            return View(lstDDH);
        }

        public ActionResult ChiTietHoaDonKH(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var lstCTDDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
            ViewBag.MaDDH = id;
            return View(lstCTDDH);

        }

        // Thông tin liên hệ merchant
        public ActionResult ThongTinMerchant(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhVien merchant = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == id);
            return View(merchant);
        }
    }
}