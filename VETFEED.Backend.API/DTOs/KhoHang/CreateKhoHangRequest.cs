using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.KhoHang
{
    public class CreateKhoHangRequest
    {
        [Required(ErrorMessage = "Tên kho không được để trống !")]
        public string? TenKho { get; set; }   // Tên kho

        [Required(ErrorMessage = "Địa chỉ kho không được để trống !")]
        public string? DiaChi { get; set; }            // Địa chỉ kho
        
        [Required(ErrorMessage = "Trạng thái kho không được để trống !")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        
        public TrangThaiKhoEnum TrangThai { get; set; } // Trạng thái hoạt động kho
        
        public string? GhiChu { get; set; }
    }
}
