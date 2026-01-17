using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.BaoCao;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaosController : ControllerBase
    {
        private readonly IBaoCaoService _baoCaoService;

        public BaoCaosController(IBaoCaoService baoCaoService)
        {
            _baoCaoService = baoCaoService;
        }


        //GET api/baocaos/doanhthu/phantich?from=2026-01-09&to=2026-01-16 : Lấy phân tích doanh thu theo khoảng thời gian
        [HttpGet("doanhthu/phantich")]
        [ProducesResponseType(typeof(DoanhThuPhanTichResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<DoanhThuPhanTichResponse>> GetDoanhThuPhanTich(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var result = await _baoCaoService.GetDoanhThuPhanTichAsync(from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //GET api/baocaos/doanhthu/donhang?from=2026-01-09&to=2026-01-16&page=1&limit=20 : Lấy danh sách chi tiết đơn hàng doanh thu với phân trang
        [HttpGet("doanhthu/donhang")]
        [ProducesResponseType(typeof(DoanhThuDonHangResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<DoanhThuDonHangResponse>> GetDoanhThuDonHang(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (limit < 1) limit = 20;
                if (limit > 100) limit = 100; // Max limit to prevent performance issues

                var result = await _baoCaoService.GetDoanhThuDonHangAsync(from, to, page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
