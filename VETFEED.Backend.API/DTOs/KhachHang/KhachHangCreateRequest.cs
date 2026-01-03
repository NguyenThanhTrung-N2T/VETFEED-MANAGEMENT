using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.KhachHang
{
    public class KhachHangCreateRequest
    {
        [Required]
        public string TenKH { get; set; } = null!;

        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }

        [Required]
        public string LoaiKhachHang { get; set; } = null!; // CA_NHAN | TRANG_TRAI | DAI_LY

        public decimal? HanMucCongNo { get; set; }

        [Required]
        public string TrangThai { get; set; } = null!; // HOAT_DONG | KHOA

        public string? GhiChu { get; set; }
    }
}
