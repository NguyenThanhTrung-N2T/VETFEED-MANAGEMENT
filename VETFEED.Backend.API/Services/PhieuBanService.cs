using VETFEED.Backend.API.DTOs.PhieuBan;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuBanService : IPhieuBanService
    {
        private readonly IPhieuBanRepository _phieuBanRepo;

        public PhieuBanService(IPhieuBanRepository phieuBanRepo)
        {
            _phieuBanRepo = phieuBanRepo;
        }

        // L?y danh sách phi?u bán
        public async Task<IEnumerable<PhieuBanResponse>> GetDanhSachPhieuBanAsync()
        {
            return await _phieuBanRepo.GetDanhSachPhieuBanAsync();
        }

        // L?y chi ti?t phi?u bán
        public async Task<PhieuBanResponse?> GetChiTietPhieuBanAsync(Guid maPB)
        {
            return await _phieuBanRepo.GetChiTietPhieuBanAsync(maPB);
        }

        // T?o phi?u bán
        public async Task<(PhieuBanResponse? result, string? error)> CreatePhieuBanAsync(CreatePhieuBanRequest request)
        {
            try
            {
                // ? Validate
                if (request.DanhSachChiTiet == null || request.DanhSachChiTiet.Count == 0)
                    return (null, "Danh sách chi ti?t phi?u bán không ???c ?? tr?ng!");

                if (request.TienCoc < 0)
                    return (null, "Ti?n c?c không ???c âm!");

                if (request.ChietKhauPhanTram < 0 || request.ChietKhauPhanTram > 100)
                    return (null, "Chi?t kh?u ph?i t? 0 ??n 100%!");

                // ? G?i repository t?o phi?u bán
                var result = await _phieuBanRepo.CreatePhieuBanAsync(request);
                return (result, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // C?p nh?t phi?u bán
        public async Task<(PhieuBanResponse? result, string? error)> UpdatePhieuBanAsync(Guid maPB, CreatePhieuBanRequest request)
        {
            try
            {
                var result = await _phieuBanRepo.UpdatePhieuBanAsync(maPB, request);
                if (result == null)
                    return (null, "Phi?u bán không t?n t?i!");

                return (result, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // Xóa phi?u bán
        public async Task<bool> DeletePhieuBanAsync(Guid maPB)
        {
            return await _phieuBanRepo.DeletePhieuBanAsync(maPB);
        }
    }
}
