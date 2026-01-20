using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpGet("summary")]
        [ProducesResponseType(typeof(List<CongNoTongHopResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CongNoTongHopResponse>>> GetTongHopCongNo()
        {
            // lay cong no tong hop cua tat ca doi tuong
            var result = await _congNoService.GetTongHopCongNoAsync();
            // tra ve client
            return Ok(result);
        }

        // GET : api/congnos/{maDoiTuong}/detail
        [Authorize]
        [HttpGet("{maDoiTuong}/detail")]
        public async Task<ActionResult<List<CongNoHistoryResponse>>> GetCongNoHistory(Guid maDoiTuong)
        {
            // lay lich su cong no cua doi tuong theo ma doi tuong
            var result = await _congNoService.GetCongNoHistoryAsync(maDoiTuong);
            // tra ve client
            return Ok(result);
        }

        // POST : api/congnos : tao cong no moi
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCongNoAsync([FromBody] CreateCongNoRequest request) {
            try {
                await _congNoService.CreateCongNoAsync(request);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

    }
}
