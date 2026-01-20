using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.DTOs.QuyDoiDonVi;
using VETFEED.Backend.API.Enums;
namespace VETFEED.Backend.API.DTOs.SanPham
{
    public class SanPhamUpdateRequest
    {
        [Required]
        public string TenSP { get; set; } = null!;

        [Required]
        public string LoaiSanPham { get; set; } = null!;

        public string? DonViTinh { get; set; }
        public string? AnhSanPham { get; set; }
        public string? GhiChu { get; set; }
        public decimal? GiaMoi { get; set; }
        public List<DonViQuyDoiItem> DonViQuyDoi { get; set; } = new();
    }
}
