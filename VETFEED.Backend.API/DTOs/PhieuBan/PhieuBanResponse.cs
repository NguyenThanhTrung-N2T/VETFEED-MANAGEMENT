namespace VETFEED.Backend.API.DTOs.PhieuBan
{
    public class PhieuBanResponse
    {
        public Guid MaPB { get; set; }
        public string? MaPBCode { get; set; }
        public Guid MaKH { get; set; }
        public string? TenKhachHang { get; set; }
        public DateTime NgayBan { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal ChietKhauPhanTram { get; set; }
        public decimal TienChietKhau { get; set; }
        public decimal ThanhTien { get; set; }
        public string? HinhThucThanhToan { get; set; }
        public string? TrangThaiThanhToan { get; set; }
        public decimal TienCoc { get; set; }
        public decimal TienNo { get; set; }
        public DateTime? HanTra { get; set; }
        public string? GhiChu { get; set; }
    }
}
