using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.PhieuTra;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    /// <summary>
    /// Controller quản lý Phiếu trả hàng
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PhieuTrasController : ControllerBase
    {
        private readonly IPhieuTraService _service;

        public PhieuTrasController(IPhieuTraService service)
        {
            _service = service;
        }

        //Trả về danh sách phiếu trả được sắp xếp theo ngày trả (mới nhất trước).
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PhieuTraListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDanhSachPhieuTra()
        {
            try
            {
                var result = await _service.GetDanhSachPhieuTraAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        //Trả về thông tin phiếu trả kèm theo danh sách chi tiết sản phẩm được trả.
        [Authorize]
        [HttpGet("{maPT}")]
        [ProducesResponseType(typeof(PhieuTraDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChiTietPhieuTra(Guid maPT)
        {
            try
            {
                var result = await _service.GetChiTietPhieuTraAsync(maPT);
                if (result == null)
                    return NotFound(new { error = "Phiếu trả không tồn tại!" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //Tạo mới phiếu trả hàng
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(PhieuTraDetailResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePhieuTra([FromBody] CreatePhieuTraRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Dữ liệu không hợp lệ!" });

            try
            {
                var (result, error) = await _service.CreatePhieuTraAsync(request);

                if (result == null)
                    return BadRequest(new { error = error ?? "Xảy ra lỗi khi tạo phiếu trả!" });

                return CreatedAtAction(nameof(GetChiTietPhieuTra), new { maPT = result.MaPT }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //Xóa phiếu trả hàng
        [Authorize(Roles = "QUAN_LY")]
        [HttpDelete("{maPT}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePhieuTra(Guid maPT)
        {
            try
            {
                var result = await _service.DeletePhieuTraAsync(maPT);
                if (!result)
                    return NotFound(new { error = "Phiếu trả không tồn tại!" });

                return Ok(new { message = "Xóa phiếu trả thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra có thể trả hàng được không.
        /// API này giúp FE validate trước khi tạo phiếu trả để tránh trả vượt quá số lượng đã bán.
        /// </summary>
        /// <param name="request">Thông tin kiểm tra: MaPB, MaLo, SoLuong</param>
        /// <returns>true nếu có thể trả, false nếu không</returns>
        /// <remarks>
        /// **Logic kiểm tra:**
        /// - Lấy tổng số lượng đã bán của lô trong phiếu bán
        /// - Lấy tổng số lượng đã trả của lô từ các phiếu trả trước
        /// - Tính: SoLuongCoTheTra = SoLuongDaBan - SoLuongDaTra
        /// - Trả về true nếu SoLuong <= SoLuongCoTheTra
        /// 
        /// **Trường hợp trả về false:**
        /// - Phiếu bán không tồn tại
        /// - Lô không có trong phiếu bán
        /// - Số lượng cần trả > số lượng có thể trả
        /// </remarks>
        /// <response code="200">Trả về true/false</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="500">Lỗi server</response>
        [Authorize]
        [HttpPost("check-returnable")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckReturnable([FromBody] CheckReturnableRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Dữ liệu không hợp lệ!" });

            try
            {
                var result = await _service.CheckReturnableAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
