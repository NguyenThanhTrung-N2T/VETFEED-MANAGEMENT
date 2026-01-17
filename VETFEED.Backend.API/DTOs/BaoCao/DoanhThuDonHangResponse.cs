namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Response cho danh sách đơn hàng doanh thu với phân trang
    public class DoanhThuDonHangResponse
    {
        public List<DoanhThuDonHangItemResponse>? Data { get; set; }
        public PaginationMeta? Meta { get; set; }
    }


    // Metadata cho phân trang
    public class PaginationMeta
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total_Items { get; set; }
        public int Total_Pages { get; set; }
    }
}
