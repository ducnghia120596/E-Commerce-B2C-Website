using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using PagedList;
namespace WebSiteBanHang.Controllers
{
    public class QuanLySanPhamController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: QuanLySanPham

        //[HttpGet]
        //[Authorize(Roles = "QLSanPham")]
        //public ActionResult Index()
        //{
        //    return View(db.SanPhams.Where(n=>n.DaXoa==false).OrderBy(n=>n.MaSP).ToList());
        //}
        //[HttpPost]
        //[Authorize(Roles = "QLSanPham")]
        //public ActionResult Index(string sTuKhoa)
        //{
        //    List<SanPham> lstSP = db.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa)).ToList();
        //    return View(lstSP.OrderBy(n => n.MaSP));
        //}
        [Authorize(Roles = "QuanTri")]
        public ActionResult DanhSachSP(string sortOrder,string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TenSortParm = String.IsNullOrEmpty(sortOrder) ? "tenSP_desc" : "";
            ViewBag.NgaySortParm = sortOrder == "Date" ? "ngay_desc" : "Date";
            ViewBag.DonGiaSortParm = sortOrder == "DonGia" ? "DonGia_desc" : "DonGia";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var lstSP = from s in db.SanPhams
                        where s.DaXoa==false
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                lstSP = lstSP.Where(n => n.TenSP.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "tenSP_desc":
                    lstSP = lstSP.OrderByDescending(n => n.TenSP);
                    break;
                case "Date":
                    lstSP = lstSP.OrderBy(n => n.NgayCapNhap);
                    break;
                case "ngay_desc":
                    lstSP = lstSP.OrderByDescending(n => n.NgayCapNhap);
                    break;
                case "DonGia":
                    lstSP = lstSP.OrderBy(n => n.DonGia);
                    break;
                case "DonGia_desc":
                    lstSP = lstSP.OrderByDescending(n => n.DonGia);
                    break;
                default:
                    lstSP = lstSP.OrderBy(n => n.TenSP);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            /*return View(lstSP.OrderBy(n => n.NgayCapNhap).ToPagedList(pageNumber, pageSize))*/;
            return View(lstSP.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        // danh sách sản phẩm đã đăng của merchant
        [Authorize(Roles ="QLSanPhamMerchant")]
        public ActionResult DanhSachSPDaDang()
        {
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            int maMerchant = tv.MaThanhVien;
            var lstSP = db.SanPhams.Where(n => n.MaThanhVien == maMerchant);
            return View(lstSP.OrderByDescending(n => n.NgayCapNhap));
        }
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult TaoMoi()
        {
            //Load dropdownlist nhà cung cấp và loại sản phẩm
            //ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            //ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX");
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            if(tv.TinhTrang == 2)
            {
                return RedirectToAction("KhoaTaiKhoan","Home");
            }
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");
            return View();
        }
        //Tat bat loi
        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult TaoMoi(SanPham sp,HttpPostedFileBase HinhAnh)
        {

            //ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            //ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX");

            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");


            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == sp.MaThanhVien);
            if (tv.SoLuongTin < 1)
            {
                ViewBag.ThongBao = "Số lượng tin đăng đã hết!";
                return View();
            }
            else
            {
                //Kiểm tra hình tồn tại trong csdl chưa 
                //if (HinhAnh.ContentLength > 0)
                //{
                    // Lấy tên hình
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    // Lấy hình ảnh chèn vào thư mục hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnhSP"), fileName);
                    //Nếu thư mục chứa hình ảnh rồi thì xuất ra thông báo
                    //if (System.IO.File.Exists(path))
                    //{
                    //    ViewBag.upload = "Hình đã tồn tại!";
                    //    return View();
                    //}
                    //else
                    //{
                        HinhAnh.SaveAs(path);
                        sp.HinhAnh = fileName;
                tv.SoLuongTin--;
                tv.SoLuongTinDaDang++;
                db.SanPhams.Add(sp);
                db.SaveChanges();
                ViewBag.ThongBao = "Đăng bán sản phẩm thành công";
                return RedirectToAction("DanhSachSPDaDang");

                //}
            }

                

            }

        
        [ValidateInput(false)]
        [HttpGet]
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult ChinhSua(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }


            //Load dropdownlist nhà cung cấp và loại sản phẩm
            //ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            //ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);

            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);

            return View(sp);
        }
        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "QLSanPhamMerchant")]
        public ActionResult ChinhSua(SanPham model)
        {
            //Load dropdownlist nhà cung cấp và loại sản phẩm
            //ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", model.MaNCC);
            //ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", model.MaNSX);
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == model.MaThanhVien);
            if (tv.SoLuongTin < 1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", model.MaLoaiSP);
                SanPham updateSanPham = db.SanPhams.Single(n => n.MaSP == model.MaSP);
                updateSanPham.NgayCapNhap = model.NgayCapNhap;
                updateSanPham.SoLuongTon = model.SoLuongTon;
                tv.SoLuongTin--;
                tv.SoLuongTinDaDang++;
                //db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachSPDaDang");
            }
            



            // Su dung neu cai dat valid ben meta data day du
            //if (ModelState.IsValid)
            //{
            //    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //return View(model);
        }

        //[HttpGet]
        //public ActionResult Xoa(int? id)
        //{

        //    //Lấy sản phẩm cần chỉnh sửa dựa vào id
        //    if (id == null)
        //    {
        //        Response.StatusCode = 404;
        //        return null;
        //    }
        //    SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
        //    if (sp == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    //Load dropdownlist nhà cung cấp và dropdownlist loại sp, mã nhà sản xuất
        //    ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
        //    ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
        //    ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", sp.MaNSX);
        //    return View(sp);
        //}

        //[HttpPost]
        //public ActionResult Xoa(int? id, FormCollection f)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SanPham model = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    db.SanPhams.Remove(model);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}