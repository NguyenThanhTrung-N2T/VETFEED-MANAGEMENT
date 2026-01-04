using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.TonKho;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TonKhosController : ControllerBase
    {
        private readonly ITonKhoService _tonKhoSerivce;
        public TonKhosController(ITonKhoService service)
        {
            _tonKhoSerivce = service;
        }

        // GET : api/tonkhos : lay danh sanh ton kho 
        [HttpGet]
        public async Task<IActionResult> GetTonKhoByKhoAsync()
        {
            // lay danh sach ton kho cua cac kho 
            var danhSach = await _tonKhoSerivce.GetListTonKhoByKhoAsync();
            return Ok(danhSach);
        }

        // PUT : api/tonkhos/{maKho}/{maLo} : cap nhat so luong ton kho 
        [HttpPut("{maKho}/{maLo}")]
        public async Task<IActionResult> UpdateTonKho(Guid maKho, Guid maLo, [FromBody] UpdateTonKhoRequest request)
        {
            // cap nhat so luong 
            var success = await _tonKhoSerivce.UpdateTonKhoAsync(maKho, maLo, request.SoLuong);
            if (!success) 
                return NotFound("Không tìm thấy tồn kho với mã kho và mã lô này");
            return Ok("Cập nhật tồn kho thành công");
        }

    }
}
