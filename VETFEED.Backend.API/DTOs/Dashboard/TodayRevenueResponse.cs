namespace VETFEED.Backend.API.DTOs.Dashboard
{
    /// <summary>
    /// Doanh thu hôm nay + trend so với hôm qua
    /// </summary>
    public class TodayRevenueResponse
    {
        public decimal Revenue { get; set; }
        public decimal TrendPercent { get; set; }
        public bool IsIncrease { get; set; }
    }
}
