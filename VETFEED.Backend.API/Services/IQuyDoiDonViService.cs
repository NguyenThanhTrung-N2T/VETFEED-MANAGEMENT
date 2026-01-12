using VETFEED.Backend.API.DTOs.QuyDoiDonVi;

namespace VETFEED.Backend.API.Services
{
    public interface IQuyDoiDonViService
    {
        Task<IEnumerable<QuyDoiDonViResponse>> GetByMaSPAsync(Guid maSP);
        Task<QuyDoiDonViResponse> CreateAsync(Guid maSP, QuyDoiDonViCreateRequest request);
        Task<QuyDoiDonViResponse?> UpdateAsync(Guid maQD, QuyDoiDonViUpdateRequest request);
        Task<bool> DeleteAsync(Guid maQD);

        Task<UnitsForProductResponse> GetUnitsForProductAsync(Guid maSP);
    }
}
