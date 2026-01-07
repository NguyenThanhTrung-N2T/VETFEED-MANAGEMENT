using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.DTOs.CTChuyenKho;

namespace VETFEED.Backend.API.DTOs.PhieuChuyenKho
{
    public class UpdatePhieuChuyenKhoRequest
    {
        [Required(ErrorMessage = "Mã phiếu chuyển kho không được để trống !")]
        public Guid MaCK { get; set; }
        public DateTime NgayLap { get; set; }

        [Required(ErrorMessage = "Mã kho xuất không được để trống !")]
        public Guid MaKhoNhan { get; set; }

        public string? GhiChu { get; set; }
        public List<ChiTietChuyenKhoUpdateItem>? DanhSachChiTiet { get; set; }
    }
}
