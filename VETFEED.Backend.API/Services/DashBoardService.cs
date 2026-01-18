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

        /// <inheritdoc />
        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            // Lấy dữ liệu từ repository (chạy song song để tối ưu hiệu năng)
            var todayRevenueTask = _dashboardRepository.GetRevenueByDateAsync(today);
            var yesterdayRevenueTask = _dashboardRepository.GetRevenueByDateAsync(yesterday);
            var todayOrdersTask = _dashboardRepository.GetOrderCountByDateAsync(today);
            var yesterdayOrdersTask = _dashboardRepository.GetOrderCountByDateAsync(yesterday);
            var totalInventoryTask = _dashboardRepository.GetTotalInventoryAsync();

            await Task.WhenAll(todayRevenueTask, yesterdayRevenueTask, todayOrdersTask, yesterdayOrdersTask, totalInventoryTask);

            var todayRevenue = todayRevenueTask.Result;
            var yesterdayRevenue = yesterdayRevenueTask.Result;
            var todayOrders = todayOrdersTask.Result;
            var yesterdayOrders = yesterdayOrdersTask.Result;
            var totalInventory = totalInventoryTask.Result;

            // Tính trend (business logic)
            var (revenueTrendPercent, revenueIsIncrease) = CalculateTrend(todayRevenue, yesterdayRevenue);
            var (ordersTrendPercent, ordersIsIncrease) = CalculateTrend(todayOrders, yesterdayOrders);

            return new DashboardSummaryResponse
            {
                TodayRevenue = new TodayRevenueResponse
                {
                    Revenue = todayRevenue,
                    TrendPercent = revenueTrendPercent,
                    IsIncrease = revenueIsIncrease
                },
                TodayOrders = new TodayOrdersResponse
                {
                    OrderCount = todayOrders,
                    TrendPercent = ordersTrendPercent,
                    IsIncrease = ordersIsIncrease
                },
                TotalInventory = new TotalInventoryResponse
                {
                    TotalQuantity = totalInventory
                }
            };
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
