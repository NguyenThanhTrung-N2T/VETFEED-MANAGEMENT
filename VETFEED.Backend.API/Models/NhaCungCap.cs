using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng quản lý nhà cung cấp
    /// </summary>
    public class NhaCungCap
    {
        [Key]
        public Guid MaNCC { get; set; }
        public string? MaNCCCode { get; set; }
        public string? TenNCC { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public TrangThaiNhaCungCapEnum TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation
        public ICollection<NhaCungCapSanPham>? NhaCungCapSanPhams { get; set; }
        public ICollection<PhieuNhap>? PhieuNhaps { get; set; }
    }
}
