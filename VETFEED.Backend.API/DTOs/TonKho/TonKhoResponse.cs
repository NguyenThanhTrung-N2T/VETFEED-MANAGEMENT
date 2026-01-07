namespace VETFEED.Backend.API.DTOs.TonKho
{
    public class TonKhoResponse
    {
        public Guid MaKho { get; set; }
        public string TenKho { get; set; } = string.Empty;
        public List<TonKhoItemResponse> DanhSachTonKho { get; set; } = new List<TonKhoItemResponse>();
    }
}
