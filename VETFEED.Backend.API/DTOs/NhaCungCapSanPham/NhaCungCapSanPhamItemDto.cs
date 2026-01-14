namespace VETFEED.Backend.API.DTOs.NhaCungCapSanPham
{
    /// <summary>
    /// DTO cho việc xử lý sản phẩm của nhà cung cấp (thêm/sửa)
    /// Logic xóa: NCCSP trong DB nhưng không có trong request sẽ bị xóa
    /// </summary>
    public class NhaCungCapSanPhamItemDto
    {
        /// <summary>
        /// Mã NCC sản phẩm:
        /// - null hoặc Guid.Empty: thêm mới sản phẩm
        /// - có giá trị: cập nhật sản phẩm đó
        /// </summary>
        public Guid? MaNCSP { get; set; }

        /// <summary>
        /// Mã sản phẩm (bắt buộc khi thêm mới)
        /// </summary>
        public Guid MaSP { get; set; }

        /// <summary>
        /// Giá nhập mặc định
        /// </summary>
        public decimal? GiaNhapMacDinh { get; set; }

        /// <summary>
        /// Trạng thái: HOAT_DONG | NGUNG_HOAT_DONG
        /// </summary>
        public string? TrangThai { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? GhiChu { get; set; }
    }
}
