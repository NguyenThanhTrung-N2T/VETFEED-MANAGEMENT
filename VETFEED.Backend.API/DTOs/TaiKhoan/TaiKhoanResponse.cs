using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.TaiKhoan
{
    public class TaiKhoanResponse
    {
        public Guid MaTK { get; set; }
        public string? Email { get; set; }
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? AnhDaiDien { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoleEnum Role { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrangThaiTaiKhoanEnum TrangThai { get; set; }
    }
}
