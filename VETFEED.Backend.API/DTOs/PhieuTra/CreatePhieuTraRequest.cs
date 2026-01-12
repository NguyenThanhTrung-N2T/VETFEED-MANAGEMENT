using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.PhieuTra
{
    public class CreatePhieuTraRequest
    {
        [Required(ErrorMessage = "Mã phiếu bán không được để trống !")]
        public Guid MaPB { get; set; }

        [Required(ErrorMessage = "Hình thức hoàn tiền không được để trống !")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HinhThucHoanTienEnum HinhThucHoanTien { get; set; }

        public string? LyDoTra { get; set; }

        public List<ChiTietPhieuTraRequest>? DanhSachChiTiet { get; set; }
    }

    public class ChiTietPhieuTraRequest
    {
        [Required(ErrorMessage = "Mã lô không được để trống !")]
        public Guid MaLo { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Số lượng phải > 0")]
        public decimal SoLuong { get; set; }

        public string? GhiChu { get; set; }
    }
}
