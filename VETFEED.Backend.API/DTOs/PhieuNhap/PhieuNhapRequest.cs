using System.Text.Json.Serialization;

namespace VETFEED.Backend.API.DTOs.PhieuNhap
{
    public class PhieuNhapRequest
    {
        public Guid MaNCC { get; set; }
        public Guid MaKho { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}
