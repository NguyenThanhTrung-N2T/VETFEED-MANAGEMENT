using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.SanPham;

namespace VETFEED.Backend.API.Services
{
    public interface ISanPhamService
    {
        Task<PagedResult<SanPhamResponse>> SearchAsync(SanPhamQuery query);
        Task<SanPhamResponse?> GetByIdAsync(Guid maSP);
        Task<SanPhamResponse> CreateAsync(SanPhamCreateRequest request);
        Task<SanPhamResponse?> UpdateAsync(Guid maSP, SanPhamUpdateRequest request);
        Task<(bool ok, string? error)> DeleteAsync(Guid maSP);
    }
}
