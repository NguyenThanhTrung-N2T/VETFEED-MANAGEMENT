namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Response cho phân tích lợi nhuận
    public class LoiNhuanPhanTichResponse
    {
        public LoiNhuanTongQuanResponse? TongQuan { get; set; }
        public List<TopSanPhamLoiNhuanResponse>? TopSanPhamChart { get; set; }
    }
}
