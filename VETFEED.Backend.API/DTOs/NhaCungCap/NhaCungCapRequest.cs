using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.NhaCungCap
{
    public class NhaCungCapRequest
    {
        public string? TenNCC { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrangThaiNhaCungCapEnum TrangThai { get; set; } = TrangThaiNhaCungCapEnum.HOAT_DONG; // Mặc định là hoạt động
        public string? GhiChu { get; set; }
    }
}