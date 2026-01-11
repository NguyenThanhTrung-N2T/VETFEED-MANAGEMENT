namespace VETFEED.Backend.API.DTOs.PhieuBan
{
    public class PhieuBanListResponse
    {
        public Guid MaPB { get; set; }
        public string MaPBCode { get; set; } = null!;
        public DateTime NgayBan { get; set; }

        public decimal TongTienHang { get; set; }
        public decimal TienChietKhau { get; set; }
        public decimal ThanhTien { get; set; }

        public string HinhThucThanhToan { get; set; } = null!;
        public string TrangThaiThanhToan { get; set; } = null!;

        public decimal TienNo { get; set; }
        public string? GhiChu { get; set; }
    }

}
