using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteBanHang.Models
{
    public class itemGioHang
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }

        public string NguoiBan { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string HinhAnh { get; set; }

        public itemGioHang()
        {

        }

        public itemGioHang(int iMaSP)
        {
            using (QuanLyBanHangEntities db = new QuanLyBanHangEntities())
            {
                this.MaSP = iMaSP;
                SanPham sp = db.SanPhams.Single(n => n.MaSP == iMaSP);
                this.TenSP = sp.TenSP;
                this.NguoiBan = sp.ThanhVien.TaiKhoan;
                this.DonGia = sp.DonGia.Value;
                this.HinhAnh = sp.HinhAnh;
                //khởi tạo thì sl = 1
                this.SoLuong = 1;
                this.ThanhTien = DonGia * SoLuong;

            }
        }
        public itemGioHang(int iMaSP,int sl)
        {
            using (QuanLyBanHangEntities db = new QuanLyBanHangEntities())
            {
                this.MaSP = iMaSP;
                SanPham sp = db.SanPhams.Single(n => n.MaSP == iMaSP);
                this.TenSP = sp.TenSP;
                this.NguoiBan = sp.ThanhVien.TaiKhoan;
                this.DonGia = sp.DonGia.Value;
                this.HinhAnh = sp.HinhAnh;
                this.SoLuong = sl;
                this.ThanhTien = DonGia * SoLuong;

            }

            
        }

    }
}