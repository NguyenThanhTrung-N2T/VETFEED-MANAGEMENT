namespace VETFEED.Backend.API.DTOs.PhieuChuyenKho
{
    public class SanPhamChuyenKhoItemResponse
    {
        public string? MaLoCode { get; set; }
        public string? TenSanPham { get; set; }
        public string? LoaiSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public decimal SoLuongChuyen { get; set; }
        public decimal? DonGia { get; set; } 
        public DateTime? HanSuDung { get; set; } 
        public string? GhiChu { get; set; } 
        public string? TrangThai { get; set; }
    }
}
