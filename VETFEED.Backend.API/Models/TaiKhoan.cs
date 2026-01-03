using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng quản lý tài khoản người dùng hệ thống
    /// </summary>
    public class TaiKhoan
    {
        [Key]
        public Guid MaTK { get; set; }                // Khóa chính
        public string? Email { get; set; }             // Email đăng nhập
        public string? PasswordHash { get; set; }      // Mật khẩu đã hash
        public RoleEnum Role { get; set; }            // Vai trò (enum)
        public TrangThaiTaiKhoanEnum TrangThai { get; set; } // Trạng thái tài khoản (enum)
        public string? HoTen { get; set; }           // Họ tên 
        public string? SoDienThoai { get; set; }     // Số điện thoại 
        public string? AnhDaiDien { get; set; }      // Ảnh đại diện
        public DateTime NgayTao { get; set; }         // Ngày tạo
    }
}
