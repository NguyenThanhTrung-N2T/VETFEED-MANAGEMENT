// ============================================================================
// DEPRECATED: Controller này đã bị vô hiệu hóa vì NhaCungCapSanPham được quản lý 
// trực tiếp thông qua NhaCungCapsController.
// 
// Các chức năng CRUD của NCC-SP đã được tích hợp vào:
// - POST /api/NhaCungCaps: Tạo NCC kèm danh sách sản phẩm
// - PUT /api/NhaCungCaps/{id}: Thêm/Sửa/Xóa sản phẩm thông qua SanPhams list
// - DELETE /api/NhaCungCaps/{id}: Cascade xóa tất cả NCC-SP
// ============================================================================

/*
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    /// <summary>
    /// Controller quản lý liên kết Nhà cung cấp - Sản phẩm
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class NhaCungCapSanPhamsController : ControllerBase
    {
        private readonly INhaCungCapSanPhamService _service;

        public NhaCungCapSanPhamsController(INhaCungCapSanPhamService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách tất cả liên kết NCC-SP
        /// </summary>
        /// <returns>Danh sách liên kết NCC-SP</returns>
        /// <response code="200">Trả về danh sách liên kết</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NhaCungCapSanPhamResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllNhaCungCapSanPhamsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin liên kết NCC-SP theo ID
        /// </summary>
        /// <param name="id">Mã liên kết NCC-SP (GUID)</param>
        /// <returns>Thông tin liên kết NCC-SP</returns>
        /// <response code="200">Trả về thông tin liên kết</response>
        /// <response code="404">Không tìm thấy liên kết</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NhaCungCapSanPhamResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetNhaCungCapSanPhamByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy liên kết NCC-SP" });
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo nhà cung cấp
        /// </summary>
        /// <param name="maNCC">Mã nhà cung cấp (GUID)</param>
        /// <returns>Danh sách sản phẩm của nhà cung cấp</returns>
        /// <response code="200">Trả về danh sách sản phẩm</response>
        [HttpGet("bynhacungcap/{maNCC}")]
        [ProducesResponseType(typeof(IEnumerable<NhaCungCapSanPhamResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByNhaCungCap(Guid maNCC)
        {
            var result = await _service.GetByNhaCungCapAsync(maNCC);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách nhà cung cấp theo sản phẩm
        /// </summary>
        /// <param name="maSP">Mã sản phẩm (GUID)</param>
        /// <returns>Danh sách nhà cung cấp của sản phẩm</returns>
        /// <response code="200">Trả về danh sách nhà cung cấp</response>
        [HttpGet("bysanpham/{maSP}")]
        [ProducesResponseType(typeof(IEnumerable<NhaCungCapSanPhamResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySanPham(Guid maSP)
        {
            var result = await _service.GetBySanPhamAsync(maSP);
            return Ok(result);
        }

        /// <summary>
        /// Thêm mới liên kết NCC-SP
        /// </summary>
        /// <param name="request">Thông tin liên kết cần thêm</param>
        /// <returns>Liên kết vừa được tạo</returns>
        /// <response code="201">Tạo liên kết thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc liên kết đã tồn tại</response>
        [HttpPost]
        [ProducesResponseType(typeof(NhaCungCapSanPhamResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NhaCungCapSanPhamRequest request)
        {
            try
            {
                var result = await _service.AddNhaCungCapSanPhamAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.MaNCSP }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin liên kết NCC-SP
        /// </summary>
        /// <param name="id">Mã liên kết cần cập nhật</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin liên kết sau khi cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="404">Không tìm thấy liên kết</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NhaCungCapSanPhamResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] NhaCungCapSanPhamRequest request)
        {
            try
            {
                var result = await _service.UpdateNhaCungCapSanPhamAsync(id, request);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy liên kết NCC-SP để cập nhật" });
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa liên kết NCC-SP
        /// </summary>
        /// <param name="id">Mã liên kết cần xóa</param>
        /// <returns>Không có nội dung trả về</returns>
        /// <response code="204">Xóa thành công</response>
        /// <response code="404">Không tìm thấy liên kết</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteNhaCungCapSanPhamAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy liên kết NCC-SP để xóa" });
            return NoContent();
        }
    }
}
*/
