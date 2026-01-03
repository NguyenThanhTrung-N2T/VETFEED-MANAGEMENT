using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.KhachHang;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangsController : ControllerBase
    {
        private readonly IKhachHangService _service;
        public KhachHangsController(IKhachHangService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] KhachHangQuery query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [HttpGet("{maKH:guid}")]
        public async Task<IActionResult> GetById(Guid maKH)
        {
            var kh = await _service.GetByIdAsync(maKH);
            if (kh == null) return NotFound("Không tìm thấy khách hàng.");
            return Ok(kh);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhachHangCreateRequest request)
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

        [HttpPut("{maKH:guid}")]
        public async Task<IActionResult> Update(Guid maKH, [FromBody] KhachHangUpdateRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(maKH, request);
                if (updated == null) return NotFound("Không tìm thấy khách hàng.");
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maKH:guid}")]
        public async Task<IActionResult> Delete(Guid maKH)
        {
            var (ok, error) = await _service.DeleteAsync(maKH);
            if (!ok) return BadRequest(error);
            return Ok("Xóa khách hàng thành công.");
        }
    }
}
