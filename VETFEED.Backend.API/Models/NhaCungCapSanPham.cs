using System;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Liên kết NCC và sản phẩm
    /// </summary>
    public class NhaCungCapSanPham
    {
        [Key]
        public Guid MaNCSP { get; set; }
        public Guid MaNCC { get; set; }
        public Guid MaSP { get; set; }
        public decimal? GiaNhapMacDinh { get; set; }
        public TrangThaiNhaCungCapSanPhamEnum TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation
        public NhaCungCap? NhaCungCap { get; set; }
        public SanPham? SanPham { get; set; }
    }
}
