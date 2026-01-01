using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng quản lý lô sản phẩm
    /// </summary>
    public class LoHang
    {
        [Key]
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public Guid MaSP { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }

        // Navigation
        public SanPham? SanPham { get; set; }
        public ICollection<TonKho>? TonKhos { get; set; }
        public ICollection<CTPhieuNhap>? CTPhieuNhaps { get; set; }
        public ICollection<CTPhieuBan>? CTPhieuBans { get; set; }
        public ICollection<CTPhieuChuyenKho>? CTPhieuChuyenKhos { get; set; }
        public ICollection<CTPhieuTra>? CTPhieuTras { get; set; }
    }
}
