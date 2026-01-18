using VETFEED.Backend.API.DTOs.Dashboard;

namespace VETFEED.Backend.API.Repositories
{
    /// <summary>
    /// Interface for Dashboard data access operations
    /// </summary>
    public interface IDashboardRepository
    {
        /// <summary>
        /// Lấy doanh thu theo ngày (chỉ tính đơn đã thanh toán)
        /// </summary>
        Task<decimal> GetRevenueByDateAsync(DateTime date);

        /// <summary>
        /// Lấy số đơn hàng theo ngày
        /// </summary>
        Task<int> GetOrderCountByDateAsync(DateTime date);

        /// <summary>
        /// Lấy tổng số lượng tồn kho
        /// </summary>
        Task<decimal> GetTotalInventoryAsync();

        /// <summary>
        /// Lấy doanh thu theo từng tháng trong năm (chỉ tính đơn đã thanh toán)
        /// </summary>
        Task<Dictionary<int, decimal>> GetMonthlyRevenueAsync(int year);

        /// <summary>
        /// Lấy danh sách lô hàng sắp hết hạn với thông tin sản phẩm
        /// </summary>
        Task<List<ExpiringBatchInfo>> GetExpiringBatchesAsync(DateTime thresholdDate, int limit);
    }
}
