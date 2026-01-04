namespace VETFEED.Backend.API.DTOs.NhaCungCapSanPham
{
    public class NhaCungCapSanPhamResponse
    {
        public Guid MaNCSP { get; set; }
        public Guid MaNCC { get; set; }
        public string? TenNCC { get; set; }
        public Guid MaSP { get; set; }
        public string? TenSP { get; set; }
        public decimal? GiaNhapMacDinh { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
