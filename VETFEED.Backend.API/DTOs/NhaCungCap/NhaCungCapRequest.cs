using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.NhaCungCap
{
    public class NhaCungCapRequest
    {
        public string? TenNCC { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public TrangThaiNhaCungCapEnum TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}