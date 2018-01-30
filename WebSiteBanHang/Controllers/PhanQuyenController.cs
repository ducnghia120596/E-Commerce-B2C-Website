using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
namespace WebSiteBanHang.Controllers
{
    public class PhanQuyenController : Controller
    {
        // GET: PhanQuyen
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        [Authorize(Roles = "QuanTri")]
        public ActionResult Index()
        {
            return View(db.LoaiThanhViens.OrderBy(n=>n.MaLoaiTV));
        }

        [HttpGet]
        [Authorize(Roles = "QuanTri")]
        public ActionResult PhanQuyen(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            LoaiThanhVien ltv = db.LoaiThanhViens.SingleOrDefault(n => n.MaLoaiTV == id);
            if (ltv == null)
            {
                return HttpNotFound();
            }
            // Lấy danh sách quyền để load ra checkbox
            ViewBag.MaQuyen = db.Quyens;
            //Lấy ra danh sách quyền của loại thành viên được chọn..dựa vào bảng LoaiTV_Quyen
            ViewBag.LoaiTVQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == id);

            return View(ltv);
        }

        [HttpPost]
        [Authorize(Roles = "QuanTri")]
        public ActionResult PhanQuyen(int? MaLoaiTV, IEnumerable<LoaiThanhVien_Quyen> lstPhanQuyen)
        {
            //Trường hợp : Nếu đã đã tiến hành phân quyền rồi nhưng muốn phân quyền lại
            //Bước 1: Xóa những quyền cũa thuộc loại TV đó
            var lstDaPhanQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == MaLoaiTV);
            if (lstDaPhanQuyen.Count() != 0)
            {
                db.LoaiThanhVien_Quyen.RemoveRange(lstDaPhanQuyen);
                db.SaveChanges();
            }
            if (lstPhanQuyen != null)
            {
                //Kiểm tra list danh sách quyền được check
                foreach(var item in lstPhanQuyen)
                {
                    item.MaLoaiTV = int.Parse(MaLoaiTV.ToString());
                    // Nếu được check thì insert dữ liệu vào bảng phân quyền
                    db.LoaiThanhVien_Quyen.Add(item);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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