using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.PhieuNhap;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuNhapsController : ControllerBase
    {
        private readonly IPhieuNhapService _service;

        public PhieuNhapsController(IPhieuNhapService service)
        {
            _service = service;
        }

        // GET: api/phieunhaps - Lấy tất cả phiếu nhập
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPhieuNhapsAsync();
            return Ok(result);
        }

        // GET: api/phieunhaps/{id} - Lấy theo ID (bao gồm chi tiết)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetPhieuNhapByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy phiếu nhập" });
            return Ok(result);
        }

        // POST: api/phieunhaps - Tạo phiếu nhập mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuNhapCreateRequest request)
        {
            try
            {
                var result = await _service.CreatePhieuNhapAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.MaPN }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/phieunhaps/{id} - Cập nhật phiếu nhập
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PhieuNhapUpdateRequest request)
        {
            try
            {
                var result = await _service.UpdatePhieuNhapAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/phieunhaps/{id} - Xóa phiếu nhập
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeletePhieuNhapAsync(id);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy phiếu nhập" });
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


