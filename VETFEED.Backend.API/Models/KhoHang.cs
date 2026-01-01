using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng quản lý kho vật lý
    /// </summary>
    public class KhoHang
    {
        [Key]
        public Guid MaKho { get; set; }
        public string? MaKhoCode { get; set; }         // Mã kho hiển thị
        public string? TenKho { get; set; }            // Tên kho
        public string? DiaChi { get; set; }            // Địa chỉ kho
        public TrangThaiKhoEnum TrangThai { get; set; } // Trạng thái hoạt động kho
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation
        public ICollection<TonKho>? TonKhos { get; set; }
        public ICollection<PhieuNhap>? PhieuNhaps { get; set; }
        public ICollection<CTPhieuBan>? CTPhieuBans { get; set; }
        public ICollection<PhieuChuyenKho>? PhieuChuyenKhoXuats { get; set; }
        public ICollection<PhieuChuyenKho>? PhieuChuyenKhoNhans { get; set; }
    }
}
