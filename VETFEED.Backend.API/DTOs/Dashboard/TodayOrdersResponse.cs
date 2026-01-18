namespace VETFEED.Backend.API.DTOs.Dashboard
{
    /// <summary>
    /// Số đơn hàng hôm nay + trend so với hôm qua
    /// </summary>
    public class TodayOrdersResponse
    {
        public int OrderCount { get; set; }
        public decimal TrendPercent { get; set; }
        public bool IsIncrease { get; set; }
    }
}
