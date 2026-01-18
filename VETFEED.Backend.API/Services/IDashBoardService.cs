using VETFEED.Backend.API.DTOs.Dashboard;

namespace VETFEED.Backend.API.Services
{
    public interface IDashBoardService
    {
        /// <summary>
        /// Lấy thống kê tổng quan Dashboard (doanh thu, đơn hàng, tồn kho)
        /// </summary>
        /// <returns>Thống kê tổng quan bao gồm doanh thu hôm nay, số đơn hàng và tồn kho</returns>
        Task<DashboardSummaryResponse> GetDashboardSummaryAsync();

        /// <summary>
        /// Lấy doanh thu theo tháng trong năm (chỉ tính đơn đã thanh toán)
        /// </summary>
        /// <param name="year">Năm cần thống kê</param>
        /// <returns>Doanh thu 12 tháng trong năm</returns>
        Task<MonthlyRevenueResponse> GetMonthlyRevenueAsync(int year);

        /// <summary>
        /// Lấy danh sách sản phẩm sắp hết hạn
        /// </summary>
        /// <param name="limit">Số lượng sản phẩm tối đa</param>
        /// <param name="daysThreshold">Số ngày còn lại trước khi hết hạn</param>
        /// <returns>Danh sách lô hàng sắp hết hạn</returns>
        Task<IEnumerable<ExpiringProductResponse>> GetExpiringProductsAsync(int limit = 5, int daysThreshold = 30);
    }
}
