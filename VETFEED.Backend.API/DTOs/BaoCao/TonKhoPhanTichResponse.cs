namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Response cho phân tích tồn kho
    public class TonKhoPhanTichResponse
    {
        public TonKhoTongQuanResponse? TongQuan { get; set; }
        public List<SoLuongChartItemResponse>? SoLuongChart { get; set; }
    }
}
