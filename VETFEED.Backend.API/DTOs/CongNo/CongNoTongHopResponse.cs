namespace VETFEED.Backend.API.DTOs.CongNo
{
    public class CongNoTongHopResponse
    {
        public Guid MaDoiTuong { get; set; }
        public string MaDoiTuongCode { get; set; } = null!;
        public string TenDoiTuong { get; set; } = null!;
        public string LoaiDoiTuong { get; set; } = null!;

        public decimal TongPhatSinh { get; set; }   // ≥ 0
        public decimal DaThanhToan { get; set; }    // ≥ 0
        public decimal DuNo { get; set; }            // ≥ 0
        public decimal HanMucCongNo { get; set; }  // ≥ 0

        public bool CoQuaHan { get; set; }
        public DateTime? HanThanhToanGanNhat { get; set; }
    }

}
