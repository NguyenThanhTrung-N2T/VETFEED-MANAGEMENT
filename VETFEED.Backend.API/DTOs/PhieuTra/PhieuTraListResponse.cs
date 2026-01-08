using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.PhieuTra
{
    public class PhieuTraListResponse
    {
        public Guid MaPT { get; set; }
        public string MaPTCode { get; set; } = string.Empty;
        public DateTime NgayTra { get; set; }
        public Guid MaPB { get; set; }
        public string? MaPBCode { get; set; }
        public Guid MaKH { get; set; }
        public string? TenKhachHang { get; set; }
        public decimal ThanhTien { get; set; }
        public string? HinhThucHoanTien { get; set; }
    }
}
