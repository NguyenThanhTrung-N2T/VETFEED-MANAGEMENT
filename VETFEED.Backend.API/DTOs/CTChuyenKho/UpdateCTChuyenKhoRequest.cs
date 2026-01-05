using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.CTChuyenKho
{
    public class UpdateCTChuyenKhoRequest
    {
        public Guid MaCTCK { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrangThaiPhieuChuyenKhoChiTietEnum TrangThai { get; set; }
    }
}
