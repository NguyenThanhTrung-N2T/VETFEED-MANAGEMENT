using VETFEED.Backend.API.DTOs.PhieuTra;

namespace VETFEED.Backend.API.Services
{
    /// <summary>
    /// Service xử lý logic phiếu trả hàng
    /// </summary>
    public interface IPhieuTraService
    {
        //Lấy danh sách tất cả phiếu trả
        Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync();


        //Lấy chi tiết phiếu trả
        Task<PhieuTraDetailResponse?> GetChiTietPhieuTraAsync(Guid maPT);

        //Tạo phiếu trả hàng mới - trả về chi tiết đầy đủ
        Task<(PhieuTraDetailResponse? result, string? error)> CreatePhieuTraAsync(CreatePhieuTraRequest request);

        //Xóa phiếu trả hàng
        Task<bool> DeletePhieuTraAsync(Guid maPT);

        /// <summary>
        /// Kiểm tra có thể trả hàng được không.
        /// Trả về true nếu số lượng cần trả <= (số lượng đã bán - số lượng đã trả trước đó)
        /// </summary>
        Task<bool> CheckReturnableAsync(CheckReturnableRequest request);
    }
}


