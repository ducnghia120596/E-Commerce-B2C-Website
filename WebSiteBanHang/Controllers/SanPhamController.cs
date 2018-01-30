using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using PagedList;
namespace WebSiteBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        //[ChildActionOnly]
        //public ActionResult SanPhamStyle1Partial()
        //{
        //    return PartialView();
        //}

        //[ChildActionOnly]
        //public ActionResult SanPhamStyle1Partia2()
        //{
        //    return PartialView();
        //}

        //Xây dựng trang xem chi tiết
        public ActionResult XemChiTiet(int? id, string tensp)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Nếu không thì truy xuất csdl lấy ra sản phẩm tương ứng id
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id && n.DaXoa == false);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }
        public ActionResult DanhSachGiay(int? page)
        {
            var lstSP = db.SanPhams.Where(n => n.MaLoaiSP == 1);
            if (lstSP.Count() == 0)
            {
                return HttpNotFound();
            }
            //Thực hiện chức năng phân trang
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //Tạo biến số sp trên trang
            int PageSize = 6;
            //Tạo biến thứ 2 : Số trang hiện tại
            int PageNumber = (page ?? 1);
            //ViewBag.MaLoaiSP = MaLoaiSP;
            //ViewBag.MaNSX = MaNSX;

            //return View(lstSP);
            // trả về dạng list đã sắp xếp
            return View(lstSP.OrderBy(n => n.NgayCapNhap).ToPagedList(PageNumber, PageSize));
        }

        public ActionResult DanhSachDep(int? page)
        {
            var lstSP = db.SanPhams.Where(n => n.MaLoaiSP == 2);
            if (lstSP.Count() == 0)
            {
                return HttpNotFound();
            }
            //Thực hiện chức năng phân trang
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //Tạo biến số sp trên trang
            int PageSize = 6;
            //Tạo biến thứ 2 : Số trang hiện tại
            int PageNumber = (page ?? 1);
            //ViewBag.MaLoaiSP = MaLoaiSP;
            //ViewBag.MaNSX = MaNSX;

            //return View(lstSP);
            // trả về dạng list đã sắp xếp
            return View(lstSP.OrderBy(n => n.NgayCapNhap).ToPagedList(PageNumber, PageSize));
        }
        public ActionResult SanPham(int? MaLoaiSP, int? MaNSX,int? page)
        {
            //Chặn không cho xem nếu không đăng nhập
            //if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //if (MaLoaiSP==null || MaNSX == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            // Load sản phẩm theo 2 tiêu chí là Mã loại SP và Mã nhà sản xuất ( trong bản SanPham)
            var lstSP = db.SanPhams.Where(n=>n.MaLoaiSP == MaLoaiSP && n.MaNSX ==MaNSX);
            if (lstSP.Count() == 0)
            {
                return HttpNotFound();
            }
            //Thực hiện chức năng phân trang
            if(Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //Tạo biến số sp trên trang
            int PageSize = 3;
            //Tạo biến thứ 2 : Số trang hiện tại
            int PageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSP;
            ViewBag.MaNSX = MaNSX;

            //return View(lstSP);
            // trả về dạng list đã sắp xếp
            return View(lstSP.OrderBy(n => n.MaSP).ToPagedList(PageNumber, PageSize));
        }
    }
}