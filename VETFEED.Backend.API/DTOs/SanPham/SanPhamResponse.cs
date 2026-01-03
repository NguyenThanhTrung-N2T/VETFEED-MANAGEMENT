using System;

namespace VETFEED.Backend.API.DTOs.SanPham
{
    public class SanPhamResponse
    {
        public Guid MaSP { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenSP { get; set; }
        public string? LoaiSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
