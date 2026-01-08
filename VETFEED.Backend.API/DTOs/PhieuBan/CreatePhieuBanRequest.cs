using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.PhieuBan
{
    public class CreatePhieuBanRequest
    {
        [Required(ErrorMessage = "Mã khách hàng không được để trống !")]
        public Guid MaKH { get; set; }

        [Required(ErrorMessage = "Ngày bán không được để trống !")]
        public DateTime NgayBan { get; set; }

        [Range(0, 100, ErrorMessage = "Chiết khấu phải từ 0 đến 100 % !")]
        public decimal ChietKhauPhanTram { get; set; } = 0;

        [Required(ErrorMessage = "Hình thức thanh toán không được để trống !")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HinhThucThanhToanEnum HinhThucThanhToan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc không được âm !")]
        public decimal TienCoc { get; set; } = 0;

        public DateTime? HanTra { get; set; }

        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Danh sách chi tiết phiếu bán không được để trống !")]
        public List<ChiTietPhieuBanRequest>? DanhSachChiTiet { get; set; }
    }

    public class ChiTietPhieuBanRequest
    {
        [Required(ErrorMessage = "Mã lô không được để trống !")]
        public Guid MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng cần bán không được để trống !")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0 !")]
        public decimal SoLuong { get; set; }

        [Required(ErrorMessage = "Đơn vị bán không được để trống !")]
        public string? DonViBan { get; set; }

        [Required(ErrorMessage = "Đơn giá bán không được để trống !")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm !")]
        public decimal DonGia { get; set; }

        public string? GhiChu { get; set; }
    }
}
