//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebSiteBanHang.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DonDatHangMerchant
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonDatHangMerchant()
        {
            this.ChiTietDonDatHangMerchants = new HashSet<ChiTietDonDatHangMerchant>();
        }
    
        public int MaDDHMerchant { get; set; }
        public Nullable<int> MaDDH { get; set; }
        public Nullable<System.DateTime> NgayDat { get; set; }
        public Nullable<int> MaTrangThai { get; set; }
        public Nullable<System.DateTime> NgayGiao { get; set; }
        public Nullable<bool> DaThanhToan { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<int> MaMerchant { get; set; }
        public Nullable<int> UuDai { get; set; }
        public Nullable<bool> DaHuy { get; set; }
        public Nullable<bool> DaXoa { get; set; }
        public Nullable<bool> TinhTrangGiaoHang { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonDatHangMerchant> ChiTietDonDatHangMerchants { get; set; }
        public virtual DonDatHang DonDatHang { get; set; }
        public virtual TrangThaiGiaoHang TrangThaiGiaoHang { get; set; }
        public virtual KhachHang KhachHang { get; set; }
        public virtual ThanhVien ThanhVien { get; set; }
    }
}