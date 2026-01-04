namespace VETFEED.Backend.API.DTOs.NhaCungCap
{
    public class NhaCungCapResponse
    {
        public Guid MaNCC { get; set; }
        public string? MaNCCCode { get; set; }
        public string? TenNCC { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
}