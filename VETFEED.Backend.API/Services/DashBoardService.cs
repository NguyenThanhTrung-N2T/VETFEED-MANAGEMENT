using VETFEED.Backend.API.DTOs.Dashboard;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashBoardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        /// <summary>
        /// Lấy doanh thu hôm nay (chỉ tính đơn đã thanh toán) và trend so với hôm qua
        /// </summary>
        public async Task<TodayRevenueResponse> GetTodayRevenueAsync()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            // Lấy dữ liệu từ repository
            var todayRevenue = await _dashboardRepository.GetRevenueByDateAsync(today);
            var yesterdayRevenue = await _dashboardRepository.GetRevenueByDateAsync(yesterday);

            // Tính trend (business logic)
            var (trendPercent, isIncrease) = CalculateTrend(todayRevenue, yesterdayRevenue);

            return new TodayRevenueResponse
            {
                Revenue = todayRevenue,
                TrendPercent = trendPercent,
                IsIncrease = isIncrease
            };
        }

        /// <summary>
        /// Lấy số đơn hàng hôm nay và trend so với hôm qua
        /// </summary>
        public async Task<TodayOrdersResponse> GetTodayOrdersAsync()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            // Lấy dữ liệu từ repository
            var todayOrders = await _dashboardRepository.GetOrderCountByDateAsync(today);
            var yesterdayOrders = await _dashboardRepository.GetOrderCountByDateAsync(yesterday);

            // Tính trend (business logic)
            var (trendPercent, isIncrease) = CalculateTrend(todayOrders, yesterdayOrders);

            return new TodayOrdersResponse
            {
                OrderCount = todayOrders,
                TrendPercent = trendPercent,
                IsIncrease = isIncrease
            };
        }

        /// <summary>
        /// Lấy tổng số lượng tồn kho
        /// </summary>
        public async Task<TotalInventoryResponse> GetTotalInventoryAsync()
        {
            var totalQuantity = await _dashboardRepository.GetTotalInventoryAsync();

            return new TotalInventoryResponse
            {
                TotalQuantity = totalQuantity
            };
        }

        /// <summary>
        /// Lấy doanh thu theo tháng trong năm (chỉ tính đơn đã thanh toán)
        /// </summary>
        public async Task<MonthlyRevenueResponse> GetMonthlyRevenueAsync(int year)
        {
            var monthlyData = await _dashboardRepository.GetMonthlyRevenueAsync(year);

            // Tạo mảng 12 tháng với giá trị mặc định là 0
            var data = new List<decimal>(new decimal[12]);

            foreach (var item in monthlyData)
            {
                data[item.Key - 1] = item.Value;
            }

            return new MonthlyRevenueResponse
            {
                Year = year,
                Data = data
            };
        }

        /// <summary>
        /// Lấy danh sách sản phẩm sắp hết hạn
        /// </summary>
        public async Task<IEnumerable<ExpiringProductResponse>> GetExpiringProductsAsync(int limit = 5, int daysThreshold = 30)
        {
            var thresholdDate = DateTime.Today.AddDays(daysThreshold);

            var expiringBatches = await _dashboardRepository.GetExpiringBatchesAsync(thresholdDate, limit);

            // Chuyển đổi từ repository DTO sang response DTO (business logic)
            return expiringBatches.Select(batch => new ExpiringProductResponse
            {
                MaLo = batch.MaLo,
                MaLoCode = batch.MaLoCode,
                TenSanPham = batch.TenSanPham,
                LoaiSanPham = batch.LoaiSanPham == (int)LoaiSanPhamEnum.THUOC_THU_Y ? "Thuốc thú y" : "Thức ăn chăn nuôi",
                HanSuDung = batch.HanSuDung,
                SoNgayConLai = (batch.HanSuDung.Date - DateTime.Today).Days,
                SoLuongTon = batch.SoLuongTon
            }).ToList();
        }

        /// <summary>
        /// Tính phần trăm trend và hướng tăng/giảm
        /// </summary>
        private static (decimal TrendPercent, bool IsIncrease) CalculateTrend(decimal current, decimal previous)
        {
            decimal trendPercent = 0;
            bool isIncrease = true;

            if (previous > 0)
            {
                trendPercent = Math.Abs((current - previous) / previous * 100);
                isIncrease = current >= previous;
            }
            else if (current > 0)
            {
                trendPercent = 100;
                isIncrease = true;
            }

            return (Math.Round(trendPercent, 2), isIncrease);
        }
    }
}
