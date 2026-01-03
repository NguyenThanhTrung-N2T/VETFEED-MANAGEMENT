using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.GiaBan;

namespace VETFEED.Backend.API.Services
{
    public interface IGiaBanService
    {
        Task<PagedResult<GiaBanResponse>> SearchAsync(GiaBanQuery query);
        Task<GiaBanResponse?> GetByIdAsync(Guid maGia);

        Task<GiaBanResponse> CreateAsync(GiaBanCreateRequest request);
        Task<GiaBanResponse?> UpdateAsync(Guid maGia, GiaBanUpdateRequest request);

        Task<(bool ok, string? error)> DeleteAsync(Guid maGia);
        Task<GiaBanResponse?> GetCurrentPriceAsync(Guid maSP, DateTime date);

    }
}
