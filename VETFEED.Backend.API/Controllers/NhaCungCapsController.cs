using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.NhaCungCap;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaCungCapsController : ControllerBase
    {
        private readonly INhaCungCapService _service;

        public NhaCungCapsController(INhaCungCapService service)
        {
            _service = service;
        }

        // GET: api/nhacungcaps - Lấy tất cả nhà cung cấp
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllNhaCungCapsAsync();
            return Ok(result);
        }

        // GET: api/nhacungcaps/{id} - Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetNhaCungCapByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp" });
            return Ok(result);
        }

        // POST: api/nhacungcaps - Thêm mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NhaCungCapRequest request)
        {
            var result = await _service.AddNhaCungCapAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.MaNCC }, result);
        }

        // PUT: api/nhacungcaps/{id} - Cập nhật
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NhaCungCapRequest request)
        {
            var result = await _service.UpdateNhaCungCapAsync(id, request);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp để cập nhật" });
            return Ok(result);
        }

        // DELETE: api/nhacungcaps/{id} - Xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteNhaCungCapAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp để xóa" });
            return NoContent();
        }
    }
}
