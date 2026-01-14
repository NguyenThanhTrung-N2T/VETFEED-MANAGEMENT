using VETFEED.Backend.API.DTOs.PhieuTra;

namespace VETFEED.Backend.API.Repositories
{
    /// <summary>
    /// Repository phiếu trả hàng
    /// </summary>
    public interface IPhieuTraRepository
    {
        /// <summary>
        /// Lấy danh sách phiếu trả
        /// </summary>
        Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync();

        /// <summary>
        /// Lấy chi tiết phiếu trả
        /// </summary>
        Task<PhieuTraDetailResponse?> GetChiTietPhieuTraAsync(Guid maPT);

        /// <summary>
        /// Tạo phiếu trả mới - trả về chi tiết đầy đủ
        /// </summary>
        Task<PhieuTraDetailResponse> CreatePhieuTraAsync(CreatePhieuTraRequest request);

        /// <summary>
        /// Xóa phiếu trả
        /// </summary>
        Task<bool> DeletePhieuTraAsync(Guid maPT);

        /// <summary>
        /// Lấy số lượng có thể trả được cho một phiếu bán.
        /// Tính toán: SoLuongCoTheTra = SoLuongDaBan - SoLuongDaTra (từ các phiếu trả trước)
        /// </summary>
        Task<ReturnableQuantityResponse?> GetReturnableQuantityAsync(Guid maPB);
    }
}

