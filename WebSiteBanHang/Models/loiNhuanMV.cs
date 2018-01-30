using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteBanHang.Models
{
    public class loiNhuanMV
    {
        public loiNhuanMV()
        {

        }

        public string NguoiBan { get; set; }
        public decimal? TongTien { get; set; }
        public int? TongSoLuong { get; set; }
    }
}