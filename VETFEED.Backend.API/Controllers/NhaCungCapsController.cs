using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.NhaCungCap;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    /// <summary>
    /// Controller quản lý Nhà cung cấp
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class NhaCungCapsController : ControllerBase
    {
        private readonly INhaCungCapService _service;

        public NhaCungCapsController(INhaCungCapService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách tất cả nhà cung cấp
        /// </summary>
        /// <returns>Danh sách nhà cung cấp</returns>
        /// <response code="200">Trả về danh sách nhà cung cấp</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NhaCungCapResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllNhaCungCapsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết nhà cung cấp theo ID
        /// </summary>
        /// <param name="id">Mã nhà cung cấp (GUID)</param>
        /// <returns>Thông tin chi tiết nhà cung cấp</returns>
        /// <response code="200">Trả về thông tin nhà cung cấp</response>
        /// <response code="404">Không tìm thấy nhà cung cấp</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NhaCungCapDetailedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetNhaCungCapByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp" });
            return Ok(result);
        }

        /// <summary>
        /// Thêm mới nhà cung cấp kèm danh sách sản phẩm (nếu có)
        /// </summary>
        /// <param name="request">Thông tin nhà cung cấp cần thêm</param>
        /// <returns>Nhà cung cấp vừa được tạo</returns>
        /// <remarks>
        /// **Xử lý danh sách SanPhams (không bắt buộc):**
        /// - Nếu SanPhams = null hoặc rỗng: chỉ tạo nhà cung cấp
        /// - Nếu có SanPhams: tạo nhà cung cấp và tạo liên kết NCC-SP
        /// 
        /// **Sử dụng transaction** để đảm bảo tính nhất quán dữ liệu.
        /// </remarks>
        /// <response code="201">Tạo nhà cung cấp thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost]
        [ProducesResponseType(typeof(NhaCungCapDetailedResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NhaCungCapCreateRequest request)
        {
            try
            {
                var result = await _service.AddNhaCungCapAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.MaNCC }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp và danh sách sản phẩm
        /// </summary>
        /// <param name="id">Mã nhà cung cấp cần cập nhật</param>
        /// <param name="request">Thông tin cập nhật bao gồm danh sách sản phẩm (nếu có)</param>
        /// <returns>Thông tin chi tiết nhà cung cấp sau khi cập nhật</returns>
        /// <remarks>
        /// **Xử lý danh sách SanPhams (giống PhieuNhap):**
        /// - Nếu SanPhams = null: chỉ cập nhật thông tin nhà cung cấp, KHÔNG thay đổi danh sách SP
        /// - Nếu SanPhams = []: xóa TẤT CẢ sản phẩm của nhà cung cấp
        /// - NCCSP trong DB nhưng KHÔNG có trong request: **XÓA**
        /// - MaNCSP = null/Guid.Empty: **thêm mới** sản phẩm
        /// - MaNCSP có giá trị: **cập nhật** sản phẩm
        /// 
        /// **Sử dụng transaction** để đảm bảo tính nhất quán dữ liệu.
        /// </remarks>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="404">Không tìm thấy nhà cung cấp</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NhaCungCapDetailedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] NhaCungCapUpdateRequest request)
        {
            try
            {
                var result = await _service.UpdateNhaCungCapAsync(id, request);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy nhà cung cấp để cập nhật" });
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="id">Mã nhà cung cấp cần xóa</param>
        /// <returns>Không có nội dung trả về</returns>
        /// <remarks>
        /// **Cascade delete**: Xóa nhà cung cấp sẽ tự động xóa tất cả liên kết NCC-SP liên quan.
        /// 
        /// **Sử dụng transaction** để đảm bảo tính nhất quán dữ liệu.
        /// </remarks>
        /// <response code="204">Xóa thành công</response>
        /// <response code="404">Không tìm thấy nhà cung cấp</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteNhaCungCapAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy nhà cung cấp để xóa" });
            return NoContent();
        }
    }
}
