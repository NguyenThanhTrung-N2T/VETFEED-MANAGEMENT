using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.SanPham
{
    public class SanPhamCreateRequest
    {
        [Required]
        public string TenSP { get; set; } = null!;

        [Required]
        public string LoaiSanPham { get; set; } = null!; // THUOC_THU_Y | THUC_AN_CHAN_NUOI

        public string? DonViTinh { get; set; }
        public string? GhiChu { get; set; }
    }
}
