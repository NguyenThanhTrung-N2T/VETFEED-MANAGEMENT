using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Phiếu nhập hàng từ nhà cung cấp
    /// </summary>
    public class PhieuNhap
    {
        [Key]
        public Guid MaPN { get; set; }
        public string? MaPNCode { get; set; }                // Mã phiếu nhập hiển thị
        public Guid MaNCC { get; set; }                     // FK Nhà cung cấp
        public Guid MaKho { get; set; }                     // FK Kho nhập
        public decimal ThanhTien { get; set; }              // Tổng tiền phiếu nhập
        public TrangThaiPhieuNhapEnum TrangThai { get; set; } // Trạng thái nghiệp vụ
        public string? GhiChu { get; set; }
        public DateTime NgayCapNhat { get; set; }

        // Navigation
        public NhaCungCap? NhaCungCap { get; set; }
        public KhoHang? KhoHang { get; set; }
        public ICollection<CTPhieuNhap>? CTPhieuNhaps { get; set; }
    }
}
