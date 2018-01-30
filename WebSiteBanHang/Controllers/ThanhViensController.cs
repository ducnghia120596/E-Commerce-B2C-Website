using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class ThanhViensController : Controller
    {
        private QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: ThanhViens
        // Danh sách thành viên merchant
        [Authorize(Roles = "QuanTri")]
        public ActionResult Index(string currentFilter, string searchString, int? page)
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
            var lstTV = from s in db.ThanhViens
                        where s.MaLoaiTV == 2
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstTV = lstTV.Where(n => n.HoTen.Contains(searchString));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(lstTV.OrderBy(n => n.MaThanhVien).ToPagedList(pageNumber, pageSize));
        }
        // Danh sách thành viên customer
        [Authorize(Roles = "QuanTri")]
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
            var lstTV = from s in db.ThanhViens
                        where s.MaLoaiTV == 3
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstTV = lstTV.Where(n => n.HoTen.Contains(searchString));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(lstTV.OrderBy(n => n.MaThanhVien).ToPagedList(pageNumber, pageSize));
        }

        // Quản lý thông tin cá nhân Merchant ( MERCHANT )
        [HttpGet]
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult QLThongtin()
        {
            
            if(Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "merchantLogin");
            }
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            ThanhVien newTV = db.ThanhViens.Single(n => n.MaThanhVien == tv.MaThanhVien);
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens, "MaLoaiTV", "TenLoai", newTV.MaLoaiTV);
            ViewBag.TinhTrang = new SelectList(db.TinhTrangThanhViens, "MaTinhTrang", "TenTinhTrang", newTV.TinhTrang);
            return View(newTV);
        }

        [HttpPost]
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult QLThongtin(ThanhVien thvien)
        {
            //if (ModelState.IsValid)
            //{

            //    db.Entry(thvien).State = EntityState.Modified;
            //    db.SaveChanges();
            //    //return RedirectToAction("Index");
            //    ViewBag.ThongBao = "Cập nhật thành công";
            //}
            ThanhVien UpdateThanhVien = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == thvien.MaThanhVien);
            if (thvien.MatKhau!=null)
            {
                UpdateThanhVien.MatKhau = EncodePassword(thvien.MatKhau);
            }
            
            UpdateThanhVien.HoTen = thvien.HoTen;
            UpdateThanhVien.DiaChi = thvien.DiaChi;
            UpdateThanhVien.Email = thvien.Email;
            UpdateThanhVien.SoDienThoai = thvien.SoDienThoai;
            UpdateThanhVien.CauTraLoi = thvien.CauTraLoi;
            db.SaveChanges();
            ViewBag.ThongBao = "Cập nhật thành công";
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens, "MaLoaiTV", "TenLoai", thvien.MaLoaiTV);
            ViewBag.TinhTrang = new SelectList(db.TinhTrangThanhViens, "MaTinhTrang", "TenTinhTrang", thvien.TinhTrang);
            return View(UpdateThanhVien);
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
        // GET: ThanhViens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhVien thanhVien = db.ThanhViens.Find(id);
            if (thanhVien == null)
            {
                return HttpNotFound();
            }
            return View(thanhVien);
        }

        // GET: ThanhViens/Create
        public ActionResult Create()
        {
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens, "MaLoaiTV", "TenLoai");
            ViewBag.TinhTrang = new SelectList(db.TinhTrangThanhViens, "MaTinhTrang", "TenTinhTrang");
            return View();
        }

        // POST: ThanhViens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaThanhVien,TaiKhoan,MatKhau,HoTen,DiaChi,Email,SoDienThoai,CauHoi,CauTraLoi,MaLoaiTV,SoLuongTin,SoLuongTinDaDang,TinhTrang")] ThanhVien thanhVien)
        {
            if (ModelState.IsValid)
            {
                db.ThanhViens.Add(thanhVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens, "MaLoaiTV", "TenLoai", thanhVien.MaLoaiTV);
            ViewBag.TinhTrang = new SelectList(db.TinhTrangThanhViens, "MaTinhTrang", "TenTinhTrang", thanhVien.TinhTrang);
            return View(thanhVien);
        }

        // GET: ThanhViens/Edit/5
        [Authorize(Roles ="QuanTri")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhVien thanhVien = db.ThanhViens.Find(id);
            if (thanhVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens, "MaLoaiTV", "TenLoai", thanhVien.MaLoaiTV);
            ViewBag.TinhTrang = new SelectList(db.TinhTrangThanhViens, "MaTinhTrang", "TenTinhTrang", thanhVien.TinhTrang);
            return View(thanhVien);
        }

        // POST: ThanhViens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanTri")]
        public ActionResult Edit([Bind(Include = "MaThanhVien,TaiKhoan,MatKhau,HoTen,DiaChi,Email,SoDienThoai,CauHoi,CauTraLoi,MaLoaiTV,SoLuongTin,SoLuongTinDaDang,TinhTrang")] ThanhVien thanhVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thanhVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens, "MaLoaiTV", "TenLoai", thanhVien.MaLoaiTV);
            ViewBag.TinhTrang = new SelectList(db.TinhTrangThanhViens, "MaTinhTrang", "TenTinhTrang", thanhVien.TinhTrang);
            return View(thanhVien);
        }

        // GET: ThanhViens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhVien thanhVien = db.ThanhViens.Find(id);
            if (thanhVien == null)
            {
                return HttpNotFound();
            }
            return View(thanhVien);
        }

        // POST: ThanhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThanhVien thanhVien = db.ThanhViens.Find(id);
            db.ThanhViens.Remove(thanhVien);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
