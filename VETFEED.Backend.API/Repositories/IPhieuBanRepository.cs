using VETFEED.Backend.API.DTOs.PhieuBan;

namespace VETFEED.Backend.API.Repositories
{
    public interface IPhieuBanRepository
    {
        // L?y danh sách phi?u bán
        Task<IEnumerable<PhieuBanResponse>> GetDanhSachPhieuBanAsync();

        // L?y chi ti?t phi?u bán
        Task<PhieuBanResponse?> GetChiTietPhieuBanAsync(Guid maPB);

        // T?o phi?u bán
        Task<PhieuBanResponse> CreatePhieuBanAsync(CreatePhieuBanRequest request);

        // C?p nh?t phi?u bán
        Task<PhieuBanResponse?> UpdatePhieuBanAsync(Guid maPB, CreatePhieuBanRequest request);

        // Xóa phi?u bán
        Task<bool> DeletePhieuBanAsync(Guid maPB);
    }
}
