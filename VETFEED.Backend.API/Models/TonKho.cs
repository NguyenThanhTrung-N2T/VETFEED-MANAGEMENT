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
        public decimal SoLuongCoSo { get; set; }                 // Số lượng tồn theo đơn vị cơ sở
        public decimal GiaVonBinhQuan { get; set; }              // Giá vốn bình quân theo đơn vị cơ sở
        public DateTime NgayCapNhat { get; set; }

        // Navigation
        public KhoHang? KhoHang { get; set; }
        public LoHang? LoHang { get; set; }
    }
}
