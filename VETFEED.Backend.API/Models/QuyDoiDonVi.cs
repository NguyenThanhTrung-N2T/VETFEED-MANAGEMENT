using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    public class QuyDoiDonVi
    {
        [Key]
        public Guid MaQD { get; set; }            // Khóa chính
        public Guid MaSP { get; set; }            // Khóa ngoại - Sản phẩm
        public string? DonViNhap { get; set; }     // Đơn vị nhập (Thùng/Hộp)
        public decimal TyLe { get; set; }         // 1 DonViNhap = TyLe DonViTinh (đơn vị chuẩn)

        // Navigation property (optional, nếu bạn dùng EF Core)
        public SanPham? SanPham { get; set; }      // Liên kết tới bảng SanPham
    }
}
