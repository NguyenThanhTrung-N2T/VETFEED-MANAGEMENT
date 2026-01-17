using VETFEED.Backend.API.DTOs.Dashboard;

namespace VETFEED.Backend.API.Services
{
    public interface IDashBoardService
    {
        /// <summary>
        /// Lấy doanh thu hôm nay (chỉ tính đơn đã thanh toán) và trend so với hôm qua
        /// </summary>
        Task<TodayRevenueResponse> GetTodayRevenueAsync();

        /// <summary>
        /// Lấy số đơn hàng hôm nay và trend so với hôm qua
        /// </summary>
        Task<TodayOrdersResponse> GetTodayOrdersAsync();

        /// <summary>
        /// Lấy tổng số lượng tồn kho
        /// </summary>
        Task<TotalInventoryResponse> GetTotalInventoryAsync();

        /// <summary>
        /// Lấy doanh thu theo tháng trong năm (chỉ tính đơn đã thanh toán)
        /// </summary>
        Task<MonthlyRevenueResponse> GetMonthlyRevenueAsync(int year);

        /// <summary>
        /// Lấy danh sách sản phẩm sắp hết hạn
        /// </summary>
        /// <param name="limit">Số lượng sản phẩm tối đa</param>
        /// <param name="daysThreshold">Số ngày còn lại trước khi hết hạn</param>
        Task<IEnumerable<ExpiringProductResponse>> GetExpiringProductsAsync(int limit = 5, int daysThreshold = 30);
    }
}
