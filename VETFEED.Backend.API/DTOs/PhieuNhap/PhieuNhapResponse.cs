namespace VETFEED.Backend.API.DTOs.PhieuNhap
{
    public class PhieuNhapResponse
    {
        public Guid MaPN { get; set; }
        public string? MaPNCode { get; set; }
        public Guid MaNCC { get; set; }
        public string? TenNCC { get; set; }
        public Guid MaKho { get; set; }
        public string? TenKho { get; set; }
        public decimal ThanhTien { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
