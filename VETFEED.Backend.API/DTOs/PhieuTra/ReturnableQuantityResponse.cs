using VETFEED.Backend.API.DTOs.LoHang;

namespace VETFEED.Backend.API.DTOs.PhieuTra
{
    /// <summary>
    /// Response trả về thông tin số lượng có thể trả được cho một phiếu bán
    /// </summary>
    public class ReturnableQuantityResponse
    {
        /// <summary>
        /// Mã phiếu bán
        /// </summary>
        public Guid MaPB { get; set; }

        /// <summary>
        /// Mã phiếu bán (code)
        /// </summary>
        public string? MaPBCode { get; set; }

        /// <summary>
        /// Danh sách các lô có thể trả
        /// </summary>
        public List<ReturnableLoHangItem> DanhSachLoHang { get; set; } = new();
    }

}
