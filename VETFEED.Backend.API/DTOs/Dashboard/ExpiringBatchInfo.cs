namespace VETFEED.Backend.API.DTOs.Dashboard
{
    public class ExpiringBatchInfo
    {
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public string? TenSanPham { get; set; }
        public string? LoaiSanPham { get; set; }
        public DateTime HanSuDung { get; set; }
        public decimal SoLuongTon { get; set; }
    }
}
