using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng danh mục sản phẩm
    /// </summary>
    public class SanPham
    {
        [Key]
        public Guid MaSP { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenSP { get; set; }
        public LoaiSanPhamEnum LoaiSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation
        public ICollection<NhaCungCapSanPham>? NhaCungCapSanPhams { get; set; }
        public ICollection<GiaBan>? GiaBans { get; set; }
        public ICollection<LoHang>? LoHangs { get; set; }
    }
}
