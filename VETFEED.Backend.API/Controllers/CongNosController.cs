using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.CongNo;
using VETFEED.Backend.API.Services;
namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongNosController : ControllerBase
    {
        private readonly ICongNoService _congNoService;
        public CongNosController(ICongNoService congNoService)
        {
            _congNoService = congNoService;
        }

        // GET : api/congnos/summary : lay cong no tong hop cua tat ca doi tuong
        [HttpGet("summary")]
        [ProducesResponseType(typeof(List<CongNoTongHopResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTongHopCongNo()
        {
            // lay cong no tong hop cua tat ca doi tuong
            var result = await _congNoService.GetTongHopCongNoAsync();
            // tra ve client
            return Ok(result);
        }

        // GET : api/congnos/{maDoiTuong}/detail
        [HttpGet("{maDoiTuong}/detail")]
        public async Task<IActionResult> GetCongNoHistory(Guid maDoiTuong)
        {
            // lay lich su cong no cua doi tuong theo ma doi tuong
            var result = await _congNoService.GetCongNoHistoryAsync(maDoiTuong);
            // tra ve client
            return Ok(result);
        }

        // POST : api/congnos : tao cong no moi
        [HttpPost]
        public async Task<IActionResult> CreateCongNoAsync([FromBody] CreateCongNoRequest request)
        {
            try
            {
                await _congNoService.CreateCongNoAsync(request);
                return CreatedAtAction(nameof(GetCongNoHistory), new { maDoiTuong = request.MaDoiTuong }, request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
