using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.GiaBan;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface IGiaBanRepository
    {
        Task<PagedResult<GiaBanResponse>> SearchAsync(GiaBanQuery query);
        Task<GiaBanResponse?> GetByIdAsync(Guid maGia);

        Task<GiaBanResponse> CreateAsync(GiaBan entity);
        Task<GiaBanResponse?> UpdateAsync(Guid maGia, GiaBanUpdateRequest request);

        Task<bool> DeleteAsync(Guid maGia);

        Task<bool> ExistsSanPhamAsync(Guid maSP);

        // check overlap cho cùng MaSP
        Task<bool> IsOverlappingAsync(Guid maSP, DateTime tu, DateTime? den, Guid? excludeMaGia = null);

        Task<GiaBanResponse?> GetCurrentPriceAsync(Guid maSP, DateTime date);

    }
}
