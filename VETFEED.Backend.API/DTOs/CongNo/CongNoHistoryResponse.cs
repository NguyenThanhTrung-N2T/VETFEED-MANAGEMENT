namespace VETFEED.Backend.API.DTOs.CongNo
{
    public class CongNoHistoryResponse
    {
        public DateTime Ngay { get; set; }
        public string LoaiPhieu { get; set; } = null!;
        public string MaPhieu { get; set; } = null!;
        public string GhiChu { get; set; } = null!;
        public decimal PhatSinhNo { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal SoDuSau { get; set; }
    }

}
