namespace VETFEED.Backend.API.DTOs.CTPhieuNhap
{
    /// <summary>
    /// Request cập nhật/thêm mới chi tiết phiếu nhập.
    /// </summary>
    /// <remarks>
    /// **THÊM MỚI**: Nếu MaCTPN = "00000000-0000-0000-0000-000000000000" (Guid.Empty) → tạo mới CTPN (yêu cầu MaSP, HanSuDung).
    /// 
    /// **CẬP NHẬT**: Nếu MaCTPN là GUID thực → cập nhật chi tiết đã có.
    /// 
    /// **XÓA**: Các CTPN trong DB mà KHÔNG gửi trong request sẽ bị xóa.
    /// </remarks>
    public class CTPhieuNhapUpdateRequest
    {
        /// <summary>
        /// ID của chi tiết phiếu nhập.
        /// - Để "00000000-0000-0000-0000-000000000000" (Guid.Empty) nếu muốn THÊM MỚI.
        /// - Điền GUID thực nếu muốn CẬP NHẬT chi tiết đã có.
        /// </summary>
        /// <example>00000000-0000-0000-0000-000000000000</example>
        public Guid MaCTPN { get; set; }
        
        /// <summary>
        /// Mã sản phẩm - BẮT BUỘC khi thêm mới (MaCTPN = Guid.Empty).
        /// Không cần thiết khi cập nhật chi tiết đã có.
        /// </summary>
        public Guid? MaSP { get; set; }
        
        /// <summary>
        /// Số lượng (theo đơn vị nhập).
        /// </summary>
        /// <example>100</example>
        public decimal SoLuong { get; set; }
        
        /// <summary>
        /// Đơn giá (theo đơn vị nhập). BẮT BUỘC khi chuyển trạng thái sang DA_NHAN.
        /// </summary>
        /// <example>50000</example>
        public decimal? DonGia { get; set; }
        
        /// <summary>
        /// Đơn vị giao dịch (Thùng/Hộp/Gói...).
        /// </summary>
        /// <example>Thùng</example>
        public string? DonViNhap { get; set; }
        
        /// <summary>
        /// Ngày sản xuất của lô hàng (có thể để trống).
        /// </summary>
        /// <example>2026-01-01</example>
        public DateTime? NgaySanXuat { get; set; }
        
        /// <summary>
        /// Hạn sử dụng của lô hàng - BẮT BUỘC khi thêm mới. Phải là ngày trong tương lai.
        /// </summary>
        /// <example>2027-01-01</example>
        public DateTime? HanSuDung { get; set; }
    }
}

