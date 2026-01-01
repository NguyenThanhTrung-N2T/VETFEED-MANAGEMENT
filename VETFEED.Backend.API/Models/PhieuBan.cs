using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Phiếu bán hàng cho khách
    /// </summary>
    public class PhieuBan
    {
        [Key]
        public Guid MaPB { get; set; }
        public string? MaPBCode { get; set; }
        public Guid MaKH { get; set; }
        public DateTime NgayBan { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal ChietKhauPhanTram { get; set; }
        public decimal TienChietKhau { get; set; }
        public decimal ThanhTien { get; set; }
        public HinhThucThanhToanEnum HinhThucThanhToan { get; set; }
        public TrangThaiThanhToanEnum TrangThaiThanhToan { get; set; }
        public decimal TienCoc { get; set; }
        public decimal TienNo { get; set; }
        public DateTime? HanTra { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public KhachHang? KhachHang { get; set; }
        public ICollection<CTPhieuBan>? CTPhieuBans { get; set; }
        public ICollection<PhieuTra>? PhieuTras { get; set; }
    }
}
