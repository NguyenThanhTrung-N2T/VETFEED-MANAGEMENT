using VETFEED.Backend.API.DTOs.LoHang;

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
    }
}
