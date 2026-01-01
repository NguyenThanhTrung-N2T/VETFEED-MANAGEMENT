using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.KhoHang
{
    public class KhoHangResponse
    {
        public Guid MaKho { get; set; }
        public string? MaKhoCode { get; set; }
        public string? TenKho { get; set; }
        public string? DiaChi { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}
