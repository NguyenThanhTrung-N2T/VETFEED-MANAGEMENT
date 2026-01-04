using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.NhaCungCapSanPham
{
    public class NhaCungCapSanPhamRequest
    {
        public Guid MaNCC { get; set; }
        public Guid MaSP { get; set; }
        public decimal? GiaNhapMacDinh { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrangThaiNhaCungCapSanPhamEnum? TrangThai { get; set; } = TrangThaiNhaCungCapSanPhamEnum.HOAT_DONG; // Mặc định là hoạt động
        public string? GhiChu { get; set; }
    }
}
