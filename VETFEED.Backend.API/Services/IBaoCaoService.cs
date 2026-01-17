using VETFEED.Backend.API.DTOs.BaoCao;

namespace VETFEED.Backend.API.Services
{
    public interface IBaoCaoService
    {
        //Lấy phân tích doanh thu theo khoảng thời gian
        Task<DoanhThuPhanTichResponse> GetDoanhThuPhanTichAsync(DateTime from, DateTime to);

        //Lấy danh sách chi tiết đơn hàng doanh thu với phân trang
        Task<DoanhThuDonHangResponse> GetDoanhThuDonHangAsync(DateTime from, DateTime to, int page, int limit);
    }
}
