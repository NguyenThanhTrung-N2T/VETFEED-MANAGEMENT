using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    /// <summary>
    /// Controller quản lý Lô hàng
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LoHangsController : ControllerBase
    {
        private readonly ILoHangService _service;

        public LoHangsController(ILoHangService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách tất cả lô hàng
        /// </summary>
        /// <returns>Danh sách lô hàng</returns>
        /// <response code="200">Trả về danh sách lô hàng</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LoHangResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllLoHangsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin lô hàng theo ID
        /// </summary>
        /// <param name="id">Mã lô hàng (GUID)</param>
        /// <returns>Thông tin lô hàng</returns>
        /// <response code="200">Trả về thông tin lô hàng</response>
        /// <response code="404">Không tìm thấy lô hàng</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LoHangResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetLoHangByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy lô hàng" });
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách lô hàng theo sản phẩm
        /// </summary>
        /// <param name="maSP">Mã sản phẩm (GUID)</param>
        /// <returns>Danh sách lô hàng của sản phẩm</returns>
        /// <response code="200">Trả về danh sách lô hàng</response>
        [HttpGet("bysanpham/{maSP}")]
        [ProducesResponseType(typeof(IEnumerable<LoHangResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySanPham(Guid maSP)
        {
            var result = await _service.GetBySanPhamAsync(maSP);
            return Ok(result);
        }

        /// <summary>
        /// Thêm mới lô hàng
        /// </summary>
        /// <param name="request">Thông tin lô hàng cần thêm</param>
        /// <returns>Lô hàng vừa được tạo</returns>
        /// <response code="201">Tạo lô hàng thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost]
        [ProducesResponseType(typeof(LoHangResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Cập nhật thông tin lô hàng
        /// </summary>
        /// <param name="id">Mã lô hàng cần cập nhật</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin lô hàng sau khi cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="404">Không tìm thấy lô hàng</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LoHangResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Xóa lô hàng
        /// </summary>
        /// <param name="id">Mã lô hàng cần xóa</param>
        /// <returns>Không có nội dung trả về</returns>
        /// <response code="204">Xóa thành công</response>
        /// <response code="404">Không tìm thấy lô hàng</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteLoHangAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy lô hàng để xóa" });
            return NoContent();
        }
    }
}
