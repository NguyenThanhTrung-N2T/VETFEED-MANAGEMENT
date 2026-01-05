using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.DTOs.CTChuyenKho;

namespace VETFEED.Backend.API.DTOs.PhieuChuyenKho
{
    public class PhieuChuyenKhoRequest
    {
        public DateTime NgayLap { get; set; }

        [Required(ErrorMessage = "Mã kho xuất không được để trống !")]
        public Guid MaKhoXuat { get; set; }
        [Required(ErrorMessage = "Mã kho nhập không được bỏ trống !")]
        public Guid MaKhoNhan { get; set; }
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Danh sách sản phẩm cần chuyển không được bỏ trống !")]
        public List<CTChuyenKhoRequest>? DanhSachSanPham { get; set; }
    }
}
