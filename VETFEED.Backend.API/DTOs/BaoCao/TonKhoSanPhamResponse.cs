namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Response cho danh sách tồn kho sản phẩm với phân trang
    public class TonKhoSanPhamResponse
    {
        public List<TonKhoSanPhamItemResponse>? Data { get; set; }
        public PaginationMeta? Meta { get; set; }
    }
}
