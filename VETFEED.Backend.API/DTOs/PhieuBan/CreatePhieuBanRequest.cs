using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.PhieuBan
{
    public class CreatePhieuBanRequest
    {
        [Required(ErrorMessage = "Mã khách hàng không ???c ?? tr?ng!")]
        public Guid MaKH { get; set; }

        [Required(ErrorMessage = "Ngày bán không ???c ?? tr?ng!")]
        public DateTime NgayBan { get; set; }

        [Range(0, 100, ErrorMessage = "Chi?t kh?u ph?i t? 0 ??n 100%!")]
        public decimal ChietKhauPhanTram { get; set; } = 0;

        [Required(ErrorMessage = "Hình th?c thanh toán không ???c ?? tr?ng!")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HinhThucThanhToanEnum HinhThucThanhToan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ti?n c?c không ???c âm!")]
        public decimal TienCoc { get; set; } = 0;

        public DateTime? HanTra { get; set; }

        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Danh sách chi ti?t phi?u bán không ???c ?? tr?ng!")]
        public List<ChiTietPhieuBanRequest>? DanhSachChiTiet { get; set; }
    }

    public class ChiTietPhieuBanRequest
    {
        [Required(ErrorMessage = "Mã lô không ???c ?? tr?ng!")]
        public Guid MaLo { get; set; }

        [Required(ErrorMessage = "S? l??ng c?n bán không ???c ?? tr?ng!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "S? l??ng ph?i l?n h?n 0!")]
        public decimal SoLuong { get; set; }

        [Required(ErrorMessage = "??n v? bán không ???c ?? tr?ng!")]
        public string? DonViBan { get; set; }

        [Required(ErrorMessage = "??n giá bán không ???c ?? tr?ng!")]
        [Range(0, double.MaxValue, ErrorMessage = "??n giá không ???c âm!")]
        public decimal DonGia { get; set; }

        public string? GhiChu { get; set; }
    }
}
