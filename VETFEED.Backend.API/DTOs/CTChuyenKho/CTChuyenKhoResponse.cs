namespace VETFEED.Backend.API.DTOs.CTChuyenKho
{
    public class CTChuyenKhoResponse
    {
        public Guid MaCTCK { get; set; }
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public string? TenSanPham { get; set; }
        public string? LoaiSanPham { get; set; }
        public string? DonViCoSo { get; set; }                // Đổi từ DonViTinh
        public decimal SoLuongChuyen { get; set; }
        public decimal? DonGia { get; set; } 
        public DateTime? HanSuDung { get; set; } 
        public string? GhiChu { get; set; } 
        public string? TrangThai { get; set; }
    }
}
