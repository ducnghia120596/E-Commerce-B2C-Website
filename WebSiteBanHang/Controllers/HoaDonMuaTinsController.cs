using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class HoaDonMuaTinsController : Controller
    {
        private QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: HoaDonMuaTins
        // danh sách hóa đơn mua tin ADMIN
        [Authorize(Roles ="QuanTri")]
        public ActionResult Index()
        {
            var hoaDonMuaTins = db.HoaDonMuaTins.Include(h => h.ThanhVien);
            return View(hoaDonMuaTins.ToList());
        }
        [Authorize(Roles = "QLSanPhamMerchant")]
        // danh sách hóa đơn mua tin MERCHANT
        public ActionResult danhSachHDMuaTinMerchant()
        {
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            var hoaDonMuaTins = db.HoaDonMuaTins.Where(h => h.MaThanhVien == tv.MaThanhVien);
            return View(hoaDonMuaTins.ToList());
        }

        // GET: HoaDonMuaTins/Details/5
        [Authorize(Roles = "QuanTri,QLSanPhamMerchant")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDonMuaTin hoaDonMuaTin = db.HoaDonMuaTins.Find(id);
            if (hoaDonMuaTin == null)
            {
                return HttpNotFound();
            }
            return View(hoaDonMuaTin);
        }

        // GET: HoaDonMuaTins/Create
        [Authorize(Roles = "QuanTri")]
        public ActionResult Create()
        {
            ViewBag.MaThanhVien = new SelectList(db.ThanhViens.Where(n=>n.MaLoaiTV==2), "MaThanhVien", "TaiKhoan");
            return View();
        }

        // POST: HoaDonMuaTins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanTri")]
        public ActionResult Create([Bind(Include = "Id,MaThanhVien,NgayMua,SoLuong,DonGia,ThanhTien")] HoaDonMuaTin hoaDonMuaTin)
        {
            if (ModelState.IsValid)
            {
                ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == hoaDonMuaTin.MaThanhVien);
                tv.SoLuongTin += hoaDonMuaTin.SoLuong;
                //db.Entry(hoaDonMuaTin).State = EntityState.Modified;
                //db.Entry(tv).State = EntityState.Modified;
                db.HoaDonMuaTins.Add(hoaDonMuaTin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaThanhVien = new SelectList(db.ThanhViens, "MaThanhVien", "TaiKhoan", hoaDonMuaTin.MaThanhVien);
            return View(hoaDonMuaTin);
        }

        // GET: HoaDonMuaTins/Edit/5
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDonMuaTin hoaDonMuaTin = db.HoaDonMuaTins.Find(id);
            if (hoaDonMuaTin == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaThanhVien = new SelectList(db.ThanhViens, "MaThanhVien", "TaiKhoan", hoaDonMuaTin.MaThanhVien);
            return View(hoaDonMuaTin);
        }

        // POST: HoaDonMuaTins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MaThanhVien,NgayMua,SoLuong,DonGia,ThanhTien")] HoaDonMuaTin hoaDonMuaTin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoaDonMuaTin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaThanhVien = new SelectList(db.ThanhViens, "MaThanhVien", "TaiKhoan", hoaDonMuaTin.MaThanhVien);
            return View(hoaDonMuaTin);
        }

        // GET: HoaDonMuaTins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDonMuaTin hoaDonMuaTin = db.HoaDonMuaTins.Find(id);
            if (hoaDonMuaTin == null)
            {
                return HttpNotFound();
            }
            return View(hoaDonMuaTin);
        }

        // POST: HoaDonMuaTins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HoaDonMuaTin hoaDonMuaTin = db.HoaDonMuaTins.Find(id);
            db.HoaDonMuaTins.Remove(hoaDonMuaTin);
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
