using VETFEED.Backend.API.DTOs.BaoCao;

namespace VETFEED.Backend.API.Services
{
    public interface IBaoCaoService
    {
        //Lấy phân tích doanh thu theo khoảng thời gian
        Task<DoanhThuPhanTichResponse> GetDoanhThuPhanTichAsync(DateTime from, DateTime to);

        //Lấy danh sách chi tiết đơn hàng doanh thu với phân trang
        Task<DoanhThuDonHangResponse> GetDoanhThuDonHangAsync(DateTime from, DateTime to, int page, int limit);

        //Lấy phân tích lợi nhuận theo khoảng thời gian
        Task<LoiNhuanPhanTichResponse> GetLoiNhuanPhanTichAsync(DateTime from, DateTime to);

        //Lấy danh sách lợi nhuận theo sản phẩm với phân trang và sắp xếp
        Task<LoiNhuanSanPhamResponse> GetLoiNhuanSanPhamAsync(DateTime from, DateTime to, int page, int limit, string sortBy, string order);

        //Lấy phân tích tồn kho theo kho
        Task<TonKhoPhanTichResponse> GetTonKhoPhanTichAsync(Guid maKho);
    }
}
