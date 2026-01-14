namespace VETFEED.Backend.API.DTOs.LoHang
{
    /// <summary>
    /// Thông tin số lượng có thể trả của từng lô
    /// </summary>
    public class ReturnableLoHangItem
    {
        /// <summary>
        /// Mã lô
        /// </summary>
        public Guid MaLo { get; set; }

        /// <summary>
        /// Mã lô (code)
        /// </summary>
        public string? MaLoCode { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string? TenSanPham { get; set; }

        /// <summary>
        /// Đơn vị cơ sở
        /// </summary>
        public string? DonViCoSo { get; set; }

        /// <summary>
        /// Hạn sử dụng
        /// </summary>
        public DateTime HanSuDung { get; set; }

        /// <summary>
        /// Tổng số lượng đã bán (đơn vị cơ sở)
        /// </summary>
        public decimal SoLuongDaBan { get; set; }

        /// <summary>
        /// Tổng số lượng đã trả từ các phiếu trả trước (đơn vị cơ sở)
        /// </summary>
        public decimal SoLuongDaTra { get; set; }

        /// <summary>
        /// Số lượng còn có thể trả = SoLuongDaBan - SoLuongDaTra (đơn vị cơ sở)
        /// </summary>
        public decimal SoLuongCoTheTra { get; set; }
    }
}