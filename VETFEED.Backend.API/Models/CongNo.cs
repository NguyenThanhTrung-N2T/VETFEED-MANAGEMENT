using System;
using VETFEED.Backend.API.Enums;
using System.ComponentModel.DataAnnotations;
namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng công nợ (ghi nhận phát sinh nợ)
    /// </summary>
    public class CongNo
    {
        [Key]
        public Guid MaCongNo { get; set; }
        public LoaiDoiTuongCongNoEnum LoaiDoiTuong { get; set; } // KH hoặc NCC
        public Guid MaDoiTuong { get; set; }                     // FK tới KH hoặc NCC
        public Guid? MaPhieu { get; set; }                       // Phiếu liên quan
        public decimal SoTien { get; set; }                      // >0 tăng nợ, <0 giảm nợ
        public DateTime NgayPhatSinh { get; set; }
        public DateTime? HanThanhToan { get; set; }
        public string? GhiChu { get; set; }
    }
}
