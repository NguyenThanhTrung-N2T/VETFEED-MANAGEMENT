using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;
namespace VETFEED.Backend.API.DTOs.KhachHang
{
    public class KhachHangCreateRequest
    {
        [Required]
        public string TenKH { get; set; } = null!;

        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }

        [Required]
        public LoaiKhachHangEnum LoaiKhachHang { get; set; }

        public decimal? HanMucCongNo { get; set; }

        [Required]
        public TrangThaiKhachHangEnum TrangThai { get; set; } // HOAT_DONG | KHOA

        public string? GhiChu { get; set; }
    }
}
