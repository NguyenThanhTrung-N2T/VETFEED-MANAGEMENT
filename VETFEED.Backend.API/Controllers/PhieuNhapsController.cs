using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.PhieuNhap;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    /// <summary>
    /// Controller quản lý Phiếu nhập kho
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PhieuNhapsController : ControllerBase
    {
        private readonly IPhieuNhapService _service;

        public PhieuNhapsController(IPhieuNhapService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách tất cả phiếu nhập
        /// </summary>
        /// <returns>Danh sách phiếu nhập</returns>
        /// <response code="200">Trả về danh sách phiếu nhập</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PhieuNhapResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPhieuNhapsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết phiếu nhập theo ID (bao gồm chi tiết phiếu nhập)
        /// </summary>
        /// <param name="id">Mã phiếu nhập (GUID)</param>
        /// <returns>Thông tin chi tiết phiếu nhập</returns>
        /// <response code="200">Trả về thông tin phiếu nhập</response>
        /// <response code="404">Không tìm thấy phiếu nhập</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PhieuNhapDetailedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetPhieuNhapByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy phiếu nhập" });
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới phiếu nhập kho
        /// </summary>
        /// <remarks>
        /// Tự động tạo lô hàng mới cho mỗi chi tiết phiếu nhập.
        /// Trạng thái mặc định là DANG_XU_LY.
        /// </remarks>
        /// <param name="request">Thông tin phiếu nhập và chi tiết</param>
        /// <returns>Phiếu nhập vừa được tạo</returns>
        /// <response code="201">Tạo phiếu nhập thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost]
        [ProducesResponseType(typeof(PhieuNhapDetailedResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Cập nhật phiếu nhập kho
        /// </summary>
        /// <remarks>
        /// - Chỉ được cập nhật khi phiếu ở trạng thái DANG_XU_LY.
        /// - Khi chuyển sang trạng thái DA_NHAN, hệ thống sẽ tự động cập nhật tồn kho.
        /// - Không thể sửa phiếu đã có trạng thái DA_NHAN.
        /// </remarks>
        /// <param name="id">Mã phiếu nhập cần cập nhật</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin phiếu nhập sau khi cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc không thể cập nhật</response>
        /// <response code="404">Không tìm thấy phiếu nhập</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PhieuNhapDetailedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Xóa phiếu nhập kho
        /// </summary>
        /// <remarks>
        /// - Không thể xóa phiếu nhập đã có trạng thái DA_NHAN.
        /// - Khi xóa phiếu, các chi tiết phiếu nhập và lô hàng liên quan cũng sẽ bị xóa.
        /// </remarks>
        /// <param name="id">Mã phiếu nhập cần xóa</param>
        /// <returns>Không có nội dung trả về</returns>
        /// <response code="204">Xóa thành công</response>
        /// <response code="400">Không thể xóa phiếu nhập đã xác nhận</response>
        /// <response code="404">Không tìm thấy phiếu nhập</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
