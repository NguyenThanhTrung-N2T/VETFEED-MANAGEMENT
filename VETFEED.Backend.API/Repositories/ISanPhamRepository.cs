using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.SanPham;

namespace VETFEED.Backend.API.Repositories
{
    public interface ISanPhamRepository
    {
        Task<PagedResult<SanPhamResponse>> SearchAsync(SanPhamQuery query);
        Task<SanPhamResponse?> GetByIdAsync(Guid maSP);
        Task<SanPhamResponse> CreateAsync(VETFEED.Backend.API.Models.SanPham entity);
        Task<SanPhamResponse?> UpdateAsync(Guid maSP, SanPhamUpdateRequest request);
        Task<bool> DeleteAsync(Guid maSP);

        Task<bool> HasReferencesAsync(Guid maSP); // check GiaBan/LoHang/NCSP
    }
}
