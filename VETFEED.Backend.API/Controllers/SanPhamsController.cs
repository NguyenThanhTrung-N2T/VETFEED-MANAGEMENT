using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.SanPham;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly ISanPhamService _service;
        public SanPhamsController(ISanPhamService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SanPhamQuery query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [HttpGet("{maSP:guid}")]
        public async Task<IActionResult> GetById(Guid maSP)
        {
            var sp = await _service.GetByIdAsync(maSP);
            if (sp == null) return NotFound("Không tìm thấy sản phẩm.");
            return Ok(sp);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SanPhamCreateRequest request)
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

        [HttpPut("{maSP:guid}")]
        public async Task<IActionResult> Update(Guid maSP, [FromBody] SanPhamUpdateRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(maSP, request);
                if (updated == null) return NotFound("Không tìm thấy sản phẩm.");
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maSP:guid}")]
        public async Task<IActionResult> Delete(Guid maSP)
        {
            var (ok, error) = await _service.DeleteAsync(maSP);
            if (!ok) return BadRequest(error);
            return Ok("Xóa sản phẩm thành công.");
        }
    }
}
