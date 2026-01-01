using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Phiếu trả hàng từ khách
    /// </summary>
    public class PhieuTra
    {
        [Key]
        public Guid MaPT { get; set; }
        public string? MaPTCode { get; set; }
        public DateTime NgayTra { get; set; }
        public Guid MaPB { get; set; }
        public Guid MaKH { get; set; }
        public string? LyDo { get; set; }
        public decimal ThanhTien { get; set; }
        public HinhThucHoanTienEnum HinhThucHoanTien { get; set; }

        // Navigation
        public PhieuBan? PhieuBan { get; set; }
        public KhachHang? KhachHang { get; set; }
        public ICollection<CTPhieuTra>? CTPhieuTras { get; set; }
    }
}
