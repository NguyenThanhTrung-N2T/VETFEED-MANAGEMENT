using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Chi tiết phiếu bán (kho, lô, số lượng)
    /// </summary>
    public class CTPhieuBan
    {
        [Key]
        public Guid MaCTPB { get; set; }
        public Guid MaPB { get; set; }
        public Guid MaKho { get; set; }
        public Guid MaLo { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public PhieuBan? PhieuBan { get; set; }
        public KhoHang? KhoHang { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
