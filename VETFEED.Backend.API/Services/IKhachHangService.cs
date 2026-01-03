using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.KhachHang;

namespace VETFEED.Backend.API.Services
{
    public interface IKhachHangService
    {
        Task<PagedResult<KhachHangResponse>> SearchAsync(KhachHangQuery query);
        Task<KhachHangResponse?> GetByIdAsync(Guid maKH);
        Task<KhachHangResponse> CreateAsync(KhachHangCreateRequest request);
        Task<KhachHangResponse?> UpdateAsync(Guid maKH, KhachHangUpdateRequest request);
        Task<(bool ok, string? error)> DeleteAsync(Guid maKH);
    }
}
