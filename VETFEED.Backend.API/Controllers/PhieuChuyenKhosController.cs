using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    }
}
