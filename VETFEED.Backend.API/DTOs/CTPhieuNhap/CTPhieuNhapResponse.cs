namespace VETFEED.Backend.API.DTOs.CTPhieuNhap
{
    public class CTPhieuNhapResponse
    {
        public Guid MaCTPN { get; set; }
        public Guid MaPN { get; set; }
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public string? TenSP { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? HanSuDung { get; set; }
        public decimal SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal ThanhTien => SoLuong * (DonGia ?? 0);
    }
}
