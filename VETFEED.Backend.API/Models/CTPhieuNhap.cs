using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Chi tiết phiếu nhập (lô hàng + số lượng)
    /// </summary>
    public class CTPhieuNhap
    {
        [Key]
        public Guid MaCTPN { get; set; }
        public Guid MaPN { get; set; }                          // FK Phiếu nhập
        public Guid MaLo { get; set; }                          // FK Lô hàng
        public decimal SoLuong { get; set; }                    // Số lượng theo đơn vị giao dịch
        public decimal? DonGia { get; set; }                    // Đơn giá theo đơn vị giao dịch
        public string? DonViNhap { get; set; }                  // Đơn vị giao dịch (Thùng/Hộp...)
        public decimal SoLuongQuyDoi { get; set; }              // Số lượng quy đổi về đơn vị cơ sở
        public decimal DonGiaCoSo { get; set; }                 // Giá vốn theo đơn vị cơ sở

        // Navigation
        public PhieuNhap? PhieuNhap { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
