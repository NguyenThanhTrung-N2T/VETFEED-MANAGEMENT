using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.DTOs.QuyDoiDonVi;
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
        public decimal? GiaBanDau { get; set; }
        public List<DonViQuyDoiItem> DonViQuyDoi { get; set; } = new();
    }
}
