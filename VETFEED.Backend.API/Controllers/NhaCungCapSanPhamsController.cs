using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaCungCapSanPhamsController : ControllerBase
    {
        private readonly INhaCungCapSanPhamService _service;

        public NhaCungCapSanPhamsController(INhaCungCapSanPhamService service)
        {
            _service = service;
        }

        // GET: api/nhacungcapsanphams - Lấy tất cả liên kết NCC-SP
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllNhaCungCapSanPhamsAsync();
            return Ok(result);
        }

        // GET: api/nhacungcapsanphams/{id} - Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetNhaCungCapSanPhamByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy liên kết NCC-SP" });
            return Ok(result);
        }

        // GET: api/nhacungcapsanphams/bynhacungcap/{maNCC} - Lấy theo nhà cung cấp
        [HttpGet("bynhacungcap/{maNCC}")]
        public async Task<IActionResult> GetByNhaCungCap(Guid maNCC)
        {
            var result = await _service.GetByNhaCungCapAsync(maNCC);
            return Ok(result);
        }

        // GET: api/nhacungcapsanphams/bysanpham/{maSP} - Lấy theo sản phẩm
        [HttpGet("bysanpham/{maSP}")]
        public async Task<IActionResult> GetBySanPham(Guid maSP)
        {
            var result = await _service.GetBySanPhamAsync(maSP);
            return Ok(result);
        }

        // POST: api/nhacungcapsanphams - Thêm mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NhaCungCapSanPhamRequest request)
        {
            var result = await _service.AddNhaCungCapSanPhamAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.MaNCSP }, result);
        }

        // PUT: api/nhacungcapsanphams/{id} - Cập nhật
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NhaCungCapSanPhamRequest request)
        {
            var result = await _service.UpdateNhaCungCapSanPhamAsync(id, request);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy liên kết NCC-SP để cập nhật" });
            return Ok(result);
        }

        // DELETE: api/nhacungcapsanphams/{id} - Xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteNhaCungCapSanPhamAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy liên kết NCC-SP để xóa" });
            return NoContent();
        }
    }
}
