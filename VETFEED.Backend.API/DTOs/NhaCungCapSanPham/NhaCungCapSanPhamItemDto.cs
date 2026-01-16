namespace VETFEED.Backend.API.DTOs.NhaCungCapSanPham
{
    /// <summary>
    /// DTO cho việc xử lý sản phẩm của nhà cung cấp (thêm/sửa)
    /// Logic: Backend sẽ tự kiểm tra MaSP đã tồn tại hay chưa để add/update
    /// Logic xóa: NCCSP trong DB nhưng không có trong request sẽ bị xóa
    /// </summary>
    public class NhaCungCapSanPhamItemDto
    {
        /// <summary>
        /// Mã sản phẩm (bắt buộc)
        /// </summary>
        public Guid MaSP { get; set; }
        public decimal? GiaNhapMacDinh { get; set; }
        /// <summary>
        /// Trạng thái: HOAT_DONG | NGUNG_HOAT_DONG
        /// </summary>
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}
