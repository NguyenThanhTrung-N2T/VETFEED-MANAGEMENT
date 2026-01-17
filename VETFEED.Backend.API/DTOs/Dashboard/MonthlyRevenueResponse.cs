namespace VETFEED.Backend.API.DTOs.Dashboard
{
    /// <summary>
    /// Doanh thu theo 12 tháng trong năm
    /// </summary>
    public class MonthlyRevenueResponse
    {
        public int Year { get; set; }
        public List<decimal> Data { get; set; } = new List<decimal>();
    }
}
