using VETFEED.Backend.API.DTOs.NhaCungCap;

namespace VETFEED.Backend.API.Services
{
    public interface INhaCungCapService
    {
        Task<IEnumerable<NhaCungCapResponse>> GetAllNhaCungCapsAsync();
        Task<NhaCungCapDetailedResponse?> GetNhaCungCapByIdAsync(Guid id);

        /// <summary>
        /// Tạo mới nhà cung cấp kèm danh sách sản phẩm (nếu có).
        /// Sử dụng transaction để đảm bảo tính nhất quán.
        /// </summary>
        Task<NhaCungCapDetailedResponse> AddNhaCungCapAsync(NhaCungCapCreateRequest request);

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp và danh sách sản phẩm (nếu có).
        /// Hỗ trợ thêm, sửa, xóa sản phẩm trong một request.
        /// Sử dụng transaction để đảm bảo tính nhất quán.
        /// </summary>
        Task<NhaCungCapDetailedResponse?> UpdateNhaCungCapAsync(Guid id, NhaCungCapUpdateRequest request);

        /// <summary>
        /// Xóa nhà cung cấp và cascade xóa tất cả NCC sản phẩm liên quan.
        /// Sử dụng transaction để đảm bảo tính nhất quán.
        /// </summary>
        Task<bool> DeleteNhaCungCapAsync(Guid id);
    }
}
