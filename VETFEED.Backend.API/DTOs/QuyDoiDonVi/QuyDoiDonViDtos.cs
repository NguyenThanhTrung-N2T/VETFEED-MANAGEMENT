using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.QuyDoiDonVi
{
    public class QuyDoiDonViResponse
    {
        public Guid MaQD { get; set; }
        public Guid MaSP { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenSP { get; set; }

        public string DonViNhap { get; set; } = null!;   // Thùng/Hộp...
        public decimal TyLe { get; set; }                // 1 DonViNhap = TyLe đơn vị cơ sở
    }

    public class QuyDoiDonViCreateRequest
    {
        [Required]
        public string DonViNhap { get; set; } = null!;

        [Range(0.000001, double.MaxValue, ErrorMessage = "TyLe phải > 0")]
        public decimal TyLe { get; set; }
    }

    public class QuyDoiDonViUpdateRequest
    {
        [Required]
        public string DonViNhap { get; set; } = null!;

        [Range(0.000001, double.MaxValue, ErrorMessage = "TyLe phải > 0")]
        public decimal TyLe { get; set; }
    }

    public class DonViQuyDoiItem
    {
        public string DonVi { get; set; } = null!;
        public decimal TyLe { get; set; }
    }

    public class UnitsForProductResponse
    {
        public Guid MaSP { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenSP { get; set; }

        public string? DonViCoSo { get; set; }           // VIEN/KG/LIT...
        public List<DonViQuyDoiItem> DonViNhapList { get; set; } = new();
    }
}
