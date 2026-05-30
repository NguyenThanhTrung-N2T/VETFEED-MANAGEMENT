using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.CTChuyenKho
{
    public class CTChuyenKhoRequest
    {
        [Required(ErrorMessage = "Mã lô không được bỏ trống !")]
        public Guid MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng chuyển không được bỏ trống !")]
        [Range(1, double.MaxValue, ErrorMessage = "Số lượng chuyển không được nhỏ hơn 1 !")]
        public decimal SoLuongChuyen { get; set; }

        [Required(ErrorMessage = "Trạng thái chuyển kho của lô hàng không được để trống !")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrangThaiPhieuChuyenKhoChiTietEnum TrangThai;

        public string? GhiChu { get; set; }
    }
}
