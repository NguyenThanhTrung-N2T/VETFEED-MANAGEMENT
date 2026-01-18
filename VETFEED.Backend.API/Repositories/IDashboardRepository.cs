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

    /// <summary>
    /// DTO for expiring batch information from repository
    /// </summary>
    public class ExpiringBatchInfo
    {
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public string? TenSanPham { get; set; }
        public string? LoaiSanPham { get; set; }
        public DateTime HanSuDung { get; set; }
        public decimal SoLuongTon { get; set; }
    }
}
