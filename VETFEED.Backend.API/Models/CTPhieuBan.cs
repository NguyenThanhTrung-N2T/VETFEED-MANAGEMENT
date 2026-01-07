using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Chi tiết phiếu bán (kho, lô, số lượng)
    /// </summary>
    public class CTPhieuBan
    {
        [Key]
        public Guid MaCTPB { get; set; }
        public Guid MaPB { get; set; }
        public Guid MaKho { get; set; }
        public Guid MaLo { get; set; }
        public decimal SoLuong { get; set; }                    // Số lượng theo đơn vị giao dịch
        public decimal DonGia { get; set; }                     // Đơn giá theo đơn vị giao dịch
        public string? DonViBan { get; set; }                   // Đơn vị giao dịch (Thùng/Hộp/Viên...)
        public decimal SoLuongQuyDoi { get; set; }              // Số lượng quy đổi về đơn vị cơ sở
        public decimal GiaVonCoSo { get; set; }                 // Giá vốn tại thời điểm bán (theo đơn vị cơ sở)
        public decimal ThanhTienVon { get; set; }               // Thành tiền vốn (SoLuongQuyDoi * GiaVonCoSo)
        public string? GhiChu { get; set; }

        // Navigation
        public PhieuBan? PhieuBan { get; set; }
        public KhoHang? KhoHang { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
