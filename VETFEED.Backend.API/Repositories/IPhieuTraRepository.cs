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
        /// Kiểm tra có thể trả hàng được không.
        /// Trả về true nếu số lượng cần trả <= (số lượng đã bán - số lượng đã trả trước đó)
        /// </summary>
        Task<bool> CheckReturnableAsync(Guid maPB, Guid maLo, decimal soLuong);
    }
}


