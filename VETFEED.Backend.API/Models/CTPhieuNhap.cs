using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Chi tiết phiếu nhập (lô hàng + số lượng)
    /// </summary>
    public class CTPhieuNhap
    {
        [Key]
        public Guid MaCTPN { get; set; }
        public Guid MaPN { get; set; }                      // FK Phiếu nhập
        public Guid MaLo { get; set; }                      // FK Lô hàng
        public decimal SoLuong { get; set; }
        public decimal? DonGia { get; set; }

        // Navigation
        public PhieuNhap? PhieuNhap { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
