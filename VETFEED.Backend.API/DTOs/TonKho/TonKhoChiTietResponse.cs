namespace VETFEED.Backend.API.DTOs.TonKho
{
    public class TonKhoChiTietResponse
    {
        public Guid MaTK { get; set; }
        public Guid MaKho { get; set; }
        public Guid MaLo { get; set; }
        public decimal SoLuongTon { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
