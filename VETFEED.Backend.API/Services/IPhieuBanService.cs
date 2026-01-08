using VETFEED.Backend.API.DTOs.PhieuBan;

namespace VETFEED.Backend.API.Services
{
    public interface IPhieuBanService
    {
        // lay danh sach phieu ban
        Task<IEnumerable<PhieuBanResponse>> GetDanhSachPhieuBanAsync();

        // lay chi tiet phieu ban
        Task<PhieuBanDetailResponse?> GetChiTietPhieuBanAsync(Guid maPB);

        // Tap phieu ban
        Task<(PhieuBanDetailResponse? result, string? error)> CreatePhieuBanAsync(CreatePhieuBanRequest request);

        // Xóa phiếu bán
        Task<bool> DeletePhieuBanAsync(Guid maPB);
    }
}
