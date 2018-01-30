using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc;
using System.Web.Security;
using PagedList;
using System.Security.Cryptography;
using System.Text;

namespace WebSiteBanHang.Controllers
{
    public class HomeController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        
        public ActionResult AdminPageLayout()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap2", "Home");
            }
            else
            {
                ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
                if (tv.MaLoaiTV == 3 || tv.MaLoaiTV==2)
                {
                    Session["TaiKhoan"] = null;
                    return RedirectToAction("DangNhap2", "Home");
                }
                return View();
            }
           
        }

        public ActionResult MerchantPageLayout()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap2", "Home");
            }
            else
            {
                ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
                if (tv.MaLoaiTV == 3 || tv.MaLoaiTV == 1)
                {
                    Session["TaiKhoan"] = null;
                    return RedirectToAction("DangNhap2", "Home");
                }
                return View();
            }
        }
        public ActionResult Index(int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //Tạo biến số sp trên trang
            int PageSize = 18;
            //Tạo biến thứ 2 : Số trang hiện tại
            int PageNumber = (page ?? 1);
            var lstSP = db.SanPhams.OrderByDescending(n => n.NgayCapNhap);
            return View(lstSP.ToPagedList(PageNumber, PageSize));

        }



        public ActionResult MenuPartial()
        {
            var lstSP = db.SanPhams; 
            return PartialView(lstSP);
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            return View();
        }

        
        [HttpPost]
        public ActionResult DangKy(ThanhVien tv, FormCollection f)
        {
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            //Kiểm tra Captcha hợp lệ
            bool check = Convert.ToBoolean(f["checkMerchant"].Split(',')[0]);
            if (this.IsCaptchaValid("Captcha is not valid"))
            {
                if (ModelState.IsValid)
                {
                    if (check == true)
                    {
                        tv.MaLoaiTV = 2;
                    }
                    else
                    {
                        tv.MaLoaiTV = 3;
                    }
                    tv.SoLuongTin = 0;
                    tv.SoLuongTinDaDang = 0;
                    tv.TinhTrang = 0;
                    tv.MatKhau = EncodePassword(tv.MatKhau);
                    // ViewBag.ThongBao = "Thêm thành công";
                    TempData["thongbao"] = "<script>alert('Đăng ký tài khoản thành công.');</script>";
                    db.ThanhViens.Add(tv);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ThongBao = "Thêm thất bại";
                }
                
                return View();
            }
            
            ViewBag.ThongBao = "Sai mã Captcha";
            return View();
        }
        public List<string> LoadCauHoi()
        {
            List<string> lstCauHoi = new List<string>();
            lstCauHoi.Add("Con vật mà bạn yêu thích?");
            lstCauHoi.Add("Ca sĩ mà bạn yêu thích?");
            lstCauHoi.Add("Nghề nghiệp của bạn là gì?");
            return lstCauHoi;
        }

        //Xây dựng Action đăng nhập
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            ////Kiểm tra tên đăng nhập và mật khẩu
            //string sTaiKhoan = f["txtTaiKhoan"].ToString();
            //string sMatKhau = f["txtMatKhau"].ToString();

            //ThanhVien tv = db.ThanhViens.SingleOrDefault(n=>n.TaiKhoan==sTaiKhoan && n.MatKhau==sMatKhau);

            //if (tv != null)
            //{
            //    Session["TaiKhoan"] = tv;
            //    return Content("<script>window.location.reload()</script>");
            //}
            //return Content("Tài khoản hoặc mật khẩu không đúng!");
            string taikhoan = f["txtTaiKhoan"].ToString();
            //string matkhau = f["txtMatKhau"].ToString();
            string matkhau = EncodePassword(f["txtMatKhau"].ToString());
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n=>n.TaiKhoan==taikhoan && n.MatKhau==matkhau);
            if (tv != null)
            {
                //Láy ra List quyền của thành viên tương ứng với loại thành viên
                var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV);
                //Duyệt list quyền
                string Quyen = "";
                foreach(var item in lstQuyen)
                {
                    Quyen += item.MaQuyen + ",";
                }
                // Cắt dấu ","
                Quyen = Quyen.Substring(0, Quyen.Length - 1);
                PhanQuyen(tv.TaiKhoan,Quyen);
                Session["TaiKhoan"] = tv;
                return Content("<script>window.location.reload()</script>");
            }
            return Content("Tài khoản hoặc mật khẩu không đúng!");

        }
        // Đăng nhập cho merchant và admin
        public ActionResult DangNhap2()
        {
            return View();
        }


        [HttpPost]
        public ActionResult DangNhap2(FormCollection f)
        {

            string taikhoan = f["txtTaiKhoan"].ToString();
            //string matkhau = f["txtMatKhau"].ToString();
            string matkhau = EncodePassword(f["txtMatKhau"].ToString());
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == taikhoan && n.MatKhau == matkhau);
            if (tv != null)
            {
                //Láy ra List quyền của thành viên tương ứng với loại thành viên
                var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV);
                //Duyệt list quyền
                string Quyen = "";
                foreach (var item in lstQuyen)
                {
                    Quyen += item.MaQuyen + ",";
                }
                // Cắt dấu ","
                Quyen = Quyen.Substring(0, Quyen.Length - 1);
                PhanQuyen(tv.TaiKhoan, Quyen);
                Session["TaiKhoan"] = tv;
                if (tv.MaLoaiTV == 1)
                {
                    return RedirectToAction("AdminPageLayout", "Home");
                }
                else
                {
                    if (tv.MaLoaiTV == 2 && tv.TinhTrang != 0)
                    {
                        return RedirectToAction("MerchantPageLayout", "Home");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tài khoản bán hàng chưa được kích hoạt!";
                        return View();
                    }
                }


                //return View();
            }
            ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không đúng";
            return View();
        }

        public void PhanQuyen(string TaiKhoan, string Quyen)
        {
            FormsAuthentication.Initialize();
            var ticket = new FormsAuthenticationTicket(1,
                                          TaiKhoan, //user
                                          DateTime.Now, //Thời gian bắt đầu
                                          DateTime.Now.AddHours(3), //Thời gian kết thúc
                                          false, //Ghi nhớ?
                                          Quyen, // "DangKy,QuanLyDonHang,QuanLySanPham"
                                          FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);
        }

        

        public ActionResult Dangxuat()
        {
            //Gán về null
            Session["TaiKhoan"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
        public ActionResult Dangxuat2()
        {
            //Gán về null
            Session["TaiKhoan"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("DangNhap2", "Home");
        }

        public static string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        // Tạo trang ngăn chặn truy cập
        public ActionResult LoiPhanQuyen()
        {
            return View();
        }
        // Tạo trang ngăn chặn truy cập
        public ActionResult KhoaTaiKhoan()
        {
            return View();
        }
    }
}