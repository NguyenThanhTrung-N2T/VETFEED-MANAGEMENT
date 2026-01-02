using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.TaiKhoan;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoansController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;

        public TaiKhoansController(ITaiKhoanService taikhoanService, JwtService jwtService, IConfiguration config)
        {
            _taiKhoanService = taikhoanService;
            _jwtService = jwtService;
            _config = config;
        }

        // GET : api/taikhoans/{maTK} : lấy tài khoản theo mã
        [Authorize]
        [HttpGet("{maTK}", Name = "GetTaiKhoanById")]
        public async Task<IActionResult> GetTaiKhoanByIdAsync(Guid maTK)
        {
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(maTK);
            if (taiKhoan == null)
                return NotFound(new { error = "Tài khoản không tồn tại!" });
            
            return Ok(taiKhoan);
        }

        // POST : api/taikhoans/signup : đăng ký tài khoản
        [HttpPost("signup")]
        public async Task<IActionResult> SignUpTaiKhoanAsync([FromBody] CreateTaiKhoanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Thông tin tài khoản không hợp lệ!" });

            var taiKhoan = await _taiKhoanService.CreatTaiKhoanAsync(request);
            if (taiKhoan == null)
                return Conflict(new { error = "Email đã tồn tại!" });
            
            return CreatedAtRoute("GetTaiKhoanById", new { maTK = taiKhoan.MaTK }, taiKhoan);
        }

        // PUT : api/taikhoans/{maTK} : cập nhật tài khoản
        [Authorize]
        [HttpPut("{maTK}")]
        public async Task<IActionResult> UpdateTaiKhoanAsync(Guid maTK, [FromBody] UpdateTaiKhoanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Thông tin tài khoản không hợp lệ!" });

            var result = await _taiKhoanService.UpdateTaiKhoanAsync(maTK, request);
            
            if (result == null)
                return NotFound(new { error = "Tài khoản không tồn tại!" });

            return Ok(result);
        }

        // POST : api/taikhoans/login : đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _taiKhoanService.Login(request);
            if (!result)
                return Unauthorized(new { error = "Email hoặc mật khẩu không chính xác!" });

            var taikhoan = await _taiKhoanService.GetTaiKhoanByEmailAsync(request.Email!);
            if (taikhoan == null)
                return NotFound(new { error = "Tài khoản không tồn tại!" });

            var token = _jwtService.GenerateToken(taikhoan);
            var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"]!);

            Response.Cookies.Append("AccessToken", token, new CookieOptions 
            { 
                HttpOnly = true, 
                Secure = true, 
                SameSite = SameSiteMode.Strict, 
                Expires = DateTimeOffset.UtcNow.AddMinutes(expireMinutes) 
            });

            return Ok("Đăng nhập thành công !");
        }

        // POST : api/taikhoans/logout : đăng xuất
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Đã đăng xuất thành công!" });
        }
    }
}
