using VETFEED.Backend.API.DTOs.PhieuBan;

namespace VETFEED.Backend.API.Repositories
{
    public interface IPhieuBanRepository
    {
        // lay danh sach phieu ban
        Task<IEnumerable<PhieuBanResponse>> GetDanhSachPhieuBanAsync();

        // lay chi tiet phieu ban
        Task<PhieuBanDetailResponse?> GetChiTietPhieuBanAsync(Guid maPB);

        // tao phieu ban
        Task<PhieuBanDetailResponse> CreatePhieuBanAsync(CreatePhieuBanRequest request);

        // C?p nh?t phi?u bán
        Task<PhieuBanDetailResponse?> UpdatePhieuBanAsync(Guid maPB, CreatePhieuBanRequest request);

        // Xóa phi?u bán
        Task<bool> DeletePhieuBanAsync(Guid maPB);
    }
}
