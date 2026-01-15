using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.PhieuTra
{
    /// <summary>
    /// Request để kiểm tra có thể trả hàng được không
    /// </summary>
    public class CheckReturnableRequest
    {
        /// <summary>
        /// Mã phiếu bán
        /// </summary>
        [Required(ErrorMessage = "Mã phiếu bán không được để trống!")]
        public Guid MaPB { get; set; }

        /// <summary>
        /// Mã lô cần trả
        /// </summary>
        [Required(ErrorMessage = "Mã lô không được để trống!")]
        public Guid MaLo { get; set; }

        /// <summary>
        /// Số lượng cần trả (đơn vị cơ sở)
        /// </summary>
        [Range(0.0001, double.MaxValue, ErrorMessage = "Số lượng phải > 0")]
        public decimal SoLuong { get; set; }
    }
}
