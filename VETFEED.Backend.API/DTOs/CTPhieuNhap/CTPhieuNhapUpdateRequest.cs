namespace VETFEED.Backend.API.DTOs.CTPhieuNhap
{
    /// <summary>
    /// Request cập nhật chi tiết phiếu nhập
    /// </summary>
    public class CTPhieuNhapUpdateRequest
    {
        public Guid MaCTPN { get; set; }        // ID của chi tiết phiếu nhập cần update
        public decimal SoLuong { get; set; }    // Số lượng (theo đơn vị nhập)
        public decimal? DonGia { get; set; }    // Đơn giá (bắt buộc khi chuyển sang DA_NHAN)
        public string? DonViNhap { get; set; }  // Đơn vị giao dịch (Thùng/Hộp...)
        
        // Thông tin lô hàng - cập nhật vào LoHang
        public DateTime? NgaySanXuat { get; set; }  // Ngày sản xuất (nullable)
        public DateTime? HanSuDung { get; set; }    // Hạn sử dụng (nullable nếu không thay đổi)
    }
}

