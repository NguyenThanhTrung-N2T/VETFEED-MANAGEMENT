using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.TaiKhoan;
using VETFEED.Backend.API.Services;
using VETFEED.Backend.API.Utils;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoansController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public TaiKhoansController(ITaiKhoanService taikhoanService, JwtService jwtService, IConfiguration config, EmailService emailService)
        {
            _taiKhoanService = taikhoanService;
            _jwtService = jwtService;
            _config = config;
            _emailService = emailService;
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
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("AccessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Đã đăng xuất thành công!" });
        }

        // POST : api/taikhoans/reset-password : cap nhat mat khau
        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordRequest request)
        {
            // kiem tra dau vao
            if (!ModelState.IsValid)
            {
                return BadRequest("Email hoặc mật khẩu không đạt chuẩn !");
            }
            // cap nhat mat khau 
            var result = await _taiKhoanService.UpdatePasswordAsync(request.Email!, request.Password!);
            if (!result)
            {
                return NotFound("Email không tồn tại !");
            }
            return Ok("Cập nhật mật khẩu thành công !");
        }

        // POST : api/taikhoans/forgot-password : quên mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword ([FromBody] ForgotPasswordRequest request)
        {
            // kiem tra email ton tai
            var user = await _taiKhoanService.GetTaiKhoanByEmailAsync(request.Email!); 
            if (user == null) 
            { 
                return NotFound("Email không tồn tại trong hệ thống!"); 
            }

            // sinh password 
            var newPassword = GenerateRandomPassword();
            // cap nhat mat khau
            var updated = await _taiKhoanService.UpdatePasswordAsync(request.Email!, newPassword); 
            if (!updated) 
            { 
                return StatusCode(500, "Không thể cập nhật mật khẩu!"); 
            }

            // gui mat khau qua email
            await _emailService.SendResetPasswordEmailAsync(request.Email!, newPassword);

            return Ok("Mật khẩu mới đã được gửi qua email!");


        }
        private string GenerateRandomPassword(int length = 8)
        {
            if (length < 8)
                throw new ArgumentException("Độ dài mật khẩu phải >= 8");

            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "@$!%*?&";
            const string allChars = upper + lower + digits + special;

            var random = new Random();

            // Bắt buộc mỗi loại ký tự có ít nhất 1
            var passwordChars = new List<char>
    {
        upper[random.Next(upper.Length)],
        lower[random.Next(lower.Length)],
        digits[random.Next(digits.Length)],
        special[random.Next(special.Length)]
    };

            // Sinh thêm các ký tự ngẫu nhiên cho đủ độ dài
            for (int i = passwordChars.Count; i < length; i++)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            // Trộn ngẫu nhiên để không theo thứ tự cố định
            return new string(passwordChars.OrderBy(x => random.Next()).ToArray());
        }

    }
}
