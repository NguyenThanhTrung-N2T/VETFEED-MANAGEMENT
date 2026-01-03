using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.TaiKhoan
{
    public class CreateTaiKhoanRequest
    {
        [Required(ErrorMessage = "Họ tên chủ tài khoản không được để trống !")]
        public string? HoTen { get; set; }

        [Required(ErrorMessage = "Email của tài khoản không được để trống !")]
        [EmailAddress(ErrorMessage = "Định dạng email không chính xác !")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu của tài khoản không được để trống !")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt !")]
        public string? Password { get; set; }
    }
}
