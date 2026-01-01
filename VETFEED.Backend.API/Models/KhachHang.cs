using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng quản lý khách hàng
    /// </summary>
    public class KhachHang
    {
        [Key]
        public Guid MaKH { get; set; }
        public string? MaKHCode { get; set; }
        public string? TenKH { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public LoaiKhachHangEnum LoaiKhachHang { get; set; }
        public decimal? HanMucCongNo { get; set; }
        public decimal TongMua { get; set; }
        public decimal CongNoHienTai { get; set; }
        public TrangThaiKhachHangEnum TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation
        public ICollection<PhieuBan>? PhieuBans { get; set; }
        public ICollection<PhieuTra>? PhieuTras { get; set; }
    }
}
