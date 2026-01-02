using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.TaiKhoan
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email của tài khoản không được để trống !")]
        [EmailAddress(ErrorMessage = "Định dạng email không chính xác !")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu của tài khoản không được để trống !")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt !")]
        public string? Password { get; set; }
    }
}
