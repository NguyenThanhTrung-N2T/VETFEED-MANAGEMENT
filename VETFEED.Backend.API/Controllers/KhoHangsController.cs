using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.KhoHang;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoHangsController : ControllerBase
    {
        private readonly IKhoHangService _khoHangService;
        public KhoHangsController(IKhoHangService khoHangService)
        {
            _khoHangService = khoHangService;
        }

        // GET: api/khohangs  : lấy tất cả kho hàng 
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<KhoHangResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllKhoHangsAsync()
        {
            // lấy tất cả kho hàng từ service
            var khoHangs = await _khoHangService.GetAllKhoHangsAsync();
            // trả về client 
            return Ok(khoHangs);
        }

        // GET : api/khohangs/{maKho} : lấy kho hàng theo mã kho
        [Authorize]
        [HttpGet("{maKho}", Name = "GetKhoHangById")]
        [ProducesResponseType(typeof(KhoHangResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetKhoHangByIdAsync(Guid maKho)
        {
            // lấy kho hàng theo mã kho từ service
            var khohang = await _khoHangService.GetKhoHangByIdAsync(maKho);
            if (khohang == null)
            {
                return NotFound(new { error = "Không tìm thấy kho hàng !" });
            }
            // trả về client
            return Ok(khohang);
        }

        // POST : api/khohangs : thêm kho hàng
        [Authorize(Roles = "QUAN_LY")]
        [HttpPost]
        [ProducesResponseType(typeof(KhoHangResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddKhoHangAsync([FromBody] CreateKhoHangRequest request)
        {
            try
            {
                // kiểm tra đầu vào 
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Giá trị các thuộc tính chưa đủ hoặc không đúng chuẩn !" });
                }
                // thêm kho hàng 
                var khoHang = await _khoHangService.AddKhoHangAsync(request);
                // trả về client 201 
                return CreatedAtRoute("GetKhoHangById", new { maKho = khoHang.MaKho }, khoHang);
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Xảy ra lỗi khi thêm kho hàng. Vui lòng thử lại sau." });
            }
        }

        // PUT : api/khohangs/{maKho} : cập nhật kho hàng 
        [Authorize(Roles = "QUAN_LY")]
        [HttpPut("{maKho}")]
        [ProducesResponseType(typeof(KhoHangResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateKhoHangAsync(Guid maKho, [FromBody] UpdateKhoHangRequest request)
        {
            try
            {
                // kiểm tra đầu vào 
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Giá trị các thuộc tính chưa đủ hoặc không đúng chuẩn !" });
                }

                // cập nhật kho hàng 
                var khoHang = await _khoHangService.UpdateKhoHangAsync(maKho, request);
                if(khoHang == null)
                {
                    return NotFound(new { error = "Kho hàng không tồn tại !" });
                }

                // trả về client 
                return Ok(khoHang);
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Xảy ra lỗi khi cập nhật kho hàng. Vui lòng thử lại sau." });
            }
        }

        // DELETE : api/khohangs/{maKho} : xóa kho hàng 
        [Authorize(Roles = "QUAN_LY")]
        [HttpDelete("{maKho}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteKhoHangAsync(Guid maKho)
        {
            try
            {
                // kết quả xóa kho hàng
                var result = await _khoHangService.DeleteKhoHangAsync(maKho);
                if (!result)
                {
                    return NotFound(new { error = "Kho hàng không tồn tại !" });
                }

                // trả về client
                return NoContent();
            }
            catch (Exception ex)
            {
                // Kiểm tra nếu là lỗi ràng buộc khóa ngoại (FK constraint)
                if (ex.InnerException != null && ex.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    return BadRequest(new { error = "Không thể xóa kho hàng vì vẫn còn phiếu nhập hoặc phiếu chuyển kho liên quan." });
                }
                
                // Trả về lỗi chung
                return BadRequest(new { error = "Xảy ra lỗi khi xóa kho hàng. Vui lòng thử lại sau." });
            }
        }

        // POST : api/khohangs/search
        [Authorize]
        [HttpPost("search")]
        [ProducesResponseType(typeof(IEnumerable<KhoHangResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchKhoHangAsync([FromBody] SearchKhoHangRequest request)
        {
            // danh sách kết quả tìm kiếm 
            var khoHangs = await _khoHangService.SearchKhoHangAsync(request);
            // trả về client 
            return Ok(khoHangs);
        }
    }
}
