namespace VETFEED.Backend.API.DTOs.LoHang
{
    public class LoHangResponse
    {
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public Guid MaSP { get; set; }
        public string? TenSP { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }
    }
}
