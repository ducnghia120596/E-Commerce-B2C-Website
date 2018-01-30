using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using PagedList;
namespace WebSiteBanHang.Controllers
{
    public class TimKiemController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: TimKiem
        [HttpGet]
        public ActionResult KQTimKiem(string sTuKhoa,int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //Tạo biến số sp trên trang
            int PageSize = 3;
            //Tạo biến thứ 2 : Số trang hiện tại
            int PageNumber = (page ?? 1);
            ViewBag.TuKhoa = sTuKhoa;
            // Tìm kiếm theo tên SP
            var lstSP = db.SanPhams.Where(n=>n.TenSP.Contains(sTuKhoa));
            return View(lstSP.OrderBy(n=>n.TenSP).ToPagedList(PageNumber,PageSize));
        }

        // Lưu lại từ khóa tìm kiếm
        // Bên form Submit thì bên Controller dùng HttpPost
        [HttpPost]
        public ActionResult LayTuKhoaTimKiem(string sTuKhoa)
        {
            // Gọi về hàm get tìm kiếm
            return RedirectToAction("KQTimKiem",new { @sTuKhoa=sTuKhoa});
        }


        public ActionResult KQTimKiemPartial(string sTuKhoa)
        {
            // tìm kiếm theo tên sản phẩm
            var lstSP = db.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa));
            return PartialView(lstSP.OrderBy(n => n.DonGia));
        }
    }
}