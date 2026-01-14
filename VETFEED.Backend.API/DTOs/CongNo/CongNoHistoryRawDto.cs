namespace VETFEED.Backend.API.DTOs.CongNo
{
    public class CongNoHistoryRawDto
    {
        public DateTime NgayPhatSinh { get; set; }
        public decimal SoTien { get; set; }          // >0 tăng nợ, <0 giảm nợ
        public string? GhiChu { get; set; }
        public string LoaiPhieu { get; set; } = null!;
        public string? MaPhieuCode { get; set; }
    }

}
