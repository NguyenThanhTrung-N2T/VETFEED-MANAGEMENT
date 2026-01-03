using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.SanPham
{
    public class SanPhamUpdateRequest
    {
        [Required]
        public string TenSP { get; set; } = null!;

        [Required]
        public string LoaiSanPham { get; set; } = null!;

        public string? DonViTinh { get; set; }
        public string? GhiChu { get; set; }
    }
}
