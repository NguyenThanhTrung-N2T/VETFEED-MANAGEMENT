using System;
using System.Collections.Generic;
using VETFEED.Backend.API.DTOs.QuyDoiDonVi;  
namespace VETFEED.Backend.API.DTOs.SanPham
{
    public class SanPhamResponse
    {
        public Guid MaSP { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenSP { get; set; }
        public string? LoaiSanPham { get; set; }
        public string? DonViCoSo { get; set; }
        public string? AnhSanPham { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal? DonGia { get; set; }
        public List<DonViQuyDoiItem> DonViQuyDoi { get; set; } = new();
    }
}
