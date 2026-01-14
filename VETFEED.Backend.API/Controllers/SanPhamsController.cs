using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.SanPham;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamsController : ControllerBase {
        private readonly ISanPhamService _service;

        public SanPhamsController(ISanPhamService service) {
            _service = service;
        }

        // GET: api/SanPhams?...
        [HttpGet]
        public async Task<ActionResult<List<SanPhamResponse>>> Search(
            [FromQuery] SanPhamQuery query) {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        // GET: api/SanPhams/by-code/{maSPCode}
        [HttpGet("by-code/{maSPCode}")]
        public async Task<ActionResult<SanPhamResponse>> GetByCode(string maSPCode) {
            var sp = await _service.GetByCodeAsync(maSPCode);
            if (sp == null)
                return NotFound("Không tìm thấy sản phẩm.");

            return Ok(sp);
        }

        // GET: api/SanPhams/{maSP}
        [HttpGet("{maSP:guid}")]
        public async Task<ActionResult<SanPhamResponse>> GetById(Guid maSP) {
            var sp = await _service.GetByIdAsync(maSP);
            if (sp == null)
                return NotFound("Không tìm thấy sản phẩm.");

            return Ok(sp);
        }

        // POST: api/SanPhams
        [HttpPost]
        public async Task<ActionResult<SanPhamResponse>> Create(
            [FromBody] SanPhamCreateRequest request) {
            try {
                var created = await _service.CreateAsync(request);
                return Ok(created);
            }
            catch (ArgumentException ex) {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/SanPhams/{maSP}
        [HttpPut("{maSP:guid}")]
        public async Task<ActionResult<SanPhamResponse>> Update(
            Guid maSP,
            [FromBody] SanPhamUpdateRequest request) {
            try {
                var updated = await _service.UpdateAsync(maSP, request);
                if (updated == null)
                    return NotFound("Không tìm thấy sản phẩm.");

                return Ok(updated);
            }
            catch (ArgumentException ex) {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/SanPhams/{maSP}
        [HttpDelete("{maSP:guid}")]
        public async Task<ActionResult<string>> Delete(Guid maSP) {
            var (ok, error) = await _service.DeleteAsync(maSP);
            if (!ok)
                return BadRequest(error);

            return Ok("Xóa sản phẩm thành công.");
        }
    }
}
