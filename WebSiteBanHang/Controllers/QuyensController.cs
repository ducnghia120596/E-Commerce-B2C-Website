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
    public class QuyensController : Controller
    {
        private QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: Quyens
        [Authorize(Roles = "QuanTri")]
        public ActionResult Index()
        {
            return View(db.Quyens.ToList());
        }

        // GET: Quyens/Details/5
        [Authorize(Roles = "QuanTri")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quyen quyen = db.Quyens.Find(id);
            if (quyen == null)
            {
                return HttpNotFound();
            }
            return View(quyen);
        }

        // GET: Quyens/Create
        [Authorize(Roles = "QuanTri")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Quyens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanTri")]
        public ActionResult Create([Bind(Include = "MaQuyen,TenQuyen")] Quyen quyen)
        {
            if (ModelState.IsValid)
            {
                db.Quyens.Add(quyen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quyen);
        }

        // GET: Quyens/Edit/5
        [Authorize(Roles = "QuanTri")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quyen quyen = db.Quyens.Find(id);
            if (quyen == null)
            {
                return HttpNotFound();
            }
            return View(quyen);
        }

        // POST: Quyens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanTri")]
        public ActionResult Edit([Bind(Include = "MaQuyen,TenQuyen")] Quyen quyen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quyen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quyen);
        }

        //// GET: Quyens/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Quyen quyen = db.Quyens.Find(id);
        //    if (quyen == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(quyen);
        //}

        //// POST: Quyens/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Quyen quyen = db.Quyens.Find(id);
        //    db.Quyens.Remove(quyen);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
