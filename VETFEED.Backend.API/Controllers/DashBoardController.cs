using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VETFEED.Backend.API.DTOs.Dashboard;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    /// <summary>
    /// API quản lý Dashboard - Thống kê tổng quan hệ thống
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardService;

        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        /// <summary>
        /// Lấy thống kê tổng quan Dashboard
        /// </summary>
        /// <remarks>
        /// Trả về thống kê tổng quan bao gồm:
        /// - **Doanh thu hôm nay**: Tổng doanh thu từ các đơn đã thanh toán + trend so với hôm qua
        /// - **Số đơn hàng hôm nay**: Tổng số đơn hàng + trend so với hôm qua
        /// - **Tổng tồn kho**: Tổng số lượng sản phẩm trong kho
        /// 
        /// **Cách tính Trend**:
        /// - `TrendPercent`: Phần trăm thay đổi so với hôm qua
        /// - `IsIncrease = true`: Tăng hoặc giữ nguyên
        /// - `IsIncrease = false`: Giảm so với hôm qua
        /// </remarks>
        /// <returns>Thống kê tổng quan Dashboard</returns>
        /// <response code="200">Trả về thống kê tổng quan thành công</response>
        [Authorize]
        [HttpGet("summary")]
        [ProducesResponseType(typeof(DashboardSummaryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _dashBoardService.GetDashboardSummaryAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy doanh thu theo tháng trong năm
        /// </summary>
        /// <remarks>
        /// Trả về doanh thu 12 tháng trong năm được chỉ định.
        /// - Chỉ tính các đơn hàng có trạng thái **ĐÃ THANH TOÁN**
        /// - Mảng `Data` chứa 12 phần tử tương ứng với 12 tháng (index 0 = Tháng 1, index 11 = Tháng 12)
        /// - Tháng không có doanh thu sẽ có giá trị = 0
        /// 
        /// **Ví dụ response**:
        /// ```json
        /// {
        ///   "year": 2026,
        ///   "data": [1000000, 1500000, 2000000, ..., 1800000]
        /// }
        /// ```
        /// </remarks>
        /// <param name="year">Năm cần thống kê (mặc định: năm hiện tại)</param>
        /// <returns>Doanh thu 12 tháng trong năm</returns>
        /// <response code="200">Trả về doanh thu theo tháng thành công</response>
        [Authorize]
        [HttpGet("revenue/monthly")]
        [ProducesResponseType(typeof(MonthlyRevenueResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int? year)
        {
            var targetYear = year ?? DateTime.Now.Year;
            var result = await _dashBoardService.GetMonthlyRevenueAsync(targetYear);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm sắp hết hạn
        /// </summary>
        /// <remarks>
        /// Trả về danh sách các lô hàng sắp hết hạn sử dụng, sắp xếp theo ngày hết hạn tăng dần.
        /// 
        /// **Điều kiện lọc**:
        /// - Chỉ lấy các lô có `HanSuDung` trong khoảng từ **hôm nay** đến **hôm nay + daysThreshold ngày**
        /// - Chỉ lấy các lô có **số lượng tồn kho > 0**
        /// 
        /// **Các trường response**:
        /// - `MaLo`: Mã lô hàng (GUID)
        /// - `MaLoCode`: Mã lô hàng hiển thị
        /// - `TenSanPham`: Tên sản phẩm
        /// - `LoaiSanPham`: "Thuốc thú y" hoặc "Thức ăn chăn nuôi"
        /// - `HanSuDung`: Ngày hết hạn
        /// - `SoNgayConLai`: Số ngày còn lại trước khi hết hạn
        /// - `SoLuongTon`: Tổng số lượng tồn kho của lô này (đơn vị cơ sở)
        /// </remarks>
        /// <param name="limit">Số lượng sản phẩm tối đa trả về (mặc định: 30, tối đa: 100)</param>
        /// <param name="daysThreshold">Số ngày còn lại trước khi hết hạn (mặc định: 30)</param>
        /// <returns>Danh sách lô hàng sắp hết hạn</returns>
        /// <response code="200">Trả về danh sách sản phẩm sắp hết hạn thành công</response>
        [Authorize]
        [HttpGet("products/expiring")]
        [ProducesResponseType(typeof(IEnumerable<ExpiringProductResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExpiringProducts(
            [FromQuery] int limit = 30, 
            [FromQuery] int daysThreshold = 30)
        {
            // Giới hạn limit tối đa là 100
            limit = Math.Min(limit, 100);
            
            var result = await _dashBoardService.GetExpiringProductsAsync(limit, daysThreshold);
            return Ok(result);
        }
    }
}
