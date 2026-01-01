using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.Models
{
    /// <summary>
    /// Bảng quản lý tồn kho theo kho + lô
    /// </summary>
    public class TonKho
    {
        [Key]
        public Guid MaTonKho { get; set; }
        public Guid MaKho { get; set; }
        public Guid MaLo { get; set; }
        public decimal SoLuong { get; set; }
        public DateTime NgayCapNhat { get; set; }

        // Navigation
        public KhoHang? KhoHang { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
