using VETFEED.Backend.API.DTOs.CTPhieuNhap;

namespace VETFEED.Backend.API.DTOs.PhieuNhap
{
    /// <summary>
    /// Request tạo phiếu nhập kèm danh sách chi tiết
    /// Service sẽ tự gán TrangThai = DA_DAT, ThanhTien = 0
    /// </summary>
    public class PhieuNhapCreateRequest
    {
        public Guid MaNCC { get; set; }
        public Guid MaKho { get; set; }
        public string? GhiChu { get; set; }
        
        /// <summary>
        /// Danh sách chi tiết phiếu nhập (lô hàng + số lượng)
        /// </summary>
        public List<CTPhieuNhapRequest>? DanhSachChiTiet { get; set; }
    }
}
