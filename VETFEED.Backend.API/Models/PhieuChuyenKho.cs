using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Phiếu chuyển kho giữa các kho
    /// </summary>
    public class PhieuChuyenKho
    {
        [Key]
        public Guid MaCK { get; set; }
        public string? MaCKCode { get; set; }
        public DateTime NgayLap { get; set; }
        public Guid MaKhoXuat { get; set; }
        public Guid MaKhoNhan { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public KhoHang? KhoXuat { get; set; }
        public KhoHang? KhoNhan { get; set; }
        public ICollection<CTPhieuChuyenKho>? CTPhieuChuyenKhos { get; set; }
    }
}
