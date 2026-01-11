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

        // POST : api/tonkhos/check : kiem tra ton kho theo ma kho va ma lo 
        [HttpPost("check")]
        public async Task<IActionResult> CheckTonKhoAsync(CheckTonKhoRequest request)
        {
            // kiem tra dau vao 
            if (!ModelState.IsValid)
            {
                return BadRequest("Thông tin không đầy đủ hoặc không đúng định dạng !");
            }
            // kiem tra ton kho 
            var result = await _tonKhoSerivce.IsTonKhoEnough(request.MaKhoXuat, request.MaLo, request.SoLuongChuyen);
            if (!result)
            {
                return BadRequest("Số lượng tồn kho không đủ !");
            }
            return Ok(result);
        }

        // POST : api/tonkhos/checkallkho : kiem tra ton kho theo ma lo tai tat ca cac kho
        [HttpPost("checkallkho")]
        public async Task<IActionResult> CheckTonKhoAllKhoAsync(CheckAllKhoRequest request)
        {
            // kiem tra dau vao 
            if (!ModelState.IsValid)
            {
                return BadRequest("Thông tin không đầy đủ hoặc không đúng định dạng !");
            }
            // kiem tra ton kho 
            var result = await _tonKhoSerivce.IsTonKhoEnoughAllKhoAsync(request.MaLo, request.SoLuongCan);
            if (!result)
            {
                return BadRequest("Tồn kho tại tất cả các kho không đủ !");
            }
            return Ok(result);

        }
    }
}
