using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.TaiKhoan
{
    public class UpdateTaiKhoanRequest
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? AnhDaiDien { get; set; }
    }
}
