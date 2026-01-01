using System;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Chi tiết phiếu chuyển kho (lô, số lượng, trạng thái)
    /// </summary>
    public class CTPhieuChuyenKho
    {
        [Key]
        public Guid MaCTCK { get; set; }
        public Guid MaCK { get; set; }
        public Guid MaLo { get; set; }
        public decimal SoLuongChuyen { get; set; }
        public TrangThaiPhieuChuyenKhoChiTietEnum TrangThai { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public PhieuChuyenKho? PhieuChuyenKho { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
