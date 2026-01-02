using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VETFEED.Backend.API.DTOs.TaiKhoan;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoansController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;
        public TaiKhoansController(ITaiKhoanService taikhoanService)
        {
            _taiKhoanService = taikhoanService;
        }

        // GET : api/taikhoans/{maTK} : lay tai khoan theo ma tai khoan
        [HttpGet("{maTK}", Name = "GetTaiKhoanById")]
        public async Task<IActionResult> GetTaiKhoanByIdAsync(Guid maTK)
        {
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(maTK);
            if(taiKhoan == null)
            {
                return NotFound("Tài khoản không tồn tại !");
            }
            return Ok(taiKhoan);
        }


        // POST : api/taikhoans/signup : dang ky tai khoan 
        [HttpPost("signup")]
        public async Task<IActionResult> SignUpTaiKhoanAsync([FromBody] CreateTaiKhoanRequest request)
        {
            // kiem tra dau vao
            if (!ModelState.IsValid)
            {
                return BadRequest("Thông tin tài khoản không đạt chuẩn !");
            }

            // dang ky tai khoan
            var taiKhoan = await _taiKhoanService.CreatTaiKhoanAsync(request);
            if(taiKhoan == null)
            {
                return Conflict("Email của tài khoản đã tồn tại !");
            }
            return CreatedAtRoute("GetTaiKhoanById", new { maTK = taiKhoan.MaTK }, taiKhoan);
        }
    }
}
