using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.KhachHang
{
    public class KhachHangUpdateRequest
    {
        [Required]
        public string TenKH { get; set; } = null!;

        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }

        [Required]
        public string LoaiKhachHang { get; set; } = null!;

        public decimal? HanMucCongNo { get; set; }

        [Required]
        public string TrangThai { get; set; } = null!;

        public string? GhiChu { get; set; }
    }
}
