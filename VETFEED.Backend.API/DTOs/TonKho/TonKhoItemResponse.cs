namespace VETFEED.Backend.API.DTOs.TonKho
{
    public class TonKhoItemResponse
    {
        public Guid MaLo { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string? MaPNCode { get; set; } // Ma Code Cua phieu nhap 
        public decimal? DonGia { get; set; }
        public decimal SoLuong { get; set; }
    }
}
