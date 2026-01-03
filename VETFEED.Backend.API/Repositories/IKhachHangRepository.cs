using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.KhachHang;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface IKhachHangRepository
    {
        Task<PagedResult<KhachHangResponse>> SearchAsync(KhachHangQuery query);
        Task<KhachHangResponse?> GetByIdAsync(Guid maKH);

        Task<KhachHangResponse> CreateAsync(KhachHang entity);
        Task<KhachHangResponse?> UpdateAsync(Guid maKH, KhachHangUpdateRequest request);

        Task<bool> DeleteAsync(Guid maKH);
        Task<bool> HasReferencesAsync(Guid maKH); // PhieuBan / PhieuTra / CongNo(KH)
        Task<bool> PhoneExistsAsync(string phone, Guid? excludeMaKH = null);
    }
}
