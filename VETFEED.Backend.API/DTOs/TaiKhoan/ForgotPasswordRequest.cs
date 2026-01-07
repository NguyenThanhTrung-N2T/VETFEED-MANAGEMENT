using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.TaiKhoan
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email của tài khoản không được để trống !")]
        [EmailAddress(ErrorMessage = "Định dạng email không chính xác !")]
        public string? Email { get; set; }
    }
}
