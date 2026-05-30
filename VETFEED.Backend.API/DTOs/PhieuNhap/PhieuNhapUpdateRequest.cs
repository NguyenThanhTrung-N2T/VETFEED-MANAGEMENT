using VETFEED.Backend.API.DTOs.CTPhieuNhap;

namespace VETFEED.Backend.API.DTOs.PhieuNhap
{
    /// <summary>
    /// Request cập nhật phiếu nhập bao gồm:
    /// - Thông tin phiếu nhập (MaNCC, MaKho, TrangThai, GhiChu)
    /// - Danh sách chi tiết (những chi tiết không có trong danh sách sẽ bị xóa)
    /// </summary>
    public class PhieuNhapUpdateRequest
    {
        public Guid MaNCC { get; set; }
        public Guid MaKho { get; set; }
        public string? TrangThai { get; set; }  // DA_DAT, DA_NHAN, DA_HUY
        public string? GhiChu { get; set; }
        public decimal ThanhTien { get; set; }  // Tổng tiền

        /// <summary>
        /// Danh sách chi tiết phiếu nhập muốn giữ lại và cập nhật.
        /// Những chi tiết trong database mà KHÔNG có trong danh sách này sẽ bị xóa.
        /// </summary>
        public List<CTPhieuNhapUpdateRequest>? DanhSachChiTiet { get; set; }
    }
}



