using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.CTChuyenKho
{
    public class ChiTietChuyenKhoUpdateItem
    {
        public Guid? MaCTCK { get; set; }

        [Required(ErrorMessage = "Mã lô không được để trống !")]
        public Guid MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng chuyển không được bỏ trống !")]
        [Range(1, double.MaxValue, ErrorMessage = "Số lượng chuyển không được nhỏ hơn 1 !")]
        public decimal SoLuongChuyen { get; set; }
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Trạng thái chuyển kho của lô hàng không được để trống !")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrangThaiPhieuChuyenKhoChiTietEnum TrangThai { get; set; }
        public bool IsDeleted { get; set; }
    }
}
