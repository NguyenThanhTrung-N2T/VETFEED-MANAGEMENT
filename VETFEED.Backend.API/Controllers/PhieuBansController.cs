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
        [HttpGet]
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
        [HttpGet("{maPB}")]
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
        [HttpPost]
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

        // PUT: api/phieubans/{maPB}
        [HttpPut("{maPB}")]
        public async Task<IActionResult> UpdatePhieuBan(Guid maPB, [FromBody] CreatePhieuBanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "D? li?u không h?p l?!" });

            try
            {
                var (result, error) = await _phieuBanService.UpdatePhieuBanAsync(maPB, request);

                if (result == null)
                    return StatusCode(400, new { error = error ?? "X?y ra l?i khi c?p nh?t phi?u bán!" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE: api/phieubans/{maPB}
        [HttpDelete("{maPB}")]
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
    }
}
