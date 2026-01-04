using VETFEED.Backend.API.DTOs.NhaCungCap;

namespace VETFEED.Backend.API.Services
{
    public interface INhaCungCapService
    {
        Task<IEnumerable<NhaCungCapResponse>> GetAllNhaCungCapsAsync();
        Task<NhaCungCapResponse?> GetNhaCungCapByIdAsync(Guid id);
        Task<NhaCungCapResponse> AddNhaCungCapAsync(NhaCungCapRequest request);
        Task<NhaCungCapResponse?> UpdateNhaCungCapAsync(Guid id, NhaCungCapRequest request);
        Task<bool> DeleteNhaCungCapAsync(Guid id);
    }
}
