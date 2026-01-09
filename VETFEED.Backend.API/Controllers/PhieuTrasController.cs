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
    }
}
