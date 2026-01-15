using VETFEED.Backend.API.DTOs.PhieuTra;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuTraService : IPhieuTraService
    {
        private readonly IPhieuTraRepository _phieuTraRepository;

        public PhieuTraService(IPhieuTraRepository phieuTraRepository)
        {
            _phieuTraRepository = phieuTraRepository;
        }

        //Lấy danh sách phiếu trả
        public async Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync()
        {
            return await _phieuTraRepository.GetDanhSachPhieuTraAsync();
        }

        //Lấy chi tiết phiếu trả
        public async Task<PhieuTraDetailResponse?> GetChiTietPhieuTraAsync(Guid maPT)
        {
            return await _phieuTraRepository.GetChiTietPhieuTraAsync(maPT);
        }

        //Tạo phiếu trả hàng
        public async Task<(PhieuTraDetailResponse? result, string? error)> CreatePhieuTraAsync(CreatePhieuTraRequest request)
        {
            try
            {
                // Kiểm tra input
                if (request == null)
                    return (null, "Request không được null!");

                if (request.MaPB == Guid.Empty)
                    return (null, "Mã phiếu bán không hợp lệ!");

                if (request.DanhSachChiTiet == null || !request.DanhSachChiTiet.Any())
                    return (null, "Danh sách chi tiết trả không được để trống!");

                var result = await _phieuTraRepository.CreatePhieuTraAsync(request);
                return (result, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        //Xóa phiếu trả
        public async Task<bool> DeletePhieuTraAsync(Guid maPT)
        {
            try
            {
                return await _phieuTraRepository.DeletePhieuTraAsync(maPT);
            } catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa phiếu trả: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra có thể trả hàng được không.
        /// </summary>
        public async Task<bool> CheckReturnableAsync(CheckReturnableRequest request)
        {
            return await _phieuTraRepository.CheckReturnableAsync(
                request.MaPB, 
                request.MaLo, 
                request.SoLuong);
        }

    }
}
