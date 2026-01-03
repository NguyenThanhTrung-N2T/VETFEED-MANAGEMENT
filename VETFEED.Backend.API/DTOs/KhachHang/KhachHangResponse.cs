using System;

namespace VETFEED.Backend.API.DTOs.KhachHang
{
    public class KhachHangResponse
    {
        public Guid MaKH { get; set; }
        public string? MaKHCode { get; set; }
        public string? TenKH { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? LoaiKhachHang { get; set; }
        public decimal? HanMucCongNo { get; set; }
        public decimal TongMua { get; set; }
        public decimal CongNoHienTai { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
