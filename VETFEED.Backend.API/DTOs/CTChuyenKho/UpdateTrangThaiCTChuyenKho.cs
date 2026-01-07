using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.CTChuyenKho
{
    public class UpdateTrangThaiCTChuyenKho
    {
        [Required(ErrorMessage = "Trạng thái chi tiết chuyển kho không được để trống !")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrangThaiPhieuChuyenKhoChiTietEnum? TrangThai { get; set; }
    }
}
