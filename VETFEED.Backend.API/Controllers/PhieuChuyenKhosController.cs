using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.CTChuyenKho;
using VETFEED.Backend.API.DTOs.PhieuChuyenKho;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuChuyenKhosController : ControllerBase
    {
        private readonly IPhieuChuyenKhoService _service;
        public PhieuChuyenKhosController(IPhieuChuyenKhoService service)
        {
            _service = service;
        }

        // GET : api/phieuchuyenkhos : lay danh sach phieu chuyen kho 
        [HttpGet]
        public async Task<IActionResult> GetDanhSachPhieuChuyenKho()
        {
            // lay danh sach phieu chuyen kho 
            var result = await _service.GetDanhSachPhieuChuyenKhoAsync(); 
            // tra ve client
            return Ok(result);
        }

        // GET : api/phieuchuyenkhos/{maCK} : lay chi tiet phieu chuyen kho 
        [HttpGet("{maCK}")] 
        public async Task<IActionResult> GetChiTietPhieuChuyenKho(Guid maCK) 
        { 
            // lay chi tiet phieu chuyen kho
            var result = await _service.GetChiTietPhieuChuyenKhoAsync(maCK); 
            if (result == null) 
                return NotFound("Không tìm thấy phiếu chuyển kho"); 
            // tra ve chi tiet phieu
            return Ok(result); 
        }

        // POST : api/phieuchuyenkhos : tao phieu chuyen kho
        [HttpPost]
        public async Task<IActionResult> TaoPhieuChuyenKho([FromBody] PhieuChuyenKhoRequest request)
        {
            // kiem tra dau vao 
            if (!ModelState.IsValid)
                return BadRequest("Thông tin không đủ hoặc không đúng định dạng !");

            // tạo phieu chuyen kho
            var result = await _service.AddPhieuChuyenKhoAsync(request);
            return Ok(result);
        }

        // PUT : api/phieuchuyenkhos/{maCK} : cap nhat phieu chuyen kho
        [HttpPut("{maCK}")]
        public async Task<IActionResult> CapNhatPhieuChuyenKho(Guid maCK, [FromBody] UpdatePhieuChuyenKhoRequest request)
        {
            if (maCK != request.MaCK)
                return BadRequest("Mã phiếu không khớp!");

            var result = await _service.UpdatePhieuChuyenKhoAsync(request);
            if (result == null)
                return NotFound("Không tìm thấy phiếu chuyển kho!");

            // tra ve phieu chuyen kho sau khi cap nhat
            return Ok(result); 
        }

        // PUT : api/phieuchuyenkhos/chitiet/{maCTCK}/trangthai : cap nhat trang thai chi tiet chuyen kho
        [HttpPut("chitiet/{maCTCK}/trangthai")]
        public async Task<IActionResult> CapNhatTrangThaiChiTiet(Guid maCTCK, [FromBody] UpdateTrangThaiCTChuyenKho trangThai)
        {
            if (!ModelState.IsValid || !trangThai.TrangThai.HasValue) 
            { 
                return BadRequest("Trạng thái chi tiết chuyển kho không được để trống hoặc không hợp lệ!"); 
            }

            // cap nhat trang thai
            var result = await _service.UpdateTrangThaiChiTietAsync(maCTCK, trangThai);
            if (result == null)
                return NotFound("Không tìm thấy chi tiết phiếu chuyển kho!");

            // tra ve trang thai chi tiet 
            return Ok(result);
        }

        [HttpDelete("{maCK}")]
        public async Task<IActionResult> XoaPhieuChuyenKho(Guid maCK)
        {
            // xóa phiếu 
            var result = await _service.XoaPhieuChuyenKhoAsync(maCK);
            if (!result)
                return NotFound("Không tìm thấy phiếu chuyển kho !");

            return Ok("Phiếu chuyển kho đã được xóa!");
        }

    }
}
