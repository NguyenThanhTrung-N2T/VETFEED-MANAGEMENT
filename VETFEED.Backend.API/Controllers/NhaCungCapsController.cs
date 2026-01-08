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
        /// Thêm mới nhà cung cấp
        /// </summary>
        /// <param name="request">Thông tin nhà cung cấp cần thêm</param>
        /// <returns>Nhà cung cấp vừa được tạo</returns>
        /// <response code="201">Tạo nhà cung cấp thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost]
        [ProducesResponseType(typeof(NhaCungCapResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NhaCungCapRequest request)
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
        /// Cập nhật thông tin nhà cung cấp
        /// </summary>
        /// <param name="id">Mã nhà cung cấp cần cập nhật</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin nhà cung cấp sau khi cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="404">Không tìm thấy nhà cung cấp</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NhaCungCapResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] NhaCungCapRequest request)
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
