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

        // doanh thu từ ngày from đến ngày to
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
    }
}
