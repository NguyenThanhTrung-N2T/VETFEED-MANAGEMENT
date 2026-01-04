using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoHangsController : ControllerBase
    {
        private readonly ILoHangService _service;

        public LoHangsController(ILoHangService service)
        {
            _service = service;
        }

        // GET: api/lohangs - Lấy tất cả lô hàng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllLoHangsAsync();
            return Ok(result);
        }

        // GET: api/lohangs/{id} - Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetLoHangByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy lô hàng" });
            return Ok(result);
        }

        // GET: api/lohangs/bysanpham/{maSP} - Lấy theo sản phẩm
        [HttpGet("bysanpham/{maSP}")]
        public async Task<IActionResult> GetBySanPham(Guid maSP)
        {
            var result = await _service.GetBySanPhamAsync(maSP);
            return Ok(result);
        }

        // POST: api/lohangs - Thêm mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoHangRequest request)
        {
            try
            {
                var result = await _service.AddLoHangAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.MaLo }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/lohangs/{id} - Cập nhật
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LoHangRequest request)
        {
            try
            {
                var result = await _service.UpdateLoHangAsync(id, request);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy lô hàng để cập nhật" });
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/lohangs/{id} - Xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteLoHangAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy lô hàng để xóa" });
            return NoContent();
        }
    }
}
