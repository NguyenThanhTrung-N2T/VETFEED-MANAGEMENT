namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Response cho danh sách lợi nhuận sản phẩm với phân trang
    public class LoiNhuanSanPhamResponse
    {
        public List<LoiNhuanSanPhamItemResponse>? Data { get; set; }
        public PaginationMeta? Meta { get; set; }
    }
}
