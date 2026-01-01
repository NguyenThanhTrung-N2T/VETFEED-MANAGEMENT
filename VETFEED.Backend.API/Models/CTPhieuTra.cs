using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Chi tiết phiếu trả hàng (lô, số lượng, đơn giá hoàn)
    /// </summary>
    public class CTPhieuTra
    {
        [Key]
        public Guid MaCTPT { get; set; }
        public Guid MaPT { get; set; }
        public Guid MaLo { get; set; }
        public decimal SoLuongTra { get; set; }
        public decimal DonGiaHoan { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public PhieuTra? PhieuTra { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
