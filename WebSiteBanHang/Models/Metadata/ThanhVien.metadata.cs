using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebSiteBanHang.Models
{
    [MetadataTypeAttribute(typeof(ThanhVienMetaData))]
    public  partial class ThanhVien
    {
        internal sealed class ThanhVienMetaData
        {

            public int MaThanhVien { get; set; }

            [DisplayName("Tài khoản")]
            [Required(ErrorMessage = "{0} không được bỏ trống" )]
            public string TaiKhoan { get; set; }

            public string MatKhau { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string Email { get; set; }
            public string SoDienThoai { get; set; }
            public string CauHoi { get; set; }
            public string CauTraLoi { get; set; }
            public Nullable<int> MaLoaiTV { get; set; }
        }
    }
}