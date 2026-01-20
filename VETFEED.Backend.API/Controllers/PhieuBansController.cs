using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.PhieuBan;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuBansController : ControllerBase
    {
        private readonly IPhieuBanService _phieuBanService;

        public PhieuBansController(IPhieuBanService phieuBanService)
        {
            _phieuBanService = phieuBanService;
        }

        // GET: api/phieubans
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PhieuBanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDanhSachPhieuBan()
        {
            try
            {
                var result = await _phieuBanService.GetDanhSachPhieuBanAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/phieubans/{maPB}
        [Authorize]
        [HttpGet("{maPB}")]
        [ProducesResponseType(typeof(PhieuBanDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChiTietPhieuBan(Guid maPB)
        {
            try
            {
                var result = await _phieuBanService.GetChiTietPhieuBanAsync(maPB);
                if (result == null)
                    return NotFound(new { error = "Phiếu bán không tồn tại !" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/phieubans
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(PhieuBanResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePhieuBan([FromBody] CreatePhieuBanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Dữ liệu không hợp lệ !" });

            try
            {
                var (result, error) = await _phieuBanService.CreatePhieuBanAsync(request);

                if (result == null)
                    return StatusCode(400, new { error = error ?? "Xảy ra lỗi khi tạo phiếu bán !" });

                return CreatedAtAction(nameof(GetChiTietPhieuBan), new { maPB = result.MaPB }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // DELETE: api/phieubans/{maPB}
        [Authorize(Roles = "QUAN_LY")]
        [HttpDelete("{maPB}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePhieuBan(Guid maPB)
        {
            try
            {
                var result = await _phieuBanService.DeletePhieuBanAsync(maPB);
                if (!result)
                    return NotFound(new { error = "Phiếu bán không tồn tại!" });

                return Ok(new { message = "Xóa phiếu bán thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/phieubans/khachhang/{maKH}
        [HttpGet("khachhang/{maKH}")]
        [ProducesResponseType(typeof(KhachHangPhieuBanResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPhieuBanTheoKhachHang(Guid maKH)
        {
            var result = await _phieuBanService.GetPhieuBanTheoKhachHangAsync(maKH);

            if (result == null)
                return NotFound("Khách hàng không tồn tại");

            return Ok(result);
        }
    }
}
