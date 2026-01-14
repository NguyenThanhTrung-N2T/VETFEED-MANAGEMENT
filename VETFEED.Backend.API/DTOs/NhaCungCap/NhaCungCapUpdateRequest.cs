using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
namespace VETFEED.Backend.API.DTOs.NhaCungCap
{
    /// <summary>
    /// DTO để cập nhật thông tin nhà cung cấp kèm danh sách sản phẩm
    /// </summary>
    public class NhaCungCapUpdateRequest
    {
        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string? TenNCC { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? SoDienThoai { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? DiaChi { get; set; }

        /// <summary>
        /// Trạng thái: HOAT_DONG | NGUNG_HOAT_DONG
        /// </summary>
        public string? TrangThai { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? GhiChu { get; set; }

        /// <summary>
        /// Danh sách sản phẩm của nhà cung cấp cần xử lý:
        /// - MaNCSP = null/Guid.Empty + IsDeleted = false: thêm mới
        /// - MaNCSP có giá trị + IsDeleted = false: cập nhật
        /// - MaNCSP có giá trị + IsDeleted = true: xóa
        /// Nếu null hoặc rỗng: không xử lý danh sách sản phẩm.
        /// </summary>
        public List<NhaCungCapSanPhamItemDto>? SanPhams { get; set; }
    }
}
