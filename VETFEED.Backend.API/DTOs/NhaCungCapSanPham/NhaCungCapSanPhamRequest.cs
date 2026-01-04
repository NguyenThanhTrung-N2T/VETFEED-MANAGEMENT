namespace VETFEED.Backend.API.DTOs.NhaCungCapSanPham
{
    public class NhaCungCapSanPhamRequest
    {
        public Guid MaNCC { get; set; }
        public Guid MaSP { get; set; }
        public decimal? GiaNhapMacDinh { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}
