using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface ILoHangRepository
    {
        Task<IEnumerable<LoHangResponse>> GetAllLoHangsAsync();
        Task<LoHangResponse?> GetLoHangByIdAsync(Guid id);
        Task<IEnumerable<LoHangResponse>> GetBySanPhamAsync(Guid maSP);
        Task<LoHangResponse> AddLoHangAsync(LoHangRequest request);
        Task<LoHangResponse?> UpdateLoHangAsync(Guid id, LoHangRequest request);
        Task<bool> DeleteLoHangAsync(Guid id);

        // Lay entity LoHang  de dung trong service
        Task<LoHang?> GetLoHangEntityByIdAsync(Guid id);

        // Cap nhat NgaySanXuat va HanSuDung cho LoHang
        Task<bool> UpdateLoHangDatesAsync(Guid maLo, DateTime? ngaySanXuat, DateTime hanSuDung);

        // Xoa nhieu LoHang cung luc (batch delete)
        Task<int> DeleteLoHangsByIdsAsync(IEnumerable<Guid> maLoList);
    }
}


