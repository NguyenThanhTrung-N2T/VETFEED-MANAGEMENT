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
        [HttpGet]
        public async Task<IActionResult> GetAllKhoHangsAsync()
        {
            // lấy tất cả kho hàng từ service
            var khoHangs = await _khoHangService.GetAllKhoHangsAsync();
            // trả về client 
            return Ok(khoHangs);
        }

        // GET : api/khohangs/{maKho} : lấy kho hàng theo mã kho
        [HttpGet("{maKho}", Name = "GetKhoHangById")]
        public async Task<IActionResult> GetKhoHangByIdAsync(Guid maKho)
        {
            // lấy kho hàng theo mã kho từ service
            var khohang = await _khoHangService.GetKhoHangByIdAsync(maKho);
            if (khohang == null)
            {
                return NotFound("Không tìm thấy kho hàng !");
            }
            // trả về client
            return Ok(khohang);
        }

        // POST : api/khohangs : thêm kho hàng
        [HttpPost]
        public async Task<IActionResult> AddKhoHangAsync([FromBody] CreateKhoHangRequest request)
        {
            // kiểm tra đầu vào 
            if (!ModelState.IsValid)
            {
                return BadRequest("Giá trị các thuộc tính chưa đủ hoặc không đúng chuẩn !");
            }
            // thêm kho hàng 
            var khoHang = await _khoHangService.AddKhoHangAsync(request);
            // trả về client 201 
            return CreatedAtRoute("GetKhoHangById", new { maKho = khoHang.MaKho }, khoHang);
        }

        // PUT : api/khohangs/{maKho} : cập nhật kho hàng 
        [HttpPut("{maKho}")]
        public async Task<IActionResult> UpdateKhoHangAsync(Guid maKho, [FromBody] UpdateKhoHangRequest request)
        {
            // kiểm tra đầu vào 
            if (!ModelState.IsValid)
            {
                return BadRequest("Giá trị các thuộc tính chưa đủ hoặc không đúng chuẩn !");
            }

            // cập nhật kho hàng 
            var khoHang = await _khoHangService.UpdateKhoHangAsync(maKho, request);
            if(khoHang == null)
            {
                return NotFound("Kho hàng không tồn tại !");
            }

            // trả về client 
            return Ok(khoHang);
        }

        // DELETE : api/khohangs/{maKho} : xóa kho hàng 
        [HttpDelete("{maKho}")]
        public async Task<IActionResult> DeleteKhoHangAsync(Guid maKho)
        {
            // kết quả xóa kho hàng
            var result = await _khoHangService.DeleteKhoHangAsync(maKho);
            if (!result)
            {
                return NotFound("Kho hàng không tồn tại !");
            }

            // trả về client
            return NoContent();
        }

        // POST : api/khohangs/search
        [HttpPost("search")]
        public async Task<IActionResult> SearchKhoHangAsync([FromBody] SearchKhoHangRequest request)
        {
            // danh sách kết quả tìm kiếm 
            var khoHangs = await _khoHangService.SearchKhoHangAsync(request);
            // trả về client 
            return Ok(khoHangs);
        }
    }
}
