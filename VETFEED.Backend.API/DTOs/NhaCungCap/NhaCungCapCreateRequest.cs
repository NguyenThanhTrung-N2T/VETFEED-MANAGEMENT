using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
namespace VETFEED.Backend.API.DTOs.NhaCungCap
{
    /// <summary>
    /// DTO để tạo mới nhà cung cấp kèm danh sách sản phẩm (nếu có)
    /// </summary>
    public class NhaCungCapCreateRequest
    {
        /// <summary>
        /// Tên nhà cung cấp (bắt buộc)
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
        /// Danh sách sản phẩm của NCC (không bắt buộc).
        /// Nếu có, sẽ tạo liên kết NCC-SP khi tạo NCC.
        /// </summary>
        public List<NhaCungCapSanPhamItemDto>? SanPhams { get; set; }
    }
}
