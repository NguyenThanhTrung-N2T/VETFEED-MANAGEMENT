namespace VETFEED.Backend.API.DTOs.CTPhieuNhap
{
    /// <summary>
    /// Request cập nhật/thêm mới chi tiết phiếu nhập
    /// - Nếu MaCTPN = Guid.Empty: tạo mới (yêu cầu MaSP, HanSuDung)
    /// - Nếu MaCTPN != Guid.Empty: cập nhật chi tiết đã có
    /// </summary>
    public class CTPhieuNhapUpdateRequest
    {
        public Guid MaCTPN { get; set; }        // ID của chi tiết phiếu nhập (Guid.Empty = tạo mới)
        
        // Trường bắt buộc khi tạo mới (MaCTPN = Guid.Empty)
        public Guid? MaSP { get; set; }         // Mã sản phẩm (bắt buộc khi tạo mới)
        
        public decimal SoLuong { get; set; }    // Số lượng (theo đơn vị nhập)
        public decimal? DonGia { get; set; }    // Đơn giá (bắt buộc khi chuyển sang DA_NHAN)
        public string? DonViNhap { get; set; }  // Đơn vị giao dịch (Thùng/Hộp...)
        
        // Thông tin lô hàng - cập nhật vào LoHang
        public DateTime? NgaySanXuat { get; set; }  // Ngày sản xuất (nullable)
        public DateTime? HanSuDung { get; set; }    // Hạn sử dụng (bắt buộc khi tạo mới)
    }
}

