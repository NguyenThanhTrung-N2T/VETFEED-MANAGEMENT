namespace VETFEED.Backend.API.DTOs.NhaCungCapSanPham
{
    public class NhaCungCapSanPhamResponse
    {
        public Guid MaNCSP { get; set; }
        public Guid MaSP { get; set; }

        // Flattened product details (Backend joins tables to populate)
        public string? TenSanPham { get; set; }
        public string? MaSanPhamCode { get; set; }
        public string? DonViCoSo { get; set; }

        // Link details
        public decimal GiaNhapMacDinh { get; set; }
        public string? GhiChu { get; set; }
        public string? TrangThai { get; set; }
    }
}
