namespace VETFEED.Backend.API.DTOs.PhieuNhap
{
    public class PhieuNhapRequest
    {
        public Guid MaNCC { get; set; }
        public Guid MaKho { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}

