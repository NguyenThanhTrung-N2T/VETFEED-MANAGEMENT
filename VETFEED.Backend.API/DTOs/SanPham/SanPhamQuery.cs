namespace VETFEED.Backend.API.DTOs.SanPham
{
    public class SanPhamQuery
    {
        public string? Keyword { get; set; }
        public string? LoaiSanPham { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
