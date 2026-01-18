namespace VETFEED.Backend.API.DTOs.Dashboard
{
    /// <summary>
    /// Thống kê tổng quan Dashboard (gộp doanh thu, đơn hàng, tồn kho)
    /// </summary>
    public class DashboardSummaryResponse
    {
        /// <summary>
        /// Thống kê doanh thu hôm nay
        /// </summary>
        public TodayRevenueResponse TodayRevenue { get; set; } = new();

        /// <summary>
        /// Thống kê đơn hàng hôm nay
        /// </summary>
        public TodayOrdersResponse TodayOrders { get; set; } = new();

        /// <summary>
        /// Tổng số lượng tồn kho
        /// </summary>
        public TotalInventoryResponse TotalInventory { get; set; } = new();
    }
}
