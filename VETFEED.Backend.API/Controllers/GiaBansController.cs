using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.GiaBan;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiaBansController : ControllerBase
    {
        private readonly IGiaBanService _service;
        public GiaBansController(IGiaBanService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] GiaBanQuery query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [HttpGet("{maGia:guid}")]
        public async Task<IActionResult> GetById(Guid maGia)
        {
            var gb = await _service.GetByIdAsync(maGia);
            if (gb == null) return NotFound("Không tìm thấy giá bán.");
            return Ok(gb);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiaBanCreateRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(request);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{maGia:guid}")]
        public async Task<IActionResult> Update(Guid maGia, [FromBody] GiaBanUpdateRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(maGia, request);
                if (updated == null) return NotFound("Không tìm thấy giá bán.");
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maGia:guid}")]
        public async Task<IActionResult> Delete(Guid maGia)
        {
            var (ok, error) = await _service.DeleteAsync(maGia);
            if (!ok) return BadRequest(error);
            return Ok("Xóa giá bán thành công.");
        }
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent([FromQuery] Guid maSP, [FromQuery] DateTime? date)
        {
            var d = (date ?? DateTime.Now).Date.AddDays(1).AddTicks(-1); // cuối ngày để đúng “tính cả ngày”
            var result = await _service.GetCurrentPriceAsync(maSP, d);
            if (result == null) return NotFound("Không tìm thấy giá hiện tại cho sản phẩm tại thời điểm này.");
            return Ok(result);
        }

    }
}
