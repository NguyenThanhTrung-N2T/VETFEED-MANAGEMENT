using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng giá bán sản phẩm theo thời gian
    /// </summary>
    public class GiaBan
    {
        [Key]
        public Guid MaGia { get; set; }
        public Guid MaSP { get; set; }
        public decimal DonGiaBan { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation
        public SanPham? SanPham { get; set; }
    }
}
